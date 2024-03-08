using System.IO;
using BepInEx;
using BepInEx.Logging;
using LethalLib.Modules;
using UnityEngine;

namespace LethalFragGrenade
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(LethalLib.Plugin.ModGUID)] 
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource TheLogger;
        private void Awake()
        {
            TheLogger = Logger;
            // Keyfram
            Keyframe[] ks = new Keyframe[2];
            ks[0] = new Keyframe(0, 0);
            ks[1] = new Keyframe(1, 1);
            // No longer Keyframes
            // Plugin startup logic
            string modPath = Path.GetDirectoryName(Info.Location);
            AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(modPath, "grenadeassetbundle"));
            Logger.LogInfo("Asset Bundle Loaded");
            Item grenade = bundle.LoadAsset<Item>("GrenadeItem");
            Logger.LogInfo("Item Loaded");
            FragGrenade fg = grenade.spawnPrefab.AddComponent<FragGrenade>();
            fg.grenadeFallCurve = new AnimationCurve(ks);
            fg.grenadeVerticalFallCurve = new AnimationCurve(ks);
            fg.grenadeVerticalFallCurveNoBounce = new AnimationCurve(ks);
            fg.itemProperties = grenade;
            NetworkPrefabs.RegisterNetworkPrefab(grenade.spawnPrefab);
            Logger.LogInfo("Prefab Prefabed");
            Items.RegisterShopItem(grenade, 100);
            Logger.LogInfo("Shop Shopped");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            
        }
    }
}
