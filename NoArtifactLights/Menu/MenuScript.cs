using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;
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

        private UIMenu buyMenu;
        private UIMenuItem itemPistol;
        private UIMenuItem itemPumpShotgun;

        private Vector3 ammu = new Vector3(18.18945f, -1120.384f, 28.91654f);
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
                itemPistol = new UIMenuItem(Strings.AmmuPistol, Strings.AmmuPistolSubtitle);
                itemPistol.SetRightLabel("$100");
                itemPumpShotgun = new UIMenuItem(Strings.AmmuPumpShotgun, Strings.AmmuPumpShotgunSubtitle);
                itemPumpShotgun.SetRightLabel("$200");
                buyMenu.AddItem(itemCash);
                buyMenu.AddItem(itemPistol);
                buyMenu.AddItem(itemPumpShotgun);
                buyMenu.RefreshIndex();
                pool.Add(buyMenu);
                itemPistol.Activated += ItemPistol_Activated;
                itemPumpShotgun.Activated += ItemPumpShotgun_Activated;
            }
            catch (Exception ex)
            {
                UI.ShowHelpMessage(Strings.ExceptionMenu);
                File.WriteAllText("MENUEXCEPTION.TXT", $"Exception caught: \r\n{ex.GetType().Name}\r\nException Message:\r\n{ex.Message}\r\nException StackTrace:\r\n{ex.StackTrace}");
                this.Abort();
            }
        }

        private void ItemPumpShotgun_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            if (Common.cash < 200)
            {
                UI.ShowSubtitle(Strings.BuyNoMoney);
                return;
            }
            Common.cash -= 200;
            if (!Game.Player.Character.Weapons.HasWeapon(WeaponHash.PumpShotgun))
            {
                Game.Player.Character.Weapons.Give(WeaponHash.PumpShotgun, 20, true, true);
                return;
            }
            Game.Player.Character.Weapons.Select(WeaponHash.PumpShotgun);
            Game.Player.Character.Weapons.Current.Ammo += 20;
        }

        private void ItemPistol_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            if (Common.cash < 100)
            {
                UI.ShowSubtitle(Strings.BuyNoMoney);
                return;
            }
            Common.cash -= 100;
            if (!Game.Player.Character.Weapons.HasWeapon(WeaponHash.Pistol))
            {
                Game.Player.Character.Weapons.Give(WeaponHash.Pistol, 50, true, true);
                return;
            }
            Game.Player.Character.Weapons.Select(WeaponHash.Pistol);
            Game.Player.Character.Weapons.Current.Ammo += 50;
        }

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
            SaveFile sf;
            if (!File.Exists("NALSave.xml"))
            {
                UI.Notify(Strings.NoSave);
                return;
            }
            FileStream fs = File.OpenRead("NALSave.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(SaveFile));
            sf = (SaveFile)serializer.Deserialize(fs);
            fs.Close();
            fs.Dispose();
            World.Weather = sf.Status.CurrentWeather;
            World.CurrentDayTime = new TimeSpan(sf.Status.Hour, sf.Status.Minute, 0);
            World.SetBlackout(sf.Blackout);
            itemLights.Checked = sf.Blackout;
            Game.Player.Character.Position = new GTA.Math.Vector3(sf.PlayerX, sf.PlayerY, sf.PlayerZ);
            Common.counter = sf.Kills;
            Common.cash = sf.Cash;
            Common.difficulty = sf.CurrentDifficulty;
            Game.Player.Character.Weapons.RemoveAll();
            Game.Player.Character.Weapons.Give(WeaponHash.Flashlight, 1, false, true);
            if (sf.Pistol.Existence)
            {
                Game.Player.Character.Weapons.Give(WeaponHash.Pistol, sf.Pistol.Ammo, true, true);
            }
            if (sf.PumpShotgun.Existence)
            {
                Game.Player.Character.Weapons.Give(WeaponHash.PumpShotgun, sf.PumpShotgun.Ammo, true, true);
            }
            itemDifficulty.SetRightLabel(Strings.ResourceManager.GetString("Difficulty" + Common.difficulty.ToString()));
            itemKills.SetRightLabel(Common.counter.ToString());
            itemCash.SetRightLabel("$" + Common.cash.ToString());
            UI.Notify(Strings.GameLoaded);
        }

        private void ItemSave_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            SaveFile sf = new SaveFile();
            sf.Status = new WorldStatus(World.Weather, World.CurrentDayTime.Hours, World.CurrentDayTime.Minutes);
            sf.PlayerX = Game.Player.Character.Position.X;
            sf.PlayerY = Game.Player.Character.Position.Y;
            sf.PlayerZ = Game.Player.Character.Position.Z;
            sf.Blackout = itemLights.Checked;
            sf.Kills = Common.counter;
            sf.CurrentDifficulty = Common.difficulty;
            sf.Cash = Common.cash;
            bool pistolExists = Game.Player.Character.Weapons.HasWeapon(WeaponHash.Pistol);
            if (pistolExists)
            {
                Game.Player.Character.Weapons.Select(WeaponHash.Pistol);
                sf.Pistol = new SaveWeapon(Game.Player.Character.Weapons.Current.Ammo + Game.Player.Character.Weapons.Current.AmmoInClip, true);
            }
            else
            {
                sf.Pistol = new SaveWeapon(0, false);
            }
            bool pumpExists = Game.Player.Character.Weapons.HasWeapon(WeaponHash.PumpShotgun);
            if (pumpExists)
            {
                Game.Player.Character.Weapons.Select(WeaponHash.PumpShotgun);
                sf.PumpShotgun = new SaveWeapon(Game.Player.Character.Weapons.Current.Ammo + Game.Player.Character.Weapons.Current.AmmoInClip, true);
            }
            else
            {
                sf.PumpShotgun = new SaveWeapon(0, false);
            }
            FileStream fs = File.Create("NALSave.xml");
            XmlSerializer serializer = new XmlSerializer(typeof(SaveFile));
            serializer.Serialize(fs, sf);
            UI.Notify(Strings.GameSaved);
            fs.Close();
            fs.Dispose();
        }

        private void ItemLights_CheckboxEvent(UIMenuCheckboxItem sender, bool Checked)
        {
            Function.Call(Hash._SET_BLACKOUT, Checked);
        }

        private void ItemWithdraw_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            Common.Withdraw(100);
        }

        private void ItemDeposit_Activated(UIMenu sender, UIMenuItem selectedItem)
        {
            Common.Deposit(100);
        }

        private void MenuScript_Tick(object sender, EventArgs e)
        {
            pool.ProcessMenus();
            if (ammu.DistanceTo(Game.Player.Character.Position) <= 15f)
            {
                UI.ShowHelpMessage("");
            }
        }

        private void MenuScript_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.N)
            {
                mainMenu.Visible = !mainMenu.Visible;
                itemDifficulty.SetRightLabel(Strings.ResourceManager.GetString("Difficulty" + Common.difficulty.ToString()));
                itemKills.SetRightLabel(Common.counter.ToString());
                itemCash.SetRightLabel("$" + Common.cash.ToString());
            }
            if (e.KeyCode == Keys.E)
            {
                if (mainMenu.Visible) return;
                if (buyMenu.Visible)
                {
                    buyMenu.Visible = false;
                    return;
                }
                if (ammu.DistanceTo(Game.Player.Character.Position) <= 15f)
                {
                    buyMenu.Visible = true;
                }
            }
        }
    }
}