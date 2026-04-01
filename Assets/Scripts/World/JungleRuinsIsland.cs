using UnityEngine;

namespace DefaultNamespace
{
    public class JungleRuinsIsland : Level
    {
        private const int MAP_WIDTH = 80;
        private const int MAP_HEIGHT = 60;

        public JungleRuinsIsland()
            : base("level_2", "Jungle Ruins Island", MAP_WIDTH, MAP_HEIGHT)
        {
        }

        public override void Initialize()
        {
            BuildMapLayout();
            SpawnNPCs();
        }

        private void BuildMapLayout()
        {
            for (int x = 0; x < MAP_WIDTH; x++)
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                _mapLayout.SetTile(x, y, TileType.Walkable, "jungle_floor");
            }
        }

        public override void Update() {}
        public override void OnLevelComplete(Player player) {}
        public override void SpawnNPCs() {}
    }

}