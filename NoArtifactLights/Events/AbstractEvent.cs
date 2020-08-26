using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoArtifactLights.Events
{
    public abstract class AbstractEvent
    {
        public virtual Ped EventPed { get; }

        public abstract void ProceedLogicOnce(Ped p);
        public abstract void Initialize(Ped p);
        public virtual void End()
        {
            if(EventPed != null && EventPed.Exists())
            {
                EventPed.MarkAsNoLongerNeeded();
            }
        }
    }
}
