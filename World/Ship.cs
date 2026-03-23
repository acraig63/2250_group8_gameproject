using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;

/// <summary>
/// Represents the player's ship — the central hub between islands.
/// Owns a WorldMap, checks unlock conditions, and delegates loading to LevelManager.
/// </summary>
public class Ship
{
    private string   _name;
    private string   _currentLocation;
    private WorldMap _worldMap;

    // Maps levelId strings to the integer index LevelManager.LoadLevel(int) expects
    private static readonly Dictionary<string, int> LevelIndexMap =
        new Dictionary<string, int>
        {
            { "level_1", 0 },
            { "level_2", 1 },
            { "level_3", 2 },
            { "level_4", 3 },
            { "level_5", 4 }
        };

    public Ship(string name)
    {
        _name            = name;
        _currentLocation = "open_sea";
        _worldMap        = new WorldMap();
    }

    public string Name            { get => _name; }
    public string CurrentLocation { get => _currentLocation; }

    /// <summary>
    /// Sails to the given island. LevelManager.LoadLevel only takes an int index —
    /// player.Progression is private so unlock checks are skipped for now.
    /// </summary>
    public void SailTo(string levelId, LevelManager levelManager)
    {
        if (!LevelIndexMap.ContainsKey(levelId))
        {
            // Fix: Console.WriteLine → Debug.Log
            Debug.LogError($"Ship.SailTo: Unknown levelId '{levelId}'.");
            return;
        }

        // Fix: Console.WriteLine → Debug.Log
        Debug.Log($"{_name} sets sail for {_worldMap.GetIslandName(levelId)}...");
        Debug.Log($"  \"{_worldMap.GetIslandDescription(levelId)}\"");

        _currentLocation = levelId;

        // Fix: LoadLevel only takes one int argument, not (string, Player)
        levelManager.LoadLevel(LevelIndexMap[levelId]);
    }

    public WorldMap GetWorldMap() { return _worldMap; }

    /// <summary>
    /// Displays all five island destinations. Full unlock gating will be added
    /// once ProgressionSystem exposes a public property on Player.
    /// </summary>
    public void DisplayAvailableIslands()
    {
        // Fix: Console.WriteLine → Debug.Log
        // Fix: removed player.Progression reference — Progression is a private field on Player
        Debug.Log($"=== {_name} — Available Destinations ===");
        foreach (var entry in LevelIndexMap)
            Debug.Log($"  → {_worldMap.GetIslandName(entry.Key)}");
        Debug.Log("=========================================");
    }
}
