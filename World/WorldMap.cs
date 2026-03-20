namespace DefaultNamespace;

/// <summary>
/// Represents the overworld sea map: a registry of all five islands,
/// their unlock requirements, and their display metadata.
///
/// WorldMap is owned by Ship and queried by LevelManager to verify
/// whether the player can sail to a given island.
/// Does NOT manage level loading — delegates that to LevelManager.
/// </summary>
public class WorldMap
{
    // All five islands registered at construction time
    private Dictionary<string, IslandInfo> _islands;

    public WorldMap()
    {
        _islands = new Dictionary<string, IslandInfo>();
        RegisterIslands();
    }

    /// <summary>
    /// Registers all five islands with their unlock chains and environment descriptions.
    /// A null UnlockRequirement means the island is always available.
    /// </summary>
    private void RegisterIslands()
    {
        _islands["level_1"] = new IslandInfo(
            displayName:        "Smuggler's Island",
            unlockRequirement:  null,
            description:        "Small sandy cove with pirate camps — tutorial island."
        );
        _islands["level_2"] = new IslandInfo(
            displayName:        "Jungle Ruins Island",
            unlockRequirement:  "level_1",
            description:        "Dense jungle with ancient ruins and hidden traps."
        );
        _islands["level_3"] = new IslandInfo(
            displayName:        "Stormbreaker Island",
            unlockRequirement:  "level_2",
            description:        "Rocky storm-battered coastline — survive the weather."
        );
        _islands["level_4"] = new IslandInfo(
            displayName:        "Blackstone Fortress",
            unlockRequirement:  "level_3",
            description:        "Dark stone fortress with locked gates and alert systems."
        );
        _islands["level_5"] = new IslandInfo(
            displayName:        "Pirate King's Island",
            unlockRequirement:  "all_keys", // Special case: requires all four keys + all four maps
            description:        "Grand pirate palace and arena — final showdown."
        );
    }

    /// <summary>
    /// Returns a list of levelIds the player has unlocked given their current ProgressionSystem state.
    /// </summary>
    public List<string> GetUnlockedIslands(ProgressionSystem progression)
    {
        List<string> unlocked = new List<string>();
        foreach (string levelId in _islands.Keys)
            if (IsIslandUnlocked(levelId, progression))
                unlocked.Add(levelId);
        return unlocked;
    }

    /// <summary>
    /// Returns true if the player meets the unlock requirement for the given island.
    /// Level 1 is always unlocked. Level 5 requires all four keys.
    /// All others require the previous level's key to have been collected.
    /// </summary>
    public bool IsIslandUnlocked(string levelId, ProgressionSystem progression)
    {
        if (!_islands.ContainsKey(levelId))
        {
            Console.WriteLine($"WorldMap: Unknown island id '{levelId}'.");
            return false;
        }

        IslandInfo info = _islands[levelId];

        // Level 1 — always available
        if (info.UnlockRequirement == null)
            return true;

        // Level 5 — needs all four keys collected
        if (info.UnlockRequirement == "all_keys")
            return progression.HasAllKeys();

        // Levels 2–4 — need the previous level's key
        return progression.IsLevelUnlocked(info.UnlockRequirement);
    }

    /// <summary>Returns the display name of an island by levelId.</summary>
    public string GetIslandName(string levelId)
    {
        return _islands.ContainsKey(levelId) ? _islands[levelId].DisplayName : "Unknown Island";
    }

    /// <summary>Returns the environment description of an island by levelId.</summary>
    public string GetIslandDescription(string levelId)
    {
        return _islands.ContainsKey(levelId) ? _islands[levelId].Description : "";
    }

    /// <summary>
    /// Prints the full sea map to the console showing which islands are locked/unlocked.
    /// Visual rendering of the actual map is handled by the Unity scene layer.
    /// </summary>
    public void DisplayMap(ProgressionSystem progression)
    {
        Console.WriteLine("\n========== SEA MAP ==========");
        foreach (var entry in _islands)
        {
            bool unlocked = IsIslandUnlocked(entry.Key, progression);
            string status = unlocked ? "[ OPEN ]" : "[LOCKED]";
            Console.WriteLine($"  {status}  {entry.Value.DisplayName}");
            Console.WriteLine($"            {entry.Value.Description}");
        }
        Console.WriteLine("=============================\n");
    }
}

/// <summary>
/// Immutable data container holding an island's display metadata.
/// Created once during WorldMap.RegisterIslands() and never modified.
/// </summary>
public class IslandInfo
{
    public string DisplayName       { get; }
    public string UnlockRequirement { get; } // levelId of required prior level, "all_keys", or null
    public string Description       { get; }

    public IslandInfo(string displayName, string unlockRequirement, string description)
    {
        DisplayName       = displayName;
        UnlockRequirement = unlockRequirement;
        Description       = description;
    }
}
