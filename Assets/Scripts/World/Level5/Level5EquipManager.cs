using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    // ============================================================
    // Static stat lookup
    // ============================================================

    /// <summary>
    /// Auto-equip system for Level 5. Items in inventory automatically apply
    /// ATK/DEF bonuses — no equip UI. If the item is in the inventory, its stats
    /// are active.
    ///
    /// ATK APPROACH:
    /// BattleData.PlayerAttackPower is a public static field (confirmed from
    /// BattleData.cs). When the "Battle" scene loads, the persistent singleton
    /// Level5EquipManager adds GetTotalBonusATK() to this field. When any
    /// non-battle scene loads, the original value is restored. This means items
    /// in the inventory boost player damage dealt during boss battles.
    ///
    /// DEF APPROACH (overworld hazards):
    /// PlayerHazardShield.TakeDamage(int) calls GetTotalBonusDEF() and applies the
    /// defense floor (Mathf.Max(0, damage - DEF)) for all HazardZone hits. This
    /// covers all SendMessage("TakeDamage") calls in Blackwater scenes.
    ///
    /// DEF APPROACH (battle damage):
    /// The battle manager applies damage via BattleData fields, not SendMessage.
    /// PlayerController has no TakeDamage method (confirmed from PlayerController.cs),
    /// so battle damage bypasses PlayerHazardShield. A full interception would require
    /// modifying the battle manager (teammate file — off-limits). The DEF bonus
    /// therefore applies to overworld hazards only. Battle balance is achieved via
    /// the ATK bonus (player kills enemies faster) and question difficulty.
    /// </summary>
    public static class Level5EquipManager
    {
        // ── Hardcoded stat table ──────────────────────────────────────────────────
        private static readonly Dictionary<string, (int atk, int def)> _stats =
            new Dictionary<string, (int, int)>
            {
                { "Pirate Cutlass",    (5, 0) },
                { "Iron Shield Vest",  (0, 8) },
                { "Sea Boots",         (0, 2) },
                { "Coral Ring",        (0, 3) },
            };

        /// <summary>Sum of ATK bonuses for all stat items currently in inventory.</summary>
        public static int GetTotalBonusATK() => SumStat(isAtk: true);

        /// <summary>Sum of DEF bonuses for all stat items currently in inventory.</summary>
        public static int GetTotalBonusDEF() => SumStat(isAtk: false);

        private static int SumStat(bool isAtk)
        {
            InventoryUI ui = Object.FindObjectOfType<InventoryUI>();
            if (ui == null) return 0;

            // InventoryUI._inventory is private — access via reflection
            FieldInfo invField = typeof(InventoryUI).GetField(
                "_inventory", BindingFlags.NonPublic | BindingFlags.Instance);

            if (invField == null)
            {
                Debug.LogWarning("Level5EquipManager: InventoryUI._inventory field not found.");
                return 0;
            }

            Inventory inv = invField.GetValue(ui) as Inventory;
            if (inv == null) return 0;

            int total = 0;
            foreach (Item item in inv.Items)
            {
                if (_stats.TryGetValue(item.Name, out var bonus))
                    total += isAtk ? bonus.atk : bonus.def;
            }
            return total;
        }

        /// <summary>
        /// Ensures the persistent Level5EquipManager singleton exists so it can
        /// hook SceneManager.sceneLoaded and apply ATK bonuses during battles.
        /// Call this from BlackwaterSceneBuilder when setting up any Blackwater scene.
        /// </summary>
        public static void AttachToPlayer(GameObject player)
        {
            // The MonoBehaviour singleton is scene-independent and self-managing.
            // We just ensure it exists; it does not need to be on the player object.
            Level5EquipManagerMono.EnsureExists();
        }
    }

    // ============================================================
    // Persistent singleton MonoBehaviour
    // ============================================================

    /// <summary>
    /// Persistent singleton that modifies BattleData.PlayerAttackPower when the
    /// "Battle" scene loads and restores it when any non-battle scene loads.
    /// Uses DontDestroyOnLoad to survive scene transitions.
    /// </summary>
    public class Level5EquipManagerMono : MonoBehaviour
    {
        private static Level5EquipManagerMono _instance;
        private static int _originalPlayerAtk = -1;

        public static void EnsureExists()
        {
            if (_instance != null) return;
            GameObject go = new GameObject("Level5EquipManager");
            go.AddComponent<Level5EquipManagerMono>();
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

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_instance == this) _instance = null;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            bool isBattle = scene.name == "Battle" || scene.name == "pirateBattleScene";

            if (isBattle)
            {
                // Save original and add ATK bonus before battle manager reads it
                _originalPlayerAtk = BattleData.PlayerAttackPower;
                BattleData.PlayerAttackPower = _originalPlayerAtk + Level5EquipManager.GetTotalBonusATK();
                Debug.Log("[Level5EquipManager] ATK bonus applied: +" +
                          Level5EquipManager.GetTotalBonusATK() +
                          " → PlayerAttackPower=" + BattleData.PlayerAttackPower);
            }
            else if (_originalPlayerAtk >= 0)
            {
                // Restore when returning from battle
                BattleData.PlayerAttackPower = _originalPlayerAtk;
                _originalPlayerAtk = -1;
            }
        }
    }
}
