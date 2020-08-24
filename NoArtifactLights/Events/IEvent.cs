using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoArtifactLights.Events
{
    public interface IEvent
    {
        void Initialize();
        void ProceedLogicOnce(Ped p);
        void End();
    }
}
