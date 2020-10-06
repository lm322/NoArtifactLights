// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using System.Drawing;
using GTA;
using GTA.Math;
using GTA.UI;
using LemonUI.Menus;
using NoArtifactLights.Engine.Mod.API;
using NoArtifactLights.Engine.Mod.Scripts;
using NoArtifactLights.Resources;

namespace NoArtifactLights.Engine.Mod.Controller
{
	public enum Foods
	{
		Hamburger,
		Chicken
	}

	public static class HungryController
	{
		public static float Hungry { get; private set; } = 10.0f;
		public static float Water { get; private set; } = 10.0f;

		private static bool hinted = false;
		private static Vector3[] resellers = { new Vector3(386.585f, -872.4678f, 29.2917f), new Vector3(194.2787f, -1764.063f, 29.321f), new Vector3(408.3806f, -1908.731f, 25.50163f) 
			, new Vector3(1.830167f, -1604.896f, 29.25797f)
		};

		internal static void RefillWater()
		{
			Water = 10.0f;
		}

		internal static void WaterLoop()
		{
			if (Water >= 2.0f && hinted) hinted = false;
			if (Water <= 2.0f && !hinted)
			{
				hinted = true;
				GameUI.DisplayHelp(Strings.WaterTooLow);
			}
			if (Water <= 1.5f && !(Water <= 0.5f)) Game.Player.Character.Health -= 1;
			if (Water <= 0.5f) Game.Player.Character.Health -= 10;

			if (Water != 0f)
			{
				if (Game.Player.Character.Health >= 100 && !Game.Player.Character.IsRunning && !Game.Player.Character.IsSprinting)
				{
					MenuScript.ChangeWaterBarColor(Color.Aquamarine);
					Water -= 0.01f;
				}
				else
				{
					MenuScript.ChangeWaterBarColor(Color.OrangeRed);
					Water -= 0.05f;
				}
			}
		}

		internal static void AddHungry(Foods food, float amount)
		{
			float buffer = Hungry;
			buffer += amount;
			if (buffer > 10.0f) buffer = 10.0f;
			Hungry = buffer;
		}

		internal static float ProgressBarStatus => Hungry * 10f;
		internal static float WaterBarStatus => Water * 10f;

		internal static void AlterHungry(int value)
		{
			float buffer = Hungry;
			buffer = value;
			if (buffer > 10.0f) buffer = 10.0f;
			if (buffer < 0f) buffer = 0f;
			Hungry = buffer;
		}

		internal static void ResetHungry()
		{
			Hungry = 10.0f;
		}

		internal static void AddBlipsToAllResellers()
		{
			foreach(Vector3 coord in resellers)
			{
				Blip b = World.CreateBlip(coord);
				b.Sprite = BlipSprite.Store;
				b.Name = "Food Resellers";
				Entry.blips.Add(b);
			}
		}
		internal static bool IsPlayerCloseReseller()
		{
			Vector3 player = Game.Player.Character.Position;
			foreach(Vector3 coord in resellers)
			{
				if (player.DistanceTo(coord) <= 1.5f) return true;
			}
			return false;
		}

		internal static void Loop()
		{
			if (Hungry >= 2.0f && hinted) hinted = false;
			if (Hungry <= 2.0f && !hinted)
			{
				hinted = true;
				GameUI.DisplayHelp(Strings.Hungry);
			}
			if (Hungry <= 1.5f && !(Hungry <= 0.5f)) Game.Player.Character.Health -= 1;
			if (Hungry <= 0.5f) Game.Player.Character.Health -= 10;

			if(Hungry != 0f)
			{
				if(Game.Player.Character.Health >= 100 && !Game.Player.Character.IsSprinting)
				{
					MenuScript.ChangeHungryBarColor(Color.White);	
					Hungry -= 0.01f;
				}
				else
				{
					MenuScript.ChangeHungryBarColor(Color.Red);
					Hungry -= 0.05f;
				}
			}	

		}

		internal static void EatFood(Foods food)
		{
			switch(food)
			{
				default:
					AddHungry(food,0.0f);
					break;
				case Foods.Hamburger:
					AddHungry(food, 2.5f);
					break;
				case Foods.Chicken:
					AddHungry(food, 3.0f);
					break;
			}
		}

		internal static void SellFood(Foods food, int price)
		{
			if(Hungry == 10.0f)
			{
				Screen.ShowSubtitle(Strings.SellNotHungry);
			}
			if (!Common.Cost(price)) return;
			EatFood(food);
		}

		internal static NativeItem CreateFoodSellerItem(string context, Foods food, int price)
		{
			NativeItem result = new NativeItem(context);
			result.AltTitle = "$" + price.ToString();
			result.Activated += (s, i) =>
			{
				SellFood(food, price);
			};
			return result;
		}
	}
}
