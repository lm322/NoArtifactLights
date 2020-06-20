using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
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
        private Timer fall;
        private List<int> ids = new List<int>();
        private List<int> killedIds = new List<int>();
        private List<int> weaponedIds = new List<int>();
        private int eplased = 0;
        CallbackMarker cb = new CallbackMarker();
        private bool dead;

        public Entry()
        {
            Function.Call(Hash._SET_BLACKOUT, true);
            UI.ShowHelpMessage(Strings.Start);
            if(!File.Exists("scripts\\PlayerReliveNoResetModel.dll"))
            {
                UI.Notify("");
            }
            this.Interval = 100;
            this.Tick += Entry_Tick;
            Game.Player.ChangeModel("a_m_m_bevhills_02");
            Function.Call(Hash._LOAD_MP_DLC_MAPS);
            Function.Call(Hash._LOWER_MAP_PROP_DENSITY, true);
            Game.Player.Character.Position = new Vector3(459.8501f, -1001.404f, 24.91487f);
            Game.Player.Character.Weapons.Give(WeaponHash.Flashlight, 1, true, true);
            Game.Player.Character.Weapons.Give(WeaponHash.Pistol, 50, false, false);
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
                Game.Player.WantedLevel = 0;
                Ped[] peds = World.GetAllPeds();
                foreach (Ped ped in peds)
                {
                    if (ped.Exists() && ped.HasBeenDamagedBy(Game.Player.Character) && ped.IsDead && !killedIds.Contains(ped.Handle))
                    {
                        killedIds.Add(ped.Handle);
                        Common.counter++;
                        if (weaponedIds.Contains(ped.Handle))
                        {
                            Common.cash += 10;
                            UI.ShowHelpMessage(Strings.ArmedBonus);
                        }
                        switch (Common.counter)
                        {
                            case 1:
                                UI.ShowHelpMessage(Strings.FirstKill);
                                break;

                            case 1000:
                                Common.difficulty = Difficulty.Easy;
                                BigMessageThread.MessageInstance.ShowSimpleShard(Strings.DifficultyChange, string.Format(Strings.DifficultyShard, Strings.DifficultyEasy));
                                UI.ShowHelpMessage(string.Format(Strings.DifficultyHelp, Strings.DifficultyEasy));
                                break;

                            case 3000:
                                Common.difficulty = Difficulty.Normal;
                                BigMessageThread.MessageInstance.ShowSimpleShard(Strings.DifficultyChange, string.Format(Strings.DifficultyShard, Strings.DifficultyNormal));
                                UI.ShowHelpMessage(string.Format(Strings.DifficultyHelp, Strings.DifficultyNormal));
                                break;

                            case 7000:
                                Common.difficulty = Difficulty.Hard;
                                BigMessageThread.MessageInstance.ShowSimpleShard(Strings.DifficultyChange, string.Format(Strings.DifficultyShard, Strings.DifficultyHard));
                                UI.ShowHelpMessage(string.Format(Strings.DifficultyHelp, Strings.DifficultyHard));
                                break;

                            case 15000:
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
                    if (new Random().Next(100, 189) == 150)
                    {
                        Vehicle v = World.GetClosestVehicle(ped.Position, 50f);
                        if (v == null || !v.Exists()) continue;
                        ped.Task.EnterVehicle(v, VehicleSeat.Driver, 6000, 8f);
                        continue;
                    }
                    ped.Task.FightAgainst(World.GetClosestPed(ped.Position, 15f), -1);
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