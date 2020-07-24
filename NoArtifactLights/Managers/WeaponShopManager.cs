using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using NativeUI;
using NoArtifactLights.Resources;

namespace NoArtifactLights.Managers
{
    internal static class WeaponShopManager
    {
        private static Logger logger = Common.logger;
        private static Vector3[] ammus = { new Vector3(18.18945f, -1120.384f, 28.91654f), new Vector3(-325.6184f, 6072.246f, 31.21228f) };
        internal static UIMenuItem GenerateWeaponSellerItem(string displayName, string description, int price)
        {
            logger.Log("Creating weapon sell item for: " + displayName + " at price " + price, "WeaponShopManager");
            UIMenuItem result = new UIMenuItem(displayName, description);
            result.SetRightLabel("$" + price);
            logger.Log("Created weapon sell item for: " + displayName);
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
            if (Common.cash < price)
            {
                Screen.ShowSubtitle(Strings.BuyNoMoney);
                return;
            }
            Common.cash -= price;
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
            if (Common.cash < price)
            {
                Screen.ShowSubtitle(Strings.BuyNoMoney);
                return;
            }
            Common.cash -= price;
            if(Game.Player.Character.Armor >= amount)
            {
                Screen.ShowSubtitle(Strings.BodyArmorAlreadyHad);
                return;
            }
            Game.Player.Character.Armor = amount;
        }
    }
}
