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