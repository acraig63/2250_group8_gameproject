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

    [Header("Battle Settings")]
    [SerializeField] private int    questionLevel    = 1;
    [SerializeField] private string returnScene      = "SmugglersIsland";

    private bool _defeated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_defeated) return;
        if (!other.CompareTag("Player")) return;

        // Write data for BattleManager to read
        BattleData.EnemyName        = enemyDisplayName;
        BattleData.EnemyMaxHealth   = enemyMaxHealth;
        BattleData.EnemyAttackPower = enemyAttackPower;
        BattleData.QuestionLevel    = questionLevel;
        BattleData.ReturnScene      = returnScene;
        BattleData.EnemySprite  = enemySprite;
        BattleData.PlayerSprite = other.GetComponent<SpriteRenderer>()?.sprite;
        BattleData.EnemyType = enemyType;
        BattleData.PlayerType = PlayerManager.playerType;

        // Carry player health into battle
        PlayerController pc = other.GetComponent<PlayerController>();
        PlayerManager pm = other.GetComponent<PlayerManager>();
        if (pc != null)
            BattleData.PlayerMaxHealth = pc.MaxHealth;
        if (pm != null && pm.player != null)
            BattleData.PlayerCurrentHealth = pm.player.GetCurrentHealth();

        SceneManager.LoadScene("Battle");
    }

    /// Called by BattleResultHandler when the player wins
    public void OnPlayerWon()
    {
        _defeated = true;
        Destroy(gameObject);
    }

    public string GetEnemyName() => enemyDisplayName;
}