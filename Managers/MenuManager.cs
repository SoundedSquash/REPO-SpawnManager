﻿using System;
using System.Linq;
using MenuLib;
using MenuLib.MonoBehaviors;
using SpawnManager.Extensions;
using UnityEngine;

namespace SpawnManager.Managers
{
    public static class MenuModManager
    {
        // Type REPOButton. Not explicitly set to support soft dependency.
        private static object? _currentPageButton;
        private static object? _mainMenuButton;
        
        public static void Initialize()
        {
            MenuAPI.AddElementToMainMenu(CreateMainMenuButton);
        }

        private static void CreateMainMenuButton(Transform parent)
        {
            _mainMenuButton = MenuAPI.CreateREPOButton("Spawn Manager",
                () => CreatePopup().OpenPage(false),
                parent,
                new Vector2(550f, 22f)
            );

            SetMainMenuButtonVisibility();
        }

        private static void SetMainMenuButtonVisibility()
        {
            if (!SemiFunc.IsMainMenu()) return;
            if (_mainMenuButton == null) return;

            var button = (REPOButton)_mainMenuButton;
            button.gameObject.SetActive(Settings.ShowSpawnManagerButton.Value);
        }

        public static void OnShowSpawnManagerButtonChanged(object sender, EventArgs args)
        {
            SetMainMenuButtonVisibility();
        }

        private static REPOPopupPage CreatePopup()
        {
            var menu = MenuAPI.CreateREPOPopupPage("Spawn Manager", REPOPopupPage.PresetSide.Left, false, true);

            LevelManager.RestoreLevels();
            ValuableManager.RestoreValuableObjects();
            ItemsManager.RestoreItems();
            
            // Initialize settings for per-levels here, long after custom levels are loaded.
            Settings.InitializeEnemiesLevels();
            Settings.InitializeItemsLevels();
            Settings.InitializeValuablesLevels();
            
            CreateSubMenuEnemies(menu); // Enemies
            CreateLevelPage(menu, out var levelButton); // Levels
            CreateSubMenuItems(menu); // Shop Items
            CreateSubMenuValuables(menu); // Valuables
            
            menu.AddElement(parent => 
                MenuAPI.CreateREPOButton("Back", 
                    () =>
                    {
                        if (!LevelManager.IsValid())
                        {
                            MenuAPI.OpenPopup("Invalid Levels", Color.red,
                                "Please make sure you have at least one level enabled. Would you like to edit the selections now?",
                                () =>
                                {
                                    levelButton.onClick.Invoke();
                                });
                            return;
                        }
                    
                        menu.ClosePage(true);
                    },
                    parent,
                    new Vector2(77f, 20f))
            );

            return menu;
        }

        private static void CreateSubMenuEnemies(REPOPopupPage menu)
        {
            menu.AddElementToScrollView(parent =>
            {
                var button = MenuAPI.CreateREPOButton("Enemies", null, parent);

                button.onClick = () =>
                {
                    var subMenu = MenuAPI.CreateREPOPopupPage("Enemies", REPOPopupPage.PresetSide.Left, false, true);
                    
                    CreateEnemyPage(subMenu);
                    CreateLevelEnemyPage(subMenu);
                    
                    subMenu.AddElement(subMenuparent => 
                        MenuAPI.CreateREPOButton("Back", 
                            () =>
                            {
                                subMenu.ClosePage(true);
                                CreatePopup().OpenPage(false);
                            },
                            subMenuparent,
                            new Vector2(77f, 20f))
                    );
                    
                    menu.ClosePage(true);
                    subMenu.OpenPage(false);
                };
                
                return button.rectTransform;
            });
        }

        private static void CreateEnemyPage(REPOPopupPage menu)
        {
            menu.AddElementToScrollView(parent =>
            {
                var button = MenuAPI.CreateREPOButton("Global", null, parent, new Vector2(0f, -80f + 0 * -34f));

                button.onClick = () =>
                {
                    if (ReferenceEquals(_currentPageButton, button))
                        return;

                    MenuAPI.CloseAllPagesAddedOnTop();

                    var enemyPage =
                        MenuAPI.CreateREPOPopupPage("Global", REPOPopupPage.PresetSide.Right, shouldCachePage: false);

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

                    _currentPageButton = button;

                    enemyPage.OpenPage(true);
                };

                return button.rectTransform;
            });
        }

        private static void CreateLevelEnemyPage(REPOPopupPage menu)
        {
            foreach (var level in LevelManager.GetAllLevels().OrderBy(vo => vo.name))
            {
                menu.AddElementToScrollView(parent =>
                {
                    var friendlyName = level.FriendlyName();
                    var button = MenuAPI.CreateREPOButton($"{friendlyName}", null, parent);

                    button.onClick = () =>
                    {
                        if (ReferenceEquals(_currentPageButton, button))
                            return;

                        MenuAPI.CloseAllPagesAddedOnTop();

                        var enemyPage =
                            MenuAPI.CreateREPOPopupPage($"{friendlyName}", REPOPopupPage.PresetSide.Right, shouldCachePage: false);

                        enemyPage.AddElement(enemyPageParent =>
                            MenuAPI.CreateREPOButton("Enable All",
                                () =>
                                {
                                    MenuAPI.OpenPopup($"Enable All", Color.red,
                                        $"Enable all enemies?",
                                        () => {
                                            if (Settings.DisabledLevelEnemies.TryGetValue(level.name, out var configEntry))
                                            {
                                                configEntry.BoxedValue = configEntry.DefaultValue;

                                                // Reopen page to refresh
                                                _currentPageButton = null;
                                                button.onClick.Invoke();
                                            }
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
                                            if (Settings.DisabledLevelEnemies.TryGetValue(level.name, out var configEntry))
                                            {
                                                configEntry.Value = string.Join(',', EnemyManager.EnemySpawnList.Select(kvp => kvp.Key));

                                                // Reopen page to refresh
                                                _currentPageButton = null;
                                                button.onClick.Invoke();
                                            }
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
                                var configEntry = Settings.DisabledLevelEnemies[level.name];
                                return MenuAPI.CreateREPOToggle(name,
                                    b => { Settings.UpdateSettingsListEntry(configEntry, name, b); },
                                    enemyPageParent, default, "ON", "OFF",
                                    Settings.IsSettingsListEntryEnabled(configEntry, name)).rectTransform;
                            });
                        }

                        _currentPageButton = button;

                        enemyPage.OpenPage(true);
                    };

                    return button.rectTransform;
                });
            }

            
        }

        private static void CreateValuablePage(REPOPopupPage menu)
        {
            menu.AddElementToScrollView(parent =>
            {
                var button = MenuAPI.CreateREPOButton("Global", null, parent);
                
                button.onClick = () =>
                {
                    if (ReferenceEquals(_currentPageButton, button))
                        return;

                    MenuAPI.CloseAllPagesAddedOnTop();
                    
                    var valuablePage =
                        MenuAPI.CreateREPOPopupPage("Global", REPOPopupPage.PresetSide.Right, shouldCachePage: false);

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

                    var tinyItems = ValuableManager.AllItems
                        .Where(ai => ai.Value.PresetType == ValuableManager.ValuablePresetType.Tiny)
                        .Select(i => i.Key);
                    
                    valuablePage.AddElementToScrollView(valuablePageParent =>
                    {
                        return MenuAPI.CreateREPOSlider("Default Valuable",
                            "This is used when not enough valuables are enabled for certain sizes. Only tiny items are allowed.",
                            onOptionChanged: stringValue =>
                            {
                                Settings.DefaultValuable.Value = stringValue;
                            },
                            valuablePageParent,
                            tinyItems.ToArray(),
                            defaultOption: Settings.DefaultValuable.Value).rectTransform;
                    });
                    
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
        
                    _currentPageButton = button;
                    valuablePage.OpenPage(true);
                };
                
                return button.rectTransform;
            });
        }

        private static void CreateSubMenuValuables(REPOPopupPage menu)
        {
            menu.AddElementToScrollView(parent =>
            {
                var button = MenuAPI.CreateREPOButton("Valuables", null, parent);

                button.onClick = () =>
                {
                    var subMenu = MenuAPI.CreateREPOPopupPage("Valuables", REPOPopupPage.PresetSide.Left, false, true);
                    
                    CreateValuablePage(subMenu);
                    CreateLevelValuablesPages(subMenu);
                    
                    subMenu.AddElement(subMenuParent => 
                        MenuAPI.CreateREPOButton("Back", 
                            () =>
                            {
                                subMenu.ClosePage(true);
                                CreatePopup().OpenPage(false);
                            },
                            subMenuParent,
                            new Vector2(77f, 20f))
                    );
                    
                    menu.ClosePage(true);
                    subMenu.OpenPage(false);
                };
                
                return button.rectTransform;
            });
        }

        private static void CreateLevelValuablesPages(REPOPopupPage menu)
        {
            foreach (var level in LevelManager.GetAllLevels().OrderBy(vo => vo.name))
            {
                menu.AddElementToScrollView(parent =>
                {
                    var friendlyName = level.FriendlyName();
                    var button = MenuAPI.CreateREPOButton($"{friendlyName}", null, parent);

                    button.onClick = () =>
                    {
                        if (ReferenceEquals(_currentPageButton, button))
                            return;

                        MenuAPI.CloseAllPagesAddedOnTop();

                        var page =
                            MenuAPI.CreateREPOPopupPage($"{friendlyName}", REPOPopupPage.PresetSide.Right, shouldCachePage: false);

                        page.AddElement(pageParent =>
                            MenuAPI.CreateREPOButton("Enable All",
                                () =>
                                {
                                    MenuAPI.OpenPopup($"Enable All", Color.red,
                                        $"Enable all valuables?",
                                        () => {
                                            if (Settings.DisabledLevelValuables.TryGetValue(level.name, out var configEntry))
                                            {
                                                configEntry.BoxedValue = configEntry.DefaultValue;

                                                // Reopen page to refresh
                                                _currentPageButton = null;
                                                button.onClick.Invoke();
                                            }
                                        });
                                },
                                pageParent,
                                new Vector2(367f, 20f)
                            )
                        );

                        page.AddElement(pageParent =>
                            MenuAPI.CreateREPOButton("Disable All",
                                () =>
                                {
                                    MenuAPI.OpenPopup($"Disable All", Color.red,
                                        $"Disable all valuables?",
                                        () =>
                                        {
                                            if (Settings.DisabledLevelValuables.TryGetValue(level.name, out var configEntry))
                                            {
                                                configEntry.Value = string.Join(',', ValuableManager.ValuableList.Select(vo => vo.name));

                                                // Reopen page to refresh
                                                _currentPageButton = null;
                                                button.onClick.Invoke();
                                            }
                                        });
                                }, pageParent, new Vector2(536f, 20f)
                            )
                        );

                        ValuableManager.RefreshAllValuables();
                        Settings.Logger.LogDebug($"Refreshed {ValuableManager.ValuableList.Count} valuable names for menu.");
                        
                        var valuablesList = ValuableManager.ValuableList.OrderBy(vo => vo.name);

                        foreach (var valuableObject in valuablesList)
                        {
                            page.AddElementToScrollView(pageParent =>
                            {
                                var configEntry = Settings.DisabledLevelValuables[level.name];
                                return MenuAPI.CreateREPOToggle(valuableObject.name,
                                    b => { Settings.UpdateSettingsListEntry(configEntry, valuableObject.name, b); },
                                    pageParent, default, "ON", "OFF",
                                    Settings.IsSettingsListEntryEnabled(configEntry, valuableObject.name)).rectTransform;
                            });
                        }

                        _currentPageButton = button;

                        page.OpenPage(true);
                    };

                    return button.rectTransform;
                });
            }

            
        }

        private static void CreateLevelPage(REPOPopupPage menu, out REPOButton levelButton)
        {
            REPOButton localButton = null!;
            menu.AddElementToScrollView(parent =>
            {
                var button = MenuAPI.CreateREPOButton("Levels", null, parent);
                
                button.onClick = () =>
                {
                    if (ReferenceEquals(_currentPageButton, button))
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
        
                    _currentPageButton = button;
                    levelPage.OpenPage(true);
                };
                
                localButton = button;
                return button.rectTransform;
            });
            
            levelButton = localButton;
        }

        private static void CreateSubMenuItems(REPOPopupPage menu)
        {
            menu.AddElementToScrollView(parent =>
            {
                var button = MenuAPI.CreateREPOButton("Shop Items", null, parent);

                button.onClick = () =>
                {
                    var subMenu = MenuAPI.CreateREPOPopupPage("Shop Items", REPOPopupPage.PresetSide.Left, false, true);
                    
                    CreateItemPage(subMenu);
                    CreateLevelItemPages(subMenu);
                    
                    subMenu.AddElement(subMenuParent => 
                        MenuAPI.CreateREPOButton("Back", 
                            () =>
                            {
                                subMenu.ClosePage(true);
                                CreatePopup().OpenPage(false);
                            },
                            subMenuParent,
                            new Vector2(77f, 20f))
                    );
                    
                    menu.ClosePage(true);
                    subMenu.OpenPage(false);
                };
                
                return button.rectTransform;
            });
        }

        private static void CreateItemPage(REPOPopupPage menu)
        {
            menu.AddElementToScrollView(parent =>
            {
                var button = MenuAPI.CreateREPOButton("Global", null, parent);
                
                button.onClick = () =>
                {
                    if (ReferenceEquals(_currentPageButton, button))
                        return;

                    MenuAPI.CloseAllPagesAddedOnTop();
                    
                    var itemPage =
                        MenuAPI.CreateREPOPopupPage("Global", REPOPopupPage.PresetSide.Right, shouldCachePage: false);

                    itemPage.AddElement(itemPageParent => 
                        MenuAPI.CreateREPOButton("Enable All",
                            () =>
                            {
                                MenuAPI.OpenPopup($"Enable All", Color.red,
                                    $"Enable all shop items?",
                                    () =>
                                    {
                                        Settings.DisabledItems.BoxedValue = Settings.DisabledItems.DefaultValue;

                                        // Reopen page to refresh
                                        _currentPageButton = null;
                                        button.onClick.Invoke();
                                    }
                                );
                            },
                            itemPageParent,
                            new Vector2(367f, 20f)
                        )
                    );
                    
                    itemPage.AddElement(itemPageParent => 
                        MenuAPI.CreateREPOButton("Disable All",
                            () =>
                            {
                                MenuAPI.OpenPopup($"Disable All", Color.red,
                                    $"Disable all shop items?",
                                    () =>
                                    {
                                        Settings.DisabledItems.Value = string.Join(',',
                                            ItemsManager.GetAllItems().Select(l => l.Key.ToItemFriendlyName()));
                                        
                                        // Reopen page to refresh
                                        _currentPageButton = null;
                                        button.onClick.Invoke();
                                    });
                            }, itemPageParent, new Vector2(536f, 20f)
                        )
                    );
                    
                    var itemsList = ItemsManager.GetAllItems().Keys.Select(item => item.ToItemFriendlyName()).OrderBy(itemName => itemName);
        
                    foreach (var item in itemsList)
                    {
                        itemPage.AddElementToScrollView(itemPageParent =>
                        {
                            return MenuAPI.CreateREPOToggle(item,
                                b => { Settings.UpdateSettingsListEntry(Settings.DisabledItems, item, b); },
                                itemPageParent, default, "ON", "OFF",
                                Settings.IsSettingsListEntryEnabled(Settings.DisabledItems, item)).rectTransform;
                        });
                    }
        
                    _currentPageButton = button;
                    itemPage.OpenPage(true);
                };
                
                return button.rectTransform;
            });
        }

        private static void CreateLevelItemPages(REPOPopupPage menu)
        {
            var levels = LevelManager.GetAllLevelsForItems().ToList();
            
            foreach (var level in levels.OrderBy(vo => vo.name))
            {
                menu.AddElementToScrollView(parent =>
                {
                    var friendlyName = level.FriendlyName();
                    var button = MenuAPI.CreateREPOButton($"{friendlyName}", null, parent);

                    button.onClick = () =>
                    {
                        if (ReferenceEquals(_currentPageButton, button))
                            return;

                        MenuAPI.CloseAllPagesAddedOnTop();

                        var itemPage =
                            MenuAPI.CreateREPOPopupPage($"{friendlyName}", REPOPopupPage.PresetSide.Right, shouldCachePage: false);

                        itemPage.AddElement(itemPageParent =>
                            MenuAPI.CreateREPOButton("Enable All",
                                () =>
                                {
                                    MenuAPI.OpenPopup($"Enable All", Color.red,
                                        $"Enable all shop items?",
                                        () => {
                                            if (Settings.DisabledLevelItems.TryGetValue(level.name, out var configEntry))
                                            {
                                                configEntry.BoxedValue = configEntry.DefaultValue;

                                                // Reopen page to refresh
                                                _currentPageButton = null;
                                                button.onClick.Invoke();
                                            }
                                        });
                                },
                                itemPageParent,
                                new Vector2(367f, 20f)
                            )
                        );

                        itemPage.AddElement(itemPageParent =>
                            MenuAPI.CreateREPOButton("Disable All",
                                () =>
                                {
                                    MenuAPI.OpenPopup($"Disable All", Color.red,
                                        $"Disable all shop items?",
                                        () =>
                                        {
                                            if (Settings.DisabledLevelItems.TryGetValue(level.name, out var configEntry))
                                            {
                                                configEntry.Value = string.Join(',', ItemsManager.GetAllItems().Select(kvp => kvp.Key.ToItemFriendlyName()));

                                                // Reopen page to refresh
                                                _currentPageButton = null;
                                                button.onClick.Invoke();
                                            }
                                        });
                                }, itemPageParent, new Vector2(536f, 20f)
                            )
                        );
                        
                        var itemNames = ItemsManager.GetAllItems().Keys
                            .Select(item => item.ToItemFriendlyName()).OrderBy(itemName => itemName);

                        foreach (var name in itemNames)
                        {
                            itemPage.AddElementToScrollView(itemPageParent =>
                            {
                                var configEntry = Settings.DisabledLevelItems[level.name];
                                return MenuAPI.CreateREPOToggle(name,
                                    b => { Settings.UpdateSettingsListEntry(configEntry, name, b); },
                                    itemPageParent, default, "ON", "OFF",
                                    Settings.IsSettingsListEntryEnabled(configEntry, name)).rectTransform;
                            });
                        }

                        _currentPageButton = button;

                        itemPage.OpenPage(true);
                    };

                    return button.rectTransform;
                });
            }
        }
    }
}