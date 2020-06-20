using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NoArtifactLights
{ 
    /// <summary>
    ///  This may have copyright issued. If it does, contact me.
    /// </summary>
    public class CallbackMarker
    {
        public CallbackMarker()
        {
            time = 0;
            initial = 0;
        }
        public CallbackMarker(int time2)
        {
            Start(time2);
        }
        public int time;
        public int initial;
        public int Time { get { return Game.GameTime - initial; } }
        public bool IsOn { get { return initial == 0 || time > initial; } }
        public void Start(int time1)
        {
            initial = Game.GameTime;
            time = time1;
        }
    }

    public static class PlayerUtil
    {
        public static int TimeSinceBusted(this Player player) => Function.Call<int>(Hash.GET_TIME_SINCE_LAST_ARREST);
        public static int TimeSinceWasted(this Player player) => Function.Call<int>(Hash.GET_TIME_SINCE_LAST_DEATH);
    }
}
