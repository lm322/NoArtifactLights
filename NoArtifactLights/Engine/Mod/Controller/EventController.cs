﻿using GTA;
using NLog;
using NoArtifactLights.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoArtifactLights.Managers
{
    internal static class EventController
    {
        public static List<Type> registeredEvents = new List<Type>();
        public static bool disable = false;
        private static NLog.Logger logger = LogManager.GetCurrentClassLogger();
        
        public static void RegisterEvent(Type t)
        {
            if (t == null) return;
            if (!t.GetInterfaces().Contains(typeof(IEvent))) return;
            registeredEvents.Add(t);
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
            IEvent @event = (IEvent)obj;
            
            try
            {
                @event.Initialize();
                @event.ProceedLogicOnce(p);
            }
            catch(Exception ex)
            {
                logger.Error("Error while executing event: " + @event.GetType().Name);
                logger.Error(ex);
            }
        }
    }
}