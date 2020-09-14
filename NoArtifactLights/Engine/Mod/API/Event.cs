// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using System;
using GTA;

namespace NoArtifactLights.Engine.Mod.API
{
	/// <summary>
	/// Represents a world event happens in the game world.
	/// </summary>
	public abstract class Event
	{
		/// <summary>
		/// The <see cref="Ped"/> processed in the world event. <b>Do not override this property</b> unless required to do so and there is no walkarounds other than
		/// overriding this property - It is important to let the event controller select a <see cref="Ped"/> and pass it to your <see cref="Event"/>.
		/// </summary>
		/// <value>
		/// The ped used in event.
		/// </value>
		public virtual Ped EventPed { get; internal set; }

		internal bool canBeEnded = false;

		[Obsolete]
		public virtual void ProceedLogicOnce(Ped p)
		{

		}
		
		/// <summary>
		/// Processes this <see cref="Event"/>.
		/// </summary>
		/// <remarks>
		/// Default implementation does following things by order: Check whether the <see cref="EventPed"/> is null and whether it still valids. If null or not valid, 
		/// this event will end.
		/// </remarks>
		public virtual void Process()
		{
			if(EventPed == null || !EventPed.Exists() || Game.Player.Character.Position.DistanceTo(EventPed.Position) >= 55f)
			{
				End();
			}
		}
		public abstract void Initialize();
		public virtual void End()
		{
			if(EventPed != null && EventPed.Exists())
			{
				EventPed.MarkAsNoLongerNeeded();
			}
			canBeEnded = true;
		}
	}
}
