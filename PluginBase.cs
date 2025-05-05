using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SpawnManager.Managers;

namespace SpawnManager
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion), BepInDependency(Constants.MenuLibGuid, BepInDependency.DependencyFlags.SoftDependency)]
    public class MuteToggleBase : BaseUnityPlugin
    {
        private const string PluginGuid = "soundedsquash.spawnmanager";
        private const string PluginName = "Enemy/Valuable Spawn Manager";
        private const string PluginVersion = "0.5.5.0";
        
        private readonly Harmony _harmony = new Harmony(PluginGuid);

        private static readonly ManualLogSource ManualLogSource = BepInEx.Logging.Logger.CreateLogSource(PluginGuid);

        public void Awake()
        {
            // Initialize global objects
            Settings.Initialize(Config, ManualLogSource);

            // Initialize menu if MenuLib is installed
            if (PluginManager.IsPluginInstalled(Constants.MenuLibGuid))
            {
                MenuModManager.Initialize();
            }
            else
            {
                Settings.Logger.LogWarning("MenuLib not loaded. Cannot add button to main menu.");
            }

            _harmony.PatchAll();
            ManualLogSource.LogInfo($"{PluginName} loaded");
        }
    }
}