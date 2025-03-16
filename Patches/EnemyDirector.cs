using HarmonyLib;
using SpawnManager.Managers;

namespace SpawnManager.Patches
{
    [HarmonyPatch(typeof(EnemyDirector))]
    public static class EnemyDirectorPatches
    {
        [HarmonyPatch(nameof(EnemyDirector.AmountSetup))]
        [HarmonyPrefix]
        static void EnemyDirectorAmountSetupPrefix(EnemyDirector __instance)
        {
            EnemyManager.RemoveEnemies();
        }
    }
}