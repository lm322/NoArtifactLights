// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using GTA;
using GTA.Native;
using GTA.UI;
using NLog;
using NoArtifactLights.Engine.Entities.Structures;
using NoArtifactLights.Resources;
using System;
using System.IO;
using System.Xml.Serialization;

namespace NoArtifactLights.Engine.Mod.Controller
{
	internal static class SaveController
	{
		private static Logger logger = LogManager.GetLogger("SaveManager");
		private const string savePath = "NAL\\Save.xml";

		internal static void CheckAndFixDataFolder()
		{
			if (!Directory.Exists("NAL")) Directory.CreateDirectory("NAL");
			if (File.Exists("NALSave.xml")) File.Move("NALSave.xml", "NAL\\Save.xml");
		}

		internal static void Load()
		{
			CheckAndFixDataFolder();
			SaveFile sf;
			if (!File.Exists(savePath))
			{
				Notification.Show(Strings.NoSave);
				return;
			}
			FileStream fs = File.OpenRead(savePath);
			XmlSerializer serializer = new XmlSerializer(typeof(SaveFile));
			
			sf = (SaveFile)serializer.Deserialize(fs);
			fs.Close();
			fs.Dispose();
			if (sf.Version != 3)
			{
				Notification.Show(Strings.SaveVersion);
				return;
			}
			World.Weather = sf.Status.CurrentWeather;
			World.CurrentTimeOfDay = new TimeSpan(sf.Status.Hour, sf.Status.Minute, 0);
			Function.Call(Hash.SET_ARTIFICIAL_LIGHTS_STATE, sf.Blackout);
			Common.blackout = sf.Blackout;
			Game.Player.Character.Position = new GTA.Math.Vector3(sf.PlayerX, sf.PlayerY, sf.PlayerZ);
			Common.counter = sf.Kills;
			Common.Cash = sf.Cash;
			Common.Bank = sf.Bank;
			Common.difficulty = sf.CurrentDifficulty;
			GameController.SetRelationship(sf.CurrentDifficulty);
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
			CheckAndFixDataFolder();
			SaveFile sf = new SaveFile();
			sf.Version = 3;
			sf.Status = new WorldStatus(World.Weather, World.CurrentTimeOfDay.Hours, World.CurrentTimeOfDay.Minutes);
			sf.PlayerX = Game.Player.Character.Position.X;
			sf.PlayerY = Game.Player.Character.Position.Y;
			sf.PlayerZ = Game.Player.Character.Position.Z;
			sf.Blackout = blackout;
			sf.Kills = Common.counter;
			sf.CurrentDifficulty = Common.difficulty;
			sf.Cash = Common.Cash;
			sf.Bank = Common.Bank;
			sf.PlayerHealth = Game.Player.Character.Health;
			sf.PlayerArmor = Game.Player.Character.Armor;
			sf.Pistol = GetSaveWeapon(WeaponHash.Pistol);
			sf.PumpShotgun = GetSaveWeapon(WeaponHash.PumpShotgun);
			FileStream fs = File.Create(savePath);
			XmlSerializer serializer = new XmlSerializer(typeof(SaveFile));
			serializer.Serialize(fs, sf);
			Notification.Show(Strings.GameSaved);
			fs.Close();
			fs.Dispose();
		}

		private static SaveWeapon GetSaveWeapon(WeaponHash weapon)
		{
			logger.Info("Acquring weapon " + weapon.ToString());
			SaveWeapon result;
			if(Game.Player.Character.Weapons.HasWeapon(weapon))
			{
				Weapon wp = Game.Player.Character.Weapons[weapon];
				result = new SaveWeapon(wp.Ammo + wp.AmmoInClip, true);
			}
			else
			{
				result = new SaveWeapon(0, false);
			}
			return result;
		}
	}
}
