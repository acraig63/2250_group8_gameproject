using UnityEngine;
using UnityEngine.SceneManagement;


public class BattleResultHandler : MonoBehaviour
{
    private void Start()
    {
        // Only process if we just returned from a battle
        // We detect this by checking if PlayerCurrentHealth was written
        // (it defaults to 100 which is also the starting HP, so we use
        // a separate flag approach — PlayerWon defaults false but we
        // only care when the scene just loaded from Battle)
        string previousScene = BattleData.ReturnScene;
        string currentScene  = SceneManager.GetActiveScene().name;

        // Only handle result if this IS the return scene
        if (currentScene != previousScene) return;

        if (BattleData.PlayerWon)
        {
            Debug.Log($"Returned from battle — player won against {BattleData.EnemyName}.");

            // Find the defeated enemy spawner by name and destroy it
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

            // Restore player HP
            PlayerController pc = GetComponent<PlayerController>();
            if (pc != null)
                pc.SetHealth(BattleData.PlayerCurrentHealth);

            // Reset flag so it doesn't trigger again on next scene load
            BattleData.PlayerWon = false;
        }
        else if (BattleData.PlayerCurrentHealth <= 0)
        {
            // Player was defeated — reload the scene or go to game over
            Debug.Log("Player was defeated — restarting level.");
            BattleData.PlayerCurrentHealth = BattleData.PlayerMaxHealth; // reset before reload
            SceneManager.LoadScene(currentScene);
        }
    }
}