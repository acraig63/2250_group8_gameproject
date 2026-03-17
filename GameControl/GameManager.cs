namespace GameControl;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public LevelManager levelManager;
    public bool isRunning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if (!isRunning) return;

        if (levelManager != null && levelManager.GetCurrentLevel() != null)
        {
            levelManager.GetCurrentLevel().UpdateLevel();
        }
    }

    public void StartGame()
    {
        Debug.Log("Game Started");

        levelManager = new LevelManager();
        levelManager.Initialize();

        isRunning = true;
    }

    public void LoadNextLevel()
    {
        levelManager.LoadNextLevel();
    }

    public void EndGame(bool victory)
    {
        isRunning = false;

        if (victory)
            Debug.Log("YOU WIN!");
        else
            Debug.Log("GAME OVER!");
    }
}