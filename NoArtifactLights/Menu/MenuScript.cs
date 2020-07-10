using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
using NoArtifactLights.Managers;
using NoArtifactLights.Resources;
using NoArtifactLights.Serialize;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

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

        private UIMenu marketMenu;
        private UIMenuItem itemSellVehicle;

        private Vector3 ammu = new Vector3(18.18945f, -1120.384f, 28.91654f);
        private Vector3 repair = new Vector3(140.683f, -1081.387f, 28.56039f);
        private Vector3[] ammus = { new Vector3(18.18945f, -1120.384f, 28.91654f), new Vector3(-325.6184f, 6072.246f, 31.21228f) };

        public MenuScript()
        {
            try
            {
                pool = new MenuPool();
                mainMenu = new UIMenu("NoArtifactLights", Strings.MenuMainTitle);
                itemLights = new UIMenuCheckboxItem(Strings.ItemLightsTitle, true, Strings.ItemLightsSubtitle);
                itemSave = new UIMenuItem(Strings.ItemSaveTitle, Strings.ItemSaveSubtitle);
                itemLoad = new UIMenuItem(Strings.ItemLoadTitle, Strings.ItemLoadSubtitle);
                itemCallCops = new UIMenuItem(Strings.ItemCopsTitle, Strings.ItemCopsSubtitle);
                itemDifficulty = new UIMenuItem(Strings.ItemDifficulty, Strings.ItemDIfficultySubtitle);
                itemKills = new UIMenuItem(Strings.ItemKills, Strings.ItemKillsSubtitle);
                itemCash = new UIMenuItem(Strings.ItemCashTitle, Strings.ItemCashSubtitle);
                mainMenu.AddItem(itemLights);
                mainMenu.AddItem(itemSave);
                mainMenu.AddItem(itemLoad);
                mainMenu.AddItem(itemCallCops);
                mainMenu.AddItem(itemDifficulty);
                mainMenu.AddItem(itemKills);
                mainMenu.AddItem(itemCash);
                mainMenu.RefreshIndex();
                pool.Add(mainMenu);
                Tick += MenuScript_Tick;
                KeyDown += MenuScript_KeyDown;
                itemLights.CheckboxEvent += ItemLights_CheckboxEvent;
                itemSave.Activated += ItemSave_Activated;
                itemLoad.Activated += ItemLoad_Activated;
                itemCallCops.Activated += ItemCallCops_Activated;
                cashBar = new TextTimerBar("Cash", "$0");
                Common.CashChanged += Common_CashChanged;
                
                buyMenu = new UIMenu(Strings.AmmuTitle, Strings.AmmuSubtitle);
                itemPistol = WeaponShopManager.GenerateWeaponSellerItem(Strings.AmmuPistol, Strings.AmmuPistolSubtitle, 100);
                itemPumpShotgun = WeaponShopManager.GenerateWeaponSellerItem(Strings.AmmuPumpShotgun, Strings.AmmuPumpShotgunSubtitle, 200);
                itemBodyArmor = new UIMenuItem(Strings.WeaponBodyArmor, Strings.WeaponBodyArmorDescription);
                itemBodyArmor.SetRightLabel("$380");
                buyMenu.AddItem(itemCash);
                buyMenu.AddItem(itemPistol);
                buyMenu.AddItem(itemPumpShotgun);
                buyMenu.AddItem(itemBodyArmor);
                buyMenu.RefreshIndex();
                pool.Add(buyMenu);
                itemPistol.Activated += ItemPistol_Activated;
                itemPumpShotgun.Activated += ItemPumpShotgun_Activated;
                itemBodyArmor.Activated += ItemBodyArmor_Activated;

                marketMenu = new UIMenu(Strings.MenuMarketTitle, Strings.MenuMarketSubtitle);
                itemSellVehicle = new UIMenuItem(Strings.ItemSellCarTitle, Strings.ItemSellCarSubtitle);
                marketMenu.AddItem(itemCash);
                marketMenu.AddItem(itemSellVehicle);
                marketMenu.RefreshIndex();
                pool.Add(marketMenu);
                itemSellVehicle.Activated += ItemSellVehicle_Activated;
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
                this.Abort();
            }
        }

        private void ItemBodyArmor_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            WeaponShopManager.SellArmor(50, 380);
        }

        private void ItemSellVehicle_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            marketMenu.Visible = false;
            if(!Game.Player.Character.IsInVehicle())
            {
                UI.ShowHelpMessage(Strings.SellNoCar);
                return;
            }
            Vehicle v = Game.Player.Character.CurrentVehicle;
            if(!v.Exists())
            {
                return;
            }
            v.IsPersistent = true;
            Game.Player.Character.Task.LeaveVehicle();
            v.LockStatus = VehicleLockStatus.Locked;
            v.MarkAsNoLongerNeeded();
            int money = new Random().Next(100, 501);
            Common.cash += money;
            UI.Notify(Strings.SellSuccess);
            selled = true;
            unlockHour = 0;
        }

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
            if (ammu.DistanceTo(Game.Player.Character.Position) <= 15f)
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
                    if (mainMenu.Visible || marketMenu.Visible) return;
                    if (buyMenu.Visible)
                    {
                        buyMenu.Visible = false;
                        return;
                    }
                    if (ammu.DistanceTo(Game.Player.Character.Position) <= 15f)
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
                case Keys.B:
                    if (mainMenu.Visible || buyMenu.Visible) return;
                    if(marketMenu.Visible)
                    {
                        marketMenu.Visible = false;
                        return;
                    }
                    else
                    {
                        marketMenu.Visible = true;
                    }
                    if(selled)
                    {
                        itemSellVehicle.Enabled = false;
                    }
                    break;
            }
        }
    }
}