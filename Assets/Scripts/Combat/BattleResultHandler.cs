using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleResultHandler : MonoBehaviour
{
    private void Start()
    {
        string previousScene = BattleData.ReturnScene;
        string currentScene  = SceneManager.GetActiveScene().name;

        // Only handle result if this is the scene we returned to
        if (currentScene != previousScene) return;

        if (BattleData.PlayerWon)
        {
            Debug.Log($"Returned from battle — player won against {BattleData.EnemyName}.");
            BattleData.PlayerWon = false;

            // Use coroutine so PlayerManager.Start() has time to create Player first
            StartCoroutine(RestoreAfterDelay());
        }
        else if (BattleData.PlayerCurrentHealth <= 0)
        {
            Debug.Log("Player was defeated — restarting level.");
            BattleData.DiedInBattle            = true;
            BattleData.PlayerCurrentHealth     = BattleData.PlayerMaxHealth;
            BattleData.ReturningFromBattle     = false;
            SceneManager.LoadScene(currentScene);
        }
    }

    private IEnumerator RestoreAfterDelay()
    {
        // Wait one frame so PlayerManager.Start() finishes creating the Player object
        yield return null;

        // Destroy the defeated enemy spawner
        EnemySpawner[] spawners = FindObjectsOfType<EnemySpawner>();
        foreach (EnemySpawner s in spawners)
        {
            if (s.gameObject.name.Contains(BattleData.EnemyName) ||
                s.GetEnemyName() == BattleData.EnemyName)
            {
                s.OnPlayerWon();
                break;
            }
        }

        // Restore player HP — PlayerManager.player is now guaranteed to exist
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
            pc.SetHealth(BattleData.PlayerCurrentHealth);

        // Reset flag after everything is done
        BattleData.ReturningFromBattle = false;
    }
}