namespace DefaultNamespace;

/// <summary>
/// Represents a single cell on a TileMap.
/// Stores position, terrain type, occupancy, and the sprite tag used for rendering.
/// Does NOT render itself — the visual layer reads VisualTag to pick the correct sprite.
/// </summary>
public class Tile
{
    private int _x;
    private int _y;
    private TileType _tileType;
    private bool _isOccupied;   // True when an NPC or player is standing on this tile
    private string _visualTag;  // Links to sprite asset in Unity (e.g. "sand", "rock", "water")

    public Tile(int x, int y, TileType tileType)
    {
        _x = x;
        _y = y;
        _tileType = tileType;
        _isOccupied = false;
        _visualTag = DeriveDefaultVisualTag(tileType);
    }

    public int X { get => _x; }
    public int Y { get => _y; }

    public TileType TileType
    {
        get => _tileType;
        set
        {
            _tileType = value;
            _visualTag = DeriveDefaultVisualTag(value); // Keep visual in sync with type
        }
    }

    public bool IsOccupied
    {
        get => _isOccupied;
        set => _isOccupied = value;
    }

    public string VisualTag
    {
        get => _visualTag;
        set => _visualTag = value; // Allows manual override for unique tiles
    }

    /// <summary>
    /// Returns true if a player or NPC can walk onto this tile.
    /// Occupied walkable tiles can still be entered (triggers interaction).
    /// </summary>
    public bool IsWalkable()
    {
        return _tileType == TileType.Walkable
            || _tileType == TileType.TreasureSpot
            || _tileType == TileType.NPCSpawn
            || _tileType == TileType.Exit
            || _tileType == TileType.Hazard; // Hazard is walkable but hurts the player
    }

    /// <summary>
    /// Auto-assigns a sprite tag based on tile type so every tile has a reasonable default.
    /// </summary>
    private string DeriveDefaultVisualTag(TileType type)
    {
        return type switch
        {
            TileType.Walkable     => "sand",
            TileType.Obstacle     => "rock",
            TileType.Water        => "water",
            TileType.Hazard       => "hazard_trap",
            TileType.TreasureSpot => "treasure_chest",
            TileType.NPCSpawn     => "sand",
            TileType.Exit         => "dock",
            _                     => "sand"
        };
    }
}
