using GTA;
using HotWorkshop.Flate.Ditno;

namespace NoArtifactLights.Engine.Mod.Controller
{
	// Yields!
	internal class DitnoController : DitnoHandler
	{
		public override void Execute()
		{
			Script.Yield();
			base.Execute();
		}
	}
}
