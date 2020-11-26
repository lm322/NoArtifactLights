// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using GTA;
using GTA.Native;
using GTA.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using NoArtifactLights.Engine.Entities.Structures;
using NoArtifactLights.Resources;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace NoArtifactLights.Engine.Mod.Controller
{
	internal static class SaveController
	{
		private static Logger logger = LogManager.GetLogger("SaveController");
		private const string savePath = "NAL\\game.dat";
		private const int saveVersion = 5;
		private const int saveLastVersion = 4;

		internal static void CheckAndFixDataFolder()
		{
			if (!Directory.Exists("NAL")) Directory.CreateDirectory("NAL");
			if (File.Exists("NALSave.xml")) File.Move("NALSave.xml", "NAL\\Save.xml");
			if (File.Exists("NAL\\Save.xml"))
			{
				logger.Warn("Deprecated save found - will not load it!");
				Notification.Show(Strings.DeprecatedXMLSave);
			}
		}

		internal static void SaveGameFile(SaveFile sf)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			try
			{
				DefaultContractResolver contractResolver = new DefaultContractResolver
				{
					NamingStrategy = new SnakeCaseNamingStrategy()
				};

				string json = JsonConvert.SerializeObject(sf, new JsonSerializerSettings
				{
					ContractResolver = contractResolver,
					Formatting = Formatting.None
				});

				byte[] data = Encoding.UTF8.GetBytes(json);
				string dat = Convert.ToBase64String(data);

				if(Directory.Exists("NAL\\temp"))
				{
					logger.Info("Cleaning up uncleaned temporary files");
					Directory.Delete("NAL\\temp", true);
				}

				Directory.CreateDirectory("NAL\\temp");
				string tempPath = "NAL\\temp\\raw.dat";
				File.WriteAllText(tempPath, dat);

				if(File.Exists("NAL\\game.dat"))
				{
					logger.Info("Overwritten save file");
					File.Delete("NAL\\game.dat");
				}

				ZipFile.CreateFromDirectory("NAL\\temp", "NAL\\game.dat");
				Directory.Delete("NAL\\temp", true);
			}
			catch (IOException ioex)
			{
				logger.Error("Error while saving game file: \r\n" + ioex.ToString());
				logger.Error("Operation is aborted.");
				Screen.ShowSubtitle("Error");
			}
			sw.Stop();
			logger.Trace("File save cost " + sw.ElapsedMilliseconds + "ms.");
		}

		internal static SaveFile LoadGameFile()
		{
			if(Directory.Exists("NAL\\temp"))
			{
				logger.Info("Overwritten temponary files");
				Directory.Delete("NAL\\temp", true);
			}

			Directory.CreateDirectory("NAL\\temp");
			ZipFile.ExtractToDirectory("NAL\\game.dat", "NAL\\temp");
			string datBase64 = File.ReadAllText("NAL\\temp\\raw.dat");
			byte[] jsonBytes = Convert.FromBase64String(datBase64);
			string json = Encoding.UTF8.GetString(jsonBytes);

			DefaultContractResolver contractResolver = new DefaultContractResolver
			{
				NamingStrategy = new SnakeCaseNamingStrategy()
			};

			SaveFile result = JsonConvert.DeserializeObject<SaveFile>(json, new JsonSerializerSettings
			{
				ContractResolver = contractResolver,
				Formatting = Formatting.None
			});

			Directory.Delete("NAL\\temp", true);
			return result;
		}

		internal static LastSaveFile LoadOldSave()
		{
			if (Directory.Exists("NAL\\temp"))
			{
				logger.Info("Overwritten temponary files");
				Directory.Delete("NAL\\temp", true);
			}

			Directory.CreateDirectory("NAL\\temp");
			ZipFile.ExtractToDirectory("NAL\\game.dat", "NAL\\temp");
			string datBase64 = File.ReadAllText("NAL\\temp\\raw.dat");
			byte[] jsonBytes = Convert.FromBase64String(datBase64);
			string json = Encoding.UTF8.GetString(jsonBytes);

			DefaultContractResolver contractResolver = new DefaultContractResolver
			{
				NamingStrategy = new SnakeCaseNamingStrategy()
			};

			LastSaveFile result = JsonConvert.DeserializeObject<LastSaveFile>(json, new JsonSerializerSettings
			{
				ContractResolver = contractResolver,
				Formatting = Formatting.None
			});

			Directory.Delete("NAL\\temp", true);
			return result;
		}

		internal static void Load()
		{
			CheckAndFixDataFolder();
			SaveFile sf;
			LastSaveFile lsf;
			if (!File.Exists(savePath))
			{
				Notification.Show(Strings.NoSave);
				return;
			}
			sf = LoadGameFile();
			if (sf.Version != saveVersion)
			{
				if(sf.Version == saveLastVersion)
				{
					lsf = LoadOldSave();
					sf = UpdateSaveFile(lsf);
				}
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
			Common.weaponSaving.FromSerializationWeapons(sf.Weapons);
		}

		internal static void Save(bool blackout)
		{
			CheckAndFixDataFolder();
			SaveFile sf = new SaveFile();
			sf.Version = saveVersion;
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
			sf.PlayerHungry = HungryController.Hungry;
			sf.PlayerHydration = HungryController.Water;

			sf.Weapons = Common.weaponSaving.GetSerializationWeapons();

			SaveGameFile(sf);
		}

		internal static SaveFile UpdateSaveFile(LastSaveFile lsf)
		{
			SaveFile result = new SaveFile();
			result.Bank = lsf.Bank;
			result.Cash = lsf.Cash;
			result.Blackout = lsf.Blackout;
			result.CurrentDifficulty = lsf.CurrentDifficulty;
			result.Kills = lsf.Kills;
			result.Model = lsf.Model;
			result.PlayerArmor = lsf.PlayerArmor;
			result.PlayerHealth = lsf.PlayerHealth;
			result.PlayerHungry = 10.0f;
			result.PlayerHydration = 10.0f;
			result.PlayerX = lsf.PlayerX;
			result.PlayerY = lsf.PlayerY;
			result.PlayerZ = lsf.PlayerZ;
			result.Status = lsf.Status;
			result.Version = saveVersion;
			result.Weapons = lsf.Weapons;
			return result;
		}
	}
}
