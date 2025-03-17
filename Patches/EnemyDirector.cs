using HarmonyLib;
using SpawnManager.Managers;

namespace SpawnManager.Patches
{
    [HarmonyPatch(typeof(EnemyDirector))]
    public static class EnemyDirectorPatches
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void EnemyDirectorStartPostfix()
        {
            if (!SemiFunc.RunIsLevel())
            {
                EnemyManager.RefreshAllEnemyNames();
                return;
            }
            
            Settings.Logger.LogDebug("Not on menu level, removing enemies.");
            EnemyManager.RemoveEnemies();
        }
    }
}