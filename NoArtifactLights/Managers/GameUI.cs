using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoArtifactLights.Managers
{
	public static class GameUI
	{

		public static void DisplayHelp(string text)
		{
			Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_HELP, "STRING");
			Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, text);
			Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_HELP, 0, false, true, 5000);
		}
	}
}
