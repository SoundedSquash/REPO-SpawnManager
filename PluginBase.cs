using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SpawnManager.Managers;

namespace SpawnManager
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class MuteToggleBase : BaseUnityPlugin
    {
        private const string PluginGuid = "soundedsquash.spawnmanager";
        private const string PluginName = "Enemy/Valuable Spawn Manager";
        private const string PluginVersion = "0.2.0.0";
        
        private readonly Harmony _harmony = new Harmony(PluginGuid);

        private static readonly ManualLogSource ManualLogSource = BepInEx.Logging.Logger.CreateLogSource(PluginGuid);

        public void Awake()
        {
            // Initialize global objects
            Settings.Initialize(Config, ManualLogSource);
            
            MenuModManager.Initialize();

            _harmony.PatchAll();
            ManualLogSource.LogInfo($"{PluginName} loaded");
        }
    }
}