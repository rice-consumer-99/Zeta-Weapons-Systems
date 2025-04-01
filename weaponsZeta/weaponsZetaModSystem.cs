using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Config;
using Vintagestory.API.Common;
using weaponsZeta.Items;

namespace weaponsZeta;

public class weaponsZetaModSystem : ModSystem
{
    // Called on server and client
    // Useful for registering block/entity classes on both sides
    public override void Start(ICoreAPI api)
    {

        api.RegisterItemClass(Mod.Info.ModID + ".itemRifleManualAction", typeof(ItemRifleManualAction));
        api.RegisterItemClass(Mod.Info.ModID + ".itemCartridge", typeof(ItemCartridge));
        api.RegisterItemClass(Mod.Info.ModID + ".itemHeatAxe",typeof(ItemHeatAxe));
        api.RegisterItemClass(Mod.Info.ModID + ".itemHeatBlade", typeof(ItemHeatBlade));
    }

    public override void StartServerSide(ICoreServerAPI api)
    {

    }

    public override void StartClientSide(ICoreClientAPI api)
    {

    }
}
