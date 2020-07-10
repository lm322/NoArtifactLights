using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoArtifactLights.Managers
{
    public static class GameContentManager
    {
        private static void SetRelationshipBetGroupsUInt(Relationship relation, uint group1, uint group2)
        {
            Function.Call(Hash.SET_RELATIONSHIP_BETWEEN_GROUPS, (int)relation, group1, group2);
        }

        public static void SetRelationship(Difficulty difficulty)
        {
            World.SetRelationshipBetweenGroups(Relationship.Hate, 0x02B8FA80, 0x47033600);
            World.SetRelationshipBetweenGroups(Relationship.Hate, 0x47033600, 0x02B8FA80);
            switch (difficulty)
            {
                default:
                case Difficulty.Initial:
                    break;
                case Difficulty.Easy:
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0xA49E591C, 0x02B8FA80);
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0x02B8FA80, 0xA49E591C);
                    break;
                case Difficulty.Normal:
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0xA49E591C, 0x02B8FA80);
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0x02B8FA80, 0xA49E591C);
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0xC26D562A, 0x02B8FA80);
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0x02B8FA80, 0xC26D562A);
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0x45897C40, 0xC26D562A);
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0xC26D562A, 0x45897C40);
                    break;
                case Difficulty.Nether:
                case Difficulty.Hard:
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0xA49E591C, 0x02B8FA80);
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0x02B8FA80, 0xA49E591C);
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0xA49E591C, 0x47033600);
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0x47033600, 0xA49E591C);
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0xC26D562A, 0x02B8FA80);
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0x02B8FA80, 0xC26D562A);
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0x45897C40, 0xC26D562A);
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0xC26D562A, 0x45897C40);
                    SetRelationshipBetGroupsUInt(Relationship.Hate, 0x02B8FA80, 0x6F0783F5);
                    break;
            }
        }
    }
}
