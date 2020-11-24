using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandPlus.Commanding;
using GTA.UI;
using NoArtifactLights.Engine.Mod.API;
using NoArtifactLights.Engine.Mod.Commands;
using NoArtifactLights.Resources;

namespace NoArtifactLights.Engine.Mod.Controller
{
	internal static class CommandController
	{
		private static CommandControl control = new CommandControl();

		#region Command Definitions

		private static SetHungryCommand setHungry = new SetHungryCommand();

		#endregion

		internal static void Init()
		{
			control.Register("sethungry", setHungry);
		}

		internal static void Run(string str)
		{
			try
			{
				control.ParseAndRun(str);
			}
			catch(Exception ex)
			{
				GameUI.DisplayHelp(Strings.CommandError);
				Notification.Show(ex.Message);
			}
		}
	}
}
