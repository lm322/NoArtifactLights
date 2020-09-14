using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using NativeUI;
using NLog;
using NoArtifactLights.Resources;

namespace NoArtifactLights.Engine.Mod.Controller
{
    internal static class AmmuController
    {
        private static NLog.Logger logger = LogManager.GetLogger("AmmuController");
        private static Vector3[] ammus = { new Vector3(18.18945f, -1120.384f, 28.91654f), new Vector3(-325.6184f, 6072.246f, 31.21228f) };
        internal static UIMenuItem GenerateWeaponSellerItem(string displayName, string description, int price)
        {
            logger.Trace("Creating weapon sell item for: " + displayName + " at price " + price);
            UIMenuItem result = new UIMenuItem(displayName, description);
            result.SetRightLabel("$" + price);
            logger.Trace("Created weapon sell item for: " + displayName);
            return result;
        }

        internal static bool DistanceToAmmu()
        {
            foreach(Vector3 ammu in ammus)
            {
                if (Game.Player.Character.Position.DistanceTo(ammu) < 7f) return true;
                else continue;
            }
            return false;
        }

        internal static void SellWeapon(int price, int ammo, WeaponHash weapon)
        {
            if (!Common.Cost(price)) return;
            try
            {
                if (Game.Player.Character.Weapons.HasWeapon(weapon))
                {
                    Game.Player.Character.Weapons[weapon].Ammo += ammo;
                }
                else
                {
                    Game.Player.Character.Weapons.Give(weapon, ammo, true, true);
                }
            }
            catch
            {
                
            }

        }

        internal static void SellArmor(int amount, int price)
        {
            if (!Common.Cost(amount)) return;
            if(Game.Player.Character.Armor >= amount)
            {
                Screen.ShowSubtitle(Strings.BodyArmorAlreadyHad);
                return;
            }
            Game.Player.Character.Armor = amount;
        }
    }
}
