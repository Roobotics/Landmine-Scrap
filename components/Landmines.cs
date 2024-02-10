using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace LandmineScrapPlugin.components
{
	internal class CustomLandmines
	{
		public static void addInteractionTrigger(Landmine mine)
		{
			GameObject mineObject = mine.gameObject;

			mineObject.GetComponent<MeshRenderer>();
			mineObject.tag = "InteractTrigger";
			mineObject.layer = LayerMask.NameToLayer("InteractableObject");
			var trigger = mineObject.AddComponent<InteractTrigger>();

			// Add interaction stuff
			trigger.hoverIcon = GameObject.Find("StartGameLever")?.GetComponent<InteractTrigger>()?.hoverIcon;
			trigger.hoverTip = "Disarm : [LMB]";
			trigger.interactable = true;
			trigger.oneHandedItemAllowed = false;
			trigger.twoHandedItemAllowed = false;
			trigger.holdInteraction = true;
			trigger.timeToHold = 5f;
			trigger.timeToHoldSpeedMultiplier = 1f;

			// Create new instances of InteractEvent for each trigger
			trigger.holdingInteractEvent = new InteractEventFloat();
			trigger.onInteract = new InteractEvent();
			trigger.onInteractEarly = new InteractEvent();
			trigger.onStopInteract = new InteractEvent();
			trigger.onCancelAnimation = new InteractEvent();

			trigger.onInteract.AddListener((player) => CustomLandmines.RollDisarm(mineObject, player));
			trigger.triggerOnce = true;

		}

		private static void RollDisarm(GameObject mineObject, PlayerControllerB player)
		{
			int failThreshold = 34;
			int roll = UnityEngine.Random.Range(0, 100);

			LandmineScrapBase.Instance.mls.LogInfo("Rolled chance: " + roll);

			if (roll <= failThreshold)
			{
				mineObject.GetComponent<Landmine>().ExplodeMineServerRpc();
				return;
			} else
			{
				CustomNetworkHandler.Instance.DeactivateMineServerRPC(mineObject.transform.parent.GetComponent<NetworkObject>().NetworkObjectId);
				return;
			}
		}
	}
}
