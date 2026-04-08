using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public static class Level5NPCSetup
    {
        private static readonly Dictionary<string, Sprite> _spriteCache =
            new Dictionary<string, Sprite>();

        private static Sprite CreatePixelSprite(string key, int size,
            System.Action<Texture2D> drawFunc)
        {
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;

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

                FillPx(tex, 5, 1, 6, 4, body);
                FillPx(tex, 9, 1, 10, 4, body);
                FillPx(tex, 5, 5, 10, 5, dark);
                FillPx(tex, 4, 6, 11, 10, body);
                FillPx(tex, 7, 10, 8, 11, skin);
                FillPx(tex, 5, 11, 10, 14, skin);
                tex.SetPixel(6, 13, black);
                tex.SetPixel(9, 13, black);

                switch (npcName)
                {
                    case "Blackwater Armsman":
                        FillPx(tex, 4, 14, 11, 15, dark);
                        FillPx(tex, 5, 15, 10, 15, dark);
                        for (int i = 0; i < 6; i++)
                        { tex.SetPixel(12 + (i > 3 ? 1 : 0), 9 - i, silver); }
                        tex.SetPixel(12, 9, silver); tex.SetPixel(13, 9, silver);
                        break;

                    case "Blackwater Cook":
                        FillPx(tex, 4, 14, 11, 15, Color.white);
                        FillPx(tex, 6, 15, 9, 15, Color.white);
                        FillPx(tex, 7, 6, 8, 10, Color.white);
                        break;

                    case "Blackwater Jailer":
                        FillPx(tex, 4, 14, 11, 15, dark);
                        FillPx(tex, 2, 5, 3, 7, gold);
                        tex.SetPixel(2, 8, gold);
                        tex.SetPixel(3, 8, gold);
                        tex.SetPixel(1, 8, gold);
                        tex.SetPixel(2, 9, gold);
                        break;

                    case "Blackwater Navigator":
                        FillPx(tex, 3, 14, 12, 15, body);
                        tex.SetPixel(7, 8, gold); tex.SetPixel(8, 8, gold);
                        tex.SetPixel(6, 7, gold); tex.SetPixel(9, 7, gold);
                        tex.SetPixel(7, 6, gold); tex.SetPixel(8, 6, gold);
                        break;

                    case "Captain Blackwater":
                        FillPx(tex, 3, 14, 12, 15, dark);
                        FillPx(tex, 5, 15, 10, 15, dark);
                        tex.SetPixel(3, 15, gold); tex.SetPixel(12, 15, gold);
                        FillPx(tex, 3, 10, 4, 10, gold);
                        FillPx(tex, 11, 10, 12, 10, gold);
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
                        for (int i = 0; i < 11; i++)
                        {
                            tex.SetPixel(3 + i, 3 + i, silver);
                            if (i < 10) tex.SetPixel(4 + i, 3 + i, silver);
                        }
                        FillPx(tex, 2, 5, 5, 6, dark);
                        FillPx(tex, 2, 2, 4, 4, brown);
                        break;

                    case "Iron Shield Vest":
                        FillPx(tex, 4, 2, 11, 13, gray);
                        FillPx(tex, 2, 5, 13, 10, gray);
                        FillPx(tex, 4, 2, 11, 2, dark);
                        FillPx(tex, 4, 13, 11, 13, dark);
                        FillPx(tex, 2, 5, 2, 10, dark);
                        FillPx(tex, 13, 5, 13, 10, dark);
                        FillPx(tex, 7, 4, 8, 11, dark);
                        FillPx(tex, 4, 7, 11, 8, dark);
                        break;

                    case "Sea Boots":
                        FillPx(tex, 3, 1, 12, 4, brown);
                        FillPx(tex, 3, 4, 7, 12, brown);
                        FillPx(tex, 8, 1, 12, 2, dark);
                        FillPx(tex, 3, 12, 7, 13, dark);
                        break;

                    case "Coral Ring":
                        for (int x = 4; x <= 11; x++) { tex.SetPixel(x, 3,  cyan); tex.SetPixel(x, 12, cyan); }
                        for (int y = 4; y <= 11; y++) { tex.SetPixel(3,  y,  cyan); tex.SetPixel(12, y, cyan); }
                        tex.SetPixel(4, 4, cyan); tex.SetPixel(11, 4, cyan);
                        tex.SetPixel(4, 11, cyan); tex.SetPixel(11, 11, cyan);
                        for (int x = 5; x <= 10; x++) { tex.SetPixel(x, 4,  dcyan); tex.SetPixel(x, 11, dcyan); }
                        for (int y = 5; y <= 10; y++) { tex.SetPixel(4,  y,  dcyan); tex.SetPixel(11, y, dcyan); }
                        break;

                    case "Gold Coins":
                        FillPx(tex, 5, 1, 10, 5, gold);
                        FillPx(tex, 3, 1, 4, 5, gold); FillPx(tex, 11, 1, 12, 5, gold);
                        FillPx(tex, 5, 5, 10, 9, gold);
                        FillPx(tex, 3, 5, 4, 9, gold); FillPx(tex, 11, 5, 12, 9, gold);
                        FillPx(tex, 5, 9, 10, 13, gold);
                        FillPx(tex, 3, 9, 4, 13, gold); FillPx(tex, 11, 9, 12, 13, gold);
                        for (int x = 3; x <= 12; x++) { tex.SetPixel(x, 1, dgold); tex.SetPixel(x, 13, dgold); }
                        for (int y = 1; y <= 13; y++) { tex.SetPixel(3, y, dgold); tex.SetPixel(12, y, dgold); }
                        break;
                }
            });
        }

        private static Sprite LoadPawnSprite()
        {
#if UNITY_EDITOR
            UnityEngine.Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(
                "Assets/Tiny Swords (Free Pack)/Units/Purple Units/Pawn/Pawn_Idle.png");
            if (assets != null)
            {
                foreach (UnityEngine.Object obj in assets)
                    if (obj is Sprite s && s.name == "Pawn_Idle_0")
                        return s;
            }
            Debug.LogWarning("[Level5NPCSetup] Pawn_Idle_0 sprite not found at expected path.");
#endif
            return null;
        }

        private static RuntimeAnimatorController LoadPawnController()
        {
#if UNITY_EDITOR
            var ctrl = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                "Assets/Tiny Swords (Free Pack)/Units/Purple Units/Pawn/Pawn_Idle_0 1.controller");
            if (ctrl == null)
            {
                ctrl = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                    "Assets/Tiny Swords (Free Pack)/Units/Purple Units/Pawn/Pawn_Idle_0.controller");
            }
            return ctrl;
#else
            return null;
#endif
        }

        private static Color NpcTint(string name)
        {
            switch (name)
            {
                case "Blackwater Armsman":   return new Color(1f, 0.7f, 0.7f);   // red tint
                case "Blackwater Cook":      return new Color(1f, 0.85f, 0.7f);  // orange tint
                case "Blackwater Jailer":    return new Color(0.7f, 0.7f, 0.7f); // gray tint
                case "Blackwater Navigator": return new Color(0.7f, 0.7f, 1f);   // blue tint
                case "Captain Blackwater":   return new Color(0.8f, 0.4f, 0.4f); // dark red tint
                default:                     return Color.white;
            }
        }

        public static void SpawnEnemyNPC(
            string name, float x, float y,
            int hp, int attack, int questionLevel,
            bool grantsSpeedBoots, bool dropsKeyPiece)
        {
            if (BlackwaterState.defeatedNPCs.Contains(name)) return;

            GameObject npcObj = new GameObject(name);
            npcObj.transform.position = new Vector3(x, y, 0f);

            // try Pawn_Idle_0, fall back to pixel art
            Sprite pawnSprite  = LoadPawnSprite();
            Sprite displaySprite;

            SpriteRenderer sr = npcObj.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 5;

            if (pawnSprite != null)
            {
                sr.sprite = pawnSprite;
                sr.color  = NpcTint(name);
                npcObj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                displaySprite = pawnSprite;

                RuntimeAnimatorController ctrl = LoadPawnController();
                if (ctrl != null)
                {
                    Animator anim = npcObj.AddComponent<Animator>();
                    anim.runtimeAnimatorController = ctrl;
                }
            }
            else
            {
                sr.sprite = GetNPCSprite(name);
                sr.color  = Color.white;
                npcObj.transform.localScale = new Vector3(2f, 2f, 1f);
                displaySprite = sr.sprite;
                Debug.Log("[Level5NPCSetup] Using pixel-art fallback sprite for " + name);
            }

            CapsuleCollider2D col = npcObj.AddComponent<CapsuleCollider2D>();
            col.isTrigger  = true;
            col.size        = new Vector2(2f, 2f);
            col.direction   = CapsuleDirection2D.Vertical;

            Rigidbody2D rb = npcObj.AddComponent<Rigidbody2D>();
            rb.bodyType     = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.constraints  = RigidbodyConstraints2D.FreezeRotation;

            EnemySpawner spawner = npcObj.AddComponent<EnemySpawner>();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            Type t = typeof(EnemySpawner);

            SetField(spawner, t, "enemyDisplayName", name,                            flags);
            SetField(spawner, t, "enemyMaxHealth",   hp,                              flags);
            SetField(spawner, t, "enemyAttackPower", attack,                          flags);
            SetField(spawner, t, "questionLevel",    questionLevel,                   flags);
            SetField(spawner, t, "returnScene",      SceneManager.GetActiveScene().name, flags);
            SetField(spawner, t, "enemyType",        "Pawn",                          flags);
            SetField(spawner, t, "enemySprite",      displaySprite,                   flags);
            SetField(spawner, t, "enemyNpc",         npcObj,                          flags);

            Debug.Log("[Level5NPCSetup] Spawned " + name + " pawnSprite=" + (pawnSprite != null));

            Level5NPCReward reward = npcObj.AddComponent<Level5NPCReward>();
            reward.npcName          = name;
            reward.grantsSpeedBoots = grantsSpeedBoots;
            reward.dropsKeyPiece    = dropsKeyPiece;
        }

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

        private static readonly Dictionary<string, Vector3> _npcSpawnPositions =
            new Dictionary<string, Vector3>
        {
            { "Blackwater Armsman",   new Vector3(16f, 11f, 0f) },
            { "Blackwater Cook",      new Vector3(16f, 11f, 0f) },
            { "Blackwater Jailer",    new Vector3(11f, 11f, 0f) },
            { "Blackwater Navigator", new Vector3(11f, 11f, 0f) },
            { "Captain Blackwater",   new Vector3(12f, 16f, 0f) },
        };

        private static readonly Dictionary<string, (bool grantsSpeedBoots, bool dropsKeyPiece)> _npcRewardData =
            new Dictionary<string, (bool, bool)>
        {
            { "Blackwater Armsman",   (false, true)  },
            { "Blackwater Cook",      (false, true)  },
            { "Blackwater Jailer",    (true,  true)  },
            { "Blackwater Navigator", (false, true)  },
            { "Captain Blackwater",   (false, false) },
        };

        public static void HandlePostBattleReward(string sceneName)
        {
            if (!BattleData.PlayerWon) return;
            BattleData.PlayerWon = false;

            string defeatedName = BattleData.EnemyName;
            if (!_npcRewardData.ContainsKey(defeatedName)) return;
            if (BlackwaterState.defeatedNPCs.Contains(defeatedName)) return;

            BlackwaterState.defeatedNPCs.Add(defeatedName);

            var (grantsSpeedBoots, dropsKeyPiece) = _npcRewardData[defeatedName];

            if (dropsKeyPiece)
            {
                Vector3 pos = _npcSpawnPositions.TryGetValue(defeatedName, out Vector3 p)
                    ? p : new Vector3(12f, 12f, 0f);
                KeyPiecePickup.SpawnKeyPiece(pos);
            }

            if (grantsSpeedBoots)
            {
                BlackwaterState.hasSpeedBoots = true;
                PopupMessage.Show("Speed Boots acquired! You can now survive the gauntlet.", 4f);
            }

            if (defeatedName == "Captain Blackwater")
                Level5EndGameHost.Show();

            Debug.Log("[Level5NPCSetup] HandlePostBattleReward: " + defeatedName +
                      " → defeatedNPCs count=" + BlackwaterState.defeatedNPCs.Count);
        }

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
                    SpawnEnemyNPC("Captain Blackwater",   12, 16, 200, 20, 6, false, false);
                    break;
            }
        }

        public static void SpawnDeckItems()
        {
            Debug.Log("[SpawnDeckItems] collectedItems=[" +
                      string.Join(", ", BlackwaterState.collectedItems) + "]");

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

        // Checks collectedItems AND live inventory so items don't respawn if tracking missed.
        private static bool ShouldSkipItem(string itemName)
        {
            if (BlackwaterState.collectedItems.Contains(itemName))
            {
                Debug.Log("[SpawnDeckItems] Skipping '" + itemName + "' (in collectedItems)");
                return true;
            }

            // Fallback: scan live inventory in case DeckItemTracker missed the pickup
            InventoryUI ui = Level5InventoryBridge.KnownInventoryUI;
            if (ui == null) return false;
            FieldInfo invField = typeof(InventoryUI).GetField(
                "_inventory", BindingFlags.NonPublic | BindingFlags.Instance);
            Inventory inv = invField?.GetValue(ui) as Inventory;
            if (inv == null) return false;
            foreach (Item item in inv.Items)
            {
                if (item.Name == itemName)
                {
                    BlackwaterState.collectedItems.Add(itemName);
                    Debug.Log("[SpawnDeckItems] Skipping '" + itemName + "' (found in inventory, back-filled)");
                    return true;
                }
            }
            Debug.Log("[SpawnDeckItems] Spawning '" + itemName + "'");
            return false;
        }

        private static void SpawnWeaponItem(
            string itemName, Vector3 position,
            string description, int goldValue, Rarity rarity,
            int baseDamage, WeaponType weaponType)
        {
            if (ShouldSkipItem(itemName)) return;

            Sprite spr = GetItemSprite(itemName);
            GameObject go = CreateItemBase(itemName, position, spr, 5, 1.5f);

            ItemPickup pickup = go.AddComponent<ItemPickup>();
            Weapon item = new Weapon(
                itemName.ToLower().Replace(' ', '_'),
                itemName, description,
                goldValue, rarity, baseDamage, weaponType);
            pickup.SetItemData(item, spr);

            DeckItemTracker tracker = go.AddComponent<DeckItemTracker>();
            tracker.trackedItemName = itemName;
        }

        private static void SpawnClothingItem(
            string itemName, Vector3 position,
            string description, int goldValue, Rarity rarity,
            ClothingSlot slot, int defenseBonus)
        {
            if (ShouldSkipItem(itemName)) return;

            Sprite spr = GetItemSprite(itemName);
            GameObject go = CreateItemBase(itemName, position, spr, 5, 1.5f);

            ItemPickup pickup = go.AddComponent<ItemPickup>();
            Clothing item = new Clothing(
                itemName.ToLower().Replace(' ', '_'),
                itemName, description,
                goldValue, rarity, slot, defenseBonus);
            pickup.SetItemData(item, spr);

            DeckItemTracker tracker = go.AddComponent<DeckItemTracker>();
            tracker.trackedItemName = itemName;
        }

        private static void SpawnGoldCoins(string itemName, Vector3 position, int amount)
        {
            if (ShouldSkipItem(itemName)) return;

            Sprite spr = GetItemSprite(itemName);
            GameObject go = CreateItemBase(itemName, position, spr, 5, 1.5f);

            ItemPickup pickup = go.AddComponent<ItemPickup>();
            TreasureItem item = new TreasureItem(
                "gold_coins", itemName, "A pile of golden coins.",
                amount, Rarity.Common, ItemType.Treasure);
            pickup.SetItemData(item, spr);

            DeckItemTracker tracker = go.AddComponent<DeckItemTracker>();
            tracker.trackedItemName = itemName;
        }

        private static GameObject CreateItemBase(
            string name, Vector3 position, Sprite spr, int sortOrder, float scale)
        {
            GameObject go = new GameObject(name);
            go.transform.position   = position;
            go.transform.localScale = new Vector3(scale, scale, 1f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = spr;
            sr.color        = Color.white;
            sr.sortingOrder = sortOrder;

            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size      = new Vector2(1.5f, 1.5f);

            return go;
        }
    }

    public class DeckItemTracker : MonoBehaviour
    {
        public string trackedItemName;
        private bool _tracked = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            MarkCollected();
        }

        private void MarkCollected()
        {
            if (_tracked) return;
            _tracked = true;
            BlackwaterState.collectedItems.Add(trackedItemName);
            BlackwaterState.heldItems.Add(trackedItemName);
            Debug.Log("[DeckItemTracker] Collected '" + trackedItemName +
                      "' | collectedItems=[" + string.Join(", ", BlackwaterState.collectedItems) + "]");
        }
    }
}
