// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using NativeUI;
using NLog;
using NoArtifactLights.Engine.Mod.Controller;
using NoArtifactLights.Engine.Mod.API;
using NoArtifactLights.Resources;
using System;
using System.IO;
using System.Windows.Forms;
using Screen = GTA.UI.Screen;
using System.Drawing;

namespace NoArtifactLights.Engine.Mod.Scripts
{
	[ScriptAttributes(Author = "RelaperCrystal", SupportURL = "https://hotworkshop.atlassian.net/projects/NAL")]
	public class MenuScript : Script
	{
		private MenuPool pool;
		private UIMenu mainMenu;
		private UIMenuItem itemSave;
		private UIMenuItem itemLoad;
		private UIMenuItem itemCallCops;
		private UIMenuItem itemModels;
		private UIMenuItem itemDifficulty;
		private UIMenuItem itemKills;
		private UIMenuItem itemDeposit;
		private UIMenuItem itemWithdraw;
		private UIMenuCheckboxItem itemLights;
		private UIMenuItem itemCash;
		private UIMenuItem itemBank;

		private UIMenu modelMenu;
		private UIMenuItem itemDefaultModel;
		private UIMenuItem itemCopModel;

		private UIMenu buyMenu;
		private UIMenuItem itemPistol;
		private UIMenuItem itemPumpShotgun;
		private UIMenuItem itemCarbineRifle;
		private UIMenuItem itemBodyArmor;

		Blip repairBlip;

		// private Vector3 ammu = new Vector3(18.18945f, -1120.384f, 28.91654f);
		private Vector3 repair = new Vector3(140.683f, -1081.387f, 28.56039f);

		private NLog.Logger logger = LogManager.GetLogger("MenuScript");

		public MenuScript()
		{
			try
			{
				logger.Trace("Loading Main Menu");
				pool = new MenuPool();
				logger.Trace("Menu Pool created");
				mainMenu = new UIMenu("", Strings.MenuMainTitle);
				if(!File.Exists("scripts\\nal.png"))
				{
					Bitmap bm = Properties.Resources.nal;
					bm.Save("scripts\\nal.png");
				}
				mainMenu.SetBannerType("scripts\\nal.png");
				itemLights = new UIMenuCheckboxItem(Strings.ItemLightsTitle, true, Strings.ItemLightsSubtitle);
				itemSave = new UIMenuItem(Strings.ItemSaveTitle, Strings.ItemSaveSubtitle);
				itemLoad = new UIMenuItem(Strings.ItemLoadTitle, Strings.ItemLoadSubtitle);
				itemCallCops = new UIMenuItem(Strings.ItemCopsTitle, Strings.ItemCopsSubtitle);
				itemDifficulty = new UIMenuItem(Strings.ItemDifficulty, Strings.ItemDIfficultySubtitle);
				itemKills = new UIMenuItem(Strings.ItemKills, Strings.ItemKillsSubtitle);
				itemDeposit = new UIMenuItem(Strings.ItemDepositTitle, Strings.ItemDepositSubtitle);
				itemWithdraw = new UIMenuItem(Strings.ItemWithdrawTitle, Strings.ItemWithdrawSubtitle);
				itemCash = new UIMenuItem(Strings.ItemCashTitle, Strings.ItemCashSubtitle);
				itemBank = new UIMenuItem(Strings.ItemBankTitle, Strings.ItemBankSubtitle);
				itemModels = new UIMenuItem(Strings.ItemModels, Strings.ItemModelsDescription);

				modelMenu = new UIMenu("", Strings.MenuModel);
				modelMenu.SetBannerType("scripts\\nal.png");

				itemDefaultModel = new UIMenuItem("Default", "The classic NAL Model.");
				itemCopModel = new UIMenuItem("LSPD Officer", "The cop.");
				modelMenu.AddItem(itemDefaultModel);
				modelMenu.AddItem(itemCopModel);
				itemDefaultModel.Activated += ItemDefaultModel_Activated;
				itemCopModel.Activated += ItemCopModel_Activated;

				logger.Trace("All instances initialized");
				mainMenu.AddItem(itemLights);
				mainMenu.AddItem(itemSave);
				mainMenu.AddItem(itemLoad);
				mainMenu.AddItem(itemCallCops);

				mainMenu.BindMenuToItem(modelMenu, itemModels);

				mainMenu.AddItem(itemDifficulty);
				mainMenu.AddItem(itemKills);
				mainMenu.AddItem(itemDeposit);
				mainMenu.AddItem(itemWithdraw);
				mainMenu.AddItem(itemCash);
				mainMenu.AddItem(itemBank);
				logger.Trace("Refreshing Index");
				mainMenu.RefreshIndex();
				pool.Add(mainMenu);
				pool.Add(modelMenu);
				logger.Trace("Main Menu Done");
				Tick += MenuScript_Tick;
				KeyDown += MenuScript_KeyDown;
				itemLights.CheckboxEvent += ItemLights_CheckboxEvent;
				itemSave.Activated += ItemSave_Activated;
				itemLoad.Activated += ItemLoad_Activated;
				itemCallCops.Activated += ItemCallCops_Activated;
				itemDeposit.Activated += ItemDeposit_Activated;
				itemWithdraw.Activated += ItemWithdraw_Activated;

				Common.Unload += Common_Unload;

				logger.Trace("Loading Ammu-Nation Menu");

				buyMenu = new UIMenu(Strings.AmmuTitle, Strings.AmmuSubtitle);
				itemPistol = AmmuController.GenerateWeaponSellerItem(Strings.AmmuPistol, Strings.AmmuPistolSubtitle, 100);
				itemPumpShotgun = AmmuController.GenerateWeaponSellerItem(Strings.AmmuPumpShotgun, Strings.AmmuPumpShotgunSubtitle, 200);
				itemCarbineRifle = AmmuController.GenerateWeaponSellerItem(Strings.AmmuCarbineRifle, Strings.AmmuCarbineRifleSubtitle, 350);
				itemBodyArmor = new UIMenuItem(Strings.WeaponBodyArmor, Strings.WeaponBodyArmorDescription);
				itemBodyArmor.SetRightLabel("$380");
				logger.Trace("Instances created");
				buyMenu.AddItem(itemCash);
				buyMenu.AddItem(itemPistol);
				buyMenu.AddItem(itemPumpShotgun);
				buyMenu.AddItem(itemBodyArmor);
				buyMenu.RefreshIndex();
				pool.Add(buyMenu);
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

				this.Aborted += MenuScript_Aborted;
			}
			catch (Exception ex)
			{
				GameUI.DisplayHelp(Strings.ExceptionMenu);
				logger.Fatal(ex, "Error while loading menu");
				Common.UnloadMod(this);
				this.Abort();
			}
		}

		private void ItemCopModel_Activated(UIMenu sender, UIMenuItem selectedItem)
		{
			Game.Player.ChangeModel("s_m_y_cop_01");
			selectedItem.SetRightBadge(UIMenuItem.BadgeStyle.Clothes);
			itemDefaultModel.SetRightBadge(UIMenuItem.BadgeStyle.None);
			itemSave.Enabled = false;
		}

		private void ItemDefaultModel_Activated(UIMenu sender, UIMenuItem selectedItem)
		{
			Game.Player.ChangeModel("a_m_m_bevhills_02");
			selectedItem.SetRightBadge(UIMenuItem.BadgeStyle.Clothes);
			itemCopModel.SetRightBadge(UIMenuItem.BadgeStyle.None);
			itemSave.Enabled = false;
		}

		private void ItemCarbineRifle_Activated(UIMenu sender, UIMenuItem selectedItem)
		{
			AmmuController.SellWeapon(350, 50, WeaponHash.CarbineRifle);
		}

		private void ItemWithdraw_Activated(UIMenu sender, UIMenuItem selectedItem)
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

		private void ItemDeposit_Activated(UIMenu sender, UIMenuItem selectedItem)
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
				itemLights.CheckboxEvent -= ItemLights_CheckboxEvent;
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

		private void ItemBodyArmor_Activated(UIMenu sender, UIMenuItem selectedItem) => AmmuController.SellArmor(50, 380);
		private void ItemPumpShotgun_Activated(UIMenu sender, UIMenuItem selectedItem) => AmmuController.SellWeapon(200, 50, WeaponHash.PumpShotgun);
		private void ItemPistol_Activated(UIMenu sender, UIMenuItem selectedItem) => AmmuController.SellWeapon(100, 100, WeaponHash.Pistol);

		private void ItemCallCops_Activated(UIMenu sender, UIMenuItem selectedItem)
		{
			Function.Call(Hash.CREATE_INCIDENT, 7, Game.Player.Character.Position.X, Game.Player.Character.Position.Y, Game.Player.Character.Position.Z, 2, 3.0f, new OutputArgument());
		}

		private void ItemLoad_Activated(UIMenu sender, UIMenuItem selectedItem)
		{
			SaveController.Load();
			itemLights.Checked = Common.blackout;
			itemDifficulty.SetRightLabel(Strings.ResourceManager.GetString("Difficulty" + Common.difficulty.ToString()));
			itemKills.SetRightLabel(Common.counter.ToString());
			itemCash.SetRightLabel("$" + Common.Cash.ToString());
			itemSave.Enabled = true;
			Notification.Show(Strings.GameLoaded);
		}

		private void ItemSave_Activated(UIMenu sender, UIMenuItem selectedItem) => SaveController.Save(itemLights.Checked);

		private void ItemLights_CheckboxEvent(UIMenuCheckboxItem sender, bool Checked)
		{
			Function.Call(Hash.SET_ARTIFICIAL_LIGHTS_STATE, Checked);
		}

		private void MenuScript_Tick(object sender, EventArgs e)
		{
			pool.ProcessMenus();
			if (AmmuController.DistanceToAmmu())
			{
				GameUI.DisplayHelp(Strings.AmmuOpenShop);
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
					itemDifficulty.SetRightLabel(Strings.ResourceManager.GetString("Difficulty" + Common.difficulty.ToString()));
					itemKills.SetRightLabel(Common.counter.ToString());
					itemCash.SetRightLabel("$" + Common.Cash.ToString());
					itemBank.SetRightLabel("$" + Common.Bank.ToString());
					break;
				case Keys.E:
					if (mainMenu.Visible) return;
					if (buyMenu.Visible)
					{
						buyMenu.Visible = false;
						return;
					}
					if (AmmuController.DistanceToAmmu())
					{
						buyMenu.Visible = true;
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
