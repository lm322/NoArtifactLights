using System;
using System.Collections.Generic;

namespace NoArtifactLights.Serialize
{
    [Serializable]
    public struct SaveFile
    {
        public int Version { get; set; }
        public float PlayerX { get; set; }
        public float PlayerY { get; set; }
        public float PlayerZ { get; set; }
        public WorldStatus Status { get; set; }
        public bool Blackout { get; set; }
        public Difficulty CurrentDifficulty { get; set; }
        public int Kills { get; set; }
        public int Cash { get; set; }
        public SaveWeapon Pistol { get; set; }
        public SaveWeapon PumpShotgun { get; set; }

        public List<SaveWeapon> WeaponDraft { get; set; }
        public int PlayerArmor { get; set; }
        public int PlayerHealth { get; set; }
    }
}