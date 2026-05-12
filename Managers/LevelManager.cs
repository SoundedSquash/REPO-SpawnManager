using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpawnManager.Managers
{
    public static class LevelManager
    {
        private static List<Level> _removedList = new List<Level>();
        private static List<Level> _removedListArena = new List<Level>();
        private static List<Level> _removedListShop = new List<Level>();

        public static bool RunManagerLevelVariableIsAvailable => RunManager.instance != null && RunManager.instance.levels != null;

        public static List<Level> GetAllLevels() => _removedList.Concat(RunManager.instance.levels).ToList();

        public static List<Level> GetAllArenaLevels() => _removedListArena.Concat(RunManager.instance.levelArena).ToList();

        public static List<Level> GetAllShopLevels() => _removedListShop.Concat(RunManager.instance.levelShop).ToList();

        public static IEnumerable<Level> GetAllLevelsForItems()
        {
            var levels = GetAllLevels();
            if (RunManagerLevelVariableIsAvailable)
            {
                // Preserve original level names for grouping and for config compatibility.
                var fakeShop = ScriptableObject.CreateInstance<Level>();
                fakeShop.name = "Shop";
                var fakeArena = ScriptableObject.CreateInstance<Level>();
                fakeArena.name = "Arena";
                levels.Add(fakeShop);
                levels.Add(fakeArena);
            }
            return levels;
        }

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

            RunManager.instance.levelArena.Where(l => disabledLevelNames.Contains(l.name)).ToList().ForEach(level =>
            {
                Settings.Logger.LogDebug($"Removed level {level.name}.");
                _removedListArena.Add(level);
                RunManager.instance.levelArena.Remove(level);
            });

            RunManager.instance.levelShop.Where(l => disabledLevelNames.Contains(l.name)).ToList().ForEach(level =>
            {
                Settings.Logger.LogDebug($"Removed level {level.name}.");
                _removedListShop.Add(level);
                RunManager.instance.levelShop.Remove(level);
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
            if (_removedList.Count == 0 && _removedListArena.Count == 0 && _removedListShop.Count == 0) return;

            for (var i = _removedList.Count - 1; i >= 0; i--)
            {
                var level = _removedList[i];
                Settings.Logger.LogDebug($"Restored level {level.name}.");
                RunManager.instance.levels.Add(level);
                _removedList.RemoveAt(i);
            }

            for (var i = _removedListArena.Count - 1; i >= 0; i--)
            {
                var level = _removedListArena[i];
                Settings.Logger.LogDebug($"Restored level {level.name}.");
                RunManager.instance.levelArena.Add(level);
                _removedListArena.RemoveAt(i);
            }

            for (var i = _removedListShop.Count - 1; i >= 0; i--)
            {
                var level = _removedListShop[i];
                Settings.Logger.LogDebug($"Restored level {level.name}.");
                RunManager.instance.levelShop.Add(level);
                _removedListShop.RemoveAt(i);
            }
        }

        /// <summary>
        /// Check if the Disabled Levels list is valid. At least one level must be available.
        /// </summary>
        public static bool IsValid()
        {
            var disabledLevelNames = Settings.GetDisabledSettingsEntryListNames(Settings.DisabledLevels);
            // Valid if at least one level is not removed.
            return RunManager.instance.levels.Any(l => !disabledLevelNames.Contains(l.name))
                || RunManager.instance.levelArena.Any(l => !disabledLevelNames.Contains(l.name))
                || RunManager.instance.levelShop.Any(l => !disabledLevelNames.Contains(l.name));
        } 
    }
}