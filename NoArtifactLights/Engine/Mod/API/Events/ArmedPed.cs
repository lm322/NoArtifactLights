// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using GTA;
using NLog;
using NoArtifactLights.Engine.Mod.Controller;

namespace NoArtifactLights.Engine.Mod.API.Events
{
	public class ArmedPed : Event
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private Ped p => EventPed;

		public override void Initialize()
		{
			// checks whether it should be equipped with weapon
			if (p == null || !p.Exists() || Entry.weaponedPeds.IsDuplicate(p))
			{
				return;
			}
			logger.Trace("Equipping weapon");
			p.EquipWeapon();
			Blip b = p.AddBlip();
			b.IsFriendly = false;
			b.Sprite = BlipSprite.Enemy;
			b.Scale = 0.5f;
			b.Color = BlipColor.Red;
			GameController.AddWeaponedPed(p);
		}
	}
}
