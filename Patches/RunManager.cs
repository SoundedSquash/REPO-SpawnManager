using HarmonyLib;
using SpawnManager.Managers;

namespace SpawnManager.Patches
{
    [HarmonyPatch(typeof(RunManager))]
    public static class RunManagerPatches
    {
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        static void RunManagerStartPrefix()
        {
            if (SemiFunc.MenuLevel()) return;
            Settings.Logger.LogDebug("Removing valuables.");
            ValuableManager.RemoveValuables();
        }
    }
}