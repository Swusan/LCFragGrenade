﻿using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
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
        private void Awake()
        {
            TheLogger = Logger;
            // Keyframe
            Keyframe[] ks = new Keyframe[2];
            ks[0] = new Keyframe(0, 0);
            ks[1] = new Keyframe(1, 1);
            // No longer Keyframes
            // Plugin startup logic
            string modPath = Path.GetDirectoryName(Info.Location);
            AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(modPath, "grenadeassetbundle")); 
            Logger.LogInfo("Asset Bundle Loaded");
            Item grenade = bundle.LoadAsset<Item>("GrenadeItem");
            GameObject grenadeObject = bundle.LoadAsset<GameObject>("Sphere");
            Utilities.FixMixerGroups(grenadeObject);
            Logger.LogInfo("Item Loaded");
            
            var fg = grenadeObject.AddComponent<FragGrenade>();
            fg.itemProperties = grenade;
            fg.grabbable = true;
            fg.grabbableToEnemies = false;
            fg.grenadeFallCurve = new AnimationCurve(ks);
            fg.grenadeVerticalFallCurve = new AnimationCurve(ks);
            fg.grenadeVerticalFallCurveNoBounce = new AnimationCurve(ks);
            
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
