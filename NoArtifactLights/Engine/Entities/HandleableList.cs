// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using GTA;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NoArtifactLights.Engine.Entities
{
    public class HandleableList : IList<Ped>
    {
        private List<int> peds = new List<int>();

        public int Count => peds.Count;

        public bool IsReadOnly => false;

        public Ped this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Add(Ped ped)
        {
            if(!peds.Contains(ped.Handle))
            {
                peds.Add(ped.Handle);
            }
        }

        public void Remove(Ped ped)
        {
            if(peds.Contains(ped.Handle))
            {
                peds.Remove(ped.Handle);
            }
        }

        public bool IsDuplicate(Ped ped)
        {
            return peds.Contains(ped.Handle);
        }

        public void Clear()
        {
            peds.Clear();
        }

        public int IndexOf(Ped item)
        {
            return peds.IndexOf(item.Handle);
        }

        public void Insert(int index, Ped item) => peds.Insert(index, item.Handle);
        public void RemoveAt(int index) => peds.RemoveAt(index);
        public bool Contains(Ped item) => IsDuplicate(item);

        public void CopyTo(Ped[] array, int arrayIndex)
        {
            List<int> handles = new List<int>();
            foreach(Ped p in array)
            {
                if(p == null || !p.Exists())
                {
                    continue;
                }
                handles.Add(p.Handle);
            }
            int[] array2 = handles.ToArray();
            peds.CopyTo(array2);
        }

        bool ICollection<Ped>.Remove(Ped item)
        {
            return peds.Remove(item.Handle);
        }

        public IEnumerator<Ped> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
