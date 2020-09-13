using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.UI;
using GTA.Math;
using GTA.Native;
using NoArtifactLights.Managers;
using NLog;
using NoArtifactLights.Events;

namespace NoArtifactLights.Engine.Process
{
	internal static class Initializer
	{
		private static bool isLoaded = false;
		private static bool isLoading = false;

		private static Logger logger = LogManager.GetLogger("Initializer");

		internal static void LoadProgram()
		{
			if (!LoadInternal()) throw new InvalidProgramException();
			
		}

		private static bool LoadInternal()
		{
			if (isLoading || isLoaded) return false;
			isLoading = true;

			// START INITIAL LOADING PROCESS

			logger.Trace("Loading multiplayer maps");
			Function.Call(Hash._LOAD_MP_DLC_MAPS);
			Function.Call(Hash._USE_FREEMODE_MAP_BEHAVIOR, true);
			logger.Trace("Setting player position and giving weapons");
			Game.Player.Character.Position = new Vector3(459.8501f, -1001.404f, 24.91487f);
			logger.Trace("Setting relationship and game settings");
			GameContentManager.SetRelationship(Difficulty.Initial);
			Game.MaxWantedLevel = 0;
			Game.Player.IgnoredByPolice = true;
			Game.Player.ChangeModel("a_m_m_bevhills_02");
			Game.Player.Character.Weapons.Give(WeaponHash.Flashlight, 1, true, true);
			Game.Player.Character.Weapons.Give(WeaponHash.Pistol, 50, false, false);
			Screen.FadeIn(1000);
			EventManager.RegisterEvent(typeof(ArmedPed));
			EventManager.RegisterEvent(typeof(StealCar));

			// END INITIAL LOADING PROCESS

			isLoaded = true;
			isLoading = false;
			return true;
		}
	}
}
