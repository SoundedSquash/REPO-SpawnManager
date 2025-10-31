using System.Collections.Generic;
using HarmonyLib;
using SpawnManager.Managers;
using UnityEngine;

namespace SpawnManager.Patches
{
    [HarmonyPatch(typeof(EnemyDirector))]
    public static class EnemyDirectorPatches
    {
        private static List<EnemySetup> _startingEnemyList = new List<EnemySetup>();
        private static bool _isListSet;
        
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void EnemyDirectorStartPostfix()
        {
            EnemyManager.RefreshAllEnemyNames();

            if (!SemiFunc.RunIsLevel()) return;
            
            Settings.Logger.LogDebug("Not on menu level, removing enemies.");
            EnemyManager.RemoveEnemies();
        }

        [HarmonyPatch("PickEnemies")]
        [HarmonyPostfix]
        static void EnemyDirectorPickEnemiesPostfix(ref List<EnemySetup> ___enemyList)
        {
            if (___enemyList.Count == 0) return;

            // Remove all null entries from the enemy list
            ___enemyList.RemoveAll(e => e == null);

            Settings.Logger.LogDebug($"Level starting with enemies: " + string.Join(", ", ___enemyList));
        }
        
        [HarmonyPatch(nameof(EnemyDirector.GetEnemy))]
        [HarmonyPrefix]
        static void EnemyDirectorGetEnemyPrefix(EnemyDirector __instance, ref List<EnemySetup> ___enemyList, int ___enemyListIndex)
        {
            if (!_isListSet) // Only run once.
            {
                _startingEnemyList = new List<EnemySetup>(___enemyList);
                _isListSet = true;
            }
            
            // Make sure at least one "enemy" exists.
            if (___enemyList.Count == 0)
            {
                var emptyEnemySetup = ScriptableObject.CreateInstance<EnemySetup>();
                emptyEnemySetup.spawnObjects = new List<PrefabRef>();
                ___enemyList.Add(emptyEnemySetup);
            }

            // Make sure the enemy list is long enough to prevent index out of range.
            while (___enemyList.Count < ___enemyListIndex + 1)
            {
                var idxToCopy = Random.Range(0, _startingEnemyList.Count);
                ___enemyList.Add(___enemyList[idxToCopy]);
            }
        }
    }
}