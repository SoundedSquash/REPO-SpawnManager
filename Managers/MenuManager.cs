using System;
using System.Linq;
using BepInEx.Configuration;
using BepInEx.Logging;
using MenuLib;
using SpawnManager.Extensions;
using UnityEngine;

namespace SpawnManager.Managers
{
    public static class MenuModManager
    {
        private static REPOButton _currentPageButton;
        
        public static void Initialize()
        {
            MenuAPI.AddElementToMainMenu(new REPOButton("Spawn Manager", () => CreatePopup().OpenPage(false)), new Vector2(48.3f, 55.5f));
        }
        
        private static REPOPopupPage CreatePopup()
        {
            var menu = new REPOPopupPage("Spawn Manager").SetBackgroundDimming(true).SetMaskPadding(new Padding(0, 70, 20, 50));
            menu.AddElementToPage(new REPOButton("Back", () => {
                var closePage = new Action(() => menu.ClosePage(true));
                
                closePage.Invoke();

            }), new Vector2(77f, 34f));
            
            CreateEnemyPage(out var enemiesButton);
            menu.AddElementToScrollView(enemiesButton, new Vector2(0f, -80f + 0 * -34f));
            
            CreateValuablePage(out var valuablesButton);
            menu.AddElementToScrollView(valuablesButton, new Vector2(0f, -80f + 1 * -34f));

            return menu;
        }

        private static void CreateEnemyPage(out REPOButton modButton)
        {
            var enemyPage = new REPOPopupPage("Enemies", enemyPage =>
            {
                enemyPage.SetPosition(new Vector2(517.00f, 190.6f));
                enemyPage.SetSize(new Vector2(335f, 342f));
                enemyPage.SetMaskPadding(new Padding(0, 70, 0, 50));
            });

            var modButtonTemp = modButton = new REPOButton("Enemies", null);
            modButton.SetOnClick(() =>
            {
                if (_currentPageButton == modButtonTemp)
                    return;

                var openPage = new Action(() =>
                {
                    MenuManager.instance.PageCloseAllAddedOnTop();

                    enemyPage.ClearButtons();
                    
                    _currentPageButton = modButtonTemp;

                    var enableAllButton = new REPOButton("Enable All", null);
                    enableAllButton.SetOnClick(() =>
                    {
                        MenuAPI.OpenPopup($"Enable All", Color.red,
                            $"Enable all enemies?", "Yes",
                            () =>
                            {
                                Settings.DisabledEnemies.BoxedValue = Settings.DisabledEnemies.DefaultValue;
                                
                                _currentPageButton = null;
                                modButtonTemp.onClick.Invoke();
                            }, "No");
                    });

                    //modPage.AddElementToPage(saveChangesButton, new Vector2(365, 34f));
                    enemyPage.AddElementToPage(enableAllButton, new Vector2(560f, 34f));
                    
                    EnemyManager.RefreshAllEnemyNames();
                    Settings.Logger.LogDebug("Refreshed enemy names for menu.");
                    var enemiesDictionary = EnemyManager.EnemySpawnList;
                    var enemyNames = enemiesDictionary.Keys.ToList();
                    enemyNames.Sort();

                    var yPosition = -80f;

                    foreach (var name in enemyNames)
                    {
                        enemyPage.AddElementToScrollView(
                            new REPOToggle(name, b => { Settings.UpdateEnemyEntry(name, b); }, "ON", "OFF",
                                Settings.IsEnemyEnabled(name)), new Vector2(120f, yPosition));
                        yPosition -= 30f;
                    }

                    enemyPage.OpenPage(true);
                });
                
                openPage.Invoke();
            });
        }

        private static void CreateValuablePage(out REPOButton modButton)
        {
            var valuablePage = new REPOPopupPage("Valuables", valuablePage =>
            {
                valuablePage.SetPosition(new Vector2(517.00f, 190.6f));
                valuablePage.SetSize(new Vector2(335f, 342f));
                valuablePage.SetMaskPadding(new Padding(0, 70, 0, 50));
            });

            var modButtonTemp = modButton = new REPOButton("Valuables", null);
            modButton.SetOnClick(() =>
            {
                if (_currentPageButton == modButtonTemp)
                    return;

                var openPage = new Action(() =>
                {
                    MenuManager.instance.PageCloseAllAddedOnTop();

                    valuablePage.ClearButtons();
                    
                    _currentPageButton = modButtonTemp;

                    var enableAllButton = new REPOButton("Enable All", null);
                    enableAllButton.SetOnClick(() =>
                    {
                        MenuAPI.OpenPopup($"Enable All", Color.red,
                            $"Enable all valuables?", "Yes",
                            () =>
                            {
                                Settings.DisabledValuables.BoxedValue = Settings.DisabledValuables.DefaultValue;
                                
                                _currentPageButton = null;
                                modButtonTemp.onClick.Invoke();
                            }, "No");
                    });

                    //modPage.AddElementToPage(saveChangesButton, new Vector2(365, 34f));
                    valuablePage.AddElementToPage(enableAllButton, new Vector2(560f, 34f));
                    
                    ValuableManager.RefreshAllValuables();
                    Settings.Logger.LogDebug($"Refreshed {ValuableManager.ValuableList.Count} valuable names for menu.");
                    
                    var valuablesList = ValuableManager.ValuableList.OrderBy(vo => vo.name);

                    var yPosition = -80f;

                    foreach (var valuableObject in valuablesList)
                    {
                        valuablePage.AddElementToScrollView(
                            new REPOToggle(valuableObject.FriendlyName(), 
                                b => { Settings.UpdateValuableEntry(valuableObject.name, b); }, "ON", "OFF",
                                Settings.IsValuableEnabled(valuableObject.name)
                                ),
                            new Vector2(120f, yPosition));
                        yPosition -= 30f;
                    }

                    valuablePage.OpenPage(true);
                });
                
                openPage.Invoke();
            });
        }
    }
}