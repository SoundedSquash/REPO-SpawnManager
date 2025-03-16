using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpawnManager.Managers
{
    public static class ValuableManager
    {
        public static List<ValuableObject> ValuableList = new List<ValuableObject>();
            
        public static void RefreshAllValuables()
        {
            ValuableList = RunManager.instance.levels.SelectMany(l => l.ValuablePresets)
                .SelectMany(lv => lv.tiny.Concat(lv.small).Concat(lv.medium).Concat(lv.big).Concat(lv.wide).Concat(lv.tall).Concat(lv.veryTall))
                .Select(go => go.GetComponent<ValuableObject>())
                .Where(vo => vo != null)
                .Distinct()
                .ToList();
        }

        public static void RemoveValuables()
        {
            var disabledValuableNames = Settings.GetDisabledValuableNames();
            if (disabledValuableNames.Count == 0) return;
            
            if (ValuableList.Count == 0) RefreshAllValuables();
            
            var director = RunManager.instance;
            var valuableObjectsToRemove = ValuableList.Where(valuableObject => disabledValuableNames.Contains(valuableObject.name))
                .ToList();
            
            Settings.Logger.LogDebug($"Enemies to disable: {string.Join(", ", disabledValuableNames)} | Removing {valuableObjectsToRemove.Count} spawnObjects from enemy director.");

            RemoveValuableObjects(valuableObjectsToRemove);
        }
        
        private static void RemoveValuableObjects(List<ValuableObject> valuableObjectsToRemove)
        {
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
            list.RemoveAll(obj => valuableObjectsToRemove.Contains(obj.GetComponent<ValuableObject>()));

            if (list.Count != 0) return;
            
            // If the list is empty, add a placeholder object to avoid errors
            var empty = new GameObject("EmptyValuable");
            empty.AddComponent<ValuableObject>();
            list.Add(empty);
        }
    }
}