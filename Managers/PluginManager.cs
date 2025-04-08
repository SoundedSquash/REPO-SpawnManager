using System.Linq;
using BepInEx.Bootstrap;

namespace SpawnManager.Managers
{
    public static class PluginManager
    {
        public static bool IsPluginInstalled(string pluginGuid)
        {
            return Chainloader.PluginInfos.Keys.Any(pluginInfo => pluginInfo == pluginGuid);
        }
    }
}