// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using GTA;
using NativeUI;
using System;

namespace NoArtifactLights.Missions
{
	[Obsolete]
	internal abstract class Mission
	{
		public virtual void UnallocateFromMission(Entity entity)
		{
			if(entity != null && entity.Exists())
			{
				entity.MarkAsNoLongerNeeded();
			}
		}
		public virtual void MissionPassed()
		{
			BigMessageThread.MessageInstance.ShowSimpleShard("Mission Complete", "");
		}

		public abstract void OnStart();
		public abstract void Process();
	}
}
