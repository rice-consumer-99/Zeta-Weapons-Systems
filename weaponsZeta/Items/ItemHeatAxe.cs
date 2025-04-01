using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;


namespace weaponsZeta.Items
{
    //Mostly copied Vanilia Axe code with Torch igniting things parts added in
    public class ItemHeatAxe : ItemAxe
    {
        string igniteAnimation;
        public override void OnLoaded(ICoreAPI api)
        {
            igniteAnimation = Attributes["igniteAnimation"].AsString();
        }


        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);

            // Commented out cause GroundStorable behavior was preventing animations and sound when using on pitkiln
            //if (handling == EnumHandHandling.PreventDefault) return;

            if (blockSel == null) return;
            Block block = byEntity.World.BlockAccessor.GetBlock(blockSel.Position);

            IPlayer byPlayer = (byEntity as EntityPlayer)?.Player;
            if (!byEntity.World.Claims.TryAccess(byPlayer, blockSel.Position, EnumBlockAccessFlags.Use))
            {
                return;
            }

            EnumIgniteState state = EnumIgniteState.NotIgnitable;
            if (!(block is IIgnitable ign) || (state = ign.OnTryIgniteBlock(byEntity, blockSel.Position, 0)) != EnumIgniteState.Ignitable)
            {
                if (state == EnumIgniteState.NotIgnitablePreventDefault) handling = EnumHandHandling.PreventDefault;
                return;
            }

            handling = EnumHandHandling.PreventDefault;


            if (api.Side == EnumAppSide.Client)
            {
                api.Event.UnregisterCallback(ObjectCacheUtil.TryGet<long>(api, "firestartersound"));
                api.ObjectCache["firestartersound"] = api.Event.RegisterCallback((dt) => byEntity.World.PlaySoundAt(new AssetLocation("sounds/player/handdrill"), byEntity, byPlayer, false, 16), 500);
            }
        }


        public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            if (blockSel == null)
            {
                api.Event.UnregisterCallback(ObjectCacheUtil.TryGet<long>(api, "firestartersound"));
                return false;
            }

            IPlayer byPlayer = (byEntity as EntityPlayer)?.Player;
            if (!byEntity.World.Claims.TryAccess(byPlayer, blockSel.Position, EnumBlockAccessFlags.Use))
            {
                api.Event.UnregisterCallback(ObjectCacheUtil.TryGet<long>(api, "firestartersound"));
                return false;
            }
            Block block = byEntity.World.BlockAccessor.GetBlock(blockSel.Position);
            EnumIgniteState igniteState = EnumIgniteState.NotIgnitable;
            if (block is IIgnitable ign) igniteState = ign.OnTryIgniteBlock(byEntity, blockSel.Position, secondsUsed);

            if (igniteState == EnumIgniteState.NotIgnitable || igniteState == EnumIgniteState.NotIgnitablePreventDefault)
            {
                api.Event.UnregisterCallback(ObjectCacheUtil.TryGet<long>(api, "firestartersound"));
                return false;
            }

            if (byEntity.World is IClientWorldAccessor)
            {
                if (secondsUsed > 0.05f && (int)(30 * secondsUsed) % 2 == 1)
                {
                    Vec3d pos = blockSel.Position.ToVec3d().Add(blockSel.HitPosition);

                    Block blockFire = byEntity.World.GetBlock(new AssetLocation("fire"));

                    AdvancedParticleProperties props = blockFire.ParticleProperties[blockFire.ParticleProperties.Length - 1].Clone();
                    props.basePos = pos;
                    props.Quantity.avg = 0.3f;
                    props.Size.avg = 0.03f;

                    byEntity.World.SpawnParticles(props, byPlayer);

                    props.Quantity.avg = 0;
                }
            }

            return igniteState == EnumIgniteState.Ignitable;
        }

        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {

            if (blockSel == null) return;
            if (api.World.Side == EnumAppSide.Client) return;
            if (api.World.Rand.NextDouble() > 0.25) return;

            Block block = byEntity.World.BlockAccessor.GetBlock(blockSel.Position);

            EnumIgniteState igniteState = EnumIgniteState.NotIgnitable;
            var ign = block as IIgnitable;
            if (ign != null) igniteState = ign.OnTryIgniteBlock(byEntity, blockSel.Position, secondsUsed);

            if (igniteState != EnumIgniteState.IgniteNow)
            {
                api.Event.UnregisterCallback(ObjectCacheUtil.TryGet<long>(api, "firestartersound"));
                return;
            }

            IPlayer byPlayer = (byEntity as EntityPlayer)?.Player;
            if (!byEntity.World.Claims.TryAccess(byPlayer, blockSel.Position, EnumBlockAccessFlags.Use))
            {
                return;
            }

            EnumHandling handled = EnumHandling.PassThrough;
            ign.OnTryIgniteBlockOver(byEntity, blockSel.Position, secondsUsed, ref handled);
        }


        public override bool OnHeldInteractCancel(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumItemUseCancelReason cancelReason)
        {
            byEntity.AnimManager.StopAnimation(igniteAnimation);
            api.Event.UnregisterCallback(ObjectCacheUtil.TryGet<long>(api, "firestartersound"));
            return true;
        }

        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            return new WorldInteraction[]
            {
                new WorldInteraction
                {
                    HotKeyCode = "shift",
                    ActionLangCode = "heldhelp-igniteblock",
                    MouseButton = EnumMouseButton.Right
                }
            };
        }

        public override void OnAttackingWith(IWorldAccessor world, Entity byEntity, Entity attackedEntity, ItemSlot itemslot)
        {
            base.OnAttackingWith(world, byEntity, attackedEntity, itemslot);


            if (attackedEntity != null && byEntity.World.Side == EnumAppSide.Server && api.World.Rand.NextDouble() < 0.5)
            {
                attackedEntity.Ignite();
            }
        }
    }
}