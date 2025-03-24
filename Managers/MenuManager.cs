using System;
using System.Linq;
using MenuLib;
using MenuLib.MonoBehaviors;
using MenuLib.Structs;
using SpawnManager.Extensions;
using UnityEngine;

namespace SpawnManager.Managers
{
    public static class MenuModManager
    {
        private static REPOButton _currentPageButton;
        
        public static void Initialize()
        {
            MenuAPI.AddElementToMainMenu(parent => 
                MenuAPI.CreateREPOButton("Spawn Manager",
                    () => CreatePopup().OpenPage(false),
                    parent,
                    new Vector2(550f, 22f)
                )
            );
        }
        
        private static REPOPopupPage CreatePopup()
        {
            var menu = MenuAPI.CreateREPOPopupPage("Spawn Manager", REPOPopupPage.PresetSide.Left, false, true);
            menu.AddElement(parent => 
                MenuAPI.CreateREPOButton("Back", 
                () => menu.ClosePage(true),
                parent,
                new Vector2(77f, 34f))
            );
            
            CreateEnemyPage(menu);
            CreateValuablePage(menu);

            return menu;
        }

        private static void CreateEnemyPage(REPOPopupPage menu)
        {
            menu.AddElementToScrollView(parent =>
            {
                var button = MenuAPI.CreateREPOButton("Enemies", null, parent, new Vector2(0f, -80f + 0 * -34f));

                button.button.onClick.AddListener(() =>
                {
                    if (_currentPageButton == button)
                        return;

                    MenuAPI.CloseAllPagesAddedOnTop();

                    var enemyPage =
                        MenuAPI.CreateREPOPopupPage("Enemies", REPOPopupPage.PresetSide.Right, false, false);

                    enemyPage.AddElement(parent => 
                        MenuAPI.CreateREPOButton("Enable All",
                            () =>
                            {
                                MenuAPI.OpenPopup($"Enable All", Color.red,
                                    $"Enable all enemies?",
                                    () => {
                                        Settings.DisabledEnemies.BoxedValue = Settings.DisabledEnemies.DefaultValue;
                                        
                                        // Reopen page to refresh
                                        _currentPageButton = null;
                                        button.button.onClick.Invoke();
                                    });
                            },
                            parent,
                            new Vector2(367f, 20f)
                        )
                    );

                    enemyPage.AddElement(parent => 
                        MenuAPI.CreateREPOButton("Disable All",
                            () =>
                            {
                                MenuAPI.OpenPopup($"Disable All", Color.red,
                                    $"Disable all enemies?",
                                    () =>
                                    {
                                        Settings.DisabledEnemies.Value = string.Join(',',
                                            EnemyManager.EnemySpawnList.Select(kvp => kvp.Key));
                                        
                                        // Reopen page to refresh
                                        _currentPageButton = null;
                                        button.button.onClick.Invoke();
                                    });
                            }, parent, new Vector2(536f, 20f)
                        )
                    );

                    EnemyManager.RefreshAllEnemyNames();
                    Settings.Logger.LogDebug("Refreshed enemy names for menu.");
                    var enemiesDictionary = EnemyManager.EnemySpawnList;
                    var enemyNames = enemiesDictionary.Keys.ToList();
                    enemyNames.Sort();

                    foreach (var name in enemyNames)
                    {
                        enemyPage.AddElementToScrollView(parent =>
                        {
                            return MenuAPI.CreateREPOToggle(name, 
                                b => { Settings.UpdateEnemyEntry(name, b); },
                                parent, default, "ON", "OFF",
                                Settings.IsEnemyEnabled(name)).rectTransform;
                        });
                    }

                    enemyPage.OpenPage(true);
                });
                
                return button.rectTransform;
            });
        }

        private static void CreateValuablePage(REPOPopupPage menu)
        {
            menu.AddElementToScrollView(parent =>
            {
                var button = MenuAPI.CreateREPOButton("Valuables", null, parent, new Vector2(0f, -80f + 1 * -34f));
                
                button.button.onClick.AddListener(() =>
                {
                    if (_currentPageButton == button)
                        return;

                    MenuAPI.CloseAllPagesAddedOnTop();
                    
                    var valuablePage =
                        MenuAPI.CreateREPOPopupPage("Valuables", REPOPopupPage.PresetSide.Right, false, false);

                    valuablePage.AddElement(parent => 
                        MenuAPI.CreateREPOButton("Enable All",
                            () =>
                            {
                                MenuAPI.OpenPopup($"Enable All", Color.red,
                                    $"Enable all valuables?",
                                    () =>
                                    {
                                        Settings.DisabledValuables.BoxedValue = Settings.DisabledValuables.DefaultValue;

                                        // Reopen page to refresh
                                        _currentPageButton = null;
                                        button.button.onClick.Invoke();
                                    }
                                );
                            },
                            parent,
                            new Vector2(367f, 20f)
                        )
                    );
                    
                    valuablePage.AddElement(parent => 
                        MenuAPI.CreateREPOButton("Disable All",
                            () =>
                            {
                                MenuAPI.OpenPopup($"Disable All", Color.red,
                                    $"Disable all valuables?",
                                    () =>
                                    {
                                        Settings.DisabledValuables.Value = string.Join(',',
                                            ValuableManager.ValuableList.Select(vo => vo.name));
                                        
                                        // Reopen page to refresh
                                        _currentPageButton = null;
                                        button.button.onClick.Invoke();
                                    });
                            }, parent, new Vector2(536f, 20f)
                        )
                    );
                    ValuableManager.RefreshAllValuables();
                    Settings.Logger.LogDebug($"Refreshed {ValuableManager.ValuableList.Count} valuable names for menu.");
                    
                    var valuablesList = ValuableManager.ValuableList.OrderBy(vo => vo.name);
        
                    foreach (var valuableObject in valuablesList)
                    {
                        valuablePage.AddElementToScrollView(parent =>
                        {
                            return MenuAPI.CreateREPOToggle(valuableObject.FriendlyName(),
                                b => { Settings.UpdateValuableEntry(valuableObject.name, b); },
                                parent, default, "ON", "OFF",
                                Settings.IsValuableEnabled(valuableObject.name)).rectTransform;
                        });
                    }
        
                    valuablePage.OpenPage(true);
                });
                
                return button.rectTransform;
            });
        }
    }
}