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
        Mod.Logger.Notification("Hello from template mod: " + api.Side);
        Mod.Logger.Notification("Hello from template mod: " + api.Side);
        api.RegisterItemClass(Mod.Info.ModID + ".itemRifleManualAction", typeof(ItemRifleManualAction));
        api.RegisterItemClass(Mod.Info.ModID + ".itemCartridge", typeof(ItemCartridge));
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        Mod.Logger.Notification("Hello from template mod server side: " + Lang.Get("weaponszeta:hello"));
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        Mod.Logger.Notification("Hello from template mod client side: " + Lang.Get("weaponszeta:hello"));
    }
}
