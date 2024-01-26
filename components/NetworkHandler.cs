using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace LandmineScrapPlugin.components
{
	public class CustomNetworkHandler : NetworkBehaviour
	{
		public static CustomNetworkHandler Instance { get; private set; }

		public override void OnNetworkSpawn()
		{
			if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
			{
				Instance?.gameObject.GetComponent<NetworkObject>().Despawn();
			}
			Instance = this;

			base.OnNetworkSpawn();
		}

		[ServerRpc(RequireOwnership = false)]
		public void DeactivateMineServerRPC(ulong landmineNetworkID)
		{
			GameObject mine = GetNetworkObject(landmineNetworkID).gameObject;
			Vector3 minePosition = mine.transform.position;

			LandmineScrapBase.Instance.mls.LogInfo("Disarming landmine at " + minePosition);

			// Spawn the scrap version
			GameObject mineScrap = Instantiate(LandmineScrapBase.Instance.landmineScrapItem.spawnPrefab, minePosition, Quaternion.identity);
			if (mineScrap == null)
			{
				LandmineScrapBase.Instance.mls.LogError("Failed to instantiate the mine scrap!");
			}

			mineScrap.GetComponent<NetworkObject>().Spawn();
			
			// Destroy the landmine
			LandmineScrapBase.Instance.mls.LogInfo("Attempting to destroy landmine!");
			Destroy(mine);

			// Manually calc scrap value
			PhysicsProp physicsProp = mineScrap.GetComponent<PhysicsProp>();
			int minValue = physicsProp.itemProperties.minValue;
			int maxValue = physicsProp.itemProperties.maxValue;
			int scrapValue = UnityEngine.Random.Range(minValue, maxValue);

			SetLandmineScrapValueClientRPC(mineScrap.GetComponent<NetworkObject>().NetworkObjectId, scrapValue);
		}

		[ClientRpc]
		public void SetLandmineScrapValueClientRPC(ulong scrapNetworkID, int scrapValue)
		{
			GameObject mineScrap = GetNetworkObject(scrapNetworkID).gameObject;

			// Set Scrap values
			ScanNodeProperties scanNodeProps = mineScrap.GetComponentInChildren<ScanNodeProperties>();
			scanNodeProps.scrapValue = scrapValue;
			scanNodeProps.subText = "Value: $" + scrapValue;

			PhysicsProp physicsProp = mineScrap.GetComponent<PhysicsProp>();
			physicsProp.scrapValue = scrapValue;
		}
	}
}
