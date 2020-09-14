// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using System;

namespace NoArtifactLights.Engine.Entities.Structures
{
    [Serializable]
    public struct SaveWeapon
    {
        public SaveWeapon(int ammo, bool exists)
        {
            Ammo = ammo;
            Existence = exists;
        }

        // public WeaponHash Weapon { get; set; }
        public bool Existence { get; set; }
        public int Ammo { get; set; }
    }
}
