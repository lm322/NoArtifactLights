using GTA;
using NativeUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoArtifactLights.Missions
{
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
