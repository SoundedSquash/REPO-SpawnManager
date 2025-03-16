using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpawnManager.Managers
{
    public static class EnemyManager
    {
        public static Dictionary<string, IEnumerable<GameObject>> EnemySpawnList = new Dictionary<string, IEnumerable<GameObject>>();
            
        public static void RefreshAllEnemyNames()
        {
            var director = GetEnemyDirector();
            
            EnemySpawnList =  director.enemiesDifficulty1.Concat(director.enemiesDifficulty2).Concat(director.enemiesDifficulty3)
                .SelectMany(ed => ed.spawnObjects)
                .GroupBy(so => so.GetInstanceID())
                .Select(so => so.First())
                .Where(so => so.GetComponent<EnemyParent>() != null)
                .GroupBy(so => so.GetComponent<EnemyParent>().enemyName)
                .ToDictionary(g => g.Key, g => g.AsEnumerable());
        }

        public static void RemoveEnemies()
        {
            var disabledEnemyNames = Settings.GetDisabledEnemyNames();
            if (disabledEnemyNames.Count == 0) return;
            
            if (EnemySpawnList.Count == 0) RefreshAllEnemyNames();
            
            var director = GetEnemyDirector();
            var spawnObjectsToRemove = EnemySpawnList.Where(kvp => disabledEnemyNames.Contains(kvp.Key))
                .SelectMany(kvp => kvp.Value)
                .ToList();
            
            Settings.Logger.LogDebug($"Enemies to disable: {string.Join(", ", disabledEnemyNames)} | Removing {spawnObjectsToRemove.Count} spawnObjects from enemy director.");

            var allSetups = director.enemiesDifficulty1.Concat(director.enemiesDifficulty2).Concat(director.enemiesDifficulty3);
            
            var setupsToRemove = allSetups.Where(s => s.spawnObjects.Any(so => spawnObjectsToRemove.Contains(so))).ToList();
            // Use reverse loop to avoid InvalidOperationException when removing items from a list while iterating over it.
            for (var i = setupsToRemove.Count - 1; i >= 0; i--)
            {
                var setup = setupsToRemove[i];
                Settings.Logger.LogDebug($"Removed enemy setup {setup.name}");
                director.enemiesDifficulty1.Remove(setup);
                director.enemiesDifficulty2.Remove(setup);
                director.enemiesDifficulty3.Remove(setup);
            }
            // Check that all enemiesDifficulty lists have at least one item. If not, create an empty one and include a gameobject in the spawnObjects list.
            var emptyEnemySetup = ScriptableObject.CreateInstance<EnemySetup>();
            emptyEnemySetup.spawnObjects = new List<GameObject>() { new GameObject("EmptyEnemy") };
            if (director.enemiesDifficulty1.Count == 0)
            {
                director.enemiesDifficulty1.Add(emptyEnemySetup);
            }
            if (director.enemiesDifficulty2.Count == 0)
            {
                director.enemiesDifficulty2.Add(emptyEnemySetup);
            }
            if (director.enemiesDifficulty3.Count == 0)
            {
                director.enemiesDifficulty3.Add(emptyEnemySetup);
            }
            
            // Currently, EnemySetup spawnObject lists are the same enemy reference in the groups. I'm accounting for the chance that some groups may be mixed in the future.
            // foreach (var setup in allSetups)
            // {
            //     setupsToRemove.Add(setup);
            //     setup.spawnObjects.RemoveAll(so => spawnObjectsToRemove.Contains(so));
            //
            //     // Remove completely if the spawnObjects list is empty.
            //     if (setup.spawnObjects.Count == 0)
            //     {
            //         setupsToRemove.Add(setup);
            //     }
            // }
        }
        
        private static EnemyDirector GetEnemyDirector()
        {
            return GameObject.Find("Enemy Director").GetComponent<EnemyDirector>();
        }
    }
}