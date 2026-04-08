using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string nextScene      = "BlackFortress";
    [SerializeField] private Color  lockedColor    = new Color(0.8f, 0.1f, 0.1f, 0.8f);
    [SerializeField] private Color  unlockedColor  = new Color(0.1f, 0.8f, 0.1f, 0.8f);

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool _isUnlocked = false;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateVisual();
    }

    void Update()
    {
        // Check every frame if all enemies are defeated
        bool allDefeated = AreAllEnemiesDefeated();

        if (allDefeated != _isUnlocked)
        {
            _isUnlocked = allDefeated;
            UpdateVisual();

            if (_isUnlocked)
                Debug.Log("All enemies defeated — exit tile unlocked!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (_isUnlocked)
        {
            InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
            if (inventoryUI != null)
                BattleData.PlayerGold = inventoryUI.GetInventory().Gold;
            Debug.Log($"Exiting Stormbreaker Island → {nextScene}");
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.Log("Exit locked — defeat all enemies first!");
            StartCoroutine(FlashLocked());
        }
    }

    private bool AreAllEnemiesDefeated()
    {
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();

        // If no spawners exist at all, treat as unlocked
        if (spawners.Length == 0) return true;

        // All spawners must be defeated
        foreach (EnemySpawner s in spawners)
            if (!s.IsDefeated()) return false;

        return true;
    }

    private void UpdateVisual()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = _isUnlocked ? unlockedColor : lockedColor;
    }

    /// <summary>Briefly flashes the tile brighter red to indicate it's locked.</summary>
    private System.Collections.IEnumerator FlashLocked()
    {
        if (spriteRenderer == null) yield break;

        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = lockedColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = lockedColor;
    }
}