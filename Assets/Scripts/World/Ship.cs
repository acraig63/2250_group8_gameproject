namespace DefaultNamespace
{
    /// <summary>
    /// Represents the player's ship — the central hub between islands.
    /// The player returns here after each island and uses the world map to sail to the next.
    ///
    /// Responsibilities:
    ///   - Owns a WorldMap instance and checks unlock conditions before sailing
    ///   - Delegates actual level loading to LevelManager
    ///   - Displays available islands to the player
    ///
    /// Does NOT handle island-specific game logic — that belongs to Level subclasses.
    /// </summary>
    public class Ship
    {
        private string _name;
        private string _currentLocation; // levelId of last visited island, or "open_sea"
        private WorldMap _worldMap;

        public Ship(string name)
    
    {
        _name = name;
        _currentLocation = "open_sea";
        _worldMap = new WorldMap();
    }

    public string Name            { get => _name; }
    public string CurrentLocation { get => _currentLocation; }

    /// <summary>
    /// Attempts to sail the ship to the given island.
    /// Checks unlock status via WorldMap before delegating to LevelManager.
    /// If the island is locked, prints a message and returns without sailing.
    /// </summary>
    public void SailTo(string levelId, Player player, LevelManager levelManager)
    {
        if (!_worldMap.IsIslandUnlocked(levelId, player.Progression))
        {
            Console.WriteLine(
                $"Cannot sail to {_worldMap.GetIslandName(levelId)} — island is still locked.\n"
            );
            return;
        }

        Console.WriteLine($"\n{_name} sets sail for {_worldMap.GetIslandName(levelId)}...");
        Console.WriteLine($"  \"{_worldMap.GetIslandDescription(levelId)}\"\n");

        _currentLocation = levelId;
        levelManager.LoadLevel(levelId, player);
    }

    /// <summary>Returns the WorldMap owned by this Ship.</summary>
    public WorldMap GetWorldMap()
    {
        return _worldMap;
    }

    /// <summary>
    /// Displays only the islands the player can currently sail to.
    /// Called when the player opens the navigation menu from the ship.
    /// </summary>
    public void DisplayUnlockedIslands(Player player)
    {
        List<string> unlocked = _worldMap.GetUnlockedIslands(player.Progression);

        Console.WriteLine($"\n=== {_name} — Available Destinations ===");
        if (unlocked.Count == 0)
        {
            Console.WriteLine("  No islands unlocked yet.");
        }
        else
        {
            foreach (string islandId in unlocked)
                Console.WriteLine($"  → {_worldMap.GetIslandName(islandId)}");
        }
        Console.WriteLine("=========================================\n");
    }

    /// <summary>
    /// Displays the full sea map including locked islands (shown as [LOCKED]).
    /// Gives the player a sense of the world even before they unlock everything.
    /// </summary>
    public void DisplayWorldMap(Player player)
    {
        _worldMap.DisplayMap(player.Progression);
    }
}

}