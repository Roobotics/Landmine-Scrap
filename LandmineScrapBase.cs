using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace LandmineScrapPlugin
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class LandmineScrapBase : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        public static LandmineScrapBase Instance;

        internal ManualLogSource mls;

        private void Awake()
        {
            if (Instance == null)
			{
                Instance = this;
			}

            mls = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID);
            mls.LogInfo("LandmineScrap has awoken!");

            Assets.PopulateAssets();

            harmony.PatchAll();
        }
    }

    public static class Assets
	{
        public static AssetBundle MainAssetBundle = null;
        public static string mainAssetBundleName = "landmine_scrap";

        private static string GetAssemblyName() => Assembly.GetExecutingAssembly().FullName.Split(',')[0];

        public static void PopulateAssets()
		{
            if (MainAssetBundle == null)
			{
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetAssemblyName() + '.' + mainAssetBundleName))
				{
                    MainAssetBundle = AssetBundle.LoadFromStream(assetStream);

                    ManualLogSource logger = LandmineScrapBase.Instance.mls;

                    string[] allAssetNames = Assets.MainAssetBundle.GetAllAssetNames();
                    string[] allScenePaths = Assets.MainAssetBundle.GetAllScenePaths();

                    logger.LogInfo($"MainAssetBundle loaded with: {string.Join(", ", allAssetNames)}");
                    logger.LogInfo($"MainAssetBundle loaded as: {string.Join(", ", allScenePaths)}");
                    logger.LogInfo($"MainAssetBundle loaded as: {Assets.MainAssetBundle.name}");
                }
			}
		}
	}
}
