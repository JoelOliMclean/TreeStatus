using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TreeStatus
{
    [HarmonyPatch(typeof(Destructible))]
    internal class PatchDestructible
    {
        private static List<string> affectedObjects = new List<string>() { "stub", "bush", "shrub", "oldlog", "beech_small", "tree_small" };

        [HarmonyPatch("RPC_Damage")]
        [HarmonyPostfix]
        internal static void RPC_Damage_Post(ref Destructible __instance, long sender, HitData hit)
        {
            var name = __instance.gameObject.name.ToLower();
            if (affectedObjects.Any(ao => name.Contains(ao)))
            {
                var m_nview = AccessTools.Field(typeof(Destructible), "m_nview").GetValue(__instance) as ZNetView;
                if (m_nview.IsValid())
                {
                    float remainingHealth = m_nview.GetZDO().GetFloat("health", __instance.m_health);
                    float remainingPercentage = remainingHealth / __instance.m_health * 100;
                    if (remainingPercentage > 0.0f)
                    {
                        Chat.instance.SetNpcText(__instance.gameObject, Vector3.up, 0, 5.0f, "", TreeStatusUtils.GetPercentageString(remainingPercentage), false);
                    }
                }
            }
        }
    }
}
