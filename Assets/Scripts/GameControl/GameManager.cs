namespace DefaultNamespace
{
    using UnityEngine;

// This class controls the overall game flow from start to update to end
    public class GameManager : MonoBehaviour
    {
        // only one gameManager exists
        public static GameManager Instance;

        // reference to levelManager, handles the levels
        public LevelManager levelManager;
    
        // tracks if game is currently running
        public bool isRunning = false;

        // awake runs before start
        void Awake()
        {
            // If no instance exists, assign this as the global instance
            if (Instance == null)
            {
                Instance = this;

                // Prevent this object from being destroyed when changing scenes
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // If another GameManager exists, destroy this one
                Destroy(gameObject);
            }
        }
    
        // start runs once
        void Start()
        {
            StartGame();
        }

        // update every frame
        void Update()
        {
            // if not running do not do anything
            if (!isRunning) return;

            // if levelmanger and curent level exist, update the level
            if (levelManager != null)
            {
                var level = levelManager.GetCurrentLevel();

                if (level != null)
                {
                    level.Update(); 
                }
            }
        }

        // inititializes game and loads first level
        public void StartGame()
        {
            Debug.Log("Game Started");
            
            levelManager = LevelManager.Instance;

            if (levelManager == null)
            {
                Debug.LogError("LevelManager not found in scene!");
                return;
            }

            levelManager.Initialize();
            
            isRunning = true;
        }

        // loads next level
        public void LoadNextLevel()
        {
            levelManager.LoadNextLevel();
        }

        // ends game and prints result
        public void EndGame(bool victory)
        {
            isRunning = false;

            if (victory)
                Debug.Log("YOU WIN!");
            else
                Debug.Log("GAME OVER!");
        }
    }
}