// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using GTA;
using GTA.Math;
using GTA.UI;
using NLog;
using NoArtifactLights.Cilent;

namespace NAL
{
	public class Cmds : Script
	{
		private static readonly Logger logger = LogManager.GetLogger("Cmds");
		internal static NALClient client;


		public static void ForceEvent()
		{
			if (client == null) return;
			client.SetForceStart();
		}

		public static void PrnCoord()
		{
			Vector3 vec3 = Game.Player.Character.Position;
			logger.Info($"Player position: {vec3.X}, {vec3.Y}, {vec3.Z}");
			Notification.Show("Player coords written to log!");
		}
	}
}
