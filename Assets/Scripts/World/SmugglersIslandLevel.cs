using System.Collections.Generic;
using UnityEngine;
using NPCs;

namespace DefaultNamespace
{
    /// <summary>
    /// Level 1 — Smuggler's Island (Tutorial Level)
    /// Owner: Toby (world subsystem)
    ///
    /// Map: 80×60, three distinct zones stacked south-to-north:
    ///   Zone 1 — Beach  (y  1–19): entry docks, sandy shore, tide rocks, driftwood
    ///   Zone 2 — Camp   (y 20–42): outer wall, two sub-camps, boss arena, quicksand hazard
    ///   Zone 3 — Jungle (y 43–58): dense canopy, west clearing, east clearing, hidden exit
    ///
    /// NPC spawn positions are marked with TileType.NPCSpawn — NPC object
    /// instantiation is handled by the NPC subsystem team.
    ///
    /// Win condition: Defeat the Pirate Camp Leader (NPCSpawn at x=58,y=26)
    ///                → receive Key 1 + Map 1.
    /// </summary>
    public class SmugglersIslandLevel : Level
    {
        private const int MAP_WIDTH  = 80;
        private const int MAP_HEIGHT = 60;

        private bool _bossDefeatedHandled;

        public SmugglersIslandLevel()
            : base("level_1", "Smuggler's Island", MAP_WIDTH, MAP_HEIGHT)
        {
            _bossDefeatedHandled = false;
        }

        // ======
        // Initialization
        // ======

        public override void Initialize()
        {
            Debug.Log("Initializing Smuggler's Island (80×60)...");
            BuildMapLayout();
            SetupChallenge();
            SpawnNPCs();
            Debug.Log("Smuggler's Island ready.");
        }

        // ======
        // BuildMapLayout — three-zone island
        // ======

        private void BuildMapLayout()
        {
            // -----------------------------------------------------------------
            // PASS 1: OCEAN BORDER
            // All tiles start as Walkable (TileMap constructor default).
            // We overwrite the perimeter and add interior water depth.
            // -----------------------------------------------------------------
            SetRow(0,            0, MAP_WIDTH - 1, TileType.Water, "water");
            SetRow(MAP_HEIGHT-1, 0, MAP_WIDTH - 1, TileType.Water, "water");
            SetCol(0,            1, MAP_HEIGHT - 2, TileType.Water, "water");
            SetCol(MAP_WIDTH-1,  1, MAP_HEIGHT - 2, TileType.Water, "water");
            // Two-tile deep water edge on left and right
            SetCol(1, 1, MAP_HEIGHT - 2, TileType.Water, "water");
            SetCol(MAP_WIDTH-2, 1, MAP_HEIGHT - 2, TileType.Water, "water");

            // ==
            // ZONE 1 — BEACH  (y 1–19)
            // ==
            // y 1–3  : shallow water fringe
            // y 4–5  : wet sand strip
            // y 6–19 : open beach
            // x 36–43: main entry dock cutting through the water fringe
            // West and east rock clusters frame the beach
            // Driftwood scatter breaks up the open sand
            // Three hidden treasure chests
            // ==

            // Shallow water fringe
            for (int y = 1; y <= 3; y++)
                for (int x = 2; x <= MAP_WIDTH - 3; x++)
                    _mapLayout.SetTile(x, y, TileType.Water, "water_shallow");

            // Wet sand
            for (int y = 4; y <= 5; y++)
                for (int x = 2; x <= MAP_WIDTH - 3; x++)
                    _mapLayout.SetTile(x, y, TileType.Walkable, "sand_wet");

            // Open beach
            for (int y = 6; y <= 19; y++)
                for (int x = 2; x <= MAP_WIDTH - 3; x++)
                    _mapLayout.SetTile(x, y, TileType.Walkable, "sand");

            // Entry dock (punches through shallow water and wet sand)
            for (int y = 1; y <= 6; y++)
                for (int x = 36; x <= 43; x++)
                    _mapLayout.SetTile(x, y, TileType.Walkable, "dock_planks");

            // West tide-rock cluster
            int[,] westRocks = {
                {2,4},{3,4},{4,4},
                {2,5},{3,5},
                {2,7},{3,7},{4,7},{5,7},
                {2,8},{3,8},
                {2,10},{3,10},{4,10},
                {2,11},{3,11},{4,11},{5,11},
                {2,13},{3,13},
                {2,15},{3,15},{4,15}
            };
            PlaceCoords(westRocks, TileType.Obstacle, "rock_tide");

            // East tide-rock cluster
            int[,] eastRocks = {
                {75,4},{76,4},{77,4},
                {76,5},{77,5},
                {73,7},{74,7},{75,7},{76,7},
                {76,8},{77,8},
                {73,10},{74,10},{75,10},
                {73,11},{74,11},{75,11},{76,11},
                {76,13},{77,13},
                {73,15},{74,15},{75,15}
            };
            PlaceCoords(eastRocks, TileType.Obstacle, "rock_tide");

            // Driftwood scatter (mid-beach obstacles, break up open sand)
            int[,] driftwood = {
                {12, 8},{13, 8},{14, 8},
                {28,11},{29,11},
                {20,14},{21,14},
                {50, 9},{51, 9},
                {62,12},{63,12},
                {55,16},{56,16},{57,16},
                {33, 7},{34, 7},
                {68, 8},{69, 8},
                {44,17},{45,17},
                {18,18},{19,18}
            };
            PlaceCoords(driftwood, TileType.Obstacle, "driftwood");

            // Beach treasure chests
            _mapLayout.SetTile(15,  9, TileType.TreasureSpot, "treasure_chest");
            _mapLayout.SetTile(63, 13, TileType.TreasureSpot, "treasure_chest");
            _mapLayout.SetTile(39, 17, TileType.TreasureSpot, "treasure_chest");

            // Friendly NPC spot beside the dock
            _mapLayout.SetTile(44, 6, TileType.NPCSpawn, "sand");

            // ==
            // ZONE 2 — CAMP  (y 20–42)
            // ==
            // y 20–21 : sandy transition strip (worn path through middle)
            // y 22–42 : camp interior, sand_path ground
            // Outer wall ring with south entrance (y=22, x 35–44)
            //   and north exit (y=42, x 35–44)
            // Horizontal dividing wall at y=31 with two gates
            // West sub-camp  (x  9–34, y 23–30): 6 tents, 2 guard spawn spots
            // East sub-camp  (x 45–70, y 23–30): boss arena inner wall,
            //   command tent, boss spawn, gold treasure
            // South courtyard (y 32–41): crossing paths, merchant spawn,
            //   courtyard treasure, quicksand hazard pocket
            // ==

            // Transition strip
            for (int y = 20; y <= 21; y++)
                for (int x = 2; x <= MAP_WIDTH - 3; x++)
                    _mapLayout.SetTile(x, y, TileType.Walkable, "sand");
            for (int y = 20; y <= 21; y++)
                for (int x = 35; x <= 44; x++)
                    _mapLayout.SetTile(x, y, TileType.Walkable, "sand_path");

            // Camp interior ground
            for (int y = 22; y <= 42; y++)
                for (int x = 8; x <= 71; x++)
                    _mapLayout.SetTile(x, y, TileType.Walkable, "sand_path");

            // Outer camp wall ring
            SetBorderRing(8, 22, 71, 42, TileType.Obstacle, "camp_wall");

            // South entrance gap (x 35–44, y 22)
            for (int x = 35; x <= 44; x++)
                _mapLayout.SetTile(x, 22, TileType.Walkable, "sand_path");

            // North exit gap (x 35–44, y 42)
            for (int x = 35; x <= 44; x++)
                _mapLayout.SetTile(x, 42, TileType.Walkable, "sand_path");

            // Horizontal dividing wall at y=31
            for (int x = 8; x <= 71; x++)
                _mapLayout.SetTile(x, 31, TileType.Obstacle, "camp_wall");
            // West gate through dividing wall
            for (int x = 20; x <= 24; x++)
                _mapLayout.SetTile(x, 31, TileType.Walkable, "sand_path");
            // East gate through dividing wall
            for (int x = 55; x <= 59; x++)
                _mapLayout.SetTile(x, 31, TileType.Walkable, "sand_path");

            // --- West sub-camp tents (2×2 each) ---
            FillRect(11, 24, 12, 25, TileType.Obstacle, "tent_red");
            FillRect(15, 24, 16, 25, TileType.Obstacle, "tent_red");
            FillRect(20, 24, 21, 25, TileType.Obstacle, "tent_red");
            FillRect(25, 24, 26, 25, TileType.Obstacle, "tent_red");
            FillRect(11, 28, 12, 29, TileType.Obstacle, "tent_red");
            FillRect(25, 28, 26, 29, TileType.Obstacle, "tent_red");

            // West sub-camp guard spawn markers
            _mapLayout.SetTile(18, 27, TileType.NPCSpawn, "sand_path");
            _mapLayout.SetTile(23, 27, TileType.NPCSpawn, "sand_path");

            // --- East sub-camp: boss arena ---
            SetBorderRing(49, 23, 68, 30, TileType.Obstacle, "camp_wall_dark");
            // Arena south entrance
            for (int x = 57; x <= 60; x++)
                _mapLayout.SetTile(x, 23, TileType.Walkable, "sand_path");

            // Boss command tent (3 wide × 2 tall)
            FillRect(56, 27, 58, 29, TileType.Obstacle, "tent_large");

            // Boss NPC spawn (center of arena)
            _mapLayout.SetTile(58, 26, TileType.NPCSpawn, "sand_path");

            // Boss gold treasure chest
            _mapLayout.SetTile(65, 26, TileType.TreasureSpot, "treasure_chest_gold");

            // Side tents flanking the boss arena
            FillRect(46, 24, 47, 25, TileType.Obstacle, "tent_red");
            FillRect(46, 28, 47, 29, TileType.Obstacle, "tent_red");

            // --- South courtyard (y 32–41) ---
            // Horizontal path (y=36) and vertical path (x=39) cross the courtyard
            for (int x = 9; x <= 70; x++)
                _mapLayout.SetTile(x, 36, TileType.Walkable, "sand_path");
            for (int y = 32; y <= 41; y++)
                _mapLayout.SetTile(39, y, TileType.Walkable, "sand_path");
            for (int y = 32; y <= 41; y++)
                _mapLayout.SetTile(40, y, TileType.Walkable, "sand_path");

            // Courtyard barrel/crate obstacles (add visual texture to open space)
            int[,] courtyard_obstacles = {
                {14,33},{15,33},
                {60,33},{61,33},
                {14,39},{15,39},
                {60,39},{61,39},
                {27,34},{28,34},
                {50,38},{51,38}
            };
            PlaceCoords(courtyard_obstacles, TileType.Obstacle, "barrels");

            // Merchant NPC spawn (east side of courtyard)
            _mapLayout.SetTile(52, 36, TileType.NPCSpawn, "sand_path");

            // Courtyard treasure
            _mapLayout.SetTile(16, 34, TileType.TreasureSpot, "treasure_chest");

            // Quicksand hazard pocket (north-west of courtyard)
            FillRect(10, 33, 19, 40, TileType.Hazard, "quicksand");

            // ==
            // ZONE 3 — JUNGLE  (y 43–58)
            // ==
            // y 43–44: sandy transition strip into the jungle edge
            // y 45–58: dense jungle canopy (Obstacle, "jungle_tree")
            //   Central path  x 34–45: cuts straight through from camp to north exit
            //   West clearing x  5–22, y 48–56: jungle floor, secret treasure, NPC
            //   East clearing x 57–74, y 48–56: jungle floor, treasure
            //   Path stubs connecting clearings to the central corridor
            // ==

            // Transition strip
            for (int y = 43; y <= 44; y++)
                for (int x = 2; x <= MAP_WIDTH - 3; x++)
                    _mapLayout.SetTile(x, y, TileType.Walkable, "sand");

            // Dense jungle fill
            for (int y = 45; y <= 58; y++)
                for (int x = 2; x <= MAP_WIDTH - 3; x++)
                    _mapLayout.SetTile(x, y, TileType.Obstacle, "jungle_tree");

            // Central corridor (x 34–45, y 43–58)
            for (int y = 43; y <= 58; y++)
                for (int x = 34; x <= 45; x++)
                    _mapLayout.SetTile(x, y, TileType.Walkable, "jungle_floor");

            // West clearing (x 5–22, y 48–56)
            for (int y = 48; y <= 56; y++)
                for (int x = 5; x <= 22; x++)
                    _mapLayout.SetTile(x, y, TileType.Walkable, "jungle_floor");

            // Stub path from central corridor to west clearing (y 51–52, x 23–33)
            for (int y = 51; y <= 52; y++)
                for (int x = 23; x <= 33; x++)
                    _mapLayout.SetTile(x, y, TileType.Walkable, "jungle_floor");

            // West clearing — some interior trees to break up open space
            int[,] westClearingTrees = {
                {7,49},{8,49},{9,49},
                {18,49},{19,49},{20,49},
                {6,55},{7,55},{8,55},
                {19,55},{20,55},{21,55}
            };
            PlaceCoords(westClearingTrees, TileType.Obstacle, "jungle_tree");

            // West clearing treasure + NPC marker
            _mapLayout.SetTile(12, 53, TileType.TreasureSpot, "treasure_chest");
            _mapLayout.SetTile(17, 54, TileType.NPCSpawn, "jungle_floor");

            // East clearing (x 57–74, y 48–56)
            for (int y = 48; y <= 56; y++)
                for (int x = 57; x <= 74; x++)
                    _mapLayout.SetTile(x, y, TileType.Walkable, "jungle_floor");

            // Stub path from central corridor to east clearing (y 51–52, x 46–56)
            for (int y = 51; y <= 52; y++)
                for (int x = 46; x <= 56; x++)
                    _mapLayout.SetTile(x, y, TileType.Walkable, "jungle_floor");

            // East clearing — some interior trees
            int[,] eastClearingTrees = {
                {59,49},{60,49},{61,49},
                {71,49},{72,49},{73,49},
                {58,55},{59,55},{60,55},
                {71,55},{72,55},{73,55}
            };
            PlaceCoords(eastClearingTrees, TileType.Obstacle, "jungle_tree");

            // East clearing treasure
            _mapLayout.SetTile(68, 52, TileType.TreasureSpot, "treasure_chest");

            // Hidden north exit (end of central corridor)
            for (int x = 37; x <= 42; x++)
                _mapLayout.SetTile(x, 58, TileType.Exit, "dock_secret");

            // Main south exit (back on the dock)
            for (int x = 37; x <= 42; x++)
                _mapLayout.SetTile(x, 1, TileType.Exit, "dock_planks");

            Debug.Log("BuildMapLayout complete: 80×60, 3 zones.");
        }

        // ===========================
        // Layout helpers
        // =====================================================================

        private void FillRect(int x0, int y0, int x1, int y1, TileType type, string tag)
        {
            for (int x = x0; x <= x1; x++)
                for (int y = y0; y <= y1; y++)
                    _mapLayout.SetTile(x, y, type, tag);
        }

        private void SetBorderRing(int x0, int y0, int x1, int y1, TileType type, string tag)
        {
            for (int x = x0; x <= x1; x++)
            {
                _mapLayout.SetTile(x, y0, type, tag);
                _mapLayout.SetTile(x, y1, type, tag);
            }
            for (int y = y0 + 1; y < y1; y++)
            {
                _mapLayout.SetTile(x0, y, type, tag);
                _mapLayout.SetTile(x1, y, type, tag);
            }
        }

        private void SetRow(int y, int x0, int x1, TileType type, string tag)
        {
            for (int x = x0; x <= x1; x++)
                _mapLayout.SetTile(x, y, type, tag);
        }

        private void SetCol(int x, int y0, int y1, TileType type, string tag)
        {
            for (int y = y0; y <= y1; y++)
                _mapLayout.SetTile(x, y, type, tag);
        }

        private void PlaceCoords(int[,] coords, TileType type, string tag)
        {
            for (int i = 0; i < coords.GetLength(0); i++)
                _mapLayout.SetTile(coords[i, 0], coords[i, 1], type, tag);
        }

        // =====================================================================
        // Challenge setup
        // =====================================================================

        private void SetupChallenge()
        {
            _challenge = new Challenge("challenge_lv1");

            _challenge.AddQuestion(new MultipleChoiceQuestion(
                question:      "What is the correct way to declare an integer variable in C#?",
                correctAnswer: "A",
                difficulty:    1, xp: 25,
                options: new List<string> {
                    "A) int x = 5;", "B) integer x = 5;",
                    "C) var x = 5.0;", "D) Int x = 5;"
                },
                hint: "Letter rhymes with 'hay'!"));

            _challenge.AddQuestion(new MultipleChoiceQuestion(
                question:      "Which keyword is used to create a class in C#?",
                correctAnswer: "B",
                difficulty:    1, xp: 25,
                options: new List<string> {
                    "A) define", "B) class", "C) struct", "D) object"
                },
                hint: "Letter rhymes with 'sea'!"));

            _challenge.AddQuestion(new MultipleChoiceQuestion(
                question:      "What does the 'new' keyword do in C#?",
                correctAnswer: "C",
                difficulty:    1, xp: 25,
                options: new List<string> {
                    "A) Declares a method", "B) Imports a namespace",
                    "C) Creates a new instance of a class", "D) Defines an interface"
                },
                hint: "Letter rhymes with 'tree'!"));

            Debug.Log("Level 1 challenge: 3 questions configured.");
        }

        // =====================================================================
        // NPC spawning — tile markers placed in BuildMapLayout; NPC objects
        // are the NPC subsystem team's responsibility.
        // =====================================================================

        public override void SpawnNPCs()
        {
            Debug.Log("SmugglersIslandLevel: NPCSpawn tiles are placed. " +
                      "NPC instantiation is handled by the NPC subsystem.");
        }

        // =====================================================================
        // Game loop
        // =====================================================================

        public override void Update()
        {
            if (_isCompleted || _bossDefeatedHandled) return;

            foreach (NPC npc in _npcs)
            {
                if (npc is EnemyNPC enemy
                    && enemy.NpcName == "Pirate Camp Leader"
                    && enemy.IsDefeated())
                {
                    _bossDefeatedHandled = true;
                    Debug.Log("The Pirate Camp Leader has been defeated!");
                    Debug.Log("Key 1 and Map 1 recovered. Head to the docks.");
                    CompleteLevel();
                    break;
                }
            }
        }

        public override void OnLevelComplete(Player player)
        {
            player.Progression.AddXP(_challenge.xpReward);
            player.Progression.AddKey(_levelId);
            Debug.Log("Smuggler's Island cleared! The Jungle Ruins await...");
        }
    }
}
