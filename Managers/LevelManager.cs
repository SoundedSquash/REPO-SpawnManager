using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpawnManager.Managers
{
    public static class LevelManager
    {
        private static List<Level> _removedList = new List<Level>();

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
            
            _removedList.ForEach(level =>
            {
                Settings.Logger.LogDebug($"Restored level {level.name}.");
                RunManager.instance.levels.Add(level);
                _removedList.Remove(level);
            });
        }

        private static bool RunManagerLevelVariableIsAvailable => RunManager.instance != null && RunManager.instance.levels != null;
    }
}