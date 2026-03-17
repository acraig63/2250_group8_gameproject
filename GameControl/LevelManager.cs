namespace GameControl;
using System.Collections.Generic;
using UnityEngine;

// handles loading and switching between levels
public class LevelManager
{
    // list of all the levels in the game
    private List<Level> levels;
    private int currentIndex = 0;

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

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levels.Count)
        {
            Debug.Log("Invalid level index");
            return;
        }

        currentIndex = index;
        levels[currentIndex].Initialize();

        Debug.Log("Loaded Level: " + levels[currentIndex].levelName);
        
        Challenge challenge = currentLevel.CreateChallenge();
        challenge.StartChallenge();

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
        Debug.Log("Restarting Level: " + levels[currentIndex].levelName);
        LoadLevel(currentIndex);
    }

    public Level GetCurrentLevel()
    {
        return levels[currentIndex];
    }
    public int GetCurrentLevelIndex()
    {
        return currentIndex;
    }

    // Returns total number of levels
    public int GetLevelCount()
    {
        return levels.Count;
    }
}