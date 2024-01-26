using HarmonyLib;
using LandmineScrapPlugin.components;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace LandmineScrapPlugin.Patches
{
	[HarmonyPatch]
	public class NetworkObjectManager
	{
		static GameObject networkPrefab;

		[HarmonyPostfix, HarmonyPatch(typeof(GameNetworkManager), "Start")]
		public static void Init()
		{
			if (networkPrefab != null)
			{
				return;
			}

			networkPrefab = LandmineScrapBase.Instance.landmineNetworkHandler;
			networkPrefab.AddComponent<CustomNetworkHandler>();

			NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);
		}

		[HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), "Awake")]
		static void SpawnNetworkHandler()
		{
			if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
			{
				var networkHandlerHost = UnityEngine.Object.Instantiate(networkPrefab, Vector3.zero, Quaternion.identity);
				networkHandlerHost.GetComponent<NetworkObject>().Spawn();
			}
		}
	}
}
