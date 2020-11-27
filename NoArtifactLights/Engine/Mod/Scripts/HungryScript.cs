using System.Threading;
using GTA;
using NoArtifactLights.Engine.Mod.Controller;

namespace NoArtifactLights.Engine.Mod.Scripts
{
	[ScriptAttributes(Author = "RelaperCrystal", SupportURL = "https://hotworkshop.atlassian.net/projects/NAL")]
	public class HungryScript : Script
	{
		public HungryScript()
		{
			Common.Start += Common_Start;
		}

		private void Common_Start(object sender, System.EventArgs e)
		{
			this.Interval = 1500;
			//Thread.CurrentThread.Name = "Hungry Worker Thread";
			this.Tick += HungryScript_Tick;
		}

		private void HungryScript_Tick(object sender, System.EventArgs e)
		{
			HungryController.Loop();
			HungryController.WaterLoop();
		}
	}
}
