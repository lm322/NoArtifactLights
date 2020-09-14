// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using GTA;

namespace NoArtifactLights.Engine.Mod.API.Events
{
	public class StealCar : Event
	{
		Blip b;
		private Ped p => EventPed;
		private Vehicle vehicle;

		public override void Process()
		{
			if (vehicle == null && p.IsInVehicle())
			{
				Vehicle v = p.CurrentVehicle;
				if (v != null && v.Exists())
				{
					vehicle = v;
				}
			}
			if(vehicle != null && !p.IsInVehicle())
			{
				vehicle = null;
			}
			base.Process();
		}

		public override void End()
		{
			if (b.Exists()) b.Delete();
			base.End();
		}

		public override void Initialize()
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
