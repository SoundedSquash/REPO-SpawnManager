using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpawnManager.Managers
{
    public static class ValuableManager
    {
        public static List<ValuableObject> ValuableList = new List<ValuableObject>();
        public static Dictionary<string, GameObject> RemovedList = new Dictionary<string, GameObject>();
        
        public static Dictionary<string, LevelValuables> LevelValuablesDictionary = new Dictionary<string, LevelValuables>();
        
        public static void RefreshAllValuables()
        {
            if (RunManager.instance != null && RunManager.instance.levels != null)
            {
                if (AllItems.Count == 0)
                {
                    foreach (Level level in RunManager.instance.levels)
                    {
                        foreach (var valuablePreset in level.ValuablePresets)
                        {
                            AddItemsToDictionary(valuablePreset.tiny, level.name, ValuablePresetType.Tiny);
                            AddItemsToDictionary(valuablePreset.small, level.name, ValuablePresetType.Small);
                            AddItemsToDictionary(valuablePreset.medium, level.name, ValuablePresetType.Medium);
                            AddItemsToDictionary(valuablePreset.big, level.name, ValuablePresetType.Big);
                            AddItemsToDictionary(valuablePreset.wide, level.name, ValuablePresetType.Wide);
                            AddItemsToDictionary(valuablePreset.tall, level.name, ValuablePresetType.Tall);
                            AddItemsToDictionary(valuablePreset.veryTall, level.name, ValuablePresetType.VeryTall);
                        }
                    }
                }

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
            var disabledValuableNames = Settings.GetDisabledSettingsEntryListNames(Settings.DisabledValuables);
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
                
                var allItems = valuablePreset.tiny.Concat(valuablePreset.small)
                    .Concat(valuablePreset.medium)
                    .Concat(valuablePreset.big)
                    .Concat(valuablePreset.wide)
                    .Concat(valuablePreset.tall)
                    .Concat(valuablePreset.veryTall)
                    .ToList();
                
                if (allItems.Count == 0) continue;
                
                if (!valuablePreset.tiny.Any()) valuablePreset.tiny.Add(allItems[Random.Range(0, allItems.Count)]);
                if (!valuablePreset.small.Any()) valuablePreset.small.Add(allItems[Random.Range(0, allItems.Count)]);
                if (!valuablePreset.medium.Any()) valuablePreset.medium.Add(allItems[Random.Range(0, allItems.Count)]);
                if (!valuablePreset.big.Any()) valuablePreset.big.Add(allItems[Random.Range(0, allItems.Count)]);
                if (!valuablePreset.wide.Any()) valuablePreset.wide.Add(allItems[Random.Range(0, allItems.Count)]);
                if (!valuablePreset.tall.Any()) valuablePreset.tall.Add(allItems[Random.Range(0, allItems.Count)]);
                if (!valuablePreset.veryTall.Any()) valuablePreset.veryTall.Add(allItems[Random.Range(0, allItems.Count)]);
            }
        }

        private static void RemoveValuableObjectsFromList(List<GameObject> list, List<ValuableObject> valuableObjectsToRemove)
        {
            var originalCount = list.Count;
            foreach (var obj in list.ToList().Where(obj => valuableObjectsToRemove.Contains(obj.GetComponent<ValuableObject>())))
            {
                Settings.Logger.LogDebug($"Removed valuable object {obj.name} from list.");
                // TODO RemovedList.TryAdd(key, obj);
                list.Remove(obj);
            }
            if (originalCount - list.Count > 0)
            {
                Settings.Logger.LogDebug($"Removed {originalCount - list.Count} valuable objects from list.");
            }
        }
        
        public static void RestoreValuableObjects()
        {
            // TODO
            return;
            if (RemovedList.Count == 0) return;
            if (RunManager.instance == null || RunManager.instance.levels == null) return;
            
            foreach (var level in RunManager.instance.levels.Select(level => new { 
                         Level = level,
                         ValuablePresets = level.ValuablePresets }))
            {
                // RemoveValuableObjectsFromList(valuablePreset.tiny, valuableObjectsToRemove);
                // RemoveValuableObjectsFromList(valuablePreset.small, valuableObjectsToRemove);
                // RemoveValuableObjectsFromList(valuablePreset.medium, valuableObjectsToRemove);
                // RemoveValuableObjectsFromList(valuablePreset.big, valuableObjectsToRemove);
                // RemoveValuableObjectsFromList(valuablePreset.wide, valuableObjectsToRemove);
                // RemoveValuableObjectsFromList(valuablePreset.tall, valuableObjectsToRemove);
                // RemoveValuableObjectsFromList(valuablePreset.veryTall, valuableObjectsToRemove);
            }
        }
        
        public enum ValuablePresetType
        {
            Tiny,
            Small,
            Medium,
            Big,
            Wide,
            Tall,
            VeryTall
        }

        public static string GetValuablePresetTypePath(ValuablePresetType type)
        {
            if (type == ValuablePresetType.Tiny)
            {
                return "01 Tiny";
            }

            if (type == ValuablePresetType.Small)
            {
                return "02 Small";
            }

            if (type == ValuablePresetType.Medium)
            {
                return "03 Medium";
            }

            if (type == ValuablePresetType.Big)
            {
                return "04 Big";
            }

            if (type == ValuablePresetType.Wide)
            {
                return "05 Wide";
            }

            if (type == ValuablePresetType.Tall)
            {
                return "06 Tall";
            }

            if (type == ValuablePresetType.VeryTall)
            {
                return "07 Very Tall";
            }

            return "Valuable Size Not Found - Spawn Manager";
        }

        public static Dictionary<string, ValuableMetaData> AllItems = new Dictionary<string, ValuableMetaData>();

        static void AddItemsToDictionary(List<GameObject> list, string levelName, ValuablePresetType type)
        {
            foreach (var item in list)
            {
                if (item != null)
                {
                    AllItems.TryAdd(item.name, new ValuableMetaData(levelName, type));
                }
            }
        }
    }
    
    public class ValuableMetaData
    {
        public ValuableMetaData(string levelName, ValuableManager.ValuablePresetType presetType)
        {
            LevelName = levelName;
            PresetType = presetType;
        }

        public string LevelName { get; set; }
        public ValuableManager.ValuablePresetType PresetType { get; set; }
    }
}