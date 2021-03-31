
using BepInEx.Configuration;

namespace TreeStatus
{
    internal class TreeStatusConfig
    {

        internal static ConfigEntry<DisplayType> displayType;
        internal static ConfigEntry<bool> modEnabled;
        internal static ConfigEntry<bool> showOnLog;
        internal static ConfigEntry<bool> showOnTree;
        internal static ConfigEntry<bool> showOnOther;

        internal static void SetupConfig(ConfigFile config)
        {
            displayType = config.Bind("General", "Display Type", DisplayType.HealthBar, "Desired display type for tree health status");
            modEnabled = config.Bind("General", "Mod enabled", true, "Sets whether or not the mod is enabled");
            showOnLog = config.Bind("General", "Show on logs", true, "Sets whether not to show damage visual on felled logs");
            showOnTree = config.Bind("General", "Show on trees", true, "Sets whether not to show damage visual on trees");
            showOnOther = config.Bind("General", "Show on mossy logs and stumps", true, "Sets whether or not to show damage visual on spawned logs, shrubs and stumps");

            config.Save();
        }
    }
}
