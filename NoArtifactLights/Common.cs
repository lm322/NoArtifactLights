using GTA;
using GTA.UI;
using NLog;
using NoArtifactLights.Managers;
using NoArtifactLights.Resources;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace NoArtifactLights
{
    internal static class Common
    {
        internal static int counter = 0;
        internal static Difficulty difficulty = Difficulty.Initial;
        internal static bool blackout;
        internal static NLog.Logger logger = LogManager.GetLogger("Common");
        internal static event EventHandler Unload;

        // internal static event EventHandler CashChanged;

        public static int Cash { get; set; } = 0;

        public static int Bank { get; set; } = 0;

        internal static void UnloadMod(object you)
        {
            Notification.Show(Strings.Unload);
            
            Unload(you, new EventArgs());
        }

        [Obsolete]
        public static void Deposit(int amount)
        {
            Bank += amount;
        }

        public static bool Cost(int amount)
        {
            if(Cash < amount)
            {
                Screen.ShowSubtitle(Strings.BuyNoMoney);
                return false;
            }
            Cash -= amount;
            return true;
        }

        public static bool Earn(int amount)
        {
            if(Cash == int.MaxValue)
            {
                logger.Info("Player's cash has reached int limit");
                Notification.Show(NotificationIcon.Blocked, "", "", Strings.CashMaximum);
                return false;
            }
            Cash += amount;
            return true;
        }

        [Obsolete]
        public static void Withdraw(int amount)
        {
            Bank -= amount;
        }
    }
}