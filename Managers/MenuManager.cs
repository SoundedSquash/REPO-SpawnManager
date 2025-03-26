﻿using System.Linq;
using MenuLib;
using MenuLib.MonoBehaviors;
using SpawnManager.Extensions;
using UnityEngine;

namespace SpawnManager.Managers
{
    public static class MenuModManager
    {
        private static REPOButton? _currentPageButton;
        
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
            CreateLevelPage(menu);

            return menu;
        }

        private static void CreateEnemyPage(REPOPopupPage menu)
        {
            menu.AddElementToScrollView(parent =>
            {
                var button = MenuAPI.CreateREPOButton("Enemies", null, parent, new Vector2(0f, -80f + 0 * -34f));

                button.onClick = () =>
                {
                    if (_currentPageButton == button)
                        return;

                    MenuAPI.CloseAllPagesAddedOnTop();

                    var enemyPage =
                        MenuAPI.CreateREPOPopupPage("Enemies", REPOPopupPage.PresetSide.Right, shouldCachePage: false);

                    enemyPage.AddElement(enemyPageParent => 
                        MenuAPI.CreateREPOButton("Enable All",
                            () =>
                            {
                                MenuAPI.OpenPopup($"Enable All", Color.red,
                                    $"Enable all enemies?",
                                    () => {
                                        Settings.DisabledEnemies.BoxedValue = Settings.DisabledEnemies.DefaultValue;
                                        
                                        // Reopen page to refresh
                                        _currentPageButton = null;
                                        button.onClick.Invoke();
                                    });
                            },
                            enemyPageParent,
                            new Vector2(367f, 20f)
                        )
                    );

                    enemyPage.AddElement(enemyPageParent => 
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
                                        button.onClick.Invoke();
                                    });
                            }, enemyPageParent, new Vector2(536f, 20f)
                        )
                    );

                    EnemyManager.RefreshAllEnemyNames();
                    Settings.Logger.LogDebug("Refreshed enemy names for menu.");
                    var enemiesDictionary = EnemyManager.EnemySpawnList;
                    var enemyNames = enemiesDictionary.Keys.ToList();
                    enemyNames.Sort();

                    foreach (var name in enemyNames)
                    {
                        enemyPage.AddElementToScrollView(enemyPageParent =>
                        {
                            return MenuAPI.CreateREPOToggle(name, 
                                b => { Settings.UpdateSettingsListEntry(Settings.DisabledEnemies, name, b); },
                                enemyPageParent, default, "ON", "OFF",
                                Settings.IsSettingsListEntryEnabled(Settings.DisabledEnemies, name)).rectTransform;
                        });
                    }

                    enemyPage.OpenPage(true);
                };
                
                return button.rectTransform;
            });
        }

        private static void CreateValuablePage(REPOPopupPage menu)
        {
            menu.AddElementToScrollView(parent =>
            {
                var button = MenuAPI.CreateREPOButton("Valuables", null, parent, new Vector2(0f, -80f + 1 * -34f));
                
                button.onClick = () =>
                {
                    if (_currentPageButton == button)
                        return;

                    MenuAPI.CloseAllPagesAddedOnTop();
                    
                    var valuablePage =
                        MenuAPI.CreateREPOPopupPage("Valuables", REPOPopupPage.PresetSide.Right, shouldCachePage: false);

                    valuablePage.AddElement(valuablePageParent => 
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
                                        button.onClick.Invoke();
                                    }
                                );
                            },
                            valuablePageParent,
                            new Vector2(367f, 20f)
                        )
                    );
                    
                    valuablePage.AddElement(valuablePageParent => 
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
                                        button.onClick.Invoke();
                                    });
                            }, valuablePageParent, new Vector2(536f, 20f)
                        )
                    );
                    ValuableManager.RefreshAllValuables();
                    Settings.Logger.LogDebug($"Refreshed {ValuableManager.ValuableList.Count} valuable names for menu.");
                    
                    var valuablesList = ValuableManager.ValuableList.OrderBy(vo => vo.name);
        
                    foreach (var valuableObject in valuablesList)
                    {
                        valuablePage.AddElementToScrollView(valuablePageParent =>
                        {
                            return MenuAPI.CreateREPOToggle(valuableObject.FriendlyName(),
                                b => { Settings.UpdateSettingsListEntry(Settings.DisabledValuables, valuableObject.name, b); },
                                valuablePageParent, default, "ON", "OFF",
                                Settings.IsSettingsListEntryEnabled(Settings.DisabledValuables, valuableObject.name)).rectTransform;
                        });
                    }
        
                    valuablePage.OpenPage(true);
                };
                
                return button.rectTransform;
            });
        }

        private static void CreateLevelPage(REPOPopupPage menu)
        {
            menu.AddElementToScrollView(parent =>
            {
                var button = MenuAPI.CreateREPOButton("Levels", null, parent, new Vector2(0f, -80f + 1 * -34f));
                
                button.onClick = () =>
                {
                    if (_currentPageButton == button)
                        return;

                    MenuAPI.CloseAllPagesAddedOnTop();
                    
                    var levelPage =
                        MenuAPI.CreateREPOPopupPage("Levels", REPOPopupPage.PresetSide.Right, shouldCachePage: false);

                    levelPage.AddElement(levelPageParent => 
                        MenuAPI.CreateREPOButton("Enable All",
                            () =>
                            {
                                MenuAPI.OpenPopup($"Enable All", Color.red,
                                    $"Enable all Levels?",
                                    () =>
                                    {
                                        Settings.DisabledLevels.BoxedValue = Settings.DisabledLevels.DefaultValue;

                                        // Reopen page to refresh
                                        _currentPageButton = null;
                                        button.onClick.Invoke();
                                    }
                                );
                            },
                            levelPageParent,
                            new Vector2(367f, 20f)
                        )
                    );
                    
                    levelPage.AddElement(levelPageParent => 
                        MenuAPI.CreateREPOButton("Disable All",
                            () =>
                            {
                                MenuAPI.OpenPopup($"Disable All", Color.red,
                                    $"Disable all Levels?",
                                    () =>
                                    {
                                        Settings.DisabledLevels.Value = string.Join(',',
                                            LevelManager.GetAllLevels().Select(l => l.name));
                                        
                                        // Reopen page to refresh
                                        _currentPageButton = null;
                                        button.onClick.Invoke();
                                    });
                            }, levelPageParent, new Vector2(536f, 20f)
                        )
                    );
                    
                    var levelsList = LevelManager.GetAllLevels().OrderBy(vo => vo.name);
        
                    foreach (var level in levelsList)
                    {
                        levelPage.AddElementToScrollView(levelPageParent =>
                        {
                            return MenuAPI.CreateREPOToggle(level.FriendlyName(),
                                b => { Settings.UpdateSettingsListEntry(Settings.DisabledLevels, level.name, b); },
                                levelPageParent, default, "ON", "OFF",
                                Settings.IsSettingsListEntryEnabled(Settings.DisabledLevels, level.name)).rectTransform;
                        });
                    }
        
                    levelPage.OpenPage(true);
                };
                
                return button.rectTransform;
            });
        }
    }
}