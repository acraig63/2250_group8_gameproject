namespace GameControl;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{
    private List<Level> levels;
    private int currentIndex = 0;

    public void Initialize()
    {
        levels = new List<Level>();

        // Add levels (basic placeholders for now)
        levels.Add(new Level("Smuggler's Island"));
        levels.Add(new Level("Jungle Ruins"));

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

    public Level GetCurrentLevel()
    {
        return levels[currentIndex];
    }
}