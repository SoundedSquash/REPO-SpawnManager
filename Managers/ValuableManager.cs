using System.Collections.Generic;
using System.Linq;
using SpawnManager.Extensions;
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
                            AddItemsToDictionary(valuablePreset.tiny, valuablePreset.name, ValuablePresetType.Tiny);
                            AddItemsToDictionary(valuablePreset.small, valuablePreset.name, ValuablePresetType.Small);
                            AddItemsToDictionary(valuablePreset.medium, valuablePreset.name, ValuablePresetType.Medium);
                            AddItemsToDictionary(valuablePreset.big, valuablePreset.name, ValuablePresetType.Big);
                            AddItemsToDictionary(valuablePreset.wide, valuablePreset.name, ValuablePresetType.Wide);
                            AddItemsToDictionary(valuablePreset.tall, valuablePreset.name, ValuablePresetType.Tall);
                            AddItemsToDictionary(valuablePreset.veryTall, valuablePreset.name, ValuablePresetType.VeryTall);
                        }
                    }
                }

                ValuableList = RunManager.instance.levels.SelectMany(l => l.ValuablePresets ?? Enumerable.Empty<LevelValuables>())
                    .SelectMany(lv => (lv?.tiny?.Where(go => go != null) ?? Enumerable.Empty<GameObject>())
                        .Concat(lv?.small?.Where(go => go != null) ?? Enumerable.Empty<GameObject>())
                        .Concat(lv?.medium?.Where(go => go != null) ?? Enumerable.Empty<GameObject>())
                        .Concat(lv?.big?.Where(go => go != null) ?? Enumerable.Empty<GameObject>())
                        .Concat(lv?.wide?.Where(go => go != null) ?? Enumerable.Empty<GameObject>())
                        .Concat(lv?.tall?.Where(go => go != null) ?? Enumerable.Empty<GameObject>())
                        .Concat(lv?.veryTall?.Where(go => go != null) ?? Enumerable.Empty<GameObject>()))
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

            bool isGenericProcessed = false;
            foreach (var valuablePreset in RunManager.instance.levels.SelectMany(level => level.ValuablePresets))
            {
                // Only need to process the generic list once.
                if (valuablePreset.IsGenericList() && isGenericProcessed) continue;
                
                RemoveValuableObjectsFromList(valuablePreset.tiny, valuableObjectsToRemove);
                RemoveValuableObjectsFromList(valuablePreset.small, valuableObjectsToRemove);
                RemoveValuableObjectsFromList(valuablePreset.medium, valuableObjectsToRemove);
                RemoveValuableObjectsFromList(valuablePreset.big, valuableObjectsToRemove);
                RemoveValuableObjectsFromList(valuablePreset.wide, valuableObjectsToRemove);
                RemoveValuableObjectsFromList(valuablePreset.tall, valuableObjectsToRemove);
                RemoveValuableObjectsFromList(valuablePreset.veryTall, valuableObjectsToRemove);
                
                // Only fill for the generic list.
                if (!valuablePreset.IsGenericList()) continue;
                
                if (!valuablePreset.tiny.Any())
                {
                    valuablePreset.tiny.Add(AllItems.First(i => i.Key == Settings.DefaultValuable.Value).Value.ValuableGameObject);
                }
                var smallerItems = valuablePreset.tiny.ToList();
                
                if (!valuablePreset.small.Any()) valuablePreset.small.Add(smallerItems[Random.Range(0, smallerItems.Count)]);
                smallerItems = smallerItems.Concat(valuablePreset.small).ToList();
                
                if (!valuablePreset.medium.Any()) valuablePreset.medium.Add(smallerItems[Random.Range(0, smallerItems.Count)]);
                smallerItems = smallerItems.Concat(valuablePreset.medium).ToList();
                
                if (!valuablePreset.big.Any()) valuablePreset.big.Add(smallerItems[Random.Range(0, smallerItems.Count)]);
                smallerItems = smallerItems.Concat(valuablePreset.big).ToList();
                
                if (!valuablePreset.wide.Any()) valuablePreset.wide.Add(smallerItems[Random.Range(0, smallerItems.Count)]);
                // Keep as big for tall/very tall to avoid potential issues with using wide objects.
                // Open to investigation.
                smallerItems = smallerItems.Concat(valuablePreset.big).ToList();
                
                if (!valuablePreset.tall.Any()) valuablePreset.tall.Add(smallerItems[Random.Range(0, smallerItems.Count)]);
                smallerItems = smallerItems.Concat(valuablePreset.tall).ToList();
                
                if (!valuablePreset.veryTall.Any()) valuablePreset.veryTall.Add(smallerItems[Random.Range(0, smallerItems.Count)]);
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
            Settings.Logger.LogDebug($"All items [{AllItems.Count}]");
            if (AllItems.Count == 0) return;
            Settings.Logger.LogDebug($"Run manager levels [{RunManager.instance?.levels.Count}]");
            if (RunManager.instance == null || RunManager.instance.levels == null) return;
            
            foreach (var level in RunManager.instance.levels)
            {
                foreach (var valuablePreset in level.ValuablePresets)
                {
                    RestoreValuableObjectsFromList(valuablePreset.tiny, valuablePreset.name, ValuablePresetType.Tiny);
                    RestoreValuableObjectsFromList(valuablePreset.small, valuablePreset.name, ValuablePresetType.Small);
                    RestoreValuableObjectsFromList(valuablePreset.medium, valuablePreset.name, ValuablePresetType.Medium);
                    RestoreValuableObjectsFromList(valuablePreset.big, valuablePreset.name, ValuablePresetType.Big);
                    RestoreValuableObjectsFromList(valuablePreset.wide, valuablePreset.name, ValuablePresetType.Wide);
                    RestoreValuableObjectsFromList(valuablePreset.tall, valuablePreset.name, ValuablePresetType.Tall);
                    RestoreValuableObjectsFromList(valuablePreset.veryTall, valuablePreset.name, ValuablePresetType.VeryTall);
                }
            }
        }

        private static void RestoreValuableObjectsFromList(List<GameObject> list, string valuablePresetName, ValuablePresetType type)
        {
            var restoredValuables = new List<string>();
            foreach (var obj in AllItems.Values.Where(
                         meta => meta.PresetType == type 
                                 && meta.ValuablePresetName == valuablePresetName
                                 && !list.Contains(meta.ValuableGameObject)))
            {
                restoredValuables.Add(obj.ValuableGameObject.name);
                list.Add(obj.ValuableGameObject);
            }
            
            // Don't log if nothing was restored.
            if (restoredValuables.Count ==0) return;
            Settings.Logger.LogDebug($"Restored valuables for {valuablePresetName}:{type} ({string.Join(", ", restoredValuables)})");
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

        static void AddItemsToDictionary(List<GameObject> list, string valuablePresetName, ValuablePresetType type)
        {
            foreach (var item in list)
            {
                if (item != null)
                {
                    AllItems.TryAdd(item.name, new ValuableMetaData(item, valuablePresetName, type));
                }
            }
        }
    }
    
    public class ValuableMetaData
    {
        public ValuableMetaData(GameObject valuable, string valuablePresetName, ValuableManager.ValuablePresetType presetType)
        {
            ValuableGameObject = valuable;
            ValuablePresetName = valuablePresetName;
            PresetType = presetType;
        }

        public GameObject ValuableGameObject { get; set; }
        public string ValuablePresetName { get; set; }
        public ValuableManager.ValuablePresetType PresetType { get; set; }
    }
}