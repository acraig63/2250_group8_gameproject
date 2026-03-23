using System.Collections.Generic;
namespace DefaultNamespace
{
    /// <summary>
/// A 2D grid of Tile objects representing the layout of one island level.
/// Owned by each Level instance. Does NOT handle rendering — visual layer reads tile data.
/// Responsibilities: grid management, walkability checks, tile queries.
/// </summary>
public class TileMap
{
    private int _width;
    private int _height;
    private Tile[,] _tiles;

    public TileMap(int width, int height)
    {
        _width = width;
        _height = height;
        _tiles = new Tile[width, height];
        InitializeWalkable();
    }

    public int Width  { get => _width; }
    public int Height { get => _height; }

    /// <summary>
    /// Fills the entire map with walkable sand tiles as a clean starting state.
    /// Levels then call SetTile() to carve obstacles, water, etc.
    /// </summary>
    private void InitializeWalkable()
    {
        for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
                _tiles[x, y] = new Tile(x, y, TileType.Walkable);
    }

    /// <summary>
    /// Returns the Tile at (x, y), or null if out of bounds.
    /// </summary>
    public Tile GetTile(int x, int y)
    {
        if (!IsInBounds(x, y))
        {
            Console.WriteLine($"TileMap.GetTile: ({x},{y}) is out of bounds.");
            return null;
        }
        return _tiles[x, y];
    }

    /// <summary>
    /// Replaces the tile type at a given position. Used by levels during initialization.
    /// </summary>
    public void SetTile(int x, int y, TileType type)
    {
        if (!IsInBounds(x, y))
        {
            Console.WriteLine($"TileMap.SetTile: ({x},{y}) is out of bounds — skipped.");
            return;
        }
        _tiles[x, y] = new Tile(x, y, type);
    }

    /// <summary>
    /// Overload: Set tile and override its visual tag (for unique sprite variants).
    /// </summary>
    public void SetTile(int x, int y, TileType type, string visualTag)
    {
        SetTile(x, y, type);
        if (IsInBounds(x, y))
            _tiles[x, y].VisualTag = visualTag;
    }

    /// <summary>
    /// Returns true if the tile at (x, y) can be moved onto by the player or an NPC.
    /// Checks both tile type and grid bounds. Does NOT check entity occupancy —
    /// movement code handles interaction when a tile is occupied.
    /// </summary>
    public bool IsWalkable(int x, int y)
    {
        if (!IsInBounds(x, y)) return false;
        return _tiles[x, y].IsWalkable();
    }

    /// <summary>
    /// Returns true when coordinates fall within the grid.
    /// </summary>
    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < _width && y >= 0 && y < _height;
    }

    /// <summary>
    /// Returns all tiles of a specific type — useful for finding spawn points.
    /// </summary>
    public List<Tile> GetTilesByType(TileType type)
    {
        List<Tile> result = new List<Tile>();
        for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
                if (_tiles[x, y].TileType == type)
                    result.Add(_tiles[x, y]);
        return result;
    }

    /// <summary>
    /// Returns the four cardinal neighbours of (x, y) that are in bounds.
    /// Used by movement and pathfinding logic.
    /// </summary>
    public List<Tile> GetAdjacentWalkableTiles(int x, int y)
    {
        List<Tile> neighbours = new List<Tile>();
        int[,] directions = { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 } };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + directions[i, 0];
            int ny = y + directions[i, 1];
            if (IsWalkable(nx, ny))
                neighbours.Add(_tiles[nx, ny]);
        }
        return neighbours;
    }
}

}