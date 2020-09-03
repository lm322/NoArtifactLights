using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoArtifactLights.Events
{
    public class StealCar : IEvent
    {
        Blip b;

        public void End()
        {
            
        }

        public void Initialize()
        {
            
        }

        public void ProceedLogicOnce(Ped p)
        {
            p.Task.EnterAnyVehicle(VehicleSeat.Driver, -1, 1, EnterVehicleFlags.AllowJacking);
            b = p.AddBlip();
            b.Scale = 0.5f;
            b.Color = BlipColor.BlueLight;
            b.IsFlashing = true;
            b.Sprite = BlipSprite.PointOfInterest;
        }
    }
}
