using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TreeStatus
{
    [BepInPlugin("uk.co.oliapps.valheim.treestatus", "Tree Status", "1.0.0")]
    public class TreeStatus : BaseUnityPlugin
    {
        public enum DisplayType { HealthBar, Percentage, Disabled }

        private static ConfigEntry<DisplayType> displayType;
        private static ConfigEntry<bool> modEnabled;
        private static ConfigEntry<bool> showOnLog;
        private static ConfigEntry<bool> showOnTree;

        public void Awake()
        {
            displayType = Config.Bind("General", "Display Type", DisplayType.HealthBar, "Desired display type for tree health status");
            modEnabled = Config.Bind("General", "Mod enabled", true, "Sets whether or not the mod is enabled");
            showOnLog = Config.Bind("General", "Show on logs", true, "Sets whether not to show damage visual on felled logs");
            showOnTree = Config.Bind("General", "Show on trees", true, "Sets whether not to show damage visual on trees");
            Config.Save();
            if (modEnabled.Value)
            {
                Harmony.CreateAndPatchAll(typeof(TreeStatus), null);
            }
        }


        [HarmonyPatch(typeof(TreeBase), "RPC_Damage")]
        [HarmonyPostfix]
        public static void TreeBase_Postfix_RPC_Damage(ref TreeBase __instance, long sender, HitData hit)
        {
            if (__instance && showOnTree.Value)
            {
                if (!displayType.Value.Equals(DisplayType.Disabled))
                {
                    var m_nview = AccessTools.Field(typeof(TreeBase), "m_nview").GetValue(__instance) as ZNetView;
                    if (m_nview.IsValid())
                    { 
                        float remainingHealth = m_nview.GetZDO().GetFloat("health", __instance.m_health);
                        float remainingPercentage = remainingHealth / __instance.m_health * 100;
                        Chat.instance.SetNpcText(__instance.gameObject, Vector3.up, 0, 5.0f, "", GetPercentageString(remainingPercentage), false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(TreeLog), "RPC_Damage")]
        [HarmonyPostfix]
        public static void TreeLog_Postfix_RPC_Damage(ref TreeLog __instance, long sender, HitData hit)
        {
            if (__instance && showOnLog.Value)
            {
                if (!displayType.Value.Equals(DisplayType.Disabled))
                {
                    var m_nview = AccessTools.Field(typeof(TreeLog), "m_nview").GetValue(__instance) as ZNetView;
                    if (m_nview.IsValid())
                    {
                        float remainingHealth = m_nview.GetZDO().GetFloat("health", __instance.m_health);
                        float remainingPercentage = remainingHealth / __instance.m_health * 100;
                        Chat.instance.SetNpcText(__instance.gameObject, Vector3.up, 0, 5.0f, "", GetPercentageString(remainingPercentage), false);
                    }
                }
            }
        }

        private static string GetPercentageString(float percentage)
        {
            switch (displayType.Value)
            {
                case DisplayType.HealthBar:
                    float percentageBase2 = percentage / 10f;
                    string progress = "";
                    for (int i = 0; i < 10; i++)
                    {
                        if (i <= percentageBase2)
                        {
                            progress += "<color=#00CC00>█</color>";
                        }
                        else
                        {
                            progress += "<color=#CC0000>█</color>";
                        }
                    }
                    return progress;
                case DisplayType.Percentage:
                default:
                    return $"<color=orange>{Convert.ToInt32(percentage)}% left</color>";

            }
            
        }
    }
}
