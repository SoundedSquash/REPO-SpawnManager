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
            var menu = MenuAPI.CreateREPOPopupPage("Spawn Manager", REPOPopupPage.PresetSide.Left, true);
            menu.AddElement(parent => 
                MenuAPI.CreateREPOButton("Back", 
                () => menu.ClosePage(true),
                parent,
                new Vector2(77f, 34f))
            );
            
            CreateEnemyPage(menu);
            
            // CreateValuablePage(out var valuablesButton);
            // menu.AddElementToScrollView(parent => valuablesButton, new Vector2(0f, -80f + 1 * -34f));

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
                        MenuAPI.CreateREPOPopupPage("Enemies", REPOPopupPage.PresetSide.Right);

                    enemyPage.AddElement(parent => 
                        MenuAPI.CreateREPOButton("Enable All",
                            () =>
                            {
                                MenuAPI.OpenPopup($"Enable All", Color.red,
                                    $"Enable all enemies?",
                                    () => { Settings.DisabledEnemies.BoxedValue = Settings.DisabledEnemies.DefaultValue; });
                                        
                                // Reopen page to refresh
                                _currentPageButton = null;
                                button.button.onClick.Invoke();
                            },
                            parent,
                            new Vector2(360f, 20f)
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

        private static void CreateValuablePage(out REPOButton modButton)
        {
            var valuablePage = new REPOPopupPage("Valuables", valuablePage =>
            {
                valuablePage.SetPosition(new Vector2(510.00f, 190.6f));
                valuablePage.SetSize(new Vector2(300f, 342f));
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
        
                    var disableAllButton = new REPOButton("Disable All", null);
                    disableAllButton.SetOnClick(() =>
                    {
                        MenuAPI.OpenPopup($"Disable All", Color.red,
                            $"Disable all valuables?", "Yes",
                            () =>
                            {
                                Settings.DisabledValuables.Value = string.Join(',', ValuableManager.ValuableList.Select(vo => vo.name));
                                
                                _currentPageButton = null;
                                modButtonTemp.onClick.Invoke();
                            }, "No");
                    });
        
                    valuablePage.AddElementToPage(enableAllButton, new Vector2(360f, 25f));
                    valuablePage.AddElementToPage(disableAllButton, new Vector2(527f, 25f));
                    
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