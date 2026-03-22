namespace DefaultNamespace;

/// <summary>
/// Simple 2D integer coordinate used to position entities on a TileMap.
/// Referenced by Player, NPC, and Level subsystems.
/// </summary>
public class Point
{
    private int _x;
    private int _y;

    public Point(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public int X
    {
        get => _x;
        set => _x = value;
    }

    public int Y
    {
        get => _y;
        set => _y = value;
    }

    /// <summary>
    /// Returns true if both coordinates match the given Point.
    /// </summary>
    public bool Equals(Point other)
    {
        return other != null && _x == other._x && _y == other._y;
    }

    public override string ToString()
    {
        return $"({_x}, {_y})";
    }
}
