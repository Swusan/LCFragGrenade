using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Dissonance;
using HarmonyLib;
using LethalLib.Modules;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Netcode.Samples;
using UnityEngine;
using NetworkPrefabs = LethalLib.Modules.NetworkPrefabs;

namespace LethalFragGrenade
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency(LethalLib.Plugin.ModGUID)] 
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource TheLogger;
        private Harmony _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        private void Awake()
        {
            Keyframe[] ks1 = new Keyframe[2];
            ks1[0] = new Keyframe(0, 0, 2, 2);
            ks1[1] = new Keyframe(1, 1, 0, 0);
            
            Keyframe[] ks2 = new Keyframe[5];
            ks2[0] = new Keyframe(0, 0, 0.1169085f, 0.1169085f, 0, 0.2723074f);
            ks2[1] = new Keyframe(0.4908112f, 1, 4.114658f, -1.81379f,  0.07234045f, 0.2831973f);
            ks2[2] = new Keyframe(0.7587703f, 1, 1.412347f, -1.367884f, 0.3199719f, 0.5691786f);
            ks2[3] = new Keyframe(0.9393898f, 1, 0.826548f, -0.02902175f, 0.5374745f, 1);
            ks2[4] = new Keyframe(1, 1);
            
            Keyframe[] ks3 = new Keyframe[3];
            ks3[0] = new Keyframe(0, 0, 0.1169085f, 0.1169085f, 0, 0.2723074f);
            ks3[1] = new Keyframe(0.4908112f, 1, 4.114658f, 0.06098772f, 0.07234045f, 0.2076876f);
            ks3[2] = new Keyframe(0.9393898f, 1, 0.06394797f, -0.02902175f, 0.1980713f, 1);
            
            
            TheLogger = Logger;
            // Plugin startup logic
            string modPath = Path.GetDirectoryName(Info.Location);
            AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(modPath, "grenadeassetbundle")); 
            Logger.LogInfo("Asset Bundle Loaded");
            Item grenade = bundle.LoadAsset<Item>("GrenadeItem");
            GameObject grenadeObject = bundle.LoadAsset<GameObject>("Sphere");
            Utilities.FixMixerGroups(grenadeObject);
            Logger.LogInfo("Item Loaded");
            Logger.LogInfo("Animation Curves Curved");
            var fg = grenadeObject.AddComponent<FragGrenade>();
            fg.itemProperties = grenade;
            fg.grabbable = true;
            fg.grabbableToEnemies = false;
            
            fg.grenadeFallCurve = new AnimationCurve(ks1);
            fg.grenadeVerticalFallCurve = new AnimationCurve(ks2);
            fg.grenadeVerticalFallCurveNoBounce = new AnimationCurve(ks3);
            InitializeNetworkBehaviours();
            NetworkPrefabs.RegisterNetworkPrefab(grenadeObject);
            
            Logger.LogInfo(fg.GetComponent<NetworkObject>());
            Logger.LogInfo("Prefab Prefabed");
            Items.RegisterShopItem(grenade,100);
            Logger.LogInfo("Shop Shopped");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            
        }
        private static void InitializeNetworkBehaviours() {
            // See https://github.com/EvaisaDev/UnityNetcodePatcher?tab=readme-ov-file#preparing-mods-for-patching
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (MethodInfo method in methods)
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
