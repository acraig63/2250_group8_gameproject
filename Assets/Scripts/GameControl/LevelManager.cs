namespace DefaultNamespace
{
                                                                                               
using System.Collections.Generic;                                                          
using UnityEngine;                                                                         
using Challenges;                                                                          
                                                                                           
// LevelManager handles creating, loading, and switching between levels                    
public class LevelManager                                                                  
{                                                                                          
    // List storing all levels in the game                                                 
    private List<Level> levels;                                                            
                                                                                           
    // Tracks which level the player is currently on                                       
    private int currentIndex = 0;                                                          
                                                                                           
    // Initializes all levels at the start of the game                                     
    public void Initialize()                                                               
    {                                                                                      
        // Create the list of levels                                                       
        levels = new List<Level>();                                                        
                                                                                           
        // Add all levels                                                                  
        // Level number is important for selecting correct questions                       
        levels.Add(new Level("Smuggler's Island", 1));                                     
        levels.Add(new Level("Jungle Ruins", 2));                                          
        levels.Add(new Level("Stormbreaker Island", 3));                                   
        levels.Add(new Level("Blackstone Fortress", 4));                                   
        levels.Add(new Level("Pirate Queen's Island", 5));                                 
                                                                                           
        // Load the first level when the game starts                                       
        LoadLevel(0);                                                                      
    }                                                                                      
                                                                                           
    // Loads a level based on its index in the list                                        
    public void LoadLevel(int index)                                                       
    {                                                                                      
        // Prevent invalid index access                                                    
        if (index < 0 || index >= levels.Count)                                            
        {                                                                                  
            Debug.Log("Invalid level index");                                              
            return;                                                                        
        }                                                                                  
                                                                                           
        // Update current level index                                                      
        currentIndex = index;                                                              
                                                                                           
        // Get the current level object                                                    
        Level currentLevel = levels[currentIndex];                                         
                                                                                           
        // Initialize the level                                                            
        currentLevel.Initialize();                                                         
                                                                                           
        // Print which level was loaded                                                    
        Debug.Log("Loaded Level: " + currentLevel.levelName);                              
                                                                                           
        // Create a challenge based on this level                                          
        Challenge challenge = currentLevel.CreateChallenge();                              
                                                                                           
        // Start the challenge                                                             
        challenge.StartChallenge();                                                        
    }                                                                                      
                                                                                           
    // Moves the player to the next level                                                  
    public void LoadNextLevel()                                                            
    {                                                                                      
        currentIndex++;                                                                    
                                                                                           
        // If there are no more levels, end the game                                       
        if (currentIndex >= levels.Count)                                                  
        {                                                                                  
            GameManager.Instance.EndGame(true);                                            
            return;                                                                        
        }                                                                                  
                                                                                           
        // Otherwise, load the next level                                                  
        LoadLevel(currentIndex);                                                           
    }                                                                                      
                                                                                           
    // Restarts the current level                                                          
    public void RestartLevel()                                                             
    {                                                                                      
        Debug.Log("Restarting Level: " + levels[currentIndex].levelName);                  
                                                                                           
        // Reload the same level                                                           
        LoadLevel(currentIndex);                                                           
    }                                                                                      
                                                                                           
    // Returns the current level object                                                    
    public Level GetCurrentLevel()                                                         
    {                                                                                      
        return levels[currentIndex];                                                       
    }                                                                                      
                                                                                           
    // Returns the index of the current level                                              
    public int GetCurrentLevelIndex()                                                      
    {                                                                                      
        return currentIndex;                                                               
    }                                                                                      
                                                                                           
    // Returns total number of levels in the game                                          
    public int GetLevelCount()                                                             
    {                                                                                      
        return levels.Count;                                                               
    }                                                                                      
}                                                                                          
}