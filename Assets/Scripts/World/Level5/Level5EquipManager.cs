using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public static class Level5EquipManager
    {
        private static readonly FieldInfo _inventoryField =
            typeof(InventoryUI).GetField("_inventory",
                BindingFlags.NonPublic | BindingFlags.Instance);

        public static int GetTotalBonusATK() => SumStat(isAtk: true);
        public static int GetTotalBonusDEF() => SumStat(isAtk: false);

        private static int SumStat(bool isAtk)
        {
            InventoryUI ui = Level5InventoryBridge.KnownInventoryUI;
            if (ui == null || _inventoryField == null) return 0;

            Inventory inv = _inventoryField.GetValue(ui) as Inventory;
            if (inv == null) return 0;

            int total = 0;
            foreach (Item item in inv.Items)
            {
                if (isAtk)
                {
                    if (item is Weapon weapon)
                        total += weapon.calculateDamage();
                }
                else
                {
                    if (item is Clothing clothing)
                        total += clothing.DefenseBonus;
                }
            }

            Debug.Log("[Level5EquipManager] Total " + (isAtk ? "ATK" : "DEF") + " = " + total);
            return total;
        }

        public static void AttachToPlayer(GameObject player)
        {
            Level5EquipManagerMono.EnsureExists();
        }
    }

    public class Level5EquipManagerMono : MonoBehaviour
    {
        private static Level5EquipManagerMono _instance;
        private static int _originalPlayerAtk = -1;
        private static int _originalEnemyAtk  = -1;

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
                // Apply ATK bonus
                _originalPlayerAtk = BattleData.PlayerAttackPower;
                BattleData.PlayerAttackPower = _originalPlayerAtk + Level5EquipManager.GetTotalBonusATK();
                Debug.Log("[Level5EquipManager] ATK bonus applied: +" +
                          Level5EquipManager.GetTotalBonusATK() +
                          " → PlayerAttackPower=" + BattleData.PlayerAttackPower);

                // Apply DEF: reduce enemy attack power
                _originalEnemyAtk = BattleData.EnemyAttackPower;
                int def = Level5EquipManager.GetTotalBonusDEF();
                BattleData.EnemyAttackPower = Mathf.Max(0, _originalEnemyAtk - def);
                Debug.Log("[Level5EquipManager] DEF applied: def=" + def +
                          " enemyAtk " + _originalEnemyAtk + "→" + BattleData.EnemyAttackPower);
            }
            else
            {
                if (_originalPlayerAtk >= 0)
                {
                    BattleData.PlayerAttackPower = _originalPlayerAtk;
                    _originalPlayerAtk = -1;
                }
                if (_originalEnemyAtk >= 0)
                {
                    BattleData.EnemyAttackPower = _originalEnemyAtk;
                    _originalEnemyAtk = -1;
                }
            }
        }
    }
}
