// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

// Partially Licensed under:
// Copyright (C) 2015 crosire & contributors
// License: https://github.com/crosire/scripthookvdotnet#license
// Other parts licensed under GNU General Public License version 3.

using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoArtifactLights.Engine.Entities.Extentions
{
    public abstract class AEntity
    {
        public GTA.Entity GEntity { get; protected set; }
        public int Handle => GEntity.Handle;

        public bool IsValid() => GEntity.Exists();
        protected void CheckInternal()
        {
            if (!IsValid()) throw new InvalidOperationException("This instance no longer valids.");
        }

        public GTA.Blip AttachBlip() => GEntity.AddBlip();

        public void Delete() => GEntity.Delete();
        public void Dismiss() => GEntity.MarkAsNoLongerNeeded();
        public void MakePersistent() => GEntity.IsPersistent = true;
        
        public float Heading
        {
            get
            {
                CheckInternal();
                return Function.Call<float>(Hash.GET_ENTITY_HEADING, Handle);
            }
            set
            {
                CheckInternal();
                Function.Call(Hash.SET_ENTITY_HEADING, Handle, value);
            }
        }

        public int Health
        {
            get
            {
                CheckInternal();
                return Function.Call<int>(Hash.GET_ENTITY_HEALTH, Handle);
            }
            set
            {
                CheckInternal();
                Function.Call(Hash.SET_ENTITY_HEALTH, Handle, value);
            }
        }

        public int MaxHealth
        {
            get
            {
                CheckInternal();
                return Function.Call<int>(Hash.GET_ENTITY_MAX_HEALTH, Handle);
            }
            set
            {
                CheckInternal();
                Function.Call(Hash.SET_ENTITY_MAX_HEALTH, Handle, value);
            }
        }

        public GTA.Math.Vector3 Position
        {
            get
            {
                CheckInternal();
                return Function.Call<GTA.Math.Vector3>(Hash.GET_ENTITY_COORDS, Handle, IsAlive);
            }
            set
            {
                CheckInternal();
                Function.Call(Hash.SET_ENTITY_COORDS, Handle, value.X, value.Y, value.Z, false, false, false, true);
            }
        }

        public bool IsAlive
        {
            get
            {
                CheckInternal();
                return !IsDead;
            }
        }

        public bool IsDead
        {
            get
            {
                CheckInternal();
                return Function.Call<bool>(Hash.IS_ENTITY_DEAD, Handle);
            }
        }

        public bool IsOnFire
        {
            get
            {
                CheckInternal();
                return Function.Call<bool>(Hash.IS_ENTITY_ON_FIRE, Handle);
            }
        }
    }

    public static class AEntityExtensions
    {
        /// <summary>
        /// Determines whether the specified instance exists.
        /// </summary>
        /// <param name="entity">The instance to check.</param>
        /// <returns>If the entity valid and not null.</returns>
        public static bool Exists(this AEntity entity)
        {
            return entity != null || entity.IsValid();
        }
    }
}
