// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using System;
using System.Collections.Generic;
using GTA;
using NLog;
using NoArtifactLights.Engine.Entities.Structures;

namespace NoArtifactLights.Engine.Mod.Controller
{
	internal class WeaponSavingController
	{
		private static readonly Logger logger = LogManager.GetLogger("WeaponSavingController");

		List<SaveWeapon> savedWeapons = new List<SaveWeapon>();

		internal void RegisterWeapon(WeaponHash hash)
		{
			// does nothing if no weapon found in inventory
			if(!Game.Player.Character.Weapons.HasWeapon(hash))
			{
				logger.Warn($"Weapon \"{hash}\" was not found in player's inventory, aborting");
				return;
			}

			Weapon wp = Game.Player.Character.Weapons[hash];
			if(wp is null)
			{
				logger.Warn($"Weapon \"{hash}\" query returned null, aborting");
				return;
			}

			// Must be like this
			bool removal = false;
			int removalIndex = -1;

			foreach(SaveWeapon sw in savedWeapons)
			{
				if(sw.Weapon == hash)
				{
					removal = true;
					removalIndex = savedWeapons.IndexOf(sw);
				}
			}

			if (removal)
			{
				logger.Warn("Removing " + removalIndex + "as it was found already exists");
				savedWeapons.RemoveAt(removalIndex);
			}

			SaveWeapon result = new SaveWeapon(hash, wp.Ammo + wp.AmmoInClip, true);
			savedWeapons.Add(result);
		}

		internal void GiveAndRegisterWeapon(WeaponHash hash, int ammo, bool equipNow)
		{
			Game.Player.Character.Weapons.Give(hash, ammo, equipNow, true);
			RegisterWeapon(hash);
		}

		internal void GiveBackWeapon(SaveWeapon weapon)
		{
			if (weapon.Existence == false) return;
			if (weapon.Ammo == 0) return;

			logger.Info("Asked to give back weapon " + weapon.Weapon.ToString());
			GiveAndRegisterWeapon(weapon.Weapon, weapon.Ammo, true);
		}

		internal SaveWeapon[] GetSerializationWeapons()
		{
			if(savedWeapons.Count == 0)
			{
				logger.Info("No weapons, returning empty array");
				return Array.Empty<SaveWeapon>();
			}
			return savedWeapons.ToArray();
		}

		internal void FromSerializationWeapons(SaveWeapon[] weapons)
		{
			if (weapons == null) return;
			if (weapons.Length == 0) return;

			savedWeapons.Clear();
			Game.Player.Character.Weapons.RemoveAll();

			foreach(SaveWeapon weapon in weapons)
			{
				GiveBackWeapon(weapon);
			}
		}

		internal void UpdateWeapons()
		{
			if (savedWeapons.Count == 0) return;

			SaveWeapon[] temp = savedWeapons.ToArray();
			savedWeapons.Clear();
			foreach(SaveWeapon sw in temp)
			{
				WeaponHash wh = sw.Weapon;
				Weapon wp = Game.Player.Character.Weapons[wh];
				if (wp == null) continue;
				SaveWeapon result = new SaveWeapon(wh, wp.Ammo + wp.AmmoInClip, true);
			}
		}
	}
}
