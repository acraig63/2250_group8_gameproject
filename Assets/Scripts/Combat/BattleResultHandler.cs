using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleResultHandler : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"BattleResultHandler Start — PlayerWon: {BattleData.PlayerWon}, CurrentHealth: {BattleData.PlayerCurrentHealth}, HasReturnPosition: {BattleData.HasReturnPosition}, DiedInBattle: {BattleData.DiedInBattle}");
        string previousScene = BattleData.ReturnScene;
        string currentScene  = SceneManager.GetActiveScene().name;

        // Only handle result if this is the scene we returned to
        if (currentScene != previousScene) return;

        if (BattleData.PlayerWon)
        {
            BattleData.PlayerWon = false;
            BattleData.ReturningFromBattle = true;
            BattleData.HasReturnPosition = true;
            BattleData.DefeatedEnemies.Add(BattleData.EnemyName);
            Debug.Log($"Added to defeated list: {BattleData.EnemyName}, list now: {string.Join(", ", BattleData.DefeatedEnemies)}");
            
            StartCoroutine(RestoreAfterDelay());
            
        }
        else if (BattleData.PlayerCurrentHealth <= 0)
        {
            Debug.Log("Player was defeated — restarting level.");
            BattleData.DiedInBattle            = true;
            BattleData.PlayerCurrentHealth     = BattleData.PlayerMaxHealth;
            BattleData.HasReturnPosition = false;
            BattleData.ReturningFromBattle     = false;
            BattleData.DefeatedEnemies.Clear();
            
            if (BattleData.PlayerCurrentHealth <= 0) BattleData.PlayerCurrentHealth = BattleData.PlayerMaxHealth;
            SceneManager.LoadScene(currentScene);
        }
    }

    private IEnumerator RestoreAfterDelay()
    {
        yield return null;
    
        Debug.Log($"Looking for enemy: {BattleData.EnemyName}");
    
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
        Debug.Log($"Found {spawners.Length} spawners in scene");
    
        foreach (EnemySpawner s in spawners)
        {
            Debug.Log($"Checking spawner: {s.gameObject.name}, enemyName: {s.GetEnemyName()}");
            if (s.gameObject.name.Contains(BattleData.EnemyName) ||
                s.GetEnemyName() == BattleData.EnemyName)
            {
                Debug.Log($"Found matching spawner, calling OnPlayerWon");
                s.OnPlayerWon();
                break;
            }
        }
    
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
            pc.SetHealth(BattleData.PlayerCurrentHealth);
    
        BattleData.ReturningFromBattle = false;
    }
    
    
}