//using GTA;
//using NoArtifactLights.Serialize;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NoArtifactLights.Managers
//{
//    public static class InventoryManager
//    {
//        internal static List<SaveWeapon> weapons = new List<SaveWeapon>();

//        public static void GiveNewWeapon(WeaponHash weapon, int ammo)
//        {
//            if (Game.Player.Character.Weapons.HasWeapon(weapon))
//            {
//                return;
//            }
//            SaveWeapon sv = new SaveWeapon(ammo, true, weapon);
//        }
//    }
//}
