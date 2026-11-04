using HarmonyLib;
using SpawnManager.Managers;

namespace SpawnManager.Patches
{
    [HarmonyPatch(typeof(LevelGenerator))]
    public static class LevelGeneratorPatches
    {
        [HarmonyPatch(nameof(LevelGenerator.ItemSetup))]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        static void LevelGeneratorItemSetupPrefix()
        {
            Settings.InitializeItemsLevels();
            ItemsManager.RemoveItems();
        }
    }
}