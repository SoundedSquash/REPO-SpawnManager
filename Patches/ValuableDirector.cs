using HarmonyLib;
using SpawnManager.Managers;
using UnityEngine;

namespace SpawnManager.Patches
{
    [HarmonyPatch(typeof(ValuableDirector))]
    public static class ValuableDirectorPatches
    {
        /// <summary>
        /// Override the path if we injected other objects to spawn for other sizes.
        /// </summary>
        [HarmonyPatch("Spawn")]
        [HarmonyPrefix]
        static void ValuableDirectorSpawnPrefix(GameObject _valuable, ValuableVolume _volume, ref string _path)
        {
            if (ValuableManager.AllItems.TryGetValue(_valuable.name, out var data))
            {
                _path = ValuableManager.GetValuablePresetTypePath(data.PresetType);
            }
        }
    }
}