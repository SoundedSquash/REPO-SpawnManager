using HarmonyLib;
using SpawnManager.Managers;

namespace SpawnManager.Patches
{
    [HarmonyPatch(typeof(EnemyDirector))]
    public static class EnemyDirectorPatches
    {
        [HarmonyPatch(nameof(EnemyDirector.AmountSetup))]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.Last)]
        static void EnemyDirectorAmountSetupPrefix(EnemyDirector __instance)
        {
            EnemyManager.RemoveEnemies();
        }
    }
}