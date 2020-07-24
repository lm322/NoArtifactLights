using GTA;
using GTA.UI;
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
        internal static int cash = 0;
        internal static bool blackout;
        internal static Logger logger = new Logger();
        internal static event EventHandler Unload;

        internal static event EventHandler CashChanged;

        internal static void UnloadMod(object you)
        {
            Notification.Show(Strings.Unload);
            logger.Log("Unloading modification. Bye!", "Common");
            Unload(you, new EventArgs());
        }

        internal static void AddCash(int amount)
        {
            cash += amount;
            CashChanged(new object(), new EventArgs());
        }

        internal static void RemoveCash(int amount)
        {
            cash -= amount;
            CashChanged(new object(), new EventArgs());
        }

        public static void Deposit(int amount)
        {
            Game.Player.Money -= amount;
            string writePath = "%USERPROFILE%\\pacific.dat";
            FileStream fs = File.Open(writePath, FileMode.OpenOrCreate, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            int already;
            if (fs.Length != 0) already = br.ReadInt32();
            else already = amount;
            already = br.ReadInt32();
            already += amount;
            br.Close();
            br.Dispose();
            fs.Dispose();
            fs = File.OpenWrite(writePath);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(already);
            bw.Close();
            bw.Dispose();
            fs.Dispose();
        }

        public static void Withdraw(int amount)
        {
            string writePath = "%USERPROFILE%\\pacific.dat";
            FileStream fs = File.Open(writePath, FileMode.OpenOrCreate, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            int already;
            if (fs.Length != 0) return;
            else already = amount;
            already = br.ReadInt32();
            if (already <= amount)
            {
                Screen.ShowSubtitle("You do not have enough money to withdraw");
            }
            already -= amount;
            br.Close();
            br.Dispose();
            fs.Dispose();
            fs = File.OpenWrite(writePath);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(already);
            bw.Close();
            bw.Dispose();
            fs.Dispose();
            Game.Player.Money += amount;
        }
    }
}