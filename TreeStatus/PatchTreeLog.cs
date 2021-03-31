using HarmonyLib;
using UnityEngine;

namespace TreeStatus
{
    [HarmonyPatch(typeof(TreeLog))]
    internal class PatchTreeLog
    {
        [HarmonyPatch("RPC_Damage")]
        [HarmonyPostfix]
        internal static void TreeLog_Postfix_RPC_Damage(ref TreeLog __instance, long sender, HitData hit)
        {
            var m_nview = AccessTools.Field(typeof(TreeLog), "m_nview").GetValue(__instance) as ZNetView;
            if (m_nview.IsValid())
            {
                float remainingHealth = m_nview.GetZDO().GetFloat("health", __instance.m_health);
                float remainingPercentage = remainingHealth / __instance.m_health * 100;
                Chat.instance.SetNpcText(__instance.gameObject, Vector3.up, 0, 5.0f, "", TreeStatusUtils.GetPercentageString(remainingPercentage), false);
            }
        }
    }
}
