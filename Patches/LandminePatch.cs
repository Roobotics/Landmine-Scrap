using HarmonyLib;
using LandmineScrapPlugin.components;
using System;
using System.Collections.Generic;
using System.Text;

namespace LandmineScrapPlugin.Patches
{
	[HarmonyPatch(typeof(Landmine))]
	internal class LandminePatch
	{
		[HarmonyPostfix, HarmonyPatch(typeof(Landmine), "Start")]
		public static void addInteraction(Landmine __instance)
		{
			CustomLandmines.addInteractionTrigger(__instance);
		}
	}
}
