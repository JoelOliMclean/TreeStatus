using BepInEx;
using HarmonyLib;

namespace TreeStatus
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class TreeStatus : BaseUnityPlugin
    {
        public const string GUID = "uk.co.oliapps.valheim.treestatus";
        public const string NAME = "Tree Status";
        public const string VERSION = "1.1.0";

        public void Awake()
        {
            TreeStatusConfig.SetupConfig(Config);
            if (TreeStatusConfig.modEnabled.Value && !DisplayType.Disabled.Equals(TreeStatusConfig.displayType.Value))
            {
                if (TreeStatusConfig.showOnTree.Value)
                {
                    Harmony.CreateAndPatchAll(typeof(PatchTreeBase), null);
                }
                if (TreeStatusConfig.showOnLog.Value)
                {
                    Harmony.CreateAndPatchAll(typeof(PatchTreeLog), null);
                }
                if (TreeStatusConfig.showOnOther.Value)
                {
                    Harmony.CreateAndPatchAll(typeof(PatchDestructible), null);
                }
            }
        }
    }
}
