using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// A 2D grid of Tile objects representing the layout of one island level.
    /// Owned by each Level instance. Does NOT handle rendering.
    /// </summary>
    public class TileMap
    {
        private int _width;
        private int _height;
        private Tile[,] _tiles;

        public TileMap(int width, int height)
        {
            _width  = width;
            _height = height;
            _tiles  = new Tile[width, height];
            InitializeWalkable();
        }

        public int Width  { get => _width; }
        public int Height { get => _height; }

        private void InitializeWalkable()
        {
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    _tiles[x, y] = new Tile(x, y, TileType.Walkable);
        }

        public Tile GetTile(int x, int y)
        {
            if (!IsInBounds(x, y))
            {
                // Fix: Console.WriteLine → Debug.Log
                Debug.Log($"TileMap.GetTile: ({x},{y}) is out of bounds.");
                return null;
            }
            return _tiles[x, y];
        }

        public void SetTile(int x, int y, TileType type)
        {
            if (!IsInBounds(x, y))
            {
                // Fix: Console.WriteLine → Debug.Log
                Debug.Log($"TileMap.SetTile: ({x},{y}) is out of bounds — skipped.");
                return;
            }
            _tiles[x, y] = new Tile(x, y, type);
        }

        public void SetTile(int x, int y, TileType type, string visualTag)
        {
            SetTile(x, y, type);
            if (IsInBounds(x, y))
                _tiles[x, y].VisualTag = visualTag;
        }

        public bool IsWalkable(int x, int y)
        {
            if (!IsInBounds(x, y)) return false;
            return _tiles[x, y].IsWalkable();
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }

        public List<Tile> GetTilesByType(TileType type)
        {
            List<Tile> result = new List<Tile>();
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    if (_tiles[x, y].TileType == type)
                        result.Add(_tiles[x, y]);
            return result;
        }

        public List<Tile> GetAdjacentWalkableTiles(int x, int y)
        {
            List<Tile> neighbours = new List<Tile>();
            int[,] directions = { { 0,1 },{ 0,-1 },{ 1,0 },{ -1,0 } };
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
