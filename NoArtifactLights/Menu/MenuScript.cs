using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using NoArtifactLights.Managers;
using NoArtifactLights.Resources;
using System;
using System.IO;
using System.Windows.Forms;

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
        private UIMenuCheckboxItem itemLights;
        private UIMenuItem itemCash;
        private int unlockHour;
        private bool selled;

        private UIMenu buyMenu;
        private UIMenuItem itemPistol;
        private UIMenuItem itemPumpShotgun;
        private UIMenuItem itemBodyArmor;

        private Vector3 ammu = new Vector3(18.18945f, -1120.384f, 28.91654f);
        private Vector3 repair = new Vector3(140.683f, -1081.387f, 28.56039f);

        private Logger logger = Common.logger;

        public MenuScript()
        {
            try
            {
                logger.Log("Loading Main Menu", "MenuScript");
                pool = new MenuPool();
                logger.Log("Menu Pool created", "MenuScript");
                mainMenu = new UIMenu("NoArtifactLights", Strings.MenuMainTitle);
                itemLights = new UIMenuCheckboxItem(Strings.ItemLightsTitle, true, Strings.ItemLightsSubtitle);
                itemSave = new UIMenuItem(Strings.ItemSaveTitle, Strings.ItemSaveSubtitle);
                itemLoad = new UIMenuItem(Strings.ItemLoadTitle, Strings.ItemLoadSubtitle);
                itemCallCops = new UIMenuItem(Strings.ItemCopsTitle, Strings.ItemCopsSubtitle);
                itemDifficulty = new UIMenuItem(Strings.ItemDifficulty, Strings.ItemDIfficultySubtitle);
                itemKills = new UIMenuItem(Strings.ItemKills, Strings.ItemKillsSubtitle);
                itemCash = new UIMenuItem(Strings.ItemCashTitle, Strings.ItemCashSubtitle);
                logger.Log("All instances initialized", "MenuScript");
                mainMenu.AddItem(itemLights);
                mainMenu.AddItem(itemSave);
                mainMenu.AddItem(itemLoad);
                mainMenu.AddItem(itemCallCops);
                mainMenu.AddItem(itemDifficulty);
                mainMenu.AddItem(itemKills);
                mainMenu.AddItem(itemCash);
                logger.Log("Refreshing Index", "MenuScript");
                mainMenu.RefreshIndex();
                pool.Add(mainMenu);
                logger.Log("Main Menu Done", "MenuScript");
                Tick += MenuScript_Tick;
                KeyDown += MenuScript_KeyDown;
                itemLights.CheckboxEvent += ItemLights_CheckboxEvent;
                itemSave.Activated += ItemSave_Activated;
                itemLoad.Activated += ItemLoad_Activated;
                itemCallCops.Activated += ItemCallCops_Activated;
                cashBar = new TextTimerBar("Cash", "$0");
                Common.CashChanged += Common_CashChanged;

                Common.Unload += Common_Unload;

                logger.Log("Loading Ammu-Nation Menu", "MenuScript");

                buyMenu = new UIMenu(Strings.AmmuTitle, Strings.AmmuSubtitle);
                itemPistol = WeaponShopManager.GenerateWeaponSellerItem(Strings.AmmuPistol, Strings.AmmuPistolSubtitle, 100);
                itemPumpShotgun = WeaponShopManager.GenerateWeaponSellerItem(Strings.AmmuPumpShotgun, Strings.AmmuPumpShotgunSubtitle, 200);
                itemBodyArmor = new UIMenuItem(Strings.WeaponBodyArmor, Strings.WeaponBodyArmorDescription);
                itemBodyArmor.SetRightLabel("$380");
                logger.Log("Instances created", "MenuScript");
                buyMenu.AddItem(itemCash);
                buyMenu.AddItem(itemPistol);
                buyMenu.AddItem(itemPumpShotgun);
                buyMenu.AddItem(itemBodyArmor);
                buyMenu.RefreshIndex();
                pool.Add(buyMenu);
                itemPistol.Activated += ItemPistol_Activated;
                itemPumpShotgun.Activated += ItemPumpShotgun_Activated;
                itemBodyArmor.Activated += ItemBodyArmor_Activated;

                Blip repairBlip = World.CreateBlip(repair);
                repairBlip.IsFriendly = true;
                repairBlip.IsShortRange = true;
                repairBlip.Sprite = BlipSprite.Garage;
                repairBlip.Color = BlipColor.Blue;
                repairBlip.Name = Strings.RepairBlip;
            }
            catch (Exception ex)
            {
                UI.ShowHelpMessage(Strings.ExceptionMenu);
                File.WriteAllText("MENUEXCEPTION.TXT", $"Exception caught: \r\n{ex.GetType().Name}\r\nException Message:\r\n{ex.Message}\r\nException StackTrace:\r\n{ex.StackTrace}");
                Common.UnloadMod(this);
                this.Abort();
            }
        }

        private void Common_Unload(object sender, EventArgs e)
        {
            if(!sender.Equals(this))
            {
                mainMenu.Visible = false;
                buyMenu.Visible = false;
                Tick -= MenuScript_Tick;
                KeyDown -= MenuScript_KeyDown;
                itemLights.CheckboxEvent -= ItemLights_CheckboxEvent;
                itemSave.Activated -= ItemSave_Activated;
                itemLoad.Activated -= ItemLoad_Activated;
                itemCallCops.Activated -= ItemCallCops_Activated;
                Abort();
            }
        }

        private void ItemBodyArmor_Activated(UIMenu sender, UIMenuItem selectedItem) => WeaponShopManager.SellArmor(50, 380);
        private void ItemPumpShotgun_Activated(UIMenu sender, UIMenuItem selectedItem) => WeaponShopManager.SellWeapon(200, 50, WeaponHash.PumpShotgun);
        private void ItemPistol_Activated(UIMenu sender, UIMenuItem selectedItem) => WeaponShopManager.SellWeapon(100, 100, WeaponHash.Pistol);

        private void Common_CashChanged(object sender, EventArgs e)
        {
            cashBar.Text = $"${Common.cash}";
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
            itemCash.SetRightLabel("$" + Common.cash.ToString());
            UI.Notify(Strings.GameLoaded);
        }

        private void ItemSave_Activated(UIMenu sender, UIMenuItem selectedItem) => SaveManager.Save(itemLights.Checked);

        private void ItemLights_CheckboxEvent(UIMenuCheckboxItem sender, bool Checked)
        {
            Function.Call(Hash._SET_BLACKOUT, Checked);
        }

        private void MenuScript_Tick(object sender, EventArgs e)
        {
            pool.ProcessMenus();
            if (WeaponShopManager.DistanceToAmmu())
            {
                UI.ShowHelpMessage(Strings.AmmuOpenShop);
            }
            if (selled && World.CurrentDayTime.Hours == unlockHour)
            {
                selled = false;
            }
            if (repair.DistanceTo(Game.Player.Character.Position) <= 10f && Game.Player.Character.IsInVehicle())
            {
                UI.ShowHelpMessage(Strings.RepairHelp);
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
                    itemCash.SetRightLabel("$" + Common.cash.ToString());
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
                        if(Common.cash < 100)
                        {
                            UI.ShowSubtitle(Strings.BuyNoMoney);
                            return;
                        }
                        if(Game.Player.Character.CurrentVehicle.IsDamaged == false)
                        {
                            UI.ShowSubtitle(Strings.RepairUndamaged);
                            return;
                        }
                        Common.cash -= 100;
                        Game.Player.Character.CurrentVehicle.Repair();
                        UI.ShowSubtitle(Strings.RepairSuccess);
                    }
                    break;
            }
        }
    }
}