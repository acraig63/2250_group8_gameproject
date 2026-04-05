using UnityEngine;

public class SpawnerLevel4 : MonoBehaviour
{
    [SerializeField] private string enemyName;
    [SerializeField] private GameObject enemyVisual; // optional (sprite/NPC)

    void Start()
    {
        if (BattleData.DefeatedEnemies.Contains(enemyName))
        {
            Debug.Log($"[EnemyPersistence] {enemyName} already defeated — removing");

            if (enemyVisual != null)
                enemyVisual.SetActive(false);

            Destroy(gameObject);
        }
    }
}