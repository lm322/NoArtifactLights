﻿// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using GTA;
using GTA.Native;
using GTA.UI;
using NLog;
using NoArtifactLights.Engine.Mod.API;
using NoArtifactLights.Engine.Mod.Controller;
using NoArtifactLights.Resources;
using System;
using System.IO;
using System.Timers;
using Logger = NLog.Logger;
using NoArtifactLights.Engine.Process;
using NoArtifactLights.Engine.Entities;
using NoArtifactLights.Engine.Entities.Enums;
using System.Collections.Generic;
using LemonUI.Scaleform;
using System.Threading;
using NoArtifactLights.Cilent;
using System.Reflection;

namespace NoArtifactLights
{
	[ScriptAttributes(Author = "RelaperCrystal", SupportURL = "https://hotworkshop.atlassian.net/projects/NAL")]
	public class Entry : Script
	{
		private Pickup weapon;
		private Blip weaponBlip;
		internal static HandleableList peds1 = new HandleableList();
		internal static HandleableList killedPeds = new HandleableList();
		internal static HandleableList weaponedPeds = new HandleableList();
		private int eplased = 0;
		private Vehicle deliveryCar;
		private Ped delivery;

		internal static List<Blip> blips = new List<Blip>();

		private Logger logger = LogManager.GetLogger("Entry");
		internal static bool forcestart;

		public NALClient Client { get; private set; }

		public static void ForceStartEvent()
		{
			forcestart = true;
		}

		public void LoadMod()
		{
			try
			{
				Thread.CurrentThread.Name = "main";
				Screen.FadeOut(1000);
				logger.Info("Starting NAL");
				GameUI.DisplayHelp(Strings.Start);
				if (!File.Exists("scripts\\NoArtifactLights.pdb"))
				{
					logger.Warn("Attention: No debug database found in game scripts folder. This means logs cannot provide additional information related to exception.");
				}
				//Initializer.LoadProgram();


				this.Client = new NALClient(Assembly.GetExecutingAssembly().GetName().Version);
				this.Interval = 100;
				this.Tick += Ticking1;
				Common.Unload += Common_Unload1;
				this.Aborted += Entry_Aborted;
			}
			catch (Exception ex)
			{
				Screen.FadeIn(500);
				logger.Fatal(ex, "Excepting during initial load process");
				Abort();
			}
		}

		public Entry()
		{
			this.KeyDown += Entry_KeyDown;

			//try
			//{
			//	Thread.CurrentThread.Name = "main";
			//	Screen.FadeOut(1000);
			//	logger.Info("Starting NAL");
			//	GameUI.DisplayHelp(Strings.Start);
			//	if (!File.Exists("scripts\\NoArtifactLights.pdb"))
			//	{
			//		logger.Warn("Attention: No debug database found in game scripts folder. This means logs cannot provide additional information related to exception.");
			//	}
			//	//Initializer.LoadProgram();

			//	this.Client = new NALClient(Assembly.GetExecutingAssembly().GetName().Version);
			//	this.Interval = 100;
			//	this.Tick += Ticking1;
			//	Common.Unload += Common_Unload1;
			//	this.Aborted += Entry_Aborted;
			//}
			//catch(Exception ex)
			//{
			//	Screen.FadeIn(500);
			//	logger.Fatal(ex, "Excepting during initial load process");
			//	Abort();
			//}
		}

		private void Entry_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == System.Windows.Forms.Keys.L && e.Alt)
			{
				this.KeyDown -= Entry_KeyDown;
				LoadMod();
			}
		}

		private void Ticking1(object sender, EventArgs e) => Client.Tick();

		private void Common_Unload1(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		private void Entry_Aborted(object sender, EventArgs e)
		{
			GameController.SetRelationshipBetGroupsUInt(Relationship.Pedestrians, 0x02B8FA80, 0x47033600);
			GameController.SetRelationshipBetGroupsUInt(Relationship.Pedestrians, 0x47033600, 0x02B8FA80);
			this.Tick -= Entry_Tick;
			foreach(Blip blip in blips)
			{
				if(blip.Exists())
				blip.Delete();
			}
		}

		private void Common_Unload(object sender, EventArgs e)
		{
			if(!sender.Equals(this))
			{
				GameController.SetRelationshipBetGroupsUInt(Relationship.Pedestrians, 0x02B8FA80, 0x47033600);
				GameController.SetRelationshipBetGroupsUInt(Relationship.Pedestrians, 0x47033600, 0x02B8FA80);
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
						Game.MaxWantedLevel = 0;
						Game.Player.IgnoredByPolice = true;
						if (Game.Player.Character.Position.DistanceTo(ped.Position) <= 2.5f)
						{
							Common.Earn(new Random().Next(4, 16));
						}
						Common.counter++;
						if (weaponedPeds.IsDuplicate(ped))
						{
							Common.Earn(50);
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
								new BigMessage(Strings.DifficultyChange, string.Format(Strings.DifficultyShard, Strings.DifficultyEasy));
								GameUI.DisplayHelp(string.Format(Strings.DifficultyHelp, Strings.DifficultyEasy));
								GameController.SetRelationship(Difficulty.Easy);
								break;

							case 300:
								Common.difficulty = Difficulty.Normal;
								new BigMessage(Strings.DifficultyChange, string.Format(Strings.DifficultyShard, Strings.DifficultyNormal));
								GameUI.DisplayHelp(string.Format(Strings.DifficultyHelp, Strings.DifficultyNormal));
								GameController.SetRelationship(Difficulty.Normal);
								break;

							case 700:
								Common.difficulty = Difficulty.Hard;
								new BigMessage(Strings.DifficultyChange, string.Format(Strings.DifficultyShard, Strings.DifficultyHard));
								GameUI.DisplayHelp(string.Format(Strings.DifficultyHelp, Strings.DifficultyHard));
								GameController.SetRelationship(Difficulty.Hard);
								break;

							case 1500:
								Common.difficulty = Difficulty.Nether;
								new BigMessage(Strings.DifficultyChange, string.Format(Strings.DifficultyShard, Strings.DifficultyNether));
								GameUI.DisplayHelp(string.Format(Strings.DifficultyHelp, Strings.DifficultyNether));
								GameController.SetRelationship(Difficulty.Nether);
								break;
						}
					}
					if (peds1.IsDuplicate(ped) || !ped.IsHuman)
					{
						continue;
					}
					peds1.Add(ped);
					EventController.Process();
					if (new Random().Next(1000000, 2000001) == 1100000 && (delivery == null || !delivery.Exists() || !deliveryCar.Exists()))
					{
						logger.Debug("Hit deliverycar");
						bool success = GameController.CreateDelivery(out deliveryCar, out delivery);
						if (!success)
						{
							deliveryCar = null;
							delivery = null;
						}
					}

					if (new Random().Next(9, 89) == 10 || forcestart == true)
					{
						forcestart = false;
						EventController.StartRandomEvent(ped);
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
					new BigMessage(Strings.WeaponsShard, Strings.WeaponsShardSubtitle);
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
