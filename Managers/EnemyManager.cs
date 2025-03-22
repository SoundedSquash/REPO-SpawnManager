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
            var director = EnemyDirector.instance;
            
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
            
            var director = EnemyDirector.instance;
            var spawnObjectsToRemove = EnemySpawnList.Where(kvp => disabledEnemyNames.Contains(kvp.Key))
                .SelectMany(kvp => kvp.Value)
                .ToList();
            
            Settings.Logger.LogDebug($"Enemies to disable: {string.Join(", ", disabledEnemyNames)} | Removing {spawnObjectsToRemove.Count} spawnObjects from enemy director.");
            
            var setupsToRemove = director
                .enemiesDifficulty1.Concat(director.enemiesDifficulty2).Concat(director.enemiesDifficulty3)
                .Where(s => s.spawnObjects.Any(so => spawnObjectsToRemove.Contains(so)))
                .ToList();
            // Use reverse loop to avoid InvalidOperationException when removing items from a list while iterating over it.
            for (var i = setupsToRemove.Count - 1; i >= 0; i--)
            {
                var setup = setupsToRemove[i];
                Settings.Logger.LogDebug($"Removed enemy setup {setup.name}");
                director.enemiesDifficulty1.Remove(setup);
                director.enemiesDifficulty2.Remove(setup);
                director.enemiesDifficulty3.Remove(setup);
            }
        }
    }
}