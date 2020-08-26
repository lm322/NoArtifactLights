using GTA;
using NLog;
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
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
            logger.Trace("Equipping weapon");
            p.EquipWeapon();
            Blip b = p.AddBlip();
            b.IsFriendly = false;
            b.Sprite = BlipSprite.Enemy;
            b.Scale = 0.5f;
            b.Color = BlipColor.Red;
            GameContentManager.AddWeaponedPed(p);
        }
    }
}
