using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using NativeUI;
using NLog;
using NoArtifactLights.Managers;
using NoArtifactLights.Resources;
using System;
using System.IO;
using System.Windows.Forms;
using Screen = GTA.UI.Screen;

namespace NoArtifactLights.Menu
{
    public class MenuScript : Script
    {
        private TextTimerBar cashBar;
        private MenuPool pool;
        private UIMenu mainMenu;
        private UIMenuItem itemSave;
        private UIMenuItem itemLoad;
        private UIMenuItem itemCallCops;
        private UIMenuItem itemDifficulty;
        private UIMenuItem itemKills;
        private UIMenuItem itemDeposit;
        private UIMenuItem itemWithdraw;
        private UIMenuCheckboxItem itemLights;
        private UIMenuItem itemCash;
        private UIMenuItem itemBank;

        private UIMenu buyMenu;
        private UIMenuItem itemPistol;
        private UIMenuItem itemPumpShotgun;
        private UIMenuItem itemBodyArmor;

        Blip repairBlip;

        // private Vector3 ammu = new Vector3(18.18945f, -1120.384f, 28.91654f);
        private Vector3 repair = new Vector3(140.683f, -1081.387f, 28.56039f);

        private NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public MenuScript()
        {
            try
            {
                logger.Trace("Loading Main Menu");
                pool = new MenuPool();
                logger.Trace("Menu Pool created");
                mainMenu = new UIMenu("NoArtifactLights", Strings.MenuMainTitle);
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
                logger.Trace("All instances initialized");
                mainMenu.AddItem(itemLights);
                mainMenu.AddItem(itemSave);
                mainMenu.AddItem(itemLoad);
                mainMenu.AddItem(itemCallCops);
                mainMenu.AddItem(itemDifficulty);
                mainMenu.AddItem(itemKills);
                mainMenu.AddItem(itemDeposit);
                mainMenu.AddItem(itemWithdraw);
                mainMenu.AddItem(itemCash);
                mainMenu.AddItem(itemBank);
                logger.Trace("Refreshing Index");
                mainMenu.RefreshIndex();
                pool.Add(mainMenu);
                logger.Trace("Main Menu Done");
                Tick += MenuScript_Tick;
                KeyDown += MenuScript_KeyDown;
                itemLights.CheckboxEvent += ItemLights_CheckboxEvent;
                itemSave.Activated += ItemSave_Activated;
                itemLoad.Activated += ItemLoad_Activated;
                itemCallCops.Activated += ItemCallCops_Activated;
                itemDeposit.Activated += ItemDeposit_Activated;
                itemWithdraw.Activated += ItemWithdraw_Activated;
                cashBar = new TextTimerBar("Cash", "$0");
               // Common.CashChanged += Common_CashChanged;

                Common.Unload += Common_Unload;

                logger.Trace("Loading Ammu-Nation Menu");

                buyMenu = new UIMenu(Strings.AmmuTitle, Strings.AmmuSubtitle);
                itemPistol = WeaponShopManager.GenerateWeaponSellerItem(Strings.AmmuPistol, Strings.AmmuPistolSubtitle, 100);
                itemPumpShotgun = WeaponShopManager.GenerateWeaponSellerItem(Strings.AmmuPumpShotgun, Strings.AmmuPumpShotgunSubtitle, 200);
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

        private void ItemBodyArmor_Activated(UIMenu sender, UIMenuItem selectedItem) => WeaponShopManager.SellArmor(50, 380);
        private void ItemPumpShotgun_Activated(UIMenu sender, UIMenuItem selectedItem) => WeaponShopManager.SellWeapon(200, 50, WeaponHash.PumpShotgun);
        private void ItemPistol_Activated(UIMenu sender, UIMenuItem selectedItem) => WeaponShopManager.SellWeapon(100, 100, WeaponHash.Pistol);

        private void Common_CashChanged(object sender, EventArgs e)
        {
            // cashBar.Text = $"${Common.Cash}";
        }

        private void ItemCallCops_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            Function.Call(Hash.CREATE_INCIDENT, 7, Game.Player.Character.Position.X, Game.Player.Character.Position.Y, Game.Player.Character.Position.Z, 2, 3.0f, new OutputArgument());
        }

        private void ItemLoad_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            SaveManager.Load();
            itemLights.Checked = Common.blackout;
            itemDifficulty.SetRightLabel(Strings.ResourceManager.GetString("Difficulty" + Common.difficulty.ToString()));
            itemKills.SetRightLabel(Common.counter.ToString());
            itemCash.SetRightLabel("$" + Common.Cash.ToString());
            Notification.Show(Strings.GameLoaded);
        }

        private void ItemSave_Activated(UIMenu sender, UIMenuItem selectedItem) => SaveManager.Save(itemLights.Checked);

        private void ItemLights_CheckboxEvent(UIMenuCheckboxItem sender, bool Checked)
        {
            Function.Call(Hash.SET_ARTIFICIAL_LIGHTS_STATE, Checked);
        }

        private void MenuScript_Tick(object sender, EventArgs e)
        {
            pool.ProcessMenus();
            if (WeaponShopManager.DistanceToAmmu())
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
                    if (WeaponShopManager.DistanceToAmmu())
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