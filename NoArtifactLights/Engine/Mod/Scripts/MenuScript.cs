// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using NLog;
using NoArtifactLights.Engine.Mod.Controller;
using NoArtifactLights.Engine.Mod.API;
using NoArtifactLights.Resources;
using System;
using System.Windows.Forms;
using Screen = GTA.UI.Screen;
using System.Drawing;
using LemonUI;
using LemonUI.Menus;
using LemonUI.TimerBars;
using System.Threading;
using CommandPlus.Exceptions;

namespace NoArtifactLights.Engine.Mod.Scripts
{
	[ScriptAttributes(Author = "RelaperCrystal", SupportURL = "https://hotworkshop.atlassian.net/projects/NAL")]
	public class MenuScript : Script
	{
		private ObjectPool lemonPool;
		private NativeMenu mainMenu;
		private NativeItem itemSave;
		private NativeItem itemLoad;
		private NativeItem itemCallCops;
		private NativeItem itemModels;
		private NativeItem itemDifficulty;
		private NativeItem itemKills;
		private NativeCheckboxItem itemCheatEnabled;
		private NativeItem itemCommand;
		private NativeItem itemDeposit;
		private NativeItem itemWithdraw;
		private NativeCheckboxItem itemLights;
		private NativeItem itemCash;
		private NativeItem itemBank;

		private NativeMenu modelMenu;
		private NativeItem itemDefaultModel;
		private NativeItem itemCopModel;

		private NativeMenu buyMenu;
		private NativeItem itemPistol;
		private NativeItem itemPumpShotgun;
		private NativeItem itemCarbineRifle;
		private NativeItem itemBodyArmor;

		private NativeMenu foodMenu;
		private NativeItem itemChicken;
		private NativeItem itemHamburger;
		private NativeItem itemWater;

		Blip repairBlip;

		private TimerBarCollection timerBars = new TimerBarCollection();
		internal TimerBarProgress hungryBar = new TimerBarProgress(Strings.BarHungry);
		internal TimerBarProgress waterBar = new TimerBarProgress(Strings.BarWater);
		private Vector3 repair = new Vector3(140.683f, -1081.387f, 28.56039f);

		internal static MenuScript instance;

		internal static void ChangeHungryBarColor(Color cl)
		{
			instance.hungryBar.ForegroundColor = cl;
		}

		internal static void ChangeWaterBarColor(Color cl)
		{
			instance.waterBar.ForegroundColor = cl;
		}

		private NLog.Logger logger = LogManager.GetLogger("MenuScript");

		public MenuScript()
		{
			Common.Start += Common_Start;
			
		}

		private void Common_Start(object sender, EventArgs e)
		{
			try
			{
				Thread.CurrentThread.Name = "UI Thread";
				logger.Trace("Loading Main Menu");
				lemonPool = new ObjectPool();
				logger.Trace("Menu Pool created");
#if DEBUG
				mainMenu = new NativeMenu("NAL Beta", Strings.MenuMainTitle);
#else
				mainMenu = new NativeMenu("NAL", Strings.MenuMainTitle);
#endif
				itemLights = new NativeCheckboxItem(Strings.ItemLightsTitle, Strings.ItemLightsSubtitle, true);
				itemSave = new NativeItem(Strings.ItemSaveTitle, Strings.ItemSaveSubtitle);
				itemLoad = new NativeItem(Strings.ItemLoadTitle, Strings.ItemLoadSubtitle);
				itemCallCops = new NativeItem(Strings.ItemCopsTitle, Strings.ItemCopsSubtitle);
				itemDifficulty = new NativeItem(Strings.ItemDifficulty, Strings.ItemDIfficultySubtitle);
				itemKills = new NativeItem(Strings.ItemKills, Strings.ItemKillsSubtitle);
				itemCheatEnabled = new NativeCheckboxItem(Strings.ItemCheat, Strings.ItemCheatDescription);
				itemCommand = new NativeItem(Strings.ItemCommand, Strings.ItemCommandDescription);
				itemDeposit = new NativeItem(Strings.ItemDepositTitle, Strings.ItemDepositSubtitle);
				itemWithdraw = new NativeItem(Strings.ItemWithdrawTitle, Strings.ItemWithdrawSubtitle);
				itemCash = new NativeItem(Strings.ItemCashTitle, Strings.ItemCashSubtitle);
				itemBank = new NativeItem(Strings.ItemBankTitle, Strings.ItemBankSubtitle);
				itemModels = new NativeItem(Strings.ItemModels, Strings.ItemModelsDescription);

				modelMenu = new NativeMenu("Models", Strings.MenuModel);

				itemDefaultModel = new NativeItem("Default", "The classic NAL Model.");
				itemCopModel = new NativeItem("LSPD Officer", "The cop.");
				modelMenu.Add(itemDefaultModel);
				modelMenu.Add(itemCopModel);
				itemDefaultModel.Activated += ItemDefaultModel_Activated;
				itemCopModel.Activated += ItemCopModel_Activated;

				foodMenu = new NativeMenu("Food", Strings.MenuFoodShopSubtitle);

				itemHamburger = HungryController.CreateFoodSellerItem(Strings.FoodBurger, Foods.Hamburger, 1);
				itemChicken = HungryController.CreateFoodSellerItem(Strings.FoodChicken, Foods.Chicken, 3);
				itemWater = HungryController.GenerateWaterSellItem(Strings.ItemWater, 2);

				foodMenu.Add(itemHamburger);
				foodMenu.Add(itemChicken);
				foodMenu.Add(itemWater);

				logger.Trace("All instances initialized");
				mainMenu.Add(itemLights);
				mainMenu.Add(itemSave);
				mainMenu.Add(itemLoad);
				mainMenu.Add(itemCallCops);

				itemModels = mainMenu.AddSubMenu(modelMenu);
				itemModels.Title = Strings.ItemModels;
				itemModels.Description = Strings.ItemModelsDescription;

				mainMenu.Add(itemDifficulty);
				mainMenu.Add(itemKills);
				mainMenu.Add(itemDeposit);
				mainMenu.Add(itemWithdraw);
				mainMenu.Add(itemCash);
				mainMenu.Add(itemBank);
				lemonPool.Add(mainMenu);
				lemonPool.Add(modelMenu);
				lemonPool.Add(foodMenu);
				logger.Trace("Main Menu Done");
				Tick += MenuScript_Tick;
				KeyDown += MenuScript_KeyDown;
				itemLights.CheckboxChanged += ItemLights_CheckboxEvent;
				itemSave.Activated += ItemSave_Activated;
				itemLoad.Activated += ItemLoad_Activated;
				itemCallCops.Activated += ItemCallCops_Activated;
				itemCommand.Activated += ItemCommand_Activated;
				itemCheatEnabled.Activated += ItemCheatEnabled_Activated;
				itemDeposit.Activated += ItemDeposit_Activated;
				itemWithdraw.Activated += ItemWithdraw_Activated;

				timerBars.Add(hungryBar);

				Common.Unload += Common_Unload;

				logger.Trace("Loading Ammu-Nation Menu");

				buyMenu = new NativeMenu(Strings.AmmuTitle, Strings.AmmuSubtitle);
				itemPistol = AmmuController.GenerateWeaponSellerItem(Strings.AmmuPistol, Strings.AmmuPistolSubtitle, 100);
				itemPumpShotgun = AmmuController.GenerateWeaponSellerItem(Strings.AmmuPumpShotgun, Strings.AmmuPumpShotgunSubtitle, 200);
				itemCarbineRifle = AmmuController.GenerateWeaponSellerItem(Strings.AmmuCarbineRifle, Strings.AmmuCarbineRifleSubtitle, 350);
				itemBodyArmor = new NativeItem(Strings.WeaponBodyArmor, Strings.WeaponBodyArmorDescription);
				itemBodyArmor.AltTitle = "$380";
				logger.Trace("Instances created");
				buyMenu.Add(itemCash);
				buyMenu.Add(itemPistol);
				buyMenu.Add(itemPumpShotgun);
				buyMenu.Add(itemBodyArmor);
				lemonPool.Add(buyMenu);
				itemPistol.Activated += ItemPistol_Activated;
				itemPumpShotgun.Activated += ItemPumpShotgun_Activated;
				itemCarbineRifle.Activated += ItemCarbineRifle_Activated;
				itemBodyArmor.Activated += ItemBodyArmor_Activated;

				repairBlip = World.CreateBlip(repair);
				repairBlip.IsFriendly = true;
				repairBlip.IsShortRange = true;
				repairBlip.Sprite = BlipSprite.Garage;
				repairBlip.Color = BlipColor.Blue;
				repairBlip.Name = Strings.RepairBlip;

				HungryController.AddBlipsToAllResellers();
				instance = this;

				this.Aborted += MenuScript_Aborted;
				CommandController.Init();
			}
			catch (Exception ex)
			{
				GameUI.DisplayHelp(Strings.ExceptionMenu);
				logger.Fatal(ex, "Error while loading menu");
				Common.UnloadMod(this);
				this.Abort();
			}
		}

		private void ItemCheatEnabled_Activated(object sender, EventArgs e)
		{
			Common.IsCheatEnabled = itemCheatEnabled.Checked;
		}

		private void ItemCommand_Activated(object sender, EventArgs e)
		{
			string str = Game.GetUserInput(WindowTitle.EnterMessage60, "", short.MaxValue);
			if(!Common.IsCheatEnabled)
			{
				GameUI.DisplayHelp(Strings.NoPermission);
				return;
			}
			try
			{
				CommandController.Run(str);
			}
			catch(UnexceptedValueException uvex)
			{
				Notification.Show(uvex.Message);
			}
			catch(Exception ex)
			{
				Notification.Show(ex.ToString());
			}
			finally
			{
				logger.Info("User typed command: " + str);
			}

		}

		private void ItemCopModel_Activated(object sender, EventArgs args)
		{
			Game.Player.ChangeModel("s_m_y_cop_01");
			//selectedItem.SetRightBadge(UIMenuItem.BadgeStyle.Clothes);
			//itemDefaultModel.SetRightBadge(UIMenuItem.BadgeStyle.None);
			itemSave.Enabled = false;
		}

		private void ItemDefaultModel_Activated(object sender, EventArgs args)
		{
			Game.Player.ChangeModel("a_m_m_bevhills_02");
			//selectedItem.SetRightBadge(UIMenuItem.BadgeStyle.Clothes);
			//itemCopModel.SetRightBadge(UIMenuItem.BadgeStyle.None);
			itemSave.Enabled = false;
		}

		private void ItemCarbineRifle_Activated(object sender, EventArgs args)
		{
			AmmuController.SellWeapon(350, 50, WeaponHash.CarbineRifle);
		}

		private void ItemWithdraw_Activated(object sender, EventArgs args)
		{
			mainMenu.Visible = false;
			string cash = Game.GetUserInput(WindowTitle.EnterMessage20, "", 20);
			int result;
			bool success = int.TryParse(cash, out result);
			if (!success)
			{
				Screen.ShowSubtitle(Strings.InputNotNumber);
				return;
			}
			if(Common.Bank < result)
			{
				Screen.ShowSubtitle(Strings.WithdrawNoCurrency);
				return;
			}
			Common.Bank -= result;
			Common.Cash += result;
			GameUI.DisplayHelp(Strings.TransactionSuccess);
		}

		private void ItemDeposit_Activated(object sender, EventArgs args)
		{
			mainMenu.Visible = false;
			string cash = Game.GetUserInput(WindowTitle.EnterMessage20, "", 20);
			int result;
			bool success = int.TryParse(cash, out result);
			if (!success)
			{
				Screen.ShowSubtitle(Strings.InputNotNumber);
				return;
			}
			if (!Common.Cost(result))
			{
				return;
			}
			Common.Bank += result;
			GameUI.DisplayHelp(Strings.TransactionSuccess);
		}

		private void MenuScript_Aborted(object sender, EventArgs e)
		{
			if(repairBlip != null && repairBlip.Exists())
			{
				repairBlip.Delete();
				
			}

			if (buyMenu != null) buyMenu.Visible = false;
			if (mainMenu != null)
			{
				mainMenu.Visible = false;
				itemLights.CheckboxChanged -= ItemLights_CheckboxEvent;
				itemSave.Activated -= ItemSave_Activated;
				itemLoad.Activated -= ItemLoad_Activated;
				itemCallCops.Activated -= ItemCallCops_Activated;
			}
			
			Tick -= MenuScript_Tick;
			KeyDown -= MenuScript_KeyDown;

			mainMenu = null;
			buyMenu = null;

		}

		private void Common_Unload(object sender, EventArgs e)
		{
			if(!sender.Equals(this))
			{
				Abort();
			}
		}

		private void ItemBodyArmor_Activated(object sender, EventArgs args) => AmmuController.SellArmor(50, 380);
		private void ItemPumpShotgun_Activated(object sender, EventArgs args) => AmmuController.SellWeapon(200, 50, WeaponHash.PumpShotgun);
		private void ItemPistol_Activated(object sender, EventArgs args) => AmmuController.SellWeapon(100, 100, WeaponHash.Pistol);

		private void ItemCallCops_Activated(object sender, EventArgs args)
		{
			Function.Call(Hash.CREATE_INCIDENT, 7, Game.Player.Character.Position.X, Game.Player.Character.Position.Y, Game.Player.Character.Position.Z, 2, 3.0f, new OutputArgument());
		}

		private void ItemLoad_Activated(object sender, EventArgs args)
		{
			SaveController.Load();
			itemLights.Checked = Common.blackout;
			itemDifficulty.AltTitle = Strings.ResourceManager.GetString("Difficulty" + Common.difficulty.ToString());
			itemKills.AltTitle = Common.counter.ToString();
			itemCash.AltTitle = "$" + Common.Cash.ToString();
			itemSave.Enabled = true;
			Notification.Show(Strings.GameLoaded);
		}

		private void ItemSave_Activated(object sender, EventArgs args) => SaveController.Save(itemLights.Checked);

		private void ItemLights_CheckboxEvent(object sender, EventArgs args)
		{
			Function.Call(Hash.SET_ARTIFICIAL_LIGHTS_STATE, itemLights.Checked);
		}

		private void MenuScript_Tick(object sender, EventArgs e)
		{
			lemonPool.Process();
			timerBars.Process();

			hungryBar.Progress = HungryController.ProgressBarStatus;
			waterBar.Progress = HungryController.WaterBarStatus;
			if (AmmuController.DistanceToAmmu())
			{
				GameUI.DisplayHelp(Strings.AmmuOpenShop);
			}
			if (HungryController.IsPlayerCloseReseller())
			{
				GameUI.DisplayHelp(Strings.FoodOpenShop);
			}
			if (repair.DistanceTo(Game.Player.Character.Position) <= 10f && Game.Player.Character.IsInVehicle())
			{
				GameUI.DisplayHelp(Strings.RepairHelp);
			}
		}

		private void MenuScript_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.N:
					mainMenu.Visible = !mainMenu.Visible;
					itemDifficulty.AltTitle = Strings.ResourceManager.GetString("Difficulty" + Common.difficulty.ToString());
					itemKills.AltTitle = Common.counter.ToString();
					itemCash.AltTitle = "$" + Common.Cash.ToString();
					itemBank.AltTitle = "$" + Common.Bank.ToString();
					break;
				case Keys.E:
					if (mainMenu.Visible) return;
					if (buyMenu.Visible)
					{
						buyMenu.Visible = false;
						return;
					}
					if (AmmuController.DistanceToAmmu() && !lemonPool.AreAnyVisible)
					{
						buyMenu.Visible = true;
					}
					if (HungryController.IsPlayerCloseReseller() && !lemonPool.AreAnyVisible)
					{
						foodMenu.Visible = true;
					}
					if (repair.DistanceTo(Game.Player.Character.Position) <= 10f && Game.Player.Character.IsInVehicle())
					{
		
						if(Game.Player.Character.CurrentVehicle.IsDamaged == false)
						{
							Screen.ShowSubtitle(Strings.RepairUndamaged);
							return;
						}
						if (!Common.Cost(100)) break;
						Game.Player.Character.CurrentVehicle.Repair();
						Screen.ShowSubtitle(Strings.RepairSuccess);
					}
					break;
			}
		}
	}
}
