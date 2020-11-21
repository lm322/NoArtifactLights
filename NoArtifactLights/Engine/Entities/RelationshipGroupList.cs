using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;

namespace NoArtifactLights.Engine.Entities
{
	internal class RelationshipGroupList
	{
		List<RelationshipGroup> Groups = new List<RelationshipGroup>();
		int selected;

		internal RelationshipGroupList(RelationshipGroup[] groups)
		{
			Groups.AddRange(groups);
		}

		internal void SelectFirst()
		{
			selected = 0;
		}

		internal void SelectLast()
		{
			selected = Groups.Count - 1;
		}

		internal void SelectIndex(int index)
		{
			selected = index;
		}

		internal void SelectHash(int hash)
		{
			for (int i = 0; i < Groups.Count; i++)
			{
				if (Groups[i].Hash == hash) selected = i;
			}
		}

		internal RelationshipGroup this[int hash]
		{
			get
			{
				SelectHash(hash);
				return Groups[selected];
			}
		}
	}
}
