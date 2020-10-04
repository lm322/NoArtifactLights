// NoArtifactLights
// (C) RelaperCrystal and contributors. Licensed under GPLv3 or later.

using GTA;
using GTA.Math;
using GTA.UI;
using NativeUI;
using NoArtifactLights.Engine.Mod.API;
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

		private static bool hinted = false;
		private static Vector3[] resellers = { new Vector3(386.585f, -872.4678f, 29.2917f), new Vector3(194.2787f, -1764.063f, 29.321f) };

		internal static void AddHungry(Foods food, float amount)
		{
			float buffer = Hungry;
			buffer += amount;
			if (buffer > 10.0f) buffer = 10.0f;
			Hungry = buffer;
		}

		internal static void ResetHungry()
		{
			Hungry = 10.0f;
		}

		internal static void Loop()
		{
			if (Hungry >= 2.0f && hinted) hinted = false;
			if (Hungry <= 2.0f && !hinted)
			{
				hinted = true;
				GameUI.DisplayHelp(Strings.Hungry);
			}
			if (Hungry <= 1.5f) Game.Player.Character.Health -= 1;
			if (Hungry <= 0.5f) Game.Player.Character.Health -= 10;

			if(Hungry != 0f) Hungry -= 0.001f;

		}

		internal static void EatFood(Foods food)
		{
			switch(food)
			{
				default:
					Hungry += 0.0f;
					break;
				case Foods.Hamburger:
					Hungry += 2.5f;
					break;
				case Foods.Chicken:
					Hungry += 3.0f;
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

		internal static UIMenuItem CreateFoodSellerItem(string context, Foods food, int price)
		{
			UIMenuItem result = new UIMenuItem(context);
			result.SetRightLabel(price.ToString());
			result.Activated += (s, i) =>
			{
				SellFood(food, price);
			};
			return result;
		}
	}
}
