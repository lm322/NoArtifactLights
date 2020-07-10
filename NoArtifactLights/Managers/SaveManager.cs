using GTA;
using GTA.Native;
using NoArtifactLights.Resources;
using NoArtifactLights.Serialize;
using System;
using System.IO;
using System.Xml.Serialization;

namespace NoArtifactLights.Managers
{
    internal static class SaveManager
    {
        internal static void CheckAndFixDataFolder()
        {
            if (!Directory.Exists("NAL")) Directory.CreateDirectory("NAL");
            if (File.Exists("NALSave.xml")) File.Move("NALSave.xml", "NAL\\Save.xml");
        }

        internal static void Load()
        {
            SaveFile sf;
            if (!File.Exists("NALSave.xml"))
            {
                UI.Notify(Strings.NoSave);
                return;
            }
            FileStream fs = File.OpenRead("NALSave.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(SaveFile));
            sf = (SaveFile)serializer.Deserialize(fs);
            fs.Close();
            fs.Dispose();
            if (sf.Version != 2)
            {
                UI.Notify("");
                return;
            }
            World.Weather = sf.Status.CurrentWeather;
            World.CurrentDayTime = new TimeSpan(sf.Status.Hour, sf.Status.Minute, 0);
            World.SetBlackout(sf.Blackout);
            Common.blackout = sf.Blackout;
            Game.Player.Character.Position = new GTA.Math.Vector3(sf.PlayerX, sf.PlayerY, sf.PlayerZ);
            Common.counter = sf.Kills;
            Common.cash = sf.Cash;
            Common.difficulty = sf.CurrentDifficulty;
            GameContentManager.SetRelationship(sf.CurrentDifficulty);
            Game.Player.Character.Weapons.RemoveAll();
            Game.Player.Character.Weapons.Give(WeaponHash.Flashlight, 1, false, true);
            if(sf.PlayerHealth > 0)
            {
                Game.Player.Character.Health = sf.PlayerHealth;
            }
            Game.Player.Character.Armor = sf.PlayerArmor;
            if (sf.Pistol.Existence)
            {
                Game.Player.Character.Weapons.Give(WeaponHash.Pistol, sf.Pistol.Ammo, true, true);
            }
            if (sf.PumpShotgun.Existence)
            {
                Game.Player.Character.Weapons.Give(WeaponHash.PumpShotgun, sf.PumpShotgun.Ammo, true, true);
            }
        }

        internal static void Save(bool blackout)
        {
            SaveFile sf = new SaveFile();
            sf.Version = 2;
            sf.Status = new WorldStatus(World.Weather, World.CurrentDayTime.Hours, World.CurrentDayTime.Minutes);
            sf.PlayerX = Game.Player.Character.Position.X;
            sf.PlayerY = Game.Player.Character.Position.Y;
            sf.PlayerZ = Game.Player.Character.Position.Z;
            sf.Blackout = blackout;
            sf.Kills = Common.counter;
            sf.CurrentDifficulty = Common.difficulty;
            sf.Cash = Common.cash;
            sf.PlayerHealth = Game.Player.Character.Health;
            sf.PlayerArmor = Game.Player.Character.Armor;
            if (Game.Player.Character.Weapons.HasWeapon(WeaponHash.Pistol))
            {
                sf.Pistol = new SaveWeapon(Game.Player.Character.Weapons[WeaponHash.Pistol].Ammo + Game.Player.Character.Weapons.Current.AmmoInClip, true);
            }
            else
            {
                sf.Pistol = new SaveWeapon(0, false);
            }
            if (Game.Player.Character.Weapons.HasWeapon(WeaponHash.PumpShotgun))
            {
                sf.PumpShotgun = new SaveWeapon(Game.Player.Character.Weapons[WeaponHash.PumpShotgun].Ammo + Game.Player.Character.Weapons.Current.AmmoInClip, true);
            }
            else
            {
                sf.PumpShotgun = new SaveWeapon(0, false);
            }
            FileStream fs = File.Create("NALSave.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(SaveFile));
            serializer.Serialize(fs, sf);
            UI.Notify(Strings.GameSaved);
            fs.Close();
            fs.Dispose();
        }
    }
}
