using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TreeStatus
{
    [BepInPlugin("uk.co.oliapps.valheim.treestatus", "Tree Status", "0.0.3")]
    public class TreeStatus : BaseUnityPlugin
    {
        public enum DisplayType { HealthBar, Percentage, Disabled }

        private static ConfigEntry<DisplayType> displayType;
        public void Awake()
        {
            displayType = Config.Bind("General", "Display Type", DisplayType.HealthBar, "Desired display type for tree health status");
            Config.Save();
            Harmony.CreateAndPatchAll(typeof(TreeStatus), null);
        }


        [HarmonyPatch(typeof(TreeBase), "RPC_Damage")]
        [HarmonyPrefix]
        public static bool TreeBase_Prefix_RPC_Damage(ref TreeBase __instance, long sender, HitData hit)
        {
            if (!displayType.Value.Equals(DisplayType.Disabled))
            {
                if (__instance)
                {
                    var m_nview = (ZNetView)AccessTools.Field(typeof(TreeBase), "m_nview").GetValue(__instance);
                    if (m_nview)
                    {
                        float remainingHealth = m_nview.GetZDO().GetFloat("health", __instance.m_health);
                        float remainingPercentage = remainingHealth / __instance.m_health * 100;
                        Chat.instance.SetNpcText(__instance.gameObject, Vector3.up, 0, 5.0f, "", GetPercentageString(remainingPercentage), false);
                    }
                }
            }
            return true;
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
