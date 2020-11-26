using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandPlus.Commanding;
using GTA;
using GTA.UI;
using NoArtifactLights.Engine.Entities.Enums;
using NoArtifactLights.Engine.Mod.API;
using NoArtifactLights.Resources;

namespace NoArtifactLights.Engine.Entities
{
	internal class NALPlayer : IExecutor
	{
		public NALPlayer(Player plyr)
		{
			Player = plyr;
		}

		public Player Player { get; private set; }
		public Permission Permission { get; private set; }

		public void Run(CommandControl control, string command, Permission permission)
		{
			if((int)Permission < (int)permission)
			{
				GameUI.DisplayHelp(Strings.NoPermission);
				return;
			}

			try
			{
				control.ParseAndRun(command);
			}
			catch(Exception ex)
			{
				Notification.Show(ex.ToString());
			}
		}
	}
}
