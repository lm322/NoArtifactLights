using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using NativeUI;
using NoArtifactLights.Events;
using NLog;
using NoArtifactLights.Managers;
using NoArtifactLights.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Logger = NLog.Logger;

namespace NoArtifactLights
{
    public class Entry : Script
    {
        private Pickup weapon;
        private Blip weaponBlip;
        internal static DuplicateManager peds1 = new DuplicateManager();
        internal static DuplicateManager killedPeds = new DuplicateManager();
        internal static DuplicateManager weaponedPeds = new DuplicateManager();
        private int eplased = 0;
        private Vehicle deliveryCar;
        private Ped delivery;

        private Logger logger = LogManager.GetCurrentClassLogger();
        internal static bool forcestart;

        public static void ForceStartEvent()
        {
            Entry.forcestart = true;
        }

        public Entry()
        {
            try
            {
                Screen.FadeOut(1000);
                logger.Info("Initialized");
                Function.Call(Hash.SET_ARTIFICIAL_LIGHTS_STATE, true);
                GameUI.DisplayHelp(Strings.Start);
                if (!File.Exists("scripts\\PlayerReliveNoResetModel.dll"))
                {
                    logger.Warn("No PlayerReliveNoResetModel to provide Screen.FadeIn upon player wasted or busted. The game will faded out and never fade in upon death or arrest.");
                    Notification.Show(Strings.NoModelWarning);
                    Notification.Show(Strings.NoModelWarning2);
                }
                if (!File.Exists("scripts\\NoArtifactLights.pdb"))
                {
                    logger.Warn("Attention: No debug database found in game scripts folder. This means logs cannot provide additional information related to exception.");
                }
                this.Interval = 100;
                this.Tick += Entry_Tick;
                logger.Trace("Loading multiplayer maps");
                Function.Call(Hash._LOAD_MP_DLC_MAPS);
                Function.Call(Hash._USE_FREEMODE_MAP_BEHAVIOR, true);
                logger.Trace("Setting player position and giving weapons");
                Game.Player.Character.Position = new Vector3(459.8501f, -1001.404f, 24.91487f);
                Game.Player.Character.Weapons.Give(WeaponHash.Flashlight, 1, true, true);
                Game.Player.Character.Weapons.Give(WeaponHash.Pistol, 50, false, false);
                logger.Trace("Setting relationship and game settings");
                GameContentManager.SetRelationship(Difficulty.Initial);
                Game.MaxWantedLevel = 0;
                Game.Player.IgnoredByPolice = true;
                Game.Player.ChangeModel("a_m_m_bevhills_02");
                Screen.FadeIn(1000);
                Common.Unload += Common_Unload;
                EventManager.RegisterEvent(typeof(ArmedPed));
                this.Aborted += Entry_Aborted;
            }
            catch(Exception ex)
            {
                Screen.FadeIn(500);
                logger.Fatal(ex, "Excepting during initial load process");
                Abort();
            }
        }

        private void Entry_Aborted(object sender, EventArgs e)
        {
            
        }

        private void Common_Unload(object sender, EventArgs e)
        {
            if(!sender.Equals(this))
            {
                GameContentManager.SetRelationshipBetGroupsUInt(Relationship.Pedestrians, 0x02B8FA80, 0x47033600);
                GameContentManager.SetRelationshipBetGroupsUInt(Relationship.Pedestrians, 0x47033600, 0x02B8FA80);
                this.Tick -= Entry_Tick;
                Abort();
            }
        }

        private void T_Elapsed1(object sender, ElapsedEventArgs e)
        {
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
        }



        private void Entry_Tick(object sender, EventArgs e)
        {
            try
            {
                Ped[] peds = World.GetAllPeds();
                foreach (Ped ped in peds)
                {
                    if (ped == delivery)
                    {
                        continue;
                    }
                    if (ped == null)
                    {
                        continue;
                    }
                    if (ped.Exists() && ped.HasBeenDamagedBy(Game.Player.Character) && ped.IsDead && !killedPeds.IsDuplicate(ped))
                    {
                        killedPeds.Add(ped);
                        logger.Debug("A ped has been killed");
                        if (Game.Player.Character.Position.DistanceTo(ped.Position) <= 2.5f)
                        {
                            Common.Earn(new Random().Next(4, 16));
                        }
                        Common.counter++;
                        if (weaponedPeds.IsDuplicate(ped))
                        {
                            Common.Earn(10);
                            GameUI.DisplayHelp(Strings.ArmedBonus);
                            if (ped.AttachedBlip != null && ped.AttachedBlip.Exists())
                            {
                                ped.AttachedBlip.Delete();
                            }
                        }
                        switch (Common.counter)
                        {
                            case 1:
                                GameUI.DisplayHelp(Strings.FirstKill);
                                break;

                            case 100:
                                Common.difficulty = Difficulty.Easy;
                                BigMessageThread.MessageInstance.ShowSimpleShard(Strings.DifficultyChange, string.Format(Strings.DifficultyShard, Strings.DifficultyEasy));
                                GameUI.DisplayHelp(string.Format(Strings.DifficultyHelp, Strings.DifficultyEasy));
                                GameContentManager.SetRelationship(Difficulty.Easy);
                                break;

                            case 300:
                                Common.difficulty = Difficulty.Normal;
                                BigMessageThread.MessageInstance.ShowSimpleShard(Strings.DifficultyChange, string.Format(Strings.DifficultyShard, Strings.DifficultyNormal));
                                GameUI.DisplayHelp(string.Format(Strings.DifficultyHelp, Strings.DifficultyNormal));
                                GameContentManager.SetRelationship(Difficulty.Normal);
                                break;

                            case 700:
                                Common.difficulty = Difficulty.Hard;
                                BigMessageThread.MessageInstance.ShowSimpleShard(Strings.DifficultyChange, string.Format(Strings.DifficultyShard, Strings.DifficultyHard));
                                GameUI.DisplayHelp(string.Format(Strings.DifficultyHelp, Strings.DifficultyHard));
                                GameContentManager.SetRelationship(Difficulty.Hard);
                                break;

                            case 1500:
                                Common.difficulty = Difficulty.Nether;
                                BigMessageThread.MessageInstance.ShowSimpleShard(Strings.DifficultyChange, string.Format(Strings.DifficultyShard, Strings.DifficultyNether));
                                GameUI.DisplayHelp(string.Format(Strings.DifficultyHelp, Strings.DifficultyNether));
                                GameContentManager.SetRelationship(Difficulty.Nether);
                                break;
                        }
                    }
                    if (peds1.IsDuplicate(ped) || !ped.IsHuman)
                    {
                        continue;
                    }
                    peds1.Add(ped);
                    if (new Random().Next(1000000, 2000001) == 1100000 && (delivery == null || !delivery.Exists() || !deliveryCar.Exists()))
                    {
                        logger.Debug("Hit deliverycar");
                        bool success = GameContentManager.CreateDelivery(out deliveryCar, out delivery);
                        if (!success)
                        {
                            deliveryCar = null;
                            delivery = null;
                        }
                    }

                    if (new Random().Next(9, 89) == 10 || forcestart == true)
                    {
                        forcestart = false;
                        EventManager.StartRandomEvent(ped);
                    }
                }

                if (killedPeds.Count >= 6000)
                {
                    killedPeds.Clear();
                }
                if (weaponedPeds.Count >= 600)
                {
                    weaponedPeds.Clear();
                }
                if (weapon != null && weapon.Exists() && weapon.IsCollected)
                {
                    weapon = null;
                }
                eplased++;
                if (eplased >= 60000 && weapon == null)
                {
                    weapon = World.CreatePickup(PickupType.AmmoPistol, World.GetNextPositionOnSidewalk(Game.Player.Character.Position.Around(100f)), "w_pi_pistol", 30);
                    weaponBlip = World.CreateBlip(weapon.Position);
                    weaponBlip.Sprite = BlipSprite.AmmuNation;
                    weaponBlip.Name = Strings.WeaponPistol;
                    BigMessageThread.MessageInstance.ShowSimpleShard(Strings.WeaponsShard, Strings.WeaponsShardSubtitle);
                }
                if (peds1.Count >= 60000)
                {
                    peds1.Clear();
                }
                if(delivery != null && delivery.Exists() && deliveryCar.Exists() && !delivery.IsInVehicle(deliveryCar) && !delivery.IsGettingIntoVehicle)
                {
                    delivery.Task.EnterVehicle(deliveryCar, VehicleSeat.Driver);
                }
                if(delivery != null && delivery.Exists() && deliveryCar.Exists() && delivery.Position.DistanceTo2D(Game.Player.Character.Position) >= 350f && !deliveryCar.IsOnScreen)
                {
                    delivery.AttachedBlip.Delete();
                    delivery.MarkAsNoLongerNeeded();
                    deliveryCar.MarkAsNoLongerNeeded();
                    delivery = null;
                    deliveryCar = null;
                }
                if (delivery != null && delivery.Exists() && deliveryCar.Exists() && delivery.HasBeenDamagedBy(Game.Player.Character) && (deliveryCar.IsDead || delivery.IsDead || !delivery.IsInVehicle(deliveryCar)))
                {
                    delivery.AttachedBlip.Delete();
                    delivery = null;
                    deliveryCar = null;
                    GameUI.DisplayHelp(Strings.StolenDelivery);
                    Common.Earn(100);
                }
            }
            catch (Exception ex)
            {
                GameUI.DisplayHelp(Strings.ExceptionMain);
                logger.Fatal(ex);
                throw;
            }
        }
    }
}