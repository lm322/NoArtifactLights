using System;

namespace NoArtifactLights.Serialize
{
    [Serializable]
    public struct SaveWeapon
    {
        public SaveWeapon(int ammo, bool exists)
        {
            Ammo = ammo;
            Existence = exists;
        }

        public bool Existence { get; set; }
        public int Ammo { get; set; }
    }
}