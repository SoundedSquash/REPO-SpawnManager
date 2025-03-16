using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpawnManager.Managers
{
    public static class ValuableManager
    {
        public static List<ValuableObject> ValuableList = new List<ValuableObject>();
        
        public static Dictionary<string, LevelValuables> LevelValuablesDictionary = new Dictionary<string, LevelValuables>();
            
        public static void RefreshAllValuables()
        {
            if (RunManager.instance != null && RunManager.instance.levels != null)
            {
                ValuableList = RunManager.instance.levels.SelectMany(l => l.ValuablePresets ?? Enumerable.Empty<LevelValuables>())
                    .SelectMany(lv => (lv?.tiny ?? Enumerable.Empty<GameObject>())
                        .Concat(lv?.small ?? Enumerable.Empty<GameObject>())
                        .Concat(lv?.medium ?? Enumerable.Empty<GameObject>())
                        .Concat(lv?.big ?? Enumerable.Empty<GameObject>())
                        .Concat(lv?.wide ?? Enumerable.Empty<GameObject>())
                        .Concat(lv?.tall ?? Enumerable.Empty<GameObject>())
                        .Concat(lv?.veryTall ?? Enumerable.Empty<GameObject>()))
                    .Select(go => go?.GetComponent<ValuableObject>())
                    .Where(vo => vo != null)
                    .Distinct()
                    .ToList()!;
            }
            else
            {
                ValuableList = new List<ValuableObject>();
            }
        }

        public static void RemoveValuables()
        {
            var disabledValuableNames = Settings.GetDisabledValuableNames();
            if (disabledValuableNames.Count == 0) return;
            
            RefreshAllValuables();
            
            var valuableObjectsToRemove = ValuableList.Where(valuableObject => disabledValuableNames.Contains(valuableObject.name))
                .ToList();
            
            Settings.Logger.LogDebug($"Valuables to disable: {string.Join(", ", disabledValuableNames)} | Removing {valuableObjectsToRemove.Count} valuables from levels.");

            RemoveValuableObjects(valuableObjectsToRemove);
        }
        
        private static void RemoveValuableObjects(List<ValuableObject> valuableObjectsToRemove)
        {
            if (RunManager.instance == null || RunManager.instance.levels == null) return;
            
            foreach (var valuablePreset in RunManager.instance.levels.SelectMany(level => level.ValuablePresets))
            {
                RemoveValuableObjectsFromList(valuablePreset.tiny, valuableObjectsToRemove);
                RemoveValuableObjectsFromList(valuablePreset.small, valuableObjectsToRemove);
                RemoveValuableObjectsFromList(valuablePreset.medium, valuableObjectsToRemove);
                RemoveValuableObjectsFromList(valuablePreset.big, valuableObjectsToRemove);
                RemoveValuableObjectsFromList(valuablePreset.wide, valuableObjectsToRemove);
                RemoveValuableObjectsFromList(valuablePreset.tall, valuableObjectsToRemove);
                RemoveValuableObjectsFromList(valuablePreset.veryTall, valuableObjectsToRemove);
            }
        }

        private static void RemoveValuableObjectsFromList(List<GameObject> list, List<ValuableObject> valuableObjectsToRemove)
        {
            var originalCount = list.Count;
            foreach (var obj in list.ToList().Where(obj => valuableObjectsToRemove.Contains(obj.GetComponent<ValuableObject>())))
            {
                Settings.Logger.LogDebug($"Removed valuable object {obj.name} from list.");
                list.Remove(obj);
            }
            if (originalCount - list.Count > 0)
            {
                Settings.Logger.LogDebug($"Removed {originalCount - list.Count} valuable objects from list.");
            }
        }
    }
}