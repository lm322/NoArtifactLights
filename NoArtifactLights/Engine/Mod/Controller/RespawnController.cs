using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;
using GTA.UI;
using NLog;

namespace NoArtifactLights.Engine.Mod.Controller
{
	internal static class RespawnController
	{
		private static int callMarker;
		private static bool fitRespawn;
		private static bool proceeding;

		private static readonly Logger logger = LogManager.GetLogger("RespawnController");

		internal static bool IsPlayerJustFitRespawn()
		{
			int deathTime = Function.Call<int>(Hash.GET_TIME_SINCE_LAST_DEATH);
			int arrestTime = Function.Call<int>(Hash.GET_TIME_SINCE_LAST_ARREST);
			return deathTime == 0 || arrestTime == 0;
		}

		internal static void ResetCallMarker()
		{
			callMarker = Game.GameTime;
		}

		internal static int GetMsPassed()
		{
			return callMarker == 0 ? 0 : Game.GameTime - callMarker;
		}

		internal static void Loop()
		{
			if(!fitRespawn && IsPlayerJustFitRespawn())
			{
				fitRespawn = true;
				logger.Info("Player is respawning");
			}
			if(fitRespawn && !proceeding)
			{
				ResetCallMarker();
				proceeding = true;
			}
			if(proceeding)
			{
				if (GetMsPassed() != 7000) return;
				logger.Info("Proceeding to respawn!");
				if (Screen.IsFadedIn) Screen.FadeIn(500);
				proceeding = false;
				fitRespawn = false;
			}
		}
	}
}
