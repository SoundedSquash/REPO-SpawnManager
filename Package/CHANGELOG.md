## 0.6.3
- Read disabled name settings more consistently (ignore spaces around the values).

## 0.6.2
- Minor bug fix that could cause the level not to load.

## 0.6.1
- Fix bug with enemy group spawns.

## 0.6.0
- Support for the Monster Update.
 
## 0.5.5
- Added per-level valuables to the Spawn Manager menu.

## 0.5.4
- Fix a bug where per-level items wouldn't work in the same session.

## 0.5.3
- Added per-level items to the Spawn Manager menu. Includes the Shop level.

## 0.5.2
- Add setting to disable the Spawn Manager button on the main menu.

## 0.5.1
- Moved enemies buttons into a submenu to prevent clutter on the main page. 
- Sorted buttons in the menu alphabetically.

## 0.5.0
- Added support for disabling shop items!
- Reordered the menu so per-level enemies buttons are last.

## 0.4.2
- Fixed a bug where per-level enemies would not be applied if the spawn manager menu had not been opened before starting a run.
- Bumped MenuLib to 2.4.1.

## 0.4.1
- Fixed menus for custom level enemies not working.

## 0.4.0
- Added feature to disable enemy spawns on a per-level basis (thanks [Justin-dr](https://github.com/Justin-dr))

## 0.3.3
- MenuLib is now a soft dependency, meaning it's not a requirement.
  - It is still highly recommended to keep MenuLib as the in-game Spawn Manager menu is missing without it.
- Fixed a small performance issue with the menu.
- Bumped MenuLib to 2.3.0.

## 0.3.2
- Hide configs from REPOConfig to prevent confusion.
- Fill tall items from big instead of wide for stability.
- Developer stability when using Unity Editor.

## 0.3.1
- Prevent leaving the menu if level selection is invalid.

## 0.3.0
- Valuables can be changed without restarting.
- The chance of spawning large items in small spots is greatly reduced.
  - Added a configurable default valuable. Only seen if not enough valuables are enabled.
- Various bug fixes.

## 0.2.2
- Bump MenuLib version to 2.1.3.

## 0.2.0
- Added level manager to enable/disable levels.

## 0.1.5
- Updated to support MenuLib 2.1.1. This fixes the issue where on/off would not show the correct values.

## 0.1.4
- Fixed error while loading level in a multiplayer lobby.

## 0.1.3
- Updated to support MenuLib 2.0.0.

## 0.1.2
- Fixed error while loading level when there weren't enough unique valuables. This still requires at least one generic valuable to be enabled.
- Known Issue: Disabling too many valuables can cause a soft-lock if there is not at least one generic valuable enabled.

## 0.1.1
- Fixed error while loading level when there weren't enough enemies enabled causing a soft-lock.
- Fixed popup text on Disable All enemies.

## 0.1.0
- Initial release.