using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    // Disables the pickup collider for 0.5s after spawn so dropped items aren't instantly re-picked up.
    public class DropPickupCooldown : MonoBehaviour
    {
        private void Start()
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) StartCoroutine(EnableAfterDelay(col));
        }

        private IEnumerator EnableAfterDelay(Collider2D col)
        {
            col.enabled = false;
            yield return new WaitForSeconds(0.5f);
            if (col != null) col.enabled = true;
        }
    }

    public class Level5ItemDropHandler : MonoBehaviour
    {
        private static Level5ItemDropHandler _instance;

        private static readonly HashSet<string> _deckItemNames = new HashSet<string>
        {
            "Pirate Cutlass",
            "Iron Shield Vest",
            "Sea Boots",
            "Coral Ring",
        };

        private HashSet<string> _prevSnapshot = new HashSet<string>();

        // grace period after scene load to skip false drop detections
        private int _graceFrames = 0;

        public static void EnsureExists()
        {
            if (_instance != null) return;
            var go = new GameObject("Level5ItemDropHandler");
            go.AddComponent<Level5ItemDropHandler>();
        }

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

        private void Start()
        {
            // Mirror OnSceneLoaded for the initial scene load (sceneLoaded doesn't fire on first load)
            _graceFrames = 3;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_instance == this) _instance = null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _graceFrames = 3;
        }

        private void Update()
        {
            if (_graceFrames > 0)
            {
                _graceFrames--;
                if (_graceFrames == 0)
                {
                    _prevSnapshot = ReadDeckItemsInInventory();
                    Debug.Log("[Level5ItemDropHandler] Baseline after scene load: [" +
                              string.Join(", ", _prevSnapshot) + "]");
                }
                return;
            }

            string scene = SceneManager.GetActiveScene().name;
            if (!scene.StartsWith("Blackwater")) return;

            HashSet<string> current = ReadDeckItemsInInventory();

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

        private void HandleDrop(string itemName)
        {
            BlackwaterState.heldItems.Remove(itemName);
            BlackwaterState.collectedItems.Remove(itemName);

            Debug.Log("[Level5ItemDropHandler] Synced drop of '" + itemName +
                      "'. heldItems=[" + string.Join(", ", BlackwaterState.heldItems) + "]");
        }

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
