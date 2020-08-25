using GTA;
using NLog;
using NoArtifactLights.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoArtifactLights.Managers
{
    internal static class EventManager
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
            if (registeredEvents.Count == 0) return;
            if (disable) return;
            Random r = new Random();
            int result = r.Next(0, registeredEvents.Count + 1);
            if (registeredEvents.Count > result) return;
            
            object obj = Activator.CreateInstance(registeredEvents[result]);
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
