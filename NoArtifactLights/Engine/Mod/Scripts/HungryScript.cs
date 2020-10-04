using GTA;
using NoArtifactLights.Engine.Mod.Controller;

namespace NoArtifactLights.Engine.Mod.Scripts
{
	[ScriptAttributes(Author = "RelaperCrystal", SupportURL = "https://hotworkshop.atlassian.net/projects/NAL")]
	public class HungryScript : Script
	{
		public HungryScript()
		{
			this.Interval = 1500;
			this.Tick += HungryScript_Tick;
		}

		private void HungryScript_Tick(object sender, System.EventArgs e)
		{
			HungryController.Loop();
		}
	}
}
