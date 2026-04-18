
# unity-junior-test

Unity shop system implementation (ScriptableObjects, UI, inventory)

---

## Project Structure

The project is divided into three main scenes: Menu, Shop, and Game, with a GameManager handling transitions.  
Scripts contain all core systems including player movement, stats, shop logic, inventory, and data persistence.  
ScriptableObjects are used to define shop items and their stat modifiers in a data-driven way.  
Prefabs store reusable objects such as characters, springs, UI elements, and visual effects.  
PlayerDataManager acts as a central system for saving/loading coins, owned items, and equipped items.  
Assets and UI resources are organized separately to keep visuals and logic cleanly separated.

---

## How Systems Work

The ShopController handles purchases and equips: when the player buys an item, it checks/spends coins through PlayerInventory, marks the item as owned, and saves that state through PlayerDataManager. When an item is equipped, the equipped selection is also saved in PlayerDataManager, which acts as the persistence layer for coins, owned items, and equipped items.

The inventory system is the combination of visible owned items in the shop and the saved data in PlayerDataManager. PlayerInventory handles coin logic and UI, while PlayerDataManager stores long-term progress across scenes and sessions.

The stat modification system is handled by PlayerStats. When an item is equipped, SimpleEquipController loads it, applies its effects, and updates visuals. PlayerStats resets to base values and then applies bonuses such as jump force, extra air jumps, shield, double coins, or charge effect so gameplay always reflects the current equipment.

---

## Limitations & Future Improvements

The shield system is currently visual only and does not yet provide gameplay protection. This is mainly due to the current game design, where falling off the level results in an immediate loss with no recovery opportunity. Given more time, I would introduce mid-air enemies or hazards so the shield can absorb damage and add meaningful gameplay value.

Additionally, I would improve the lower boundary interaction by either allowing the player to bounce back (e.g. water launching the player upward) or by redesigning the fail state to feel less abrupt.

On the content side, I would expand the game with more characters, items, enemies, and obstacle variations to create more engaging and dynamic gameplay.

---

## Theory Answers

### Q1 - What is a ScriptableObject and why is it useful? When would you use one instead of a MonoBehaviour?

A ScriptableObject is basically an object that holds data and can be reused across the project. Instead of writing new scripts for every variation, you create one structure and reuse it with different values.

For example, like in games such as Fortnite, many items share the same behavior but differ in visuals or effects. Instead of rewriting logic, you just plug in different data (animations, stats, etc.).

In my game, I used ScriptableObjects for shop items, so I can easily create new characters or springs and assign properties like extra jump force, shield, double coins, or extra air jumps without changing code.

I also used this approach in previous projects for reward systems, where UI prefabs were reused and only the data (like coin amount or reward type) was changed instead of rebuilding the UI each time.

I would use ScriptableObjects instead of MonoBehaviours when I need reusable, data-driven systems that don’t require scene-based behavior or lifecycle functions like Update().

---

### Q2 - Explain the Single Responsibility Principle in your own words. How would you extend your system to support new stat types without rewriting existing code?

The Single Responsibility Principle means each script should handle one clear part of the system and not more than that. For example, the shop should manage buying and equipping, the player should manage movement and stats, and enemies should manage enemy behavior. We do not want one script doing too many unrelated things, because later it becomes messy and much harder to debug.

If responsibilities are mixed, even a small bug can become confusing. For example, if the health bar is not showing correctly, I should be able to check the UI or health-related scripts, not search through enemy or shop code. Keeping scripts focused makes the system easier to understand, extend, and maintain.

To support new stat types without rewriting existing code, I would extend the data-driven approach. I would add new stat fields to the item data object and let the stats system read and apply them in one place, instead of hardcoding logic in multiple systems. This way, adding new features requires minimal changes and keeps the system clean.
