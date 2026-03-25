using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

        private List<Level> levels;
        private int currentIndex = 0;

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

        public void Initialize()
        {
            levels = new List<Level>();
            
            //levels.Add(new SmugglersIslandLevel());
            //levels.Add(new JungleRuinsLevel());
            //levels.Add(new StormbreakerIslandLevel());
            //levels.Add(new BlackstoneFortressLevel());
            //levels.Add(new PirateQueenIslandLevel());

            LoadLevel(0);
        }

        public void LoadLevel(int index)
        {
            if (levels == null || index < 0 || index >= levels.Count)
            {
                Debug.LogError("Invalid level index");
                return;
            }

            currentIndex = index;

            Level currentLevel = levels[currentIndex];
            currentLevel.Initialize();

            Debug.Log("Loaded Level: " + currentLevel.LevelName);

            Challenge challenge = currentLevel.GetChallenge();

            if (challenge != null)
            {
                challenge.StartChallenge();
            }
            else
            {
                Debug.LogWarning("No challenge found for this level");
            }
        }

        public void LoadNextLevel()
        {
            currentIndex++;

            if (currentIndex >= levels.Count)
            {
                GameManager.Instance.EndGame(true);
                return;
            }

            LoadLevel(currentIndex);
        }

        public void RestartLevel()
        {
            if (levels == null) return;

            Debug.Log("Restarting Level: " + levels[currentIndex].LevelName);
            LoadLevel(currentIndex);
        }

        public Level GetCurrentLevel()
        {
            if (levels == null || levels.Count == 0) return null;
            return levels[currentIndex];
        }

        public int GetCurrentLevelIndex()
        {
            return currentIndex;
        }

        public int GetLevelCount()
        {
            return levels?.Count ?? 0;
        }
    }
