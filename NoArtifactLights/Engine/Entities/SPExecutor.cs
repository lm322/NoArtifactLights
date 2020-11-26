using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandPlus.Commanding;
using GTA.UI;
using NoArtifactLights.Engine.Entities.Enums;
using NoArtifactLights.Engine.Mod.API;
using NoArtifactLights.Resources;

namespace NoArtifactLights.Engine.Entities
{
	internal class SPExecutor : IExecutor
	{
		public Permission Permission { get; internal set; }

		public void Run(CommandControl ctrl, string command, Permission permission)
		{
			if ((int)Permission < (int)permission)
			{
				GameUI.DisplayHelp(Strings.NoPermission);
				return;
			}

			try
			{
				ctrl.ParseAndRun(command);
			}
			catch (Exception ex)
			{
				Notification.Show(ex.ToString());
			}
		}
	}
}
