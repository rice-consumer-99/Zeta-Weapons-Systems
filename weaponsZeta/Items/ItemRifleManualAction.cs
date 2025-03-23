using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

/*
ToDoList


*/
namespace weaponsZeta.Items
{
    internal class ItemRifleManualAction : Item
    {
       WorldInteraction[] interactions;

        string aimAnimation;
        private static bool cocked = false;
        private static int magazine = 11;
        private static int magazinemax = 11;
        private static string lastcasematerial;
        private static SimpleParticleProperties? _smokeParticles;

        public override void OnLoaded(ICoreAPI api)
        {
            //smokeandpowder smoke code
            _smokeParticles = new SimpleParticleProperties(Attributes["smokePNumMin"].AsFloat(9f), Attributes["smokePNumMax"].AsFloat(14f),
            ColorUtil.ToRgba(Attributes["smokePColorR"].AsInt(190), Attributes["smokePColorG"].AsInt(140), Attributes["smokePColorB"].AsInt(140),
            Attributes["smokePColorA"].AsInt(70)), new Vec3d(Attributes["smokePPosMinX"].AsFloat(-0.4f), Attributes["smokePPosMinY"].AsFloat(-0.4f),
            Attributes["smokePPosMinZ"].AsFloat(-0.4f)), new Vec3d(Attributes["smokePPosMaxX"].AsFloat(0.4f), Attributes["smokePPosMaxY"].AsFloat(0.4f),
            Attributes["smokePPosMaxZ"].AsFloat(0.4f)), new Vec3f(Attributes["smokePVelMinX"].AsFloat(-0.125f), Attributes["smokePVelMinY"].AsFloat(0.01f),
            Attributes["smokePVelMinZ"].AsFloat(-0.125f)), new Vec3f(Attributes["smokePVelMaxX"].AsFloat(0.125f), Attributes["smokePVelMaxY"].AsFloat(0.3f),
            Attributes["smokePVelMaxZ"].AsFloat(0.125f)), Attributes["smokePLifeTime"].AsFloat(2f), Attributes["smokePGrav"].AsFloat(-0.008f),
            Attributes["smokePSizeMin"].AsFloat(1.0f), Attributes["smokePSizeMax"].AsFloat(1.9f), EnumParticleModel.Quad)
            {
                SizeEvolve = new EvolvingNatFloat(EnumTransformFunction.LINEAR, Attributes["smokePSizeEvolve"].AsFloat(-0.25f)),
                SelfPropelled = true,
                WindAffectednes = Attributes["smokePWindFactor"].AsFloat(0.7f),
                OpacityEvolve = new EvolvingNatFloat(EnumTransformFunction.LINEAR, Attributes["smokePOpacityEvolve"].AsFloat(-0.25f))
            };


            //aimAnimation = Attributes["aimAnimation"].AsString();
            magazinemax = Attributes["magazine"].AsInt();

            if (api.Side != EnumAppSide.Client) return;
            ICoreClientAPI capi = api as ICoreClientAPI;
            interactions = ObjectCacheUtil.GetOrCreate(api, "bowInteractions", () =>
            {
                List<ItemStack> stacks = new List<ItemStack>();

                foreach (CollectibleObject obj in api.World.Collectibles)
                {
                    if (obj.Code.PathStartsWith("cartridge-"))
                    {
                        stacks.Add(new ItemStack(obj));
                    }
                }
                return new WorldInteraction[]
                {
                    new WorldInteraction()
                    {
                        ActionLangCode = "heldhelp-chargebow",
                        MouseButton = EnumMouseButton.Right,
                        HotKeyCode = "dropitems",
                        Itemstacks = stacks.ToArray()
                    }
                };
            });
        }
        public override string GetHeldTpUseAnimation(ItemSlot activeHotbarSlot, Entity byEntity)
        {
            return null;
        }
        protected ItemSlot GetNextArrow(EntityAgent byEntity)
        {
            ItemSlot slot = null;
            byEntity.WalkInventory((invslot) =>
            {
                if (invslot is ItemSlotCreative) return true;
                ItemStack stack = invslot.Itemstack;
                byEntity.Api.Logger.Event(stack + "");
                if (stack != null && stack.Collectible != null && stack.Collectible.Code.PathStartsWith("cartridge-") && stack.StackSize > 0)
                {
                    slot = invslot;
                    lastcasematerial = "weaponszeta:cartridgecase-" + stack.Collectible.Variant["material"];
                    return false;
                }
                return true;
            });
            return slot;
        }
        public override void OnHeldAttackStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, ref EnumHandHandling handling)
        {
            base.OnHeldAttackStart(slot, byEntity, blockSel, entitySel, ref handling);
            if (handling == EnumHandHandling.PreventDefault) return;

            //byEntity.Api.Logger.Event(cocked+" 1");

            //slot.Itemstack.Attributes.SetInt("renderVariant", 1);

            // Not ideal to code the aiming controls this way. Needs an elegant solution - maybe an event bus?
            //byEntity.Attributes.SetInt("aiming", 1);
           // byEntity.Attributes.SetInt("aimingCancel", 0);
            //byEntity.AnimManager.StartAnimation(aimAnimation);

            handling = EnumHandHandling.PreventDefault;
        }

        /*
        public override bool OnHeldAttackStep(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel)
        {
            byEntity.Api.Logger.Event("2");
            return base.OnHeldAttackStep(secondsPassed, slot, byEntity, blockSelection, entitySel);
        }
        */

        public override bool OnHeldAttackCancel(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel, EnumItemUseCancelReason cancelReason)
        {
           // byEntity.Attributes.SetInt("aiming", 0);
            //byEntity.AnimManager.StopAnimation(aimAnimation);
            /*
            if (byEntity.World is IClientWorldAccessor)
            {
                slot.Itemstack?.TempAttributes.RemoveAttribute("renderVariant");
            }
            slot.Itemstack?.Attributes.SetInt("renderVariant", 0);
            if (cancelReason != EnumItemUseCancelReason.Destroyed) (byEntity as EntityPlayer)?.Player?.InventoryManager.BroadcastHotbarSlot();
            if (cancelReason != EnumItemUseCancelReason.ReleasedMouse)
            {
                byEntity.Attributes.SetInt("aimingCancel", 1);
            }
            */
            return base.OnHeldAttackCancel(secondsPassed, slot, byEntity, blockSelection, entitySel, cancelReason);
        }

        public override void OnHeldAttackStop(float secondsPassed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSelection, EntitySelection entitySel)
        {
            base.OnHeldAttackStop(secondsPassed, slot, byEntity, blockSelection, entitySel);
            //if (byEntity.Attributes.GetInt("aimingCancel") == 1) return;
            //byEntity.Attributes.SetInt("aiming", 0);
            //byEntity.AnimManager.StopAnimation(aimAnimation);

            if (byEntity.World.Side == EnumAppSide.Client)
            {
                slot.Itemstack.TempAttributes.RemoveAttribute("renderVariant");
                //byEntity.AnimManager.StartAnimation("bowhit");
                return;
            }

            slot.Itemstack.Attributes.SetInt("renderVariant", 0);
            (byEntity as EntityPlayer)?.Player?.InventoryManager.BroadcastHotbarSlot();

            if (cocked == false) {
                return;
            }
            else
            {
                cocked = false;
                ItemSlot arrowSlot = GetNextArrow(byEntity);
                if (arrowSlot == null) return;

                float damage = 0;
                // Bow damage
                if (slot.Itemstack.Collectible.Attributes != null)
                {
                    damage += slot.Itemstack.Collectible.Attributes["damage"].AsFloat(0);
                }
                // Arrow damage
                if (arrowSlot.Itemstack.Collectible.Attributes != null)
                {
                    damage += arrowSlot.Itemstack.Collectible.Attributes["damage"].AsFloat(0);
                }

                ItemStack stack = arrowSlot.TakeOut(0);
                arrowSlot.MarkDirty();

                float breakChance = 0.5f;
                if (stack.ItemAttributes != null) breakChance = stack.ItemAttributes["breakChanceOnImpact"].AsFloat(0.5f);

                //Saves what the case is made of

                byEntity.Api.Logger.Event(lastcasematerial);
                //gets material of bullet
                String bulletType = ((stack.ItemAttributes["arrowEntityCode"].AsString("bulletfired-" + stack.Collectible.Variant["materialbullet"])) + stack.Collectible.Variant["materialbullet"]);
                EntityProperties type = byEntity.World.GetEntityType(new AssetLocation(bulletType));
                var entityarrow = byEntity.World.ClassRegistry.CreateEntity(type) as EntityProjectile;

                entityarrow.FiredBy = byEntity;
                entityarrow.Damage = damage;
                entityarrow.DamageTier = Attributes["damageTier"].AsInt(0);
                entityarrow.ProjectileStack = stack;
                entityarrow.DropOnImpactChance = 1 - breakChance;

                //float acc = Math.Max(0.001f, 1 - byEntity.Attributes.GetFloat("aimingAccuracy", 0));
                //double rndpitch = byEntity.WatchedAttributes.GetDouble("aimingRandPitch", 1) * acc * 0.1f;
                //double rndyaw = byEntity.WatchedAttributes.GetDouble("aimingRandYaw", 1) * acc * 0.1f;

                Vec3d pos = byEntity.ServerPos.XYZ.Add(0, byEntity.LocalEyePos.Y, 0);
                //Vec3d aheadPos = pos.AheadCopy(1, byEntity.SidedPos.Pitch + rndpitch, byEntity.SidedPos.Yaw + rndyaw);
                Vec3d aheadPos = pos.AheadCopy(1, byEntity.SidedPos.Pitch, byEntity.SidedPos.Yaw);
                Vec3d velocity = (aheadPos - pos) * 5;

                entityarrow.ServerPos.SetPosWithDimension(byEntity.SidedPos.BehindCopy(0.21).XYZ.Add(0, byEntity.LocalEyePos.Y, 0));
                entityarrow.ServerPos.Motion.Set(velocity);
                entityarrow.Pos.SetFrom(entityarrow.ServerPos);
                entityarrow.World = byEntity.World;
                entityarrow.SetRotation();

                byEntity.World.SpawnEntity(entityarrow);

                if (_smokeParticles != null)
                {
                    _smokeParticles.MinPos = byEntity.SidedPos.AheadCopy(Attributes["smokePVelFOffset"].AsDouble(2.0)).XYZ.Add(0.0, byEntity.LocalEyePos.Y, 0.0);
                    byEntity.World.SpawnParticles(_smokeParticles);
                }

                IPlayer byPlayer = null;
                if (byEntity is EntityPlayer) byPlayer = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);
                byEntity.World.PlaySoundAt(AssetLocation.Create(Attributes["soundRifleFire"].AsString(), Code.Domain), byEntity, null, false, 8);

                slot.Itemstack.Collectible.DamageItem(byEntity.World, byEntity, slot);
                slot.MarkDirty();
            }
        }

        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            base.OnHeldInteractStart(slot, byEntity, blockSel, entitySel, firstEvent, ref handling);
            if (cocked == false){
                ItemSlot invslot = GetNextArrow(byEntity);
                if (invslot == null) return;
                ItemStack stack = invslot.TakeOut(1);
                magazine++;
            }
            handling = EnumHandHandling.PreventDefault;
        }
        public override bool OnHeldInteractStep(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            return secondsUsed < 0.55;
        }

        public override bool OnHeldInteractCancel(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, EnumItemUseCancelReason cancelReason)
        {
            if (cocked == false & secondsUsed <= 0.55)
            {
                ItemSlot invslot = GetNextArrow(byEntity);
                if (invslot == null) return false;
                ItemStack stack = invslot.TakeOut(-1);
            }
            return base.OnHeldInteractCancel(secondsUsed, slot, byEntity, blockSel, entitySel, cancelReason);

        }
        public override void OnHeldInteractStop(float secondsUsed, ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel)
        {
            base.OnHeldInteractStop(secondsUsed, slot, byEntity, blockSel, entitySel);
            if (secondsUsed > 0.55 & cocked == false)
            {
                IPlayer byPlayer = null;
                if (byEntity is EntityPlayer) byPlayer = byEntity.World.PlayerByUid(((EntityPlayer)byEntity).PlayerUID);
                if (magazine == 0) 
                {
                    byEntity.World.PlaySoundAt(AssetLocation.Create(Attributes["soundRifleCock0"].AsString(""), Code.Domain), byEntity, null, false, 8);
                }
                else
                {
                    byEntity.World.PlaySoundAt(AssetLocation.Create(Attributes["soundRifleCock1"].AsString(""), Code.Domain), byEntity, null, false, 8);
                    ItemStack returnedcase = new ItemStack(byEntity.World.GetItem(new AssetLocation(lastcasematerial)));
                    byEntity.World.SpawnItemEntity(returnedcase, byPlayer.Entity.Pos.XYZ.Add(0, 1, 0));
                }
                cocked = true;
                return;
            }
            else
            return;
        }

        public override void GetHeldItemInfo(ItemSlot inSlot, StringBuilder dsc, IWorldAccessor world, bool withDebugInfo)
        {
            base.GetHeldItemInfo(inSlot, dsc, world, withDebugInfo);

            if (inSlot.Itemstack.Collectible.Attributes == null) return;

            float dmg = inSlot.Itemstack.Collectible.Attributes?["damage"].AsFloat(0) ?? 0;
            if (dmg != 0) dsc.AppendLine(Lang.Get("bow-piercingdamage", dmg));

            float accuracyBonus = inSlot.Itemstack.Collectible?.Attributes["statModifier"]["rangedWeaponsAcc"].AsFloat(0) ?? 0;
            if (accuracyBonus != 0) dsc.AppendLine(Lang.Get("bow-accuracybonus", accuracyBonus > 0 ? "+" : "", (int)(100*accuracyBonus)));
        }
        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            return interactions.Append(base.GetHeldInteractionHelp(inSlot));
        }
    }
}