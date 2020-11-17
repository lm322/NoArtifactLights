using GTA;
using NoArtifactLights.Engine.Mod.Controller;

namespace NoArtifactLights.Engine.Mod.Scripts
{
	internal class DitnoScript : Script
	{
		DitnoController ditno;

		public DitnoScript()
		{
			ditno = new DitnoController();
			ditno.Interval = 100;
			this.Interval = 100;
			Tick += DitnoScript_Tick;
		}

		private void DitnoScript_Tick(object sender, System.EventArgs e)
		{
			ditno.Execute();
		}
	}
}
