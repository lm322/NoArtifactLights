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
        internal static bool CreateDelivery(out Vehicle car, out Ped driver)
        {
            try
            {
                Vehicle deliveryCar = World.CreateVehicle("MULE", World.GetNextPositionOnStreet(Game.Player.Character.Position.Around(30f)));
                Ped delivery = deliveryCar.CreateRandomPedOnSeat(VehicleSeat.Driver);
                delivery.AddBlip();
                delivery.CurrentBlip.Sprite = BlipSprite.PersonalVehicleCar;
                delivery.CurrentBlip.IsFriendly = false;
                delivery.CurrentBlip.IsFlashing = true;
                delivery.CurrentBlip.Color = BlipColor.Red;
                delivery.IsPersistent = true;
                deliveryCar.IsPersistent = true;
                delivery.AlwaysKeepTask = true;
                delivery.BlockPermanentEvents = true;
                car = deliveryCar;
                driver = delivery;
                return true;
            }
            catch(Exception ex)
            {
                Common.logger.Log("Failed to create delivery: \r\n" + ex.ToString(), Logger.LogLevel.Warning);
                car = null;
                driver = null;
                return false;
            }
        }

        internal static void EquipWeapon(this Ped ped)
        {
            WeaponHash wp;
            switch (Common.difficulty)
            {
                default:
                case Difficulty.Initial:
                    if (new Random().Next(200, 272) == 40) wp = WeaponHash.PumpShotgun;
                    else wp = WeaponHash.Pistol;
                    break;

                case Difficulty.Easy:
                    wp = WeaponHash.PumpShotgun;
                    break;

                case Difficulty.Normal:
                    wp = WeaponHash.MiniSMG;
                    break;

                case Difficulty.Hard:
                    wp = WeaponHash.CarbineRifle;
                    break;

                case Difficulty.Nether:
                    wp = WeaponHash.RPG;
                    break;
            }
            ped.Weapons.Give(wp, 9999, true, true);
            ped.AddBlip();
            ped.CurrentBlip.IsFriendly = false;
            ped.CurrentBlip.Sprite = BlipSprite.Enemy;
            ped.CurrentBlip.Scale = 0.5f;
            ped.CurrentBlip.Color = BlipColor.Red;
        }
    }
}
