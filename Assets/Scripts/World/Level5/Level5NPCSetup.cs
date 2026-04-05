// ============================================================
// READINGS FROM STEP 1 (teammate files)
// ============================================================
// EnemySpawner fields found:
//   enemyDisplayName (string, private SerializeField)
//   enemyMaxHealth   (int,    private SerializeField)
//   enemyAttackPower (int,    private SerializeField)
//   enemySprite      (Sprite, private SerializeField)
//   enemyType        (string, private SerializeField)
//   enemyNpc         (GameObject, private SerializeField)
//   questionLevel    (int,    private SerializeField)
//   returnScene      (string, private SerializeField)
//
// ItemPickup fields found:
//   itemId, itemName, itemDescription, goldValue, rarity, itemType,
//   worldSprite, baseDamage, weaponType, clothingSlot, defenseBonus
//   Public method: SetItemData(Item item, Sprite sprite)
//   Gold handled via ItemType.Treasure → InventoryUI.AddGold(goldValue)
//
// HazardZone SendMessage signature:
//   SendMessage("TakeDamage", int damagePerTick, DontRequireReceiver)
//   Parameter type: int
//
// PlayerController.TakeDamage exists: NO
//   PlayerHazardShield is therefore the sole TakeDamage handler on the player.
//
// Item constructors:
//   Weapon(id, name, desc, goldValue, rarity, baseDamage, weaponType)
//   Clothing(id, name, desc, goldValue, rarity, slot, defenseBonus)
//   TreasureItem(id, name, desc, goldValue, rarity, itemType)
//
// Scene names confirmed from BlackwaterSceneBuilder.cs:
//   BlackwaterFlagship (main deck — NOT BlackwaterMainDeck)
//   BlackwaterArmory   (NOTE: no 'u' — spec says Armoury but builder uses Armory)
//   BlackwaterMessHall, BlackwaterBrig, BlackwaterNavigationRoom,
//   BlackwaterCaptainsQuarters, BlackwaterLowerDeck
//
// Portal: NO SetPendingSpawn method. Use reflection on private static fields
//   _pendingSpawn (Vector2) and _hasPendingSpawn (bool).
// ============================================================

using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public static class Level5NPCSetup
    {
        // ----------------------------------------------------------------
        // NPC spawning
        // ----------------------------------------------------------------

        /// <summary>
        /// Spawns a single enemy NPC with EnemySpawner + Level5NPCReward.
        /// Returns immediately if the NPC was already defeated this session.
        /// </summary>
        public static void SpawnEnemyNPC(
            string name, float x, float y,
            int hp, int attack, int questionLevel,
            bool grantsSpeedBoots, bool dropsKeyPiece)
        {
            if (BlackwaterState.defeatedNPCs.Contains(name)) return;

            // ── Create root object ────────────────────────────────────────
            GameObject npcObj = new GameObject(name);
            npcObj.transform.position = new Vector3(x, y, 0f);

            // ── SpriteRenderer ────────────────────────────────────────────
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            Sprite spr = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

            SpriteRenderer sr = npcObj.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.color  = NpcColor(name);
            sr.sortingOrder = 3;
            npcObj.transform.localScale = new Vector3(2f, 2f, 1f);

            // ── Collider + Rigidbody ──────────────────────────────────────
            BoxCollider2D col = npcObj.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size      = new Vector2(2f, 2f);

            Rigidbody2D rb = npcObj.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;

            // ── EnemySpawner via reflection ───────────────────────────────
            EnemySpawner spawner = npcObj.AddComponent<EnemySpawner>();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            Type t = typeof(EnemySpawner);

            SetField(spawner, t, "enemyDisplayName", name,    flags);
            SetField(spawner, t, "enemyMaxHealth",   hp,      flags);
            SetField(spawner, t, "enemyAttackPower", attack,  flags);
            SetField(spawner, t, "questionLevel",    questionLevel, flags);
            SetField(spawner, t, "returnScene",      SceneManager.GetActiveScene().name, flags);
            SetField(spawner, t, "enemyType",        "Warrior", flags);
            // enemySprite left null — battle UI handles missing sprites gracefully
            // enemyNpc left null   — OnPlayerWon() calls Destroy(null) which is safe

            // ── Reward component ──────────────────────────────────────────
            Level5NPCReward reward = npcObj.AddComponent<Level5NPCReward>();
            reward.npcName          = name;
            reward.grantsSpeedBoots = grantsSpeedBoots;
            reward.dropsKeyPiece    = dropsKeyPiece;
        }

        /// <summary>Sets a field by name via reflection; logs a warning if not found.</summary>
        private static void SetField(object target, System.Type type, string fieldName, object value, BindingFlags flags)
        {
            FieldInfo fi = type.GetField(fieldName, flags);
            if (fi != null)
                fi.SetValue(target, value);
            else
                Debug.LogWarning("Level5NPCSetup: field '" + fieldName + "' not found on " + type.Name);
        }

        private static Color NpcColor(string name)
        {
            switch (name)
            {
                case "Blackwater Armsman":  return Color.red;
                case "Blackwater Cook":     return new Color(1f, 0.5f, 0f);
                case "Blackwater Jailer":   return new Color(0.3f, 0.3f, 0.3f);
                case "Blackwater Navigator":return Color.blue;
                case "Captain Blackwater":  return new Color(0.5f, 0f, 0f);
                default:                    return Color.white;
            }
        }

        // ----------------------------------------------------------------
        // Per-scene NPC setup
        // NOTE: "BlackwaterArmory" (no 'u') matches BlackwaterSceneBuilder.cs
        // ----------------------------------------------------------------

        public static void SetupRoomNPCs(string sceneName)
        {
            switch (sceneName)
            {
                case "BlackwaterArmory":
                    SpawnEnemyNPC("Blackwater Armsman",   16, 11, 80,  12, 5, false, true);
                    break;
                case "BlackwaterMessHall":
                    SpawnEnemyNPC("Blackwater Cook",      16, 11, 60,   8, 5, false, true);
                    break;
                case "BlackwaterBrig":
                    SpawnEnemyNPC("Blackwater Jailer",    11, 11, 100, 15, 5, true,  true);
                    break;
                case "BlackwaterNavigationRoom":
                    SpawnEnemyNPC("Blackwater Navigator", 11, 11, 70,  10, 5, false, true);
                    break;
                case "BlackwaterCaptainsQuarters":
                    // questionLevel 6 (harder boss questions); dropsKeyPiece=false because
                    // all 5 keys are required BEFORE entering this room.
                    SpawnEnemyNPC("Captain Blackwater",   12, 16, 200, 20, 6, false, false);
                    break;
            }
        }

        // ----------------------------------------------------------------
        // Flagship deck items
        // ----------------------------------------------------------------

        /// <summary>
        /// Spawns collectible items on the BlackwaterFlagship deck.
        /// Checks BlackwaterState.collectedItems before each spawn so items
        /// do NOT reappear when the player re-enters the scene.
        /// </summary>
        public static void SpawnDeckItems()
        {
            SpawnWeaponItem(
                "Pirate Cutlass",
                new Vector3(35f, 30f, 0f),
                new Color(0.85f, 0.85f, 0.85f),   // silver-white
                "A sharp cutlass fit for a pirate captain.",
                30, Rarity.Uncommon, 5, WeaponType.Cutlass);

            SpawnClothingItem(
                "Iron Shield Vest",
                new Vector3(55f, 30f, 0f),
                Color.gray,
                "A reinforced vest offering solid protection.",
                40, Rarity.Uncommon, ClothingSlot.Torso, 8);

            SpawnClothingItem(
                "Sea Boots",
                new Vector3(30f, 50f, 0f),
                new Color(0.6f, 0.3f, 0.1f),       // brown
                "Sturdy boots made for ship decks.",
                20, Rarity.Common, ClothingSlot.Feet, 2);

            SpawnClothingItem(
                "Coral Ring",
                new Vector3(60f, 50f, 0f),
                Color.cyan,
                "A ring carved from living coral.",
                25, Rarity.Common, ClothingSlot.Head, 3);

            SpawnGoldCoins(
                "Gold Coins",
                new Vector3(44f, 20f, 0f),
                50);
        }

        // ── Private deck-item helpers ─────────────────────────────────────────

        private static void SpawnWeaponItem(
            string itemName, Vector3 position, Color color,
            string description, int goldValue, Rarity rarity,
            int baseDamage, WeaponType weaponType)
        {
            if (BlackwaterState.collectedItems.Contains(itemName)) return;

            GameObject go = CreateItemBase(itemName, position, color, 5, 1.5f);

            ItemPickup pickup = go.AddComponent<ItemPickup>();
            Weapon item = new Weapon(
                itemName.ToLower().Replace(' ', '_'),
                itemName, description,
                goldValue, rarity, baseDamage, weaponType);
            pickup.SetItemData(item, null);

            DeckItemTracker tracker = go.AddComponent<DeckItemTracker>();
            tracker.trackedItemName = itemName;
        }

        private static void SpawnClothingItem(
            string itemName, Vector3 position, Color color,
            string description, int goldValue, Rarity rarity,
            ClothingSlot slot, int defenseBonus)
        {
            if (BlackwaterState.collectedItems.Contains(itemName)) return;

            GameObject go = CreateItemBase(itemName, position, color, 5, 1.5f);

            ItemPickup pickup = go.AddComponent<ItemPickup>();
            Clothing item = new Clothing(
                itemName.ToLower().Replace(' ', '_'),
                itemName, description,
                goldValue, rarity, slot, defenseBonus);
            pickup.SetItemData(item, null);

            DeckItemTracker tracker = go.AddComponent<DeckItemTracker>();
            tracker.trackedItemName = itemName;
        }

        private static void SpawnGoldCoins(string itemName, Vector3 position, int amount)
        {
            if (BlackwaterState.collectedItems.Contains(itemName)) return;

            GameObject go = CreateItemBase(itemName, position, Color.yellow, 5, 1.5f);

            // ItemType.Treasure triggers inventoryUI.AddGold(goldValue) inside ItemPickup
            ItemPickup pickup = go.AddComponent<ItemPickup>();
            TreasureItem item = new TreasureItem(
                "gold_coins", itemName, "A pile of golden coins.",
                amount, Rarity.Common, ItemType.Treasure);
            pickup.SetItemData(item, null);

            DeckItemTracker tracker = go.AddComponent<DeckItemTracker>();
            tracker.trackedItemName = itemName;
        }

        /// <summary>Creates the shared visual base for a deck item.</summary>
        private static GameObject CreateItemBase(
            string name, Vector3 position, Color color, int sortOrder, float scale)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            Sprite spr = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

            GameObject go = new GameObject(name);
            go.transform.position   = position;
            go.transform.localScale = new Vector3(scale, scale, 1f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = spr;
            sr.color        = color;
            sr.sortingOrder = sortOrder;

            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size      = new Vector2(1.5f, 1.5f);

            return go;
        }
    }

    // ────────────────────────────────────────────────────────────────────────────
    // Tracks deck-item pickups so they don't respawn when the player re-enters
    // BlackwaterFlagship. Fires alongside ItemPickup.OnTriggerEnter2D.
    // (Destroy is queued by ItemPickup so both triggers run before object cleanup.)
    // ────────────────────────────────────────────────────────────────────────────
    public class DeckItemTracker : MonoBehaviour
    {
        public string trackedItemName;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            BlackwaterState.collectedItems.Add(trackedItemName);
        }
    }
}
