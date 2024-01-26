using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalLib.Modules;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LandmineScrapPlugin
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class LandmineScrapBase : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        public static LandmineScrapBase Instance;

        internal Item landmineScrapItem;
        internal GameObject landmineNetworkHandler;

        internal ManualLogSource mls;

        private void Awake()
        {
            if (Instance == null)
			{
                Instance = this;
			}

            NetcodePatcher();

            mls = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID);

            string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "landminemod");
            AssetBundle bundle = AssetBundle.LoadFromFile(assetDir);
            if (bundle == null)
			{
                mls.LogError("Failed to load asset bundle");
                return;
			}

            mls.LogInfo("Asset Bundle Assets: " + bundle.GetAllAssetNames().Join<string>());

            landmineScrapItem = bundle.LoadAsset<Item>("Assets/LandmineResources/LandmineItem.asset");
            landmineNetworkHandler = bundle.LoadAsset<GameObject>("Assets/LandmineResources/LandmineNetworkHandler.prefab");
            if (landmineScrapItem == null || landmineNetworkHandler == null)
			{
                mls.LogError("Failed to load custom assets");
                return;
            }
            
            NetworkPrefabs.RegisterNetworkPrefab(landmineScrapItem.spawnPrefab);
            Utilities.FixMixerGroups(landmineScrapItem.spawnPrefab);
            Items.RegisterScrap(landmineScrapItem, 0);

            mls.LogInfo("LandmineScrap has awoken!");

            harmony.PatchAll();
        }

        private static void NetcodePatcher()
		{
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
}
