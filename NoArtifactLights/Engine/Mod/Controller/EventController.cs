// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using GTA;
using NLog;
using NoArtifactLights.Engine.Mod.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NoArtifactLights.Engine.Mod.Controller
{
	internal static class EventController
	{
		public static List<Type> registeredEvents = new List<Type>();
		public static bool disable = false;
		private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
		private static List<Event> processingEvents = new List<Event>();
		
		public static void RegisterEvent(Type t)
		{
			if (t == null) return;
			if (!t.GetType().IsAssignableFrom(typeof(Event))) return;
			registeredEvents.Add(t);
		}

	    internal static void Process()
		{
			foreach(Event e in processingEvents)
			{
				Script.Yield();
				if (e == null) continue;
				if (e.canBeEnded)
				{
					processingEvents.Remove(e);
					continue;
				}
				e.Process();
			}
		}

		public static void StartRandomEvent(Ped p)
		{
			logger.Info("Random event has been marked as started");
			logger.Info("Random events are: " + registeredEvents.Count);
			if (registeredEvents.Count == 0) return;
			if (disable) return;
			logger.Info("Checks accomplished");
			Random r = new Random();
			logger.Info("Random Created");
			int result = r.Next(0, registeredEvents.Count);
			logger.Info("Tracked: " + (registeredEvents.Count));
			logger.Info("Result: " + result);
			if (p.IsInVehicle())
			{
				p.Task.ReactAndFlee(Game.Player.Character);
			}
			//if (registeredEvents.Count > result)
			//{
			//    logger.Info("It is bigger than Tracked!");
			//    return;
			//}
			
			object obj = Activator.CreateInstance(registeredEvents[result]);
			logger.Info(obj.GetType().Name + " is starting");
			Event @event = (Event)obj;
			
			try
			{
				@event.Initialize();
			}
			catch(Exception ex)
			{
				logger.Error("Error while executing event: " + @event.GetType().Name);
				logger.Error(ex);
			}
		}
	}
}
