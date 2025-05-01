using System.Collections.Generic;
using System.Linq;

namespace SpawnManager.Managers
{
    public static class LevelManager
    {
        private static List<Level> _removedList = new List<Level>();

        private static bool RunManagerLevelVariableIsAvailable => RunManager.instance != null && RunManager.instance.levels != null;

        public static IEnumerable<Level> GetAllLevels() => _removedList.Concat(RunManager.instance.levels);

        public static void RemoveLevels()
        {
            if (SemiFunc.IsNotMasterClient()) return;
            if (!RunManagerLevelVariableIsAvailable) return;
            
            List<string> disabledLevelNames = Settings.GetDisabledSettingsEntryListNames(Settings.DisabledLevels);

            RunManager.instance.levels.Where(l => disabledLevelNames.Contains(l.name)).ToList().ForEach(level =>
            {
                Settings.Logger.LogDebug($"Removed level {level.name}.");
                _removedList.Add(level);
                RunManager.instance.levels.Remove(level);
            });
            
            // TODO Fix this to actually return to main menu. Or prevent leaving settings page so this can't happen.
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

        /// <summary>
        /// Check if the Disabled Levels list is valid. At least one level must be available.
        /// </summary>
        public static bool IsValid()
        {
            // Valid if at least one level is not removed.
            return RunManager.instance.levels.Any(l => !Settings.GetDisabledSettingsEntryListNames(Settings.DisabledLevels).Contains(l.name));
        } 
    }
}