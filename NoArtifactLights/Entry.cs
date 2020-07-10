using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using NoArtifactLights.Managers;
using NoArtifactLights.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace NoArtifactLights
{
    public class Entry : Script
    {
        private Pickup weapon;
        private Blip weaponBlip;
        private List<int> ids = new List<int>();
        private List<int> killedIds = new List<int>();
        private List<int> weaponedIds = new List<int>();
        private int eplased = 0;
        private Vehicle deliveryCar;
        private Ped delivery;

        private Logger logger = new Logger();

        public Entry()
        {
            logger.Log("Initialized", "Main");    
            Function.Call(Hash._SET_BLACKOUT, true);
            UI.ShowHelpMessage(Strings.Start);
            if(!File.Exists("scripts\\PlayerReliveNoResetModel.dll"))
            {
                logger.Log("No PlayerReliveNoResetModel to provide Game.FadeScreenIn upon player wasted or busted. The game will faded out and never fade in upon death or arrest.", "Main", Logger.LogLevel.Warning);
                UI.Notify(Strings.NoModelWarning);
                UI.Notify(Strings.NoModelWarning2);
            }
            this.Interval = 100;
            this.Tick += Entry_Tick;
            Game.Player.ChangeModel("a_m_m_bevhills_02");
            logger.Log("Loading multiplayer maps");
            Function.Call(Hash._LOAD_MP_DLC_MAPS);
            Function.Call(Hash._LOWER_MAP_PROP_DENSITY, true);
            logger.Log("Setting player position and giving weapons");
            Game.Player.Character.Position = new Vector3(459.8501f, -1001.404f, 24.91487f);
            Game.Player.Character.Weapons.Give(WeaponHash.Flashlight, 1, true, true);
            Game.Player.Character.Weapons.Give(WeaponHash.Pistol, 50, false, false);
            logger.Log("Setting relationship and game settings", "Main");
            GameContentManager.SetRelationship(Difficulty.Initial);
            Game.MaxWantedLevel = 0;
            Game.Player.IgnoredByPolice = true;
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
                    if(ped == delivery)
                    {
                        continue;
                    }
                    if (ped.Exists() && ped.HasBeenDamagedBy(Game.Player.Character) && ped.IsDead && !killedIds.Contains(ped.Handle))
                    {
                        killedIds.Add(ped.Handle);
                        Common.counter++;
                        if (weaponedIds.Contains(ped.Handle))
                        {
                            Common.cash += 10;
                            UI.ShowHelpMessage(Strings.ArmedBonus);
                            ped.CurrentBlip.Remove();
                        }
                        switch (Common.counter)
                        {
                            case 1:
                                UI.ShowHelpMessage(Strings.FirstKill);
                                break;

                            case 100:
                                Common.difficulty = Difficulty.Easy;
                                BigMessageThread.MessageInstance.ShowSimpleShard(Strings.DifficultyChange, string.Format(Strings.DifficultyShard, Strings.DifficultyEasy));
                                UI.ShowHelpMessage(string.Format(Strings.DifficultyHelp, Strings.DifficultyEasy));
                                break;

                            case 300:
                                Common.difficulty = Difficulty.Normal;
                                BigMessageThread.MessageInstance.ShowSimpleShard(Strings.DifficultyChange, string.Format(Strings.DifficultyShard, Strings.DifficultyNormal));
                                UI.ShowHelpMessage(string.Format(Strings.DifficultyHelp, Strings.DifficultyNormal));
                                break;

                            case 700:
                                Common.difficulty = Difficulty.Hard;
                                BigMessageThread.MessageInstance.ShowSimpleShard(Strings.DifficultyChange, string.Format(Strings.DifficultyShard, Strings.DifficultyHard));
                                UI.ShowHelpMessage(string.Format(Strings.DifficultyHelp, Strings.DifficultyHard));
                                break;

                            case 1500:
                                Common.difficulty = Difficulty.Nether;
                                BigMessageThread.MessageInstance.ShowSimpleShard(Strings.DifficultyChange, string.Format(Strings.DifficultyShard, Strings.DifficultyNether));
                                UI.ShowHelpMessage(string.Format(Strings.DifficultyHelp, Strings.DifficultyNether));
                                break;
                        }
                    }
                    if (ids.Contains(ped.Handle) || !ped.IsHuman)
                    {
                        continue;
                    }
                    ids.Add(ped.Handle);
                    if (new Random().Next(1000000, 2000001) == 1100000 &&(delivery == null || !delivery.Exists() || !deliveryCar.Exists()))
                    {
                        logger.Log("Hit deliverycar", "Main");
                        deliveryCar = World.CreateVehicle("MULE", World.GetNextPositionOnStreet(Game.Player.Character.Position.Around(30f)));
                        delivery = deliveryCar.CreateRandomPedOnSeat(VehicleSeat.Driver);
                        delivery.AddBlip();
                        delivery.CurrentBlip.Sprite = BlipSprite.PersonalVehicleCar;
                        delivery.CurrentBlip.IsFriendly = false;
                        delivery.CurrentBlip.IsFlashing = true;
                        delivery.CurrentBlip.Color = BlipColor.Red;
                        delivery.IsPersistent = true;
                        deliveryCar.IsPersistent = true;
                        delivery.AlwaysKeepTask = true;
                        delivery.BlockPermanentEvents = true;
                    }
                    


                    if (new Random().Next(9, 89) == 10)
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
                        ped.Weapons.Give(wp, short.MaxValue, true, true);
                        weaponedIds.Add(ped.Handle);
                        ped.AddBlip();
                        ped.CurrentBlip.IsFriendly = false;
                        ped.CurrentBlip.Sprite = BlipSprite.Enemy;
                        ped.CurrentBlip.Scale = 0.5f;
                        ped.CurrentBlip.Color = BlipColor.Red;
                    }
                }

                if (killedIds.Count >= 6000)
                {
                    killedIds.Clear();
                }
                if (weaponedIds.Count >= 600)
                {
                    killedIds.Clear();
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
                if (ids.Count >= 60000)
                {
                    ids.Clear();
                }
                if(delivery != null && delivery.Exists() && deliveryCar.Exists() && !delivery.IsInVehicle(deliveryCar) && !delivery.IsGettingIntoAVehicle)
                {
                    delivery.Task.EnterVehicle(deliveryCar, VehicleSeat.Driver);
                }
                if(delivery != null && delivery.Exists() && deliveryCar.Exists() && delivery.Position.DistanceTo2D(Game.Player.Character.Position) >= 350f && !deliveryCar.IsOnScreen)
                {
                    delivery.CurrentBlip.Remove();
                    delivery.MarkAsNoLongerNeeded();
                    deliveryCar.MarkAsNoLongerNeeded();
                    delivery = null;
                    deliveryCar = null;
                }
                if (delivery != null && delivery.Exists() && deliveryCar.Exists() && delivery.HasBeenDamagedBy(Game.Player.Character) && (deliveryCar.IsDead || delivery.IsDead || !delivery.IsInVehicle(deliveryCar)))
                {
                    delivery.CurrentBlip.Remove();
                    delivery = null;
                    deliveryCar = null;
                    UI.ShowHelpMessage(Strings.StolenDelivery);
                    Common.cash += 100;
                }
            }
            catch (Exception ex)
            {
                UI.ShowHelpMessage(Strings.ExceptionMain);
                File.WriteAllText("EXCEPTION.TXT", $"Exception caught: \r\n{ex.GetType().Name}\r\nException Message:\r\n{ex.Message}\r\nException StackTrace:\r\n{ex.StackTrace}");
                throw;
            }
        }
    }
}