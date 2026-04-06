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
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public static class Level5NPCSetup
    {
        // ----------------------------------------------------------------
        // Pixel-sprite helpers
        // ----------------------------------------------------------------

        private static readonly Dictionary<string, Sprite> _spriteCache =
            new Dictionary<string, Sprite>();

        /// <summary>
        /// Creates (or retrieves from cache) a pixel-art Sprite.
        /// drawFunc receives a transparent Texture2D and should paint on it;
        /// Apply() is called automatically after drawFunc returns.
        /// </summary>
        private static Sprite CreatePixelSprite(string key, int size,
            System.Action<Texture2D> drawFunc)
        {
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;  // crisp pixel art

            // Start fully transparent
            Color[] clear = new Color[size * size];
            for (int i = 0; i < clear.Length; i++) clear[i] = Color.clear;
            tex.SetPixels(clear);

            drawFunc(tex);
            tex.Apply();

            Sprite spr = Sprite.Create(tex, new Rect(0, 0, size, size),
                                       new Vector2(0.5f, 0.5f), size);
            _spriteCache[key] = spr;
            return spr;
        }

        // Fills a solid rectangle on tex (y=0 is bottom).
        private static void FillPx(Texture2D tex, int x0, int y0, int x1, int y1, Color c)
        {
            for (int y = y0; y <= y1; y++)
                for (int x = x0; x <= x1; x++)
                    tex.SetPixel(x, y, c);
        }

        private static Sprite GetNPCSprite(string npcName)
        {
            return CreatePixelSprite(npcName, 16, tex =>
            {
                Color body  = NpcColor(npcName);
                Color dark  = new Color(body.r * 0.55f, body.g * 0.55f, body.b * 0.55f);
                Color skin  = new Color(1f, 0.85f, 0.68f);
                Color black = new Color(0.08f, 0.08f, 0.08f);
                Color silver = new Color(0.75f, 0.78f, 0.85f);
                Color gold   = new Color(1f, 0.82f, 0.1f);

                // ── Base humanoid silhouette (16×16, y=0 bottom) ──────
                // Legs: rows 1-4, cols 5-6 and 9-10
                FillPx(tex, 5, 1, 6, 4, body);
                FillPx(tex, 9, 1, 10, 4, body);
                // Belt/waist: row 5, cols 5-10
                FillPx(tex, 5, 5, 10, 5, dark);
                // Torso: rows 6-10, cols 4-11
                FillPx(tex, 4, 6, 11, 10, body);
                // Neck: rows 10-11, cols 7-8
                FillPx(tex, 7, 10, 8, 11, skin);
                // Head: rows 11-14, cols 5-10
                FillPx(tex, 5, 11, 10, 14, skin);
                // Eyes
                tex.SetPixel(6, 13, black);
                tex.SetPixel(9, 13, black);

                // ── Per-NPC accessories ───────────────────────────────
                switch (npcName)
                {
                    case "Blackwater Armsman":
                        // Helmet (dark) on top of head
                        FillPx(tex, 4, 14, 11, 15, dark);
                        FillPx(tex, 5, 15, 10, 15, dark);
                        // Sword on right: diagonal
                        for (int i = 0; i < 6; i++)
                        { tex.SetPixel(12 + (i > 3 ? 1 : 0), 9 - i, silver); }
                        tex.SetPixel(12, 9, silver); tex.SetPixel(13, 9, silver);
                        break;

                    case "Blackwater Cook":
                        // Chef hat: wide white block above head
                        FillPx(tex, 4, 14, 11, 15, Color.white);
                        FillPx(tex, 6, 15, 9, 15, Color.white);
                        // Apron: white stripe down torso center
                        FillPx(tex, 7, 6, 8, 10, Color.white);
                        break;

                    case "Blackwater Jailer":
                        // Dark cap
                        FillPx(tex, 4, 14, 11, 15, dark);
                        // Key shape hanging at left hip
                        FillPx(tex, 2, 5, 3, 7, gold);    // key handle
                        tex.SetPixel(2, 8, gold);          // key bow
                        tex.SetPixel(3, 8, gold);
                        tex.SetPixel(1, 8, gold);
                        tex.SetPixel(2, 9, gold);
                        break;

                    case "Blackwater Navigator":
                        // Blue captain's hat
                        FillPx(tex, 3, 14, 12, 15, body);
                        // Compass: small circle on chest
                        tex.SetPixel(7, 8, gold); tex.SetPixel(8, 8, gold);
                        tex.SetPixel(6, 7, gold); tex.SetPixel(9, 7, gold);
                        tex.SetPixel(7, 6, gold); tex.SetPixel(8, 6, gold);
                        break;

                    case "Captain Blackwater":
                        // Tricorn hat (dark red + gold trim)
                        FillPx(tex, 3, 14, 12, 15, dark);
                        FillPx(tex, 5, 15, 10, 15, dark);
                        tex.SetPixel(3, 15, gold); tex.SetPixel(12, 15, gold);
                        // Gold epaulettes on shoulders
                        FillPx(tex, 3, 10, 4, 10, gold);
                        FillPx(tex, 11, 10, 12, 10, gold);
                        // Coat collar (dark)
                        FillPx(tex, 4, 9, 5, 10, dark);
                        FillPx(tex, 10, 9, 11, 10, dark);
                        break;
                }
            });
        }

        private static Sprite GetItemSprite(string itemName)
        {
            return CreatePixelSprite(itemName, 16, tex =>
            {
                Color silver = new Color(0.78f, 0.80f, 0.88f);
                Color dark   = new Color(0.35f, 0.35f, 0.4f);
                Color gray   = new Color(0.55f, 0.55f, 0.60f);
                Color brown  = new Color(0.55f, 0.30f, 0.10f);
                Color cyan   = new Color(0.2f, 0.85f, 0.9f);
                Color dcyan  = new Color(0.1f, 0.5f, 0.55f);
                Color gold   = new Color(1f, 0.82f, 0.1f);
                Color dgold  = new Color(0.75f, 0.55f, 0.05f);

                switch (itemName)
                {
                    case "Pirate Cutlass":
                        // Blade: diagonal silver line (bottom-left to top-right)
                        for (int i = 0; i < 11; i++)
                        {
                            tex.SetPixel(3 + i, 3 + i, silver);
                            if (i < 10) tex.SetPixel(4 + i, 3 + i, silver);
                        }
                        // Guard (crosspiece)
                        FillPx(tex, 2, 5, 5, 6, dark);
                        // Hilt
                        FillPx(tex, 2, 2, 4, 4, brown);
                        break;

                    case "Iron Shield Vest":
                        // Hexagonal shield shape
                        FillPx(tex, 4, 2, 11, 13, gray);
                        FillPx(tex, 2, 5, 13, 10, gray);
                        // Darker border
                        FillPx(tex, 4, 2, 11, 2, dark);
                        FillPx(tex, 4, 13, 11, 13, dark);
                        FillPx(tex, 2, 5, 2, 10, dark);
                        FillPx(tex, 13, 5, 13, 10, dark);
                        // Center cross
                        FillPx(tex, 7, 4, 8, 11, dark);
                        FillPx(tex, 4, 7, 11, 8, dark);
                        break;

                    case "Sea Boots":
                        // Boot silhouette: foot + shaft
                        FillPx(tex, 3, 1, 12, 4, brown);    // foot
                        FillPx(tex, 3, 4, 7, 12, brown);    // shaft
                        FillPx(tex, 8, 1, 12, 2, dark);     // sole edge
                        FillPx(tex, 3, 12, 7, 13, dark);    // cuff
                        break;

                    case "Coral Ring":
                        // Circle outline in cyan
                        for (int x = 4; x <= 11; x++) { tex.SetPixel(x, 3,  cyan); tex.SetPixel(x, 12, cyan); }
                        for (int y = 4; y <= 11; y++) { tex.SetPixel(3,  y,  cyan); tex.SetPixel(12, y, cyan); }
                        // Corners
                        tex.SetPixel(4, 4, cyan); tex.SetPixel(11, 4, cyan);
                        tex.SetPixel(4, 11, cyan); tex.SetPixel(11, 11, cyan);
                        // Inner ring (darker)
                        for (int x = 5; x <= 10; x++) { tex.SetPixel(x, 4,  dcyan); tex.SetPixel(x, 11, dcyan); }
                        for (int y = 5; y <= 10; y++) { tex.SetPixel(4,  y,  dcyan); tex.SetPixel(11, y, dcyan); }
                        break;

                    case "Gold Coins":
                        // Three stacked coin circles
                        FillPx(tex, 5, 1, 10, 5, gold);    // bottom coin
                        FillPx(tex, 3, 1, 4, 5, gold); FillPx(tex, 11, 1, 12, 5, gold);
                        FillPx(tex, 5, 5, 10, 9, gold);    // middle coin
                        FillPx(tex, 3, 5, 4, 9, gold); FillPx(tex, 11, 5, 12, 9, gold);
                        FillPx(tex, 5, 9, 10, 13, gold);   // top coin
                        FillPx(tex, 3, 9, 4, 13, gold); FillPx(tex, 11, 9, 12, 13, gold);
                        // Dark edge on each coin
                        for (int x = 3; x <= 12; x++) { tex.SetPixel(x, 1, dgold); tex.SetPixel(x, 13, dgold); }
                        for (int y = 1; y <= 13; y++) { tex.SetPixel(3, y, dgold); tex.SetPixel(12, y, dgold); }
                        break;
                }
            });
        }

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
            SpriteRenderer sr = npcObj.AddComponent<SpriteRenderer>();
            sr.sprite       = GetNPCSprite(name);
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
            SetField(spawner, t, "enemySprite",      GetNPCSprite(name), flags);
            // enemyNpc left null — OnPlayerWon() calls Destroy(null) which is safe

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
        //
        // KNOWN LIMITATION (Bug 4): BattleManager always calls EnemyTurn() after
        // PlayerAttack() if the enemy survives (see BattleManager.PlayerAttack()).
        // This means the NPC retaliates even on a correct answer. This is the
        // teammate's battle-system design — BattleManager is not our file.
        // EnemySpawner has no field that suppresses the enemy's retaliatory turn.
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
            // Bug 6 verified: Pirate Cutlass baseDamage=5 here and ATK bonus=(5,0)
            // in Level5EquipManager._stats — both agree on +5 ATK.
            SpawnWeaponItem(
                "Pirate Cutlass",
                new Vector3(35f, 30f, 0f),
                "A sharp cutlass fit for a pirate captain.",
                30, Rarity.Uncommon, 5, WeaponType.Cutlass);

            SpawnClothingItem(
                "Iron Shield Vest",
                new Vector3(55f, 30f, 0f),
                "A reinforced vest offering solid protection.",
                40, Rarity.Uncommon, ClothingSlot.Torso, 8);

            SpawnClothingItem(
                "Sea Boots",
                new Vector3(30f, 50f, 0f),
                "Sturdy boots made for ship decks.",
                20, Rarity.Common, ClothingSlot.Feet, 2);

            SpawnClothingItem(
                "Coral Ring",
                new Vector3(60f, 50f, 0f),
                "A ring carved from living coral.",
                25, Rarity.Common, ClothingSlot.Head, 3);

            SpawnGoldCoins(
                "Gold Coins",
                new Vector3(44f, 20f, 0f),
                50);
        }

        // ── Private deck-item helpers ─────────────────────────────────────────

        private static void SpawnWeaponItem(
            string itemName, Vector3 position,
            string description, int goldValue, Rarity rarity,
            int baseDamage, WeaponType weaponType)
        {
            if (BlackwaterState.collectedItems.Contains(itemName)) return;

            Sprite spr = GetItemSprite(itemName);
            GameObject go = CreateItemBase(itemName, position, spr, 5, 1.5f);

            ItemPickup pickup = go.AddComponent<ItemPickup>();
            Weapon item = new Weapon(
                itemName.ToLower().Replace(' ', '_'),
                itemName, description,
                goldValue, rarity, baseDamage, weaponType);
            // Pass the sprite so dropped items remain visible (Bug 1 fix)
            pickup.SetItemData(item, spr);

            DeckItemTracker tracker = go.AddComponent<DeckItemTracker>();
            tracker.trackedItemName = itemName;
        }

        private static void SpawnClothingItem(
            string itemName, Vector3 position,
            string description, int goldValue, Rarity rarity,
            ClothingSlot slot, int defenseBonus)
        {
            if (BlackwaterState.collectedItems.Contains(itemName)) return;

            Sprite spr = GetItemSprite(itemName);
            GameObject go = CreateItemBase(itemName, position, spr, 5, 1.5f);

            ItemPickup pickup = go.AddComponent<ItemPickup>();
            Clothing item = new Clothing(
                itemName.ToLower().Replace(' ', '_'),
                itemName, description,
                goldValue, rarity, slot, defenseBonus);
            // Pass the sprite so dropped items remain visible (Bug 1 fix)
            pickup.SetItemData(item, spr);

            DeckItemTracker tracker = go.AddComponent<DeckItemTracker>();
            tracker.trackedItemName = itemName;
        }

        private static void SpawnGoldCoins(string itemName, Vector3 position, int amount)
        {
            if (BlackwaterState.collectedItems.Contains(itemName)) return;

            Sprite spr = GetItemSprite(itemName);
            GameObject go = CreateItemBase(itemName, position, spr, 5, 1.5f);

            // ItemType.Treasure triggers inventoryUI.AddGold(goldValue) inside ItemPickup
            ItemPickup pickup = go.AddComponent<ItemPickup>();
            TreasureItem item = new TreasureItem(
                "gold_coins", itemName, "A pile of golden coins.",
                amount, Rarity.Common, ItemType.Treasure);
            pickup.SetItemData(item, spr);

            DeckItemTracker tracker = go.AddComponent<DeckItemTracker>();
            tracker.trackedItemName = itemName;
        }

        /// <summary>Creates the shared visual base for a deck item using a pixel sprite.</summary>
        private static GameObject CreateItemBase(
            string name, Vector3 position, Sprite spr, int sortOrder, float scale)
        {
            GameObject go = new GameObject(name);
            go.transform.position   = position;
            go.transform.localScale = new Vector3(scale, scale, 1f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = spr;
            sr.color        = Color.white;  // sprite already has colour baked in
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
