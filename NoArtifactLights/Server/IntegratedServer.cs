using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.UI;
using LemonUI.Scaleform;
using NLog;
using NoArtifactLights.Engine.Entities;
using NoArtifactLights.Engine.Entities.Enums;
using NoArtifactLights.Engine.Mod.API;
using NoArtifactLights.Engine.Mod.Controller;
using NoArtifactLights.Resources;

namespace NoArtifactLights.Server
{
	internal class IntegratedServer : IServer
	{
		private Logger logger = LogManager.GetLogger("IntegratedServer");

		internal static HandleableList peds1 = new HandleableList();
		internal static HandleableList killedPeds = new HandleableList();
		internal static HandleableList weaponedPeds = new HandleableList();
		private bool forcestart;

		public List<NetworkClient> Clients { get; private set; } = new List<NetworkClient>();

		public void ConnectingWithoutIndex(NetworkClient client)
		{
			Connecting(client);
		}

		public int Connecting(NetworkClient client)
		{
			logger.Info("Client " + client.Name + " connecting!");
			Clients.Add(client);
			Notification.Show($"{client.Name} {Strings.PlayerJoinedGame}");
			return Clients.IndexOf(client);
		}

		public void Tick()
		{
			try
			{


				Ped[] peds = World.GetAllPeds();
				foreach (Ped ped in peds)
				{
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
				if (peds1.Count >= 60000)
				{
					peds1.Clear();
				}
			}
			catch (Exception ex)
			{
				GameUI.DisplayHelp(Strings.ExceptionMain);
				logger.Fatal(ex);
				throw;
			}
		}

		public void Disconnect(int playerIndex)
		{
			
		}

		public void GetPerference(int perference)
		{
			throw new NotImplementedException();
		}

		public void UpdatePerferences<T>(int perferences, T value)
		{
			throw new NotImplementedException();
		}

		public void Process()
		{
			Tick();
		}
	}
}
