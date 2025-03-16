using System;
using System.Linq;
using BepInEx.Configuration;
using BepInEx.Logging;
using MenuLib;
using UnityEngine;

namespace SpawnManager.Managers
{
    public class MenuModManager
    {
        private static REPOButton currentPageButton;
        
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
            
            CreateEnemyPage(out var modButton);
            menu.AddElementToScrollView(modButton, new Vector2(0f, -80f + 0 * -34f));

            return menu;
        }

        private static void CreateEnemyPage(out REPOButton modButton)
        {
            var enemyPage = new REPOPopupPage("Enemies", enemyPage =>
            {
                enemyPage.SetPosition(new Vector2(500.52f, 190.6f));
                enemyPage.SetSize(new Vector2(310f, 342f));
                enemyPage.SetMaskPadding(new Padding(0, 70, 0, 50));
            });

            var modButtonTemp = modButton = new REPOButton("Enemies", null);
            modButton.SetOnClick(() =>
            {
                if (currentPageButton == modButtonTemp)
                    return;

                var openPage = new Action(() =>
                {
                    MenuManager.instance.PageCloseAllAddedOnTop();

                    enemyPage.ClearButtons();
                    
                    currentPageButton = modButtonTemp;

                    var enableAllButton = new REPOButton("Enable All", null);
                    enableAllButton.SetOnClick(() =>
                    {
                        MenuAPI.OpenPopup($"Enable All", Color.red,
                            $"Enable all enemies?", "Yes",
                            () =>
                            {
                                Settings.DisabledEnemies.BoxedValue = Settings.DisabledEnemies.DefaultValue;
                                
                                currentPageButton = null;
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

        // TODO
        private static void CreateValuablePage(out REPOButton modButton)
        {
            var enemyPage = new REPOPopupPage("Enemies", enemyPage =>
            {
                enemyPage.SetPosition(new Vector2(500.52f, 190.6f));
                enemyPage.SetSize(new Vector2(310f, 342f));
                enemyPage.SetMaskPadding(new Padding(0, 70, 0, 50));
            });

            var modButtonTemp = modButton = new REPOButton("Enemies", null);
            modButton.SetOnClick(() =>
            {
                if (currentPageButton == modButtonTemp)
                    return;

                var openPage = new Action(() =>
                {
                    MenuManager.instance.PageCloseAllAddedOnTop();

                    enemyPage.ClearButtons();
                    
                    currentPageButton = modButtonTemp;

                    var enableAllButton = new REPOButton("Enable All", null);
                    enableAllButton.SetOnClick(() =>
                    {
                        MenuAPI.OpenPopup($"Enable All", Color.red,
                            $"Enable all enemies?", "Yes",
                            () =>
                            {
                                Settings.DisabledEnemies.BoxedValue = Settings.DisabledEnemies.DefaultValue;
                                
                                currentPageButton = null;
                                modButtonTemp.onClick.Invoke();
                            }, "No");
                    });

                    //modPage.AddElementToPage(saveChangesButton, new Vector2(365, 34f));
                    enemyPage.AddElementToPage(enableAllButton, new Vector2(560f, 34f));
                    
                    var enemiesDictionary = EnemyManager.EnemySpawnList;
                    var enemyNames = enemiesDictionary.Keys.ToArray();

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
    }
}