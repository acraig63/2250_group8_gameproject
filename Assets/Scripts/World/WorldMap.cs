using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;

/// <summary>
/// Represents the overworld sea map: a registry of all five islands,
/// their unlock requirements, and their display metadata.
/// </summary>
public class WorldMap
{
    private Dictionary<string, IslandInfo> _islands;

    public WorldMap()
    {
        _islands = new Dictionary<string, IslandInfo>();
        RegisterIslands();
    }

    private void RegisterIslands()
    {
        _islands["level_1"] = new IslandInfo("Smuggler's Island",    null,       "Small sandy cove with pirate camps — tutorial island.");
        _islands["level_2"] = new IslandInfo("Jungle Ruins Island",  "level_1",  "Dense jungle with ancient ruins and hidden traps.");
        _islands["level_3"] = new IslandInfo("Stormbreaker Island",  "level_2",  "Rocky storm-battered coastline — survive the weather.");
        _islands["level_4"] = new IslandInfo("Blackstone Fortress",  "level_3",  "Dark stone fortress with locked gates and alert systems.");
        _islands["level_5"] = new IslandInfo("Pirate King's Island", "all_keys", "Grand pirate palace and arena — final showdown.");
    }

    public List<string> GetUnlockedIslands(ProgressionSystem progression)
    {
        List<string> unlocked = new List<string>();
        foreach (string levelId in _islands.Keys)
            if (IsIslandUnlocked(levelId, progression))
                unlocked.Add(levelId);
        return unlocked;
    }

    public bool IsIslandUnlocked(string levelId, ProgressionSystem progression)
    {
        if (!_islands.ContainsKey(levelId))
        {
            // Fix: Console.WriteLine → Debug.Log
            Debug.Log($"WorldMap: Unknown island id '{levelId}'.");
            return false;
        }

        IslandInfo info = _islands[levelId];

        // Level 1 — always available
        if (info.UnlockRequirement == null)
            return true;

        // Level 5 — needs all keys
        if (info.UnlockRequirement == "all_keys")
            return progression.HasAllKeys();

        // Fix: progression.IsLevelUnlocked() does not exist on ProgressionSystem.
        // Levels 2-4 are open for now — lock these once ProgressionSystem is extended.
        return true;
    }

    public string GetIslandName(string levelId)
    {
        return _islands.ContainsKey(levelId) ? _islands[levelId].DisplayName : "Unknown Island";
    }

    public string GetIslandDescription(string levelId)
    {
        return _islands.ContainsKey(levelId) ? _islands[levelId].Description : "";
    }

    public void DisplayMap(ProgressionSystem progression)
    {
        // Fix: Console.WriteLine → Debug.Log
        Debug.Log("========== SEA MAP ==========");
        foreach (var entry in _islands)
        {
            bool unlocked = IsIslandUnlocked(entry.Key, progression);
            string status = unlocked ? "[ OPEN ]" : "[LOCKED]";
            Debug.Log($"  {status}  {entry.Value.DisplayName}");
            Debug.Log($"            {entry.Value.Description}");
        }
        Debug.Log("=============================");
    }
}

/// <summary>
/// Immutable data container holding an island's display metadata.
/// </summary>
public class IslandInfo
{
    public string DisplayName       { get; }
    public string UnlockRequirement { get; }
    public string Description       { get; }

    public IslandInfo(string displayName, string unlockRequirement, string description)
    {
        DisplayName       = displayName;
        UnlockRequirement = unlockRequirement;
        Description       = description;
    }
}
