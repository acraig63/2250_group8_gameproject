using System.Collections.Generic;
namespace DefaultNamespace
{
    /// <summary>
/// Abstract base class for all five playable islands.
/// Defines the shared interface every Level must implement and owns
/// the level's NPC list, item spawns, challenge, and tile map.
///
/// Does NOT contain player progression logic — delegates to ProgressionSystem.
/// Concrete subclasses override Initialize(), Update(), OnLevelComplete(), SpawnNPCs().
/// </summary>
public abstract class Level
{
    // Protected so concrete level subclasses can access directly
    protected string _levelId;
    protected string _levelName;
    protected List<NPC> _npcs;
    protected List<Item> _itemSpawns;
    protected Challenge _challenge;
    protected bool _isCompleted;
    protected TileMap _mapLayout;

    protected Level(string levelId, string levelName, int mapWidth, int mapHeight)
    {
        _levelId = levelId;
        _levelName = levelName;
        _npcs = new List<NPC>();
        _itemSpawns = new List<Item>();
        _isCompleted = false;
        _mapLayout = new TileMap(mapWidth, mapHeight);
    }

    // --- Read-only public properties ---
    public string LevelId     { get => _levelId; }
    public string LevelName   { get => _levelName; }
    public bool IsCompleted   { get => _isCompleted; }
    public TileMap MapLayout  { get => _mapLayout; }

    // -------------------------------------------------------------------
    // Abstract methods — each concrete island MUST implement these
    // -------------------------------------------------------------------

    /// <summary>
    /// Called once when the level is first loaded.
    /// Should build the map layout, configure the challenge, and spawn NPCs/items.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Called every game tick. Concrete levels check win/loss conditions here.
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Called when the level's win condition is met (boss defeated).
    /// Should award XP, keys, and trigger the level-complete notification.
    /// </summary>
    public abstract void OnLevelComplete(Player player);

    /// <summary>
    /// Instantiates and positions all NPCs for this level.
    /// </summary>
    public abstract void SpawnNPCs();

    // -------------------------------------------------------------------
    // Concrete shared methods — usable by all subclasses
    // -------------------------------------------------------------------

    /// <summary>
    /// Called by LevelManager when the player enters this level.
    /// Triggers NPC spawns and prints the arrival message.
    /// </summary>
    public virtual void OnPlayerEnter(Player player)
    {
        Console.WriteLine($"\nYou have arrived at {_levelName}.");
        SpawnNPCs();
    }

    /// <summary>Returns the level's core Challenge object.</summary>
    public Challenge GetChallenge()
    {
        return _challenge;
    }

    /// <summary>Returns the full list of NPCs on this level.</summary>
    public List<NPC> GetNPCs()
    {
        return _npcs;
    }

    /// <summary>Returns all items available to be picked up on this level.</summary>
    public List<Item> GetItemSpawns()
    {
        return _itemSpawns;
    }

    // -------------------------------------------------------------------
    // Protected helpers — used by subclasses during Initialize()
    // -------------------------------------------------------------------

    /// <summary>Registers an NPC with this level.</summary>
    protected void AddNPC(NPC npc)
    {
        _npcs.Add(npc);
    }

    /// <summary>Adds an item to the level's spawn pool.</summary>
    protected void AddItemSpawn(Item item)
    {
        _itemSpawns.Add(item);
    }

    /// <summary>
    /// Marks this level as finished. Called internally once the boss is confirmed defeated.
    /// </summary>
    protected void CompleteLevel()
    {
        _isCompleted = true;
        Console.WriteLine($"{_levelName} cleared!");
    }
}

}