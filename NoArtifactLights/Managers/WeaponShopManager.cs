using GTA;
using GTA.Native;
using NativeUI;
using NoArtifactLights.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace NoArtifactLights.Managers
{
    internal static class WeaponShopManager
    {
        internal static UIMenuItem GenerateWeaponSellerItem(string displayName, string description, int price)
        {
            UIMenuItem result = new UIMenuItem(displayName, description);
            result.SetRightLabel("$" + price);
            return result;
        }

        internal static void SellWeapon(int price, int ammo, WeaponHash weapon)
        {
            if (Common.cash < price)
            {
                UI.ShowSubtitle(Strings.BuyNoMoney);
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
                UI.ShowSubtitle(Strings.BuyNoMoney);
                return;
            }
            Common.cash -= price;
            if(Game.Player.Character.Armor >= amount)
            {
                UI.ShowSubtitle(Strings.BodyArmorAlreadyHad);
                return;
            }
            Game.Player.Character.Armor = amount;
        }
    }
}
