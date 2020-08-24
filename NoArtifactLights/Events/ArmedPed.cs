using GTA;
using NoArtifactLights.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoArtifactLights.Events
{
    public class ArmedPed : IEvent
    {
        public void End()
        {
            
        }

        public void Initialize()
        {
            
        }

        public void ProceedLogicOnce(Ped p)
        {
            // checks whether it should be equipped with weapon
            if(p == null || !p.Exists() || Entry.weaponedPeds.IsDuplicate(p))
            {
                return;
            }
            p.EquipWeapon();
            GameContentManager.AddWeaponedPed(p);
        }
    }
}
