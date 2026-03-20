namespace DefaultNamespace;

/// <summary>
/// Defines the type of each tile cell on a TileMap.
/// Used by TileMap and Level to describe terrain and block movement.
/// </summary>
public enum TileType
{
    Walkable,       // Standard passable ground
    Obstacle,       // Impassable (rocks, walls, crates)
    Water,          // Impassable — surrounds islands on the overworld
    Hazard,         // Passable but deals damage when stepped on (traps, storm zones)
    TreasureSpot,   // Walkable tile that triggers an item pickup interaction
    NPCSpawn,       // Walkable tile reserved as an NPC starting position
    Exit            // Tile that triggers the return-to-ship transition
}
