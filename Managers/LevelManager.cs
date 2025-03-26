using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpawnManager.Managers
{
    public static class LevelManager
    {
        private static List<Level> _removedList = new List<Level>();

        private static bool RunManagerLevelVariableIsAvailable => RunManager.instance != null && RunManager.instance.levels != null;

        public static IEnumerable<Level> GetAllLevels() => _removedList.Concat(RunManager.instance.levels);

        public static void RemoveLevels()
        {
            if (!RunManagerLevelVariableIsAvailable) return;
            
            List<string> disabledLevelNames = Settings.GetDisabledSettingsEntryListNames(Settings.DisabledLevels);

            RunManager.instance.levels.Where(l => disabledLevelNames.Contains(l.name)).ToList().ForEach(level =>
            {
                Settings.Logger.LogDebug($"Removed level {level.name}.");
                _removedList.Add(level);
                RunManager.instance.levels.Remove(level);
            });
            
            if (RunManager.instance.levels.Count == 0)
            {
                Settings.Logger.LogError("No levels left in RunManager. Quitting to Main Menu.");
                 RunManager.instance.StartCoroutine(RunManager.instance.LeaveToMainMenu());
            }
        }

        public static void RestoreLevels()
        {
            if (!RunManagerLevelVariableIsAvailable) return;
            if (_removedList.Count == 0) return;

            for (var i = _removedList.Count - 1; i >= 0; i--)
            {
                var level = _removedList[i];
                Settings.Logger.LogDebug($"Restored level {level.name}.");
                RunManager.instance.levels.Add(level);
                _removedList.RemoveAt(i);
            }
        }
    }
}