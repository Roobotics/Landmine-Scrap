using HarmonyLib;
using LandmineScrapPlugin.components;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

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

		[HarmonyPostfix, HarmonyPatch(typeof(Landmine), "Detonate")]
		public static void remove(Landmine __instance)
		{
			CustomNetworkHandler.Instance.RemoveLandmineServerRPC(__instance.transform.parent.GetComponent<NetworkObject>().NetworkObjectId);
		}
	}
}
