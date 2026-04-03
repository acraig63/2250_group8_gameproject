using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    /// <summary>
    /// Level 5 — Blackwater Flagship
    ///
    /// Handles all 7 Blackwater scenes:
    ///   BlackwaterFlagship         — 90×90 large ship deck on water
    ///   BlackwaterLowerDeck        — 80×80 below-deck hold (ship-shaped interior)
    ///   BlackwaterArmory           — 20×15
    ///   BlackwaterMessHall         — 20×15
    ///   BlackwaterBrig             — 15×15
    ///   BlackwaterNavigationRoom   — 15×15
    ///   BlackwaterCaptainsQuarters — 25×20
    ///
    /// All hatches are ON the deck surface (not side-doors). Portal triggers,
    /// wall collision, and the minimap are all added at runtime — no scene
    /// YAML edits needed beyond the two Tilemap references in the Inspector.
    /// </summary>
    public class BlackwaterSceneBuilder : MonoBehaviour
    {
        [Header("Tilemap References (drag from Hierarchy)")]
        public Tilemap groundTilemap;
        public Tilemap wallTilemap;

        private Dictionary<string, TileBase> _tileAssets;
        private Dictionary<string, Sprite>   _spriteCache;
        private const int TEX = 32;

        // -----------------------------------------------------------------------
        // Map size constants
        // -----------------------------------------------------------------------
        private const int FLAGSHIP_W  = 90;
        private const int FLAGSHIP_H  = 90;
        private const int LOWERDECK_W = 80;
        private const int LOWERDECK_H = 80;

        // -----------------------------------------------------------------------
        // Entry point
        // -----------------------------------------------------------------------

        void Start()
        {
            _tileAssets  = new Dictionary<string, TileBase>();
            _spriteCache = new Dictionary<string, Sprite>();

            if (groundTilemap == null || wallTilemap == null)
            {
                Debug.LogError("[BlackwaterSceneBuilder] groundTilemap or wallTilemap not assigned.");
                return;
            }

            BuildCurrentScene();

            // Add collision to wallTilemap after all tiles are final.
            SetupWallCollision();

            groundTilemap.RefreshAllTiles();
            wallTilemap.RefreshAllTiles();

            // Per-scene camera bounds and optional minimap.
            string scene = SceneManager.GetActiveScene().name;
            switch (scene)
            {
                case "BlackwaterFlagship":
                    SetupCameraBounds(FLAGSHIP_W, FLAGSHIP_H);
                    SetupMinimap(40f);
                    break;
                case "BlackwaterLowerDeck":
                    SetupCameraBounds(LOWERDECK_W, LOWERDECK_H);
                    break;
                case "BlackwaterArmory":
                case "BlackwaterMessHall":
                    SetupCameraBounds(20f, 15f);
                    break;
                case "BlackwaterBrig":
                case "BlackwaterNavigationRoom":
                    SetupCameraBounds(15f, 15f);
                    break;
                case "BlackwaterCaptainsQuarters":
                    SetupCameraBounds(25f, 20f);
                    break;
            }

            Debug.Log($"[BlackwaterSceneBuilder] Built {scene}");
        }

        // -----------------------------------------------------------------------
        // Runtime systems
        // -----------------------------------------------------------------------

        private void SetupWallCollision()
        {
            if (wallTilemap.gameObject.GetComponent<TilemapCollider2D>() != null) return;
            var rb = wallTilemap.gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            wallTilemap.gameObject.AddComponent<TilemapCollider2D>();
        }

        private void SetupMinimap(float orthoSize = 20f)
        {
            if (MinimapCamera.Instance == null)
            {
                var camGO = new GameObject("MinimapCamera");
                camGO.AddComponent<Camera>();
                camGO.AddComponent<MinimapCamera>();
                // Override the default ortho size set in Awake().
                var mc = camGO.GetComponent<MinimapCamera>();
                mc.orthographicSize = orthoSize;
                camGO.GetComponent<Camera>().orthographicSize = orthoSize;
            }

            if (MinimapUI.Instance == null)
            {
                var canvasGO = new GameObject("MinimapCanvas");
                var canvas   = canvasGO.AddComponent<Canvas>();
                canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 10;
                canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                canvasGO.AddComponent<MinimapUI>();
            }
        }

        private void SetupCameraBounds(float maxX, float maxY)
        {
            var cf = Object.FindObjectOfType<CameraFollow>();
            if (cf == null)
            {
                Debug.LogWarning("[BlackwaterSceneBuilder] No CameraFollow found in scene.");
                return;
            }
            cf.SetBounds(0f, maxX, 0f, maxY);
        }

        // -----------------------------------------------------------------------
        // Scene dispatcher
        // -----------------------------------------------------------------------

        private void BuildCurrentScene()
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "BlackwaterFlagship":         BuildFlagship();           break;
                case "BlackwaterLowerDeck":        BuildLowerDeck();          break;
                case "BlackwaterArmory":           BuildArmory();             break;
                case "BlackwaterMessHall":         BuildMessHall();           break;
                case "BlackwaterBrig":             BuildBrig();               break;
                case "BlackwaterNavigationRoom":   BuildNavigationRoom();     break;
                case "BlackwaterCaptainsQuarters": BuildCaptainsQuarters();   break;
                default:
                    Debug.LogWarning("[BlackwaterSceneBuilder] Unknown scene: " +
                                     SceneManager.GetActiveScene().name);
                    break;
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        // FLAGSHIP  —  90×90  —  y=7..82 ship hull, centered at x=44.5
        // ═══════════════════════════════════════════════════════════════════════

        private void BuildFlagship()
        {
            Portal.ApplyPendingSpawn();

            // ── Step 1: ocean background ──────────────────────────────────────
            FillRect(groundTilemap, 0, 0, FLAGSHIP_W-1, FLAGSHIP_H-1, "water");

            // ── Step 2: hull sections with distinct deck-zone tiles ───────────
            //
            // QUARTERDECK (stern, y=7–24) — warm amber-brown planks
            // 12 sections providing smooth width transitions every few rows.
            //   Section 1   y=7..12    x=33..56   24 wide
            PaintDeckRow(7,  12, 33, 56, "ship_deck_aft");
            //   Section 2   y=13..18   x=31..58   28 wide
            PaintDeckRow(13, 18, 31, 58, "ship_deck_aft");
            //   Section 3   y=19..24   x=29..60   32 wide
            PaintDeckRow(19, 24, 29, 60, "ship_deck_aft");

            // MAIN DECK (y=25–60) — standard warm-brown planks
            //   Section 4   y=25..30   x=27..62   36 wide
            PaintDeckRow(25, 30, 27, 62, "ship_deck");
            //   Section 5   y=31..55   x=25..64   40 wide  ← widest
            PaintDeckRow(31, 55, 25, 64, "ship_deck");
            //   Section 6   y=56..60   x=27..62   36 wide
            PaintDeckRow(56, 60, 27, 62, "ship_deck");

            // FORECASTLE (bow, y=61–82) — lighter tan planks
            //   Section 7   y=61..65   x=29..60   32 wide
            PaintDeckRow(61, 65, 29, 60, "ship_deck_fore");
            //   Section 8   y=66..70   x=31..58   28 wide
            PaintDeckRow(66, 70, 31, 58, "ship_deck_fore");
            //   Section 9   y=71..74   x=33..56   24 wide
            PaintDeckRow(71, 74, 33, 56, "ship_deck_fore");
            //   Section 10  y=75..78   x=36..53   18 wide
            PaintDeckRow(75, 78, 36, 53, "ship_deck_fore");
            //   Section 11  y=79..80   x=39..50   12 wide
            PaintDeckRow(79, 80, 39, 50, "ship_deck_fore");
            //   Section 12  y=81..82   x=42..47    6 wide  ← bow tip
            PaintDeckRow(81, 82, 42, 47, "ship_deck_fore");

            // ── Step 3: gapless hull perimeter (rail tiles on wallTilemap) ────
            PaintHullPerimeterAuto();

            // ── Step 4: deck zone transition strip ────────────────────────────
            // A row of dock_planks trim on groundTilemap marks where the
            // quarterdeck meets the main deck (y=24/25 boundary).
            for (int x = 27; x <= 62; x++)
                groundTilemap.SetTile(new Vector3Int(x, 25, 0), GetTileAsset("dock_planks"));
            // Trim at forecastle start (y=60/61 boundary).
            for (int x = 27; x <= 62; x++)
                groundTilemap.SetTile(new Vector3Int(x, 60, 0), GetTileAsset("dock_planks"));

            // ── Step 5: mast bases (3×3 obstacle tiles on wallTilemap) ────────
            PlaceRect(wallTilemap, 43, 26, 45, 28, "mast_base"); // Mizzenmast
            PlaceRect(wallTilemap, 43, 41, 45, 43, "mast_base"); // Mainmast
            PlaceRect(wallTilemap, 43, 52, 45, 54, "mast_base"); // Foremast

            // ── Step 6: barrel/crate clusters (2-tile wide obstacles) ─────────
            PlaceObstacles("barrels", new int[]
            {
                34, 22,  35, 22,          // quarterdeck left
                54, 22,  55, 22,          // quarterdeck right
                29, 35,  30, 35,          // main deck low-left
                58, 35,  59, 35,          // main deck low-right
                29, 47,  30, 47,          // main deck high-left
                58, 47,  59, 47,          // main deck high-right
                36, 63,  37, 63,          // forecastle left
                51, 63,  52, 63,          // forecastle right
            });

            // ── Step 7: captain's ornate floor accent (3×3 dock_planks) ───────
            // Painted BEFORE the portal so the hatch marker overwrites center.
            PlaceRect(groundTilemap, 43, 11, 45, 13, "dock_planks");

            // ── Step 8: hatch/door portals ────────────────────────────────────
            // All hatches are well inside the deck (≥4 tiles from any rail).
            // Return-spawn positions are 2 tiles away from the hatch tile.

            // Captain's Quarters hatch — quarterdeck center (y=12)
            // Return spawn 2 tiles north → y=14
            PlacePortal("Captains",   44, 12, "BlackwaterCaptainsQuarters",
                        new Vector2(12f, 10f), "door_captains");

            // Brig hatch — main deck, left, lower area (y=35)
            // Return spawn 2 tiles south → (32,33)
            PlacePortal("Brig",       32, 35, "BlackwaterBrig",
                        new Vector2(7f, 7f), "door_marker");

            // Lower Deck hatch — dead center of main deck (y=38)
            // Return spawn 2 tiles south → (44,36)
            PlacePortal("LowerDeck",  44, 38, "BlackwaterLowerDeck",
                        new Vector2(39f, 40f), "hatch_marker");

            // Armory hatch — main deck, left, upper area (y=46)
            // Return spawn 2 tiles south → (32,44)
            PlacePortal("Armory",     32, 46, "BlackwaterArmory",
                        new Vector2(10f, 7f), "door_marker");

            // Mess Hall hatch — main deck, right, upper area (y=46)
            // Return spawn 2 tiles south → (57,44)
            PlacePortal("MessHall",   57, 46, "BlackwaterMessHall",
                        new Vector2(10f, 7f), "door_marker");

            // Navigation Room hatch — forecastle center (y=65)
            // Return spawn 2 tiles south → (44,63)
            PlacePortal("Navigation", 44, 65, "BlackwaterNavigationRoom",
                        new Vector2(7f, 7f), "door_marker");
        }

        // -----------------------------------------------------------------------
        // Flagship hull helpers
        // -----------------------------------------------------------------------

        /// Paints a solid rectangle with the given deckTag on groundTilemap.
        private void PaintDeckRow(int yMin, int yMax, int xMin, int xMax, string deckTag)
        {
            TileBase t = GetTileAsset(deckTag);
            for (int y = yMin; y <= yMax; y++)
                for (int x = xMin; x <= xMax; x++)
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), t);
        }

        /// Returns true if (x,y) is inside the flagship hull shape.
        private bool IsShipDeck(int x, int y)
        {
            if (y >=  7 && y <= 12 && x >= 33 && x <= 56) return true;
            if (y >= 13 && y <= 18 && x >= 31 && x <= 58) return true;
            if (y >= 19 && y <= 24 && x >= 29 && x <= 60) return true;
            if (y >= 25 && y <= 30 && x >= 27 && x <= 62) return true;
            if (y >= 31 && y <= 55 && x >= 25 && x <= 64) return true;
            if (y >= 56 && y <= 60 && x >= 27 && x <= 62) return true;
            if (y >= 61 && y <= 65 && x >= 29 && x <= 60) return true;
            if (y >= 66 && y <= 70 && x >= 31 && x <= 58) return true;
            if (y >= 71 && y <= 74 && x >= 33 && x <= 56) return true;
            if (y >= 75 && y <= 78 && x >= 36 && x <= 53) return true;
            if (y >= 79 && y <= 80 && x >= 39 && x <= 50) return true;
            if (y >= 81 && y <= 82 && x >= 42 && x <= 47) return true;
            return false;
        }

        /// Paints ship_rail on wallTilemap at every hull-edge tile.
        /// Iterates the entire 90×90 grid so corner transitions are gapless.
        private void PaintHullPerimeterAuto()
        {
            TileBase rail = GetTileAsset("ship_rail");
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0,  0, 1, -1 };

            for (int y = 0; y < FLAGSHIP_H; y++)
            for (int x = 0; x < FLAGSHIP_W; x++)
            {
                if (!IsShipDeck(x, y)) continue;
                for (int d = 0; d < 4; d++)
                {
                    if (!IsShipDeck(x + dx[d], y + dy[d]))
                    {
                        wallTilemap.SetTile(new Vector3Int(x, y, 0), rail);
                        break;
                    }
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        // LOWER DECK  —  80×80  —  ship-shaped interior, proportional to flagship
        // ═══════════════════════════════════════════════════════════════════════

        private void BuildLowerDeck()
        {
            Portal.ApplyPendingSpawn();

            // Fill everything with solid void (blocks movement).
            FillRect(groundTilemap, 0, 0, LOWERDECK_W-1, LOWERDECK_H-1, "hold_void");
            FillRect(wallTilemap,   0, 0, LOWERDECK_W-1, LOWERDECK_H-1, "hold_void");

            // Carve out the ship-interior shape.
            BuildLowerDeckHull();

            // Support beam pillars flanking the central corridor.
            PlaceObstaclesIf("support_beam", new int[]
            {
                28, 37,  50, 37,
                28, 43,  50, 43,
                28, 22,  50, 22,
                28, 50,  50, 50,
            });

            // Barrel/crate clusters along the hold sides.
            PlaceObstaclesIf("barrels", new int[]
            {
                25, 33,  26, 33,  52, 33,  53, 33,
                25, 48,  26, 48,  52, 48,  53, 48,
                34, 63,  35, 63,  43, 63,  44, 63,
            });

            // Ladder back up — at (39,38), matching flagship hatch (44,38) offset -5 in x.
            // Flagship return-spawn is 2 tiles south of the flagship hatch → (44,36).
            PlacePortal("Flagship", 39, 38, "BlackwaterFlagship",
                        new Vector2(44f, 36f), "hatch_marker");
        }

        /// Returns true if (x,y) is inside the lower-deck interior.
        /// Same shape as the flagship hull minus 3 tiles/side, translated -5 in x.
        private bool IsLowerDeckInterior(int x, int y)
        {
            if (y >=  7 && y <= 12 && x >= 31 && x <= 48) return true;
            if (y >= 13 && y <= 18 && x >= 29 && x <= 50) return true;
            if (y >= 19 && y <= 24 && x >= 27 && x <= 52) return true;
            if (y >= 25 && y <= 30 && x >= 25 && x <= 54) return true;
            if (y >= 31 && y <= 55 && x >= 23 && x <= 56) return true;
            if (y >= 56 && y <= 60 && x >= 25 && x <= 54) return true;
            if (y >= 61 && y <= 65 && x >= 27 && x <= 52) return true;
            if (y >= 66 && y <= 70 && x >= 29 && x <= 50) return true;
            if (y >= 71 && y <= 74 && x >= 31 && x <= 48) return true;
            return false;
        }

        private void BuildLowerDeckHull()
        {
            // Paint floor and clear wall tiles for all interior sections.
            PaintLowerSection( 7, 12, 31, 48);
            PaintLowerSection(13, 18, 29, 50);
            PaintLowerSection(19, 24, 27, 52);
            PaintLowerSection(25, 30, 25, 54);
            PaintLowerSection(31, 55, 23, 56);
            PaintLowerSection(56, 60, 25, 54);
            PaintLowerSection(61, 65, 27, 52);
            PaintLowerSection(66, 70, 29, 50);
            PaintLowerSection(71, 74, 31, 48);

            // Re-paint hull wall tiles at the interior perimeter.
            PaintLowerDeckPerimeter();
        }

        private void PaintLowerSection(int yMin, int yMax, int xMin, int xMax)
        {
            TileBase floor = GetTileAsset("ship_deck_dark");
            for (int y = yMin; y <= yMax; y++)
            for (int x = xMin; x <= xMax; x++)
            {
                groundTilemap.SetTile(new Vector3Int(x, y, 0), floor);
                wallTilemap.SetTile(new Vector3Int(x, y, 0), null);  // clear void → walkable
            }
        }

        private void PaintLowerDeckPerimeter()
        {
            TileBase wall = GetTileAsset("hold_wall");
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0,  0, 1, -1 };

            for (int y = 0; y < LOWERDECK_H; y++)
            for (int x = 0; x < LOWERDECK_W; x++)
            {
                if (!IsLowerDeckInterior(x, y)) continue;
                for (int d = 0; d < 4; d++)
                {
                    if (!IsLowerDeckInterior(x + dx[d], y + dy[d]))
                    {
                        wallTilemap.SetTile(new Vector3Int(x, y, 0), wall);
                        break;
                    }
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════════════
        // ROOM BUILDERS
        // ═══════════════════════════════════════════════════════════════════════
        //
        // All rooms have exits as hatches near the TOP wall (y = H-2 after
        // the 2-tile border), representing going back UP to the ship deck.
        // Players spawn at the room centre when entering.

        // ── Armory  20×15 ────────────────────────────────────────────────────

        private void BuildArmory()
        {
            Portal.ApplyPendingSpawn();
            const int W = 20, H = 15;

            FillRect(groundTilemap, 0, 0, W-1, H-1, "room_floor");
            FillBorder(wallTilemap, 0, 0, W-1, H-1, 2, "room_wall");

            // Weapon rack barrels on both sides.
            PlaceObstacles("barrels", new int[] { 3,4, 4,4, 3,9, 4,9, 15,4, 15,9 });

            // Exit hatch on inner TOP wall (y=13). Return-spawn on flagship 2 south of (32,46).
            PlacePortal("FlagshipArmory", 10, 13, "BlackwaterFlagship",
                        new Vector2(32f, 44f), "hatch_marker");
        }

        // ── Mess Hall  20×15 ─────────────────────────────────────────────────

        private void BuildMessHall()
        {
            Portal.ApplyPendingSpawn();
            const int W = 20, H = 15;

            FillRect(groundTilemap, 0, 0, W-1, H-1, "sand_path");
            FillBorder(wallTilemap, 0, 0, W-1, H-1, 2, "room_wall");

            // Mess tables / crates.
            PlaceObstacles("barrels", new int[] { 4,4, 5,4, 14,4, 15,4, 4,9, 5,9 });

            // Exit hatch top-center. Return-spawn 2 south of flagship (57,46).
            PlacePortal("FlagshipMess", 10, 13, "BlackwaterFlagship",
                        new Vector2(57f, 44f), "hatch_marker");
        }

        // ── Brig  15×15 ──────────────────────────────────────────────────────

        private void BuildBrig()
        {
            Portal.ApplyPendingSpawn();
            const int W = 15, H = 15;

            FillRect(groundTilemap, 0, 0, W-1, H-1, "camp_wall_dark");
            FillBorder(wallTilemap, 0, 0, W-1, H-1, 2, "room_wall");

            // Prison bars / pillars.
            PlaceObstacles("room_wall", new int[] { 4,5, 9,5, 4,9, 9,9 });

            // Exit hatch top-center. Return-spawn 2 south of flagship (32,35).
            PlacePortal("FlagshipBrig", 7, 13, "BlackwaterFlagship",
                        new Vector2(32f, 33f), "hatch_marker");
        }

        // ── Navigation Room  15×15 ───────────────────────────────────────────

        private void BuildNavigationRoom()
        {
            Portal.ApplyPendingSpawn();
            const int W = 15, H = 15;

            FillRect(groundTilemap, 0, 0, W-1, H-1, "sand_wet");
            FillBorder(wallTilemap, 0, 0, W-1, H-1, 2, "room_wall");

            // Navigation chart tables.
            PlaceObstacles("dock_planks", new int[] { 4,5, 5,5, 9,5, 10,5, 4,9, 9,9 });

            // Exit hatch top-center. Return-spawn 2 south of flagship (44,65).
            PlacePortal("FlagshipNav", 7, 13, "BlackwaterFlagship",
                        new Vector2(44f, 63f), "hatch_marker");
        }

        // ── Captain's Quarters  25×20 ────────────────────────────────────────

        private void BuildCaptainsQuarters()
        {
            Portal.ApplyPendingSpawn();
            const int W = 25, H = 20;

            FillRect(groundTilemap, 0, 0, W-1, H-1, "dock_planks");
            FillBorder(wallTilemap, 0, 0, W-1, H-1, 2, "camp_wall");

            // Throne / desk accent.
            PlaceRect(groundTilemap, 11, 14, 13, 16, "treasure_chest_gold");

            // Decorative barrels / crates.
            PlaceObstacles("barrels", new int[] { 3,4, 4,4, 20,4, 21,4, 3,13, 20,13 });

            // Exit hatch inner TOP wall (y=18). Return-spawn 2 north of flagship (44,12) → y=14.
            PlacePortal("FlagshipCaptains", 12, 18, "BlackwaterFlagship",
                        new Vector2(44f, 14f), "hatch_marker");
        }

        // ═══════════════════════════════════════════════════════════════════════
        // MAP HELPERS
        // ═══════════════════════════════════════════════════════════════════════

        private void FillRect(Tilemap tm, int x0, int y0, int x1, int y1, string tag)
        {
            TileBase t = GetTileAsset(tag);
            for (int x = x0; x <= x1; x++)
                for (int y = y0; y <= y1; y++)
                    tm.SetTile(new Vector3Int(x, y, 0), t);
        }

        private void FillBorder(Tilemap tm, int x0, int y0, int x1, int y1,
                                 int thickness, string tag)
        {
            TileBase t = GetTileAsset(tag);
            for (int d = 0; d < thickness; d++)
            {
                int left = x0+d, right = x1-d, bot = y0+d, top = y1-d;
                if (left > right || bot > top) break;
                for (int x = left; x <= right; x++)
                {
                    tm.SetTile(new Vector3Int(x, bot, 0), t);
                    tm.SetTile(new Vector3Int(x, top, 0), t);
                }
                for (int y = bot+1; y < top; y++)
                {
                    tm.SetTile(new Vector3Int(left,  y, 0), t);
                    tm.SetTile(new Vector3Int(right, y, 0), t);
                }
            }
        }

        /// Fills a rectangle on a tilemap (helper used for mast bases, etc.)
        private void PlaceRect(Tilemap tm, int x0, int y0, int x1, int y1, string tag)
        {
            TileBase t = GetTileAsset(tag);
            for (int x = x0; x <= x1; x++)
                for (int y = y0; y <= y1; y++)
                    tm.SetTile(new Vector3Int(x, y, 0), t);
        }

        /// Places obstacle tiles on wallTilemap at paired (x,y) coordinates.
        private void PlaceObstacles(string tag, int[] coords)
        {
            TileBase t = GetTileAsset(tag);
            for (int i = 0; i < coords.Length - 1; i += 2)
                wallTilemap.SetTile(new Vector3Int(coords[i], coords[i+1], 0), t);
        }

        /// Like PlaceObstacles but skips any position not inside the lower-deck interior.
        private void PlaceObstaclesIf(string tag, int[] coords)
        {
            TileBase t = GetTileAsset(tag);
            for (int i = 0; i < coords.Length - 1; i += 2)
                if (IsLowerDeckInterior(coords[i], coords[i+1]))
                    wallTilemap.SetTile(new Vector3Int(coords[i], coords[i+1], 0), t);
        }

        // ═══════════════════════════════════════════════════════════════════════
        // PORTAL PLACEMENT
        // ═══════════════════════════════════════════════════════════════════════

        /// Places a portal at tile (tx,ty).
        /// Clears any wallTilemap tile at that position so the marker is visible
        /// and the TilemapCollider2D doesn't block the player from reaching it.
        private void PlacePortal(string id, int tx, int ty,
                                  string targetScene, Vector2 spawnPos,
                                  string markerTag)
        {
            var pos = new Vector3Int(tx, ty, 0);
            wallTilemap.SetTile(pos, null);
            groundTilemap.SetTile(pos, GetTileAsset(markerTag));

            var go = new GameObject("Portal_" + id);
            go.transform.position = new Vector3(tx + 0.5f, ty + 0.5f, 0f);

            var col       = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size      = new Vector2(1f, 1f);

            var portal           = go.AddComponent<Portal>();
            portal.targetScene   = targetScene;
            portal.spawnPosition = spawnPos;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // TILE ASSET RESOLUTION
        // ═══════════════════════════════════════════════════════════════════════

        private TileBase GetTileAsset(string tag)
        {
            if (_tileAssets.TryGetValue(tag, out TileBase existing)) return existing;
            var tile    = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
            tile.sprite = GetSprite(tag);
            tile.flags  = TileFlags.LockColor;
            _tileAssets[tag] = tile;
            return tile;
        }

        private Sprite GetSprite(string tag)
        {
            if (_spriteCache.TryGetValue(tag, out Sprite sp)) return sp;
            sp = SpriteFrom(GenerateTexture(tag, 0, 0));
            _spriteCache[tag] = sp;
            return sp;
        }

        private Sprite SpriteFrom(Texture2D tex)
            => Sprite.Create(tex, new Rect(0, 0, TEX, TEX), new Vector2(0.5f, 0.5f), TEX);

        // ═══════════════════════════════════════════════════════════════════════
        // VORONOI PROCEDURAL TILE GENERATOR
        // ═══════════════════════════════════════════════════════════════════════

        private Color[] VoronoiTile(int tx, int ty,
            Color cellBase, Color cellLight, Color mortar,
            int numCells = 10, float borderWidth = 1.8f, float noiseStrength = 0.10f)
        {
            var rng   = new System.Random(tx * 73856093 ^ ty * 19349663);
            int total = numCells * 9;
            var cx    = new float[total];
            var cy    = new float[total];
            int idx   = 0;

            for (int oy = -1; oy <= 1; oy++)
            for (int ox = -1; ox <= 1; ox++)
            for (int i  = 0;  i < numCells; i++)
            {
                cx[idx] = (float)rng.NextDouble() * TEX + ox * TEX;
                cy[idx] = (float)rng.NextDouble() * TEX + oy * TEX;
                idx++;
            }

            var pixels = new Color[TEX * TEX];
            for (int py = 0; py < TEX; py++)
            for (int px = 0; px < TEX; px++)
            {
                float d1 = float.MaxValue, d2 = float.MaxValue;
                for (int i = 0; i < total; i++)
                {
                    float ddx = px - cx[i], ddy = py - cy[i];
                    float d   = ddx*ddx + ddy*ddy;
                    if      (d < d1) { d2 = d1; d1 = d; }
                    else if (d < d2) { d2 = d; }
                }
                d1 = Mathf.Sqrt(d1); d2 = Mathf.Sqrt(d2);
                float edge = d2 - d1;

                Color c;
                if (edge < borderWidth)
                {
                    float t = Mathf.Clamp01(edge / borderWidth);
                    c = Color.Lerp(mortar, cellBase, t * t);
                }
                else
                {
                    float radius = TEX / Mathf.Sqrt(numCells) * 0.55f;
                    float t      = Mathf.Clamp01(d1 / radius);
                    float noise  = noiseStrength > 0f
                                 ? Mathf.PerlinNoise(px*0.18f+tx*0.3f, py*0.18f+ty*0.3f) - 0.5f
                                 : 0f;
                    c = Color.Lerp(cellLight, cellBase, t);
                    c = new Color(Mathf.Clamp01(c.r + noise*noiseStrength),
                                  Mathf.Clamp01(c.g + noise*noiseStrength),
                                  Mathf.Clamp01(c.b + noise*noiseStrength), 1f);
                }
                int rim = Mathf.Min(px, py, TEX-1-px, TEX-1-py);
                if (rim == 0) c = Color.Lerp(c, mortar, 0.35f);
                pixels[py * TEX + px] = c;
            }
            return pixels;
        }

        // ═══════════════════════════════════════════════════════════════════════
        // TEXTURE DEFINITIONS
        // ═══════════════════════════════════════════════════════════════════════

        private Texture2D GenerateTexture(string tag, int tx, int ty)
        {
            Color[] px;
            switch (tag)
            {
                // ── Ocean ──────────────────────────────────────────────────────
                case "water": case "water_shallow":
                    px = VoronoiTile(tx, ty,
                        new Color(0.18f,0.46f,0.70f), new Color(0.32f,0.64f,0.88f),
                        new Color(0.06f,0.22f,0.44f), 10, 1.8f, 0.10f); break;

                // ── Main deck — warm brown planks ──────────────────────────────
                case "ship_deck":
                    px = VoronoiTile(tx, ty,
                        new Color(0.55f,0.35f,0.15f), new Color(0.72f,0.52f,0.28f),
                        new Color(0.30f,0.18f,0.06f), 8, 2.5f, 0.09f); break;

                // ── Quarterdeck — richer amber-brown ───────────────────────────
                case "ship_deck_aft":
                    px = VoronoiTile(tx, ty,
                        new Color(0.62f,0.38f,0.12f), new Color(0.80f,0.58f,0.28f),
                        new Color(0.34f,0.18f,0.04f), 8, 2.6f, 0.09f); break;

                // ── Forecastle — lighter tan planks ───────────────────────────
                case "ship_deck_fore":
                    px = VoronoiTile(tx, ty,
                        new Color(0.68f,0.52f,0.28f), new Color(0.84f,0.70f,0.46f),
                        new Color(0.38f,0.26f,0.10f), 8, 2.4f, 0.09f); break;

                // ── Lower deck — dark planks ───────────────────────────────────
                case "ship_deck_dark":
                    px = VoronoiTile(tx, ty,
                        new Color(0.38f,0.22f,0.08f), new Color(0.52f,0.34f,0.14f),
                        new Color(0.18f,0.10f,0.02f), 8, 2.5f, 0.09f); break;

                // ── Ship rail / hull perimeter — dark navy ─────────────────────
                case "ship_rail":
                    px = VoronoiTile(tx, ty,
                        new Color(0.12f,0.10f,0.22f), new Color(0.22f,0.20f,0.35f),
                        new Color(0.04f,0.04f,0.08f), 8, 2.0f, 0.06f); break;

                // ── Dock planks / trim ─────────────────────────────────────────
                case "dock_planks": case "dock_secret":
                    px = VoronoiTile(tx, ty,
                        new Color(0.62f,0.44f,0.24f), new Color(0.78f,0.60f,0.38f),
                        new Color(0.30f,0.18f,0.08f), 6, 3.0f, 0.08f); break;

                // ── Mast base — very dark charred wood ────────────────────────
                case "mast_base":
                    px = VoronoiTile(tx, ty,
                        new Color(0.18f,0.10f,0.04f), new Color(0.28f,0.18f,0.08f),
                        new Color(0.06f,0.03f,0.01f), 6, 2.8f, 0.05f); break;

                // ── Room floor — stone/wood ────────────────────────────────────
                case "room_floor":
                    px = VoronoiTile(tx, ty,
                        new Color(0.58f,0.52f,0.44f), new Color(0.74f,0.68f,0.58f),
                        new Color(0.28f,0.24f,0.18f), 9, 2.2f, 0.08f); break;

                // ── Room wall — grey stone ─────────────────────────────────────
                case "room_wall":
                    px = VoronoiTile(tx, ty,
                        new Color(0.44f,0.44f,0.44f), new Color(0.62f,0.62f,0.62f),
                        new Color(0.18f,0.18f,0.18f), 8, 2.4f, 0.07f); break;

                // ── Sand tiles (mess hall floor) ───────────────────────────────
                case "sand":
                    px = VoronoiTile(tx, ty,
                        new Color(0.88f,0.76f,0.50f), new Color(0.98f,0.90f,0.70f),
                        new Color(0.54f,0.44f,0.26f), 11, 1.6f, 0.08f); break;
                case "sand_wet":
                    px = VoronoiTile(tx, ty,
                        new Color(0.70f,0.60f,0.40f), new Color(0.82f,0.72f,0.52f),
                        new Color(0.38f,0.30f,0.18f), 11, 1.8f, 0.07f); break;
                case "sand_path":
                    px = VoronoiTile(tx, ty,
                        new Color(0.80f,0.68f,0.36f), new Color(0.94f,0.84f,0.56f),
                        new Color(0.46f,0.36f,0.14f), 10, 2.0f, 0.09f); break;

                // ── Camp wall variants (brig / captain's quarters) ────────────
                case "camp_wall":
                    px = VoronoiTile(tx, ty,
                        new Color(0.58f,0.54f,0.48f), new Color(0.74f,0.70f,0.64f),
                        new Color(0.22f,0.20f,0.18f), 8, 2.4f, 0.08f); break;
                case "camp_wall_dark":
                    px = VoronoiTile(tx, ty,
                        new Color(0.36f,0.33f,0.29f), new Color(0.50f,0.47f,0.42f),
                        new Color(0.12f,0.10f,0.08f), 8, 2.6f, 0.07f); break;

                // ── Barrels/crates ─────────────────────────────────────────────
                case "barrels":
                    px = VoronoiTile(tx, ty,
                        new Color(0.42f,0.28f,0.12f), new Color(0.58f,0.42f,0.22f),
                        new Color(0.20f,0.12f,0.04f), 8, 2.0f, 0.08f); break;

                // ── Treasure chest ────────────────────────────────────────────
                case "treasure_chest": case "treasure_chest_gold":
                    px = VoronoiTile(tx, ty,
                        new Color(0.64f,0.44f,0.12f), new Color(0.84f,0.66f,0.30f),
                        new Color(0.36f,0.22f,0.04f), 7, 2.2f, 0.10f); break;

                // ── Hatch / ladder marker — bright cyan ────────────────────────
                case "hatch_marker":
                    px = VoronoiTile(tx, ty,
                        new Color(0.10f,0.65f,0.75f), new Color(0.25f,0.82f,0.90f),
                        new Color(0.04f,0.28f,0.35f), 7, 2.0f, 0.08f); break;

                // ── Standard room door marker — bright amber ───────────────────
                case "door_marker":
                    px = VoronoiTile(tx, ty,
                        new Color(0.90f,0.65f,0.10f), new Color(1.00f,0.85f,0.30f),
                        new Color(0.50f,0.30f,0.02f), 7, 2.0f, 0.08f); break;

                // ── Captain's door/hatch — distinctive red-gold ────────────────
                case "door_captains":
                    px = VoronoiTile(tx, ty,
                        new Color(0.85f,0.20f,0.10f), new Color(1.00f,0.45f,0.30f),
                        new Color(0.40f,0.05f,0.02f), 7, 2.0f, 0.08f); break;

                // ── Lower deck void — near-black ──────────────────────────────
                case "hold_void":
                    px = VoronoiTile(tx, ty,
                        new Color(0.08f,0.04f,0.01f), new Color(0.14f,0.08f,0.03f),
                        new Color(0.02f,0.01f,0.00f), 8, 2.0f, 0.03f); break;

                // ── Lower deck hull wall — dark brown timber ───────────────────
                case "hold_wall":
                    px = VoronoiTile(tx, ty,
                        new Color(0.28f,0.20f,0.12f), new Color(0.38f,0.28f,0.18f),
                        new Color(0.14f,0.08f,0.04f), 8, 2.2f, 0.07f); break;

                // ── Support beam pillar ────────────────────────────────────────
                case "support_beam":
                    px = VoronoiTile(tx, ty,
                        new Color(0.50f,0.30f,0.10f), new Color(0.64f,0.44f,0.18f),
                        new Color(0.24f,0.14f,0.04f), 6, 3.0f, 0.06f); break;

                default:
                    px = VoronoiTile(tx, ty,
                        new Color(0.88f,0.76f,0.50f), new Color(0.98f,0.90f,0.70f),
                        new Color(0.54f,0.44f,0.26f)); break;
            }
            return MakeTex(px);
        }

        private Texture2D MakeTex(Color[] pixels)
        {
            var t = new Texture2D(TEX, TEX, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode   = TextureWrapMode.Clamp
            };
            t.SetPixels(pixels);
            t.Apply();
            return t;
        }
    }
}
