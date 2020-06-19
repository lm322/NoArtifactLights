using GTA;
using System;

namespace NoArtifactLights.Serialize
{
    [Serializable]
    public struct WorldStatus
    {
        public WorldStatus(Weather weather, int hour, int minute)
        {
            CurrentWeather = weather;
            Hour = hour;
            Minute = minute;
        }

        public Weather CurrentWeather { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}