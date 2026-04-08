using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;


public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Info")]
    [SerializeField] private string enemyDisplayName = "Pirate Guard";
    [SerializeField] private int    enemyMaxHealth   = 50;
    [SerializeField] private int    enemyAttackPower = 10;
    [SerializeField] private Sprite enemySprite;
    [SerializeField] private string enemyType = "Archer";
    [SerializeField] private GameObject enemyNpc;

    [Header("Battle Settings")]
    [SerializeField] private int    questionLevel    = 1;
    [SerializeField] private string returnScene      = "SmugglersIsland";

    private bool _defeated = false;
    public bool IsDefeated() => _defeated;

    void Start()
    {
        Debug.Log($"EnemySpawner Start() — name: {enemyDisplayName}, defeated list: {string.Join(", ", BattleData.DefeatedEnemies)}");
    
        if (BattleData.DefeatedEnemies.Contains(enemyDisplayName))
        {
            Debug.Log($"Found in defeated list, destroying {enemyDisplayName}");
            if (enemyNpc != null) enemyNpc.SetActive(false);
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_defeated) return;
        if (!other.CompareTag("Player")) return;
        if (BattleData.DefeatedEnemies.Contains(enemyDisplayName)) return;
        

        
        // Write data for BattleManager to read
        BattleData.EnemyName        = enemyDisplayName;
        BattleData.EnemyMaxHealth   = enemyMaxHealth;
        BattleData.EnemyAttackPower = enemyAttackPower;
        BattleData.QuestionLevel    = questionLevel;
        BattleData.ReturnScene = SceneManager.GetActiveScene().name;        
        BattleData.EnemySprite  = enemySprite;
        BattleData.PlayerSprite = other.GetComponent<SpriteRenderer>()?.sprite;
        BattleData.EnemyType = enemyType;
        BattleData.PlayerType = PlayerManager.playerType;
        BattleData.DefeatedEnemies.Remove(enemyDisplayName); // clean up just in case
        
        BattleData.ReturnPlayerPosition = other.transform.position;
        BattleData.HasReturnPosition = true;
        Debug.Log("Return Player Position: " +  BattleData.ReturnPlayerPosition);
        BattleData.DefeatedNpc = enemyNpc;

        // Carry player health into battle
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            BattleData.PlayerMaxHealth     = pc.MaxHealth;
            BattleData.PlayerCurrentHealth = pc.GetHealth();
        }
        
        InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI != null)
        {
            BattleData.PlayerGold = inventoryUI.GetInventory().Gold;
            inventoryUI.SaveItemsToData();
        }

        SceneManager.LoadScene("Battle");
    }

    /// Called by BattleResultHandler when the player wins
    public void OnPlayerWon()
    {
        
        _defeated = true;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        if (enemyNpc != null) enemyNpc.SetActive(false); // hide immediately
        Destroy(enemyNpc);
        Destroy(gameObject);
    }

    public string GetEnemyName() => enemyDisplayName;
}