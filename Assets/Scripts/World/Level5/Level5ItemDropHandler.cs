using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    /// <summary>
    /// Persistent singleton that monitors the InventoryUI each frame and detects
    /// when a deck item has been explicitly dropped by the player.
    ///
    /// WHY THIS EXISTS:
    ///   PlayerController.Start() calls Initialize(new Inventory(5)) on every scene
    ///   load, wiping the visual inventory. We must distinguish between:
    ///     (a) Inventory wiped by PlayerController on scene load → NOT a drop
    ///     (b) Player explicitly dropped an item via ItemDetailPopup → IS a drop
    ///
    /// HOW IT WORKS:
    ///   A grace period of 3 frames is set on every scene load. During this period
    ///   no drop detection runs. After the grace period ends (PlayerController.Start()
    ///   has had time to reset the inventory), a baseline snapshot is taken.
    ///   Each subsequent frame, if a deck item disappears from the baseline snapshot,
    ///   it is treated as a drop: removed from BlackwaterState.heldItems and
    ///   collectedItems so the equip manager stops applying its bonus and the item
    ///   can respawn in BlackwaterFlagship.
    ///
    ///   The actual world pickup creation is handled by ItemDetailPopup.DropItem()
    ///   (teammate code), which now works correctly because Level5InventoryBridge
    ///   keeps the DroppedItemTemplate ACTIVE (Instantiate creates active clones).
    /// </summary>
    public class Level5ItemDropHandler : MonoBehaviour
    {
        private static Level5ItemDropHandler _instance;

        // Names of the deck items that grant stat bonuses.
        private static readonly HashSet<string> _deckItemNames = new HashSet<string>
        {
            "Pirate Cutlass",
            "Iron Shield Vest",
            "Sea Boots",
            "Coral Ring",
        };

        // Snapshot of deck-item names present in the visual inventory last frame.
        private HashSet<string> _prevSnapshot = new HashSet<string>();

        // Frames remaining in the post-scene-load grace period.
        // During this window we skip drop detection so PlayerController.Start()
        // can wipe the inventory without us misinterpreting it as a drop.
        private int _graceFrames = 0;

        // ── Public entry point ──────────────────────────────────────────────

        public static void EnsureExists()
        {
            if (_instance != null) return;
            var go = new GameObject("Level5ItemDropHandler");
            go.AddComponent<Level5ItemDropHandler>();
        }

        // ── Singleton lifecycle ─────────────────────────────────────────────

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_instance == this) _instance = null;
        }

        // ── Scene load handling ─────────────────────────────────────────────

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Start grace period: PlayerController.Start() will fire in the next
            // frame(s) and call Initialize(new Inventory(5)). We wait 3 frames
            // before taking a new baseline snapshot.
            _graceFrames = 3;
        }

        // ── Per-frame detection ─────────────────────────────────────────────

        private void Update()
        {
            if (_graceFrames > 0)
            {
                _graceFrames--;
                if (_graceFrames == 0)
                {
                    // Grace period over — take baseline AFTER PlayerController reset.
                    _prevSnapshot = ReadDeckItemsInInventory();
                    Debug.Log("[Level5ItemDropHandler] Baseline after scene load: [" +
                              string.Join(", ", _prevSnapshot) + "]");
                }
                return;
            }

            string scene = SceneManager.GetActiveScene().name;
            if (!scene.StartsWith("Blackwater")) return;

            HashSet<string> current = ReadDeckItemsInInventory();

            // Items in prev but not in current were dropped this frame.
            foreach (string name in _deckItemNames)
            {
                if (_prevSnapshot.Contains(name) && !current.Contains(name))
                {
                    Debug.Log("[Level5ItemDropHandler] Detected drop: " + name);
                    HandleDrop(name);
                }
            }

            _prevSnapshot = current;
        }

        // ── Drop handling ───────────────────────────────────────────────────

        private void HandleDrop(string itemName)
        {
            // Remove from heldItems → equip manager no longer applies the bonus.
            BlackwaterState.heldItems.Remove(itemName);

            // Remove from collectedItems → item can respawn in BlackwaterFlagship
            // if the player re-enters that scene.
            BlackwaterState.collectedItems.Remove(itemName);

            Debug.Log("[Level5ItemDropHandler] Synced drop of '" + itemName +
                      "'. heldItems=[" + string.Join(", ", BlackwaterState.heldItems) + "]");
            // The actual world pickup is already created by ItemDetailPopup.DropItem()
            // which Instantiates Level5InventoryBridge.DroppedItemTemplate (now ACTIVE).
        }

        // ── Inventory reading ───────────────────────────────────────────────

        /// <summary>
        /// Returns the set of deck-item names currently in the visual InventoryUI.
        /// Uses reflection to read InventoryUI._inventory (private field).
        /// Returns an empty set if the UI or inventory is not available.
        /// </summary>
        private static HashSet<string> ReadDeckItemsInInventory()
        {
            var result = new HashSet<string>();

            InventoryUI ui = Level5InventoryBridge.KnownInventoryUI;
            if (ui == null) return result;

            FieldInfo invField = typeof(InventoryUI).GetField(
                "_inventory", BindingFlags.NonPublic | BindingFlags.Instance);
            if (invField == null) return result;

            Inventory inv = invField.GetValue(ui) as Inventory;
            if (inv == null) return result;

            foreach (Item item in inv.Items)
            {
                if (_deckItemNames.Contains(item.Name))
                    result.Add(item.Name);
            }
            return result;
        }
    }
}
