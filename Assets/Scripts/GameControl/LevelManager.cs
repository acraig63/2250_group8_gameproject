using System.Collections.Generic;
using UnityEngine;
using Challenges;

namespace GameControl
{
    // LevelManager handles creating, loading, and switching between levels
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance;

        // List storing all levels in the game
        private List<Level> levels;

        // Tracks which level the player is currently on 
        private int currentIndex = 0;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Initializes all levels at the start of the game
        public void Initialize()
        {
            levels = new List<Level>();

            levels.Add(new Level("Smuggler's Island", 1));
            levels.Add(new Level("Jungle Ruins", 2));
            levels.Add(new Level("Stormbreaker Island", 3));
            levels.Add(new Level("Blackstone Fortress", 4));
            levels.Add(new Level("Pirate Queen's Island", 5));

            LoadLevel(0);
        }

        // Loads a level based on its index in the list
        public void LoadLevel(int index)
        {
            if (index < 0 || index >= levels.Count)
            {
                Debug.Log("Invalid level index");
                return;
            }

            currentIndex = index;

            Level currentLevel = levels[currentIndex];
            currentLevel.Initialize();

            Debug.Log("Loaded Level: " + currentLevel.levelName);

            Challenge challenge = currentLevel.CreateChallenge();
            challenge.StartChallenge();
        }

        // Moves the player to the next level
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

        // Restarts the current level 
        public void RestartLevel()
        {
            Debug.Log("Restarting Level: " + levels[currentIndex].levelName);
            LoadLevel(currentIndex);
        }

        // Returns the current level object
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
            return levels.Count;
        }
    }
}