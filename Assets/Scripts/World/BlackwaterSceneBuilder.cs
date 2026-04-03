using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    /// <summary>
    /// Level 5 — Blackwater Flagship
    ///
    /// Single MonoBehaviour that handles all 7 Blackwater scenes:
    ///   BlackwaterFlagship        — 70x70 ship deck on water
    ///   BlackwaterLowerDeck       — 60x60 below-deck hold (ship-shaped interior)
    ///   BlackwaterArmory          — 20x15 armory room
    ///   BlackwaterMessHall        — 20x15 mess hall
    ///   BlackwaterBrig            — 15x15 brig / prison cell
    ///   BlackwaterNavigationRoom  — 15x15 navigation room
    ///   BlackwaterCaptainsQuarters — 25x20 captain's quarters
    ///
    /// Place this on a GameObject in each scene with the Ground and Walls
    /// Tilemap references assigned. The script reads the active scene name
    /// and builds the appropriate layout at runtime.
    ///
    /// Portal GameObjects are spawned procedurally — no manual wiring needed.
    /// Wall collision (TilemapCollider2D + Rigidbody2D) is also added at runtime.
    /// </summary>
    public class BlackwaterSceneBuilder : MonoBehaviour
    {
        [Header("Tilemap References (drag from Hierarchy)")]
        [Tooltip("Ground layer — ship deck, room floors, water, void backgrounds.")]
        public Tilemap groundTilemap;

        [Tooltip("Wall layer — railings, room walls, obstacles. Gets TilemapCollider2D at runtime.")]
        public Tilemap wallTilemap;

        // ---------------------------------------------------------------
        // Internal caches  (same pattern as IslandSceneBuilder)
        // ---------------------------------------------------------------
        private Dictionary<string, TileBase>  _tileAssets;
        private Dictionary<string, Sprite>    _spriteCache;
        private const int TEX = 32;

        // ---------------------------------------------------------------
        // Entry point
        // ---------------------------------------------------------------

        void Start()
        {
            _tileAssets  = new Dictionary<string, TileBase>();
            _spriteCache = new Dictionary<string, Sprite>();

            if (groundTilemap == null || wallTilemap == null)
            {
                Debug.LogError("[BlackwaterSceneBuilder] groundTilemap or wallTilemap is null — " +
                               "drag both Tilemaps into the Inspector.");
                return;
            }

            BuildCurrentScene();

            // Add wall collision AFTER tiles are placed so the TilemapCollider2D
            // generates shapes from the final tile state.
            SetupWallCollision();

            groundTilemap.RefreshAllTiles();
            wallTilemap.RefreshAllTiles();

            // Minimap only on the main flagship scene.
            if (SceneManager.GetActiveScene().name == "BlackwaterFlagship")
                SetupMinimap();

            Debug.Log($"[BlackwaterSceneBuilder] Built scene: {SceneManager.GetActiveScene().name}");
        }

        // ---------------------------------------------------------------
        // Wall collision setup (runtime — no scene-file edits needed)
        // ---------------------------------------------------------------

        private void SetupWallCollision()
        {
            if (wallTilemap.gameObject.GetComponent<TilemapCollider2D>() != null) return;

            // Rigidbody2D (Static) is required for stable collision with dynamic player.
            var rb = wallTilemap.gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;

            wallTilemap.gameObject.AddComponent<TilemapCollider2D>();
        }

        // ---------------------------------------------------------------
        // Minimap setup (runtime — BlackwaterFlagship only)
        // ---------------------------------------------------------------

        private void SetupMinimap()
        {
            // MinimapCamera creates its RenderTexture in Awake, which fires
            // immediately on AddComponent. MinimapUI.Start() then picks up the RT.
            if (MinimapCamera.Instance == null)
            {
                var camGO = new GameObject("MinimapCamera");
                camGO.AddComponent<Camera>();
                camGO.AddComponent<MinimapCamera>();
            }

            if (MinimapUI.Instance == null)
            {
                var canvasGO = new GameObject("MinimapCanvas");
                var canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 10;
                canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                canvasGO.AddComponent<MinimapUI>();
            }
        }

        // ---------------------------------------------------------------
        // Scene dispatcher
        // ---------------------------------------------------------------

        private void BuildCurrentScene()
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "BlackwaterFlagship":          BuildFlagship();           break;
                case "BlackwaterLowerDeck":         BuildLowerDeck();          break;
                case "BlackwaterArmory":            BuildArmory();             break;
                case "BlackwaterMessHall":          BuildMessHall();           break;
                case "BlackwaterBrig":              BuildBrig();               break;
                case "BlackwaterNavigationRoom":    BuildNavigationRoom();     break;
                case "BlackwaterCaptainsQuarters":  BuildCaptainsQuarters();   break;
                default:
                    Debug.LogWarning($"[BlackwaterSceneBuilder] Unknown scene: " +
                                     SceneManager.GetActiveScene().name);
                    break;
            }
        }

        // ===============================================================
        // SCENE BUILDERS
        // ===============================================================

        // ---------------------------------------------------------------
        // BlackwaterFlagship  — 70x70, ship hull on water
        // ---------------------------------------------------------------

        private void BuildFlagship()
        {
            Portal.ApplyPendingSpawn();

            const int W = 70, H = 70;

            // Fill entire grid with water on the ground layer (visual background).
            // Water itself has no collision — the ship rails on wallTilemap block
            // the player from walking off the deck.
            FillRect(groundTilemap, 0, 0, W - 1, H - 1, "water");

            // Ship hull — painted as ship_deck on groundTilemap.
            // y=65-69: bow tip  (4 wide, x 33-36)
            PaintHullRow(groundTilemap, 65, 69, 33, 36);
            // y=58-64: (8 wide, x 31-38)
            PaintHullRow(groundTilemap, 58, 64, 31, 38);
            // y=48-57: (12 wide, x 29-40)
            PaintHullRow(groundTilemap, 48, 57, 29, 40);
            // y=35-47: (16 wide, x 27-42) — main mid-section
            PaintHullRow(groundTilemap, 35, 47, 27, 42);
            // y=20-34: (14 wide, x 28-41)
            PaintHullRow(groundTilemap, 20, 34, 28, 41);
            // y=10-19: (10 wide, x 30-39)
            PaintHullRow(groundTilemap, 10, 19, 30, 39);
            // y=3-9:   (8 wide, x 31-38)
            PaintHullRow(groundTilemap, 3,  9,  31, 38);
            // y=0-2:   stern base (6 wide, x 32-37)
            PaintHullRow(groundTilemap, 0,  2,  32, 37);

            // Paint ship rails along the OUTER EDGE of the entire hull shape.
            // This auto-detects true boundary tiles (including step-change corners)
            // so there are no gaps the player can slip through.
            PaintHullPerimeterAuto();

            // ---- Portals / doors ----
            // PlacePortal always clears the wall tile at the door position so the
            // door marker is visible and the player can physically reach the trigger.
            // Spawn positions inside rooms are centered in the walkable area (not walls).

            // Door → Armory (left rail of mid-section, x=27 cleared)
            PlacePortal("Armory",     27, 38, "BlackwaterArmory",
                        new Vector2(10f, 7f), wallTilemap, "door_marker");

            // Door → MessHall (right rail of mid-section, x=42 cleared)
            PlacePortal("MessHall",   42, 38, "BlackwaterMessHall",
                        new Vector2(10f, 7f), wallTilemap, "door_marker");

            // Door → Brig (left rail of lower section, x=28 cleared)
            PlacePortal("Brig",       28, 22, "BlackwaterBrig",
                        new Vector2(7f, 7f), wallTilemap, "door_marker");

            // Door → NavigationRoom (interior of upper section, near bow)
            PlacePortal("Navigation", 35, 63, "BlackwaterNavigationRoom",
                        new Vector2(7f, 7f), wallTilemap, "door_marker");

            // Door → CaptainsQuarters (interior of lower stern section)
            // Uses a distinct red-gold tile so players recognize the captain's door.
            PlacePortal("Captains",   35, 5,  "BlackwaterCaptainsQuarters",
                        new Vector2(12f, 10f), wallTilemap, "door_captains");

            // Hatch → LowerDeck (center of main mid-section, bottom rail cleared)
            PlacePortal("LowerDeck",  35, 35, "BlackwaterLowerDeck",
                        new Vector2(30f, 37f), wallTilemap, "hatch_marker");
        }

        // ---------------------------------------------------------------
        // Hull helpers
        // ---------------------------------------------------------------

        /// Returns true if (x,y) is inside the flagship ship hull.
        private bool IsShipDeck(int x, int y)
        {
            if (y >= 65 && y <= 69 && x >= 33 && x <= 36) return true;
            if (y >= 58 && y <= 64 && x >= 31 && x <= 38) return true;
            if (y >= 48 && y <= 57 && x >= 29 && x <= 40) return true;
            if (y >= 35 && y <= 47 && x >= 27 && x <= 42) return true;
            if (y >= 20 && y <= 34 && x >= 28 && x <= 41) return true;
            if (y >= 10 && y <= 19 && x >= 30 && x <= 39) return true;
            if (y >=  3 && y <=  9 && x >= 31 && x <= 38) return true;
            if (y >=  0 && y <=  2 && x >= 32 && x <= 37) return true;
            return false;
        }

        /// Fills a horizontal band of the hull with ship_deck tile.
        private void PaintHullRow(Tilemap tm, int yMin, int yMax, int xMin, int xMax)
        {
            TileBase t = GetTileAsset("ship_deck");
            for (int y = yMin; y <= yMax; y++)
                for (int x = xMin; x <= xMax; x++)
                    tm.SetTile(new Vector3Int(x, y, 0), t);
        }

        /// Paints ship_rail on wallTilemap at every hull tile that has a non-hull neighbour.
        /// This produces a gapless perimeter — no holes at hull step-change corners.
        private void PaintHullPerimeterAuto()
        {
            TileBase rail = GetTileAsset("ship_rail");
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0,  0, 1, -1 };

            for (int y = 0; y < 70; y++)
            for (int x = 0; x < 70; x++)
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

        // ---------------------------------------------------------------
        // BlackwaterLowerDeck  — 60x60 with ship-shaped interior
        // ---------------------------------------------------------------

        private void BuildLowerDeck()
        {
            Portal.ApplyPendingSpawn();

            const int W = 60, H = 60;

            // Fill entire grid: groundTilemap gets dark void background,
            // wallTilemap gets solid void (blocks movement everywhere initially).
            FillRect(groundTilemap, 0, 0, W - 1, H - 1, "hold_void");
            FillRect(wallTilemap,   0, 0, W - 1, H - 1, "hold_void");

            // Carve out the ship-shaped interior (floor tiles on ground,
            // clear wall tiles for walkability, then re-paint hull wall perimeter).
            BuildLowerDeckHull();

            // Barrel clusters in the main hold sections (on wallTilemap = obstacle).
            TileBase barrel = GetTileAsset("barrels");
            int[] bx = { 25, 25, 35, 35, 25, 35, 28, 29, 31, 32 };
            int[] by = { 33, 34, 33, 34, 20, 20, 45, 45, 45, 45 };
            for (int i = 0; i < bx.Length; i++)
                if (IsLowerDeckInterior(bx[i], by[i]))
                    wallTilemap.SetTile(new Vector3Int(bx[i], by[i], 0), barrel);

            // Structural support beams / pillars (on wallTilemap = obstacle).
            TileBase beam = GetTileAsset("support_beam");
            int[] px = { 27, 33, 27, 33 };
            int[] py = { 37, 37, 22, 22 };
            for (int i = 0; i < px.Length; i++)
                if (IsLowerDeckInterior(px[i], py[i]))
                    wallTilemap.SetTile(new Vector3Int(px[i], py[i], 0), beam);

            // Hatch back to main deck — at centre of the main mid-section.
            // Flagship hatch return-spawn is 2 tiles south of the flagship hatch (y=33).
            PlacePortal("Flagship", 30, 35, "BlackwaterFlagship",
                        new Vector2(35f, 33f), wallTilemap, "hatch_marker");
        }

        /// Returns true if (x,y) is inside the lower-deck ship interior.
        /// Shape is proportionally scaled from the flagship hull (60/70 ratio).
        private bool IsLowerDeckInterior(int x, int y)
        {
            if (y >= 56 && y <= 59 && x >= 29 && x <= 31) return true;
            if (y >= 50 && y <= 55 && x >= 27 && x <= 33) return true;
            if (y >= 41 && y <= 49 && x >= 25 && x <= 35) return true;
            if (y >= 30 && y <= 40 && x >= 24 && x <= 36) return true;
            if (y >= 17 && y <= 29 && x >= 24 && x <= 36) return true;
            if (y >=  9 && y <= 16 && x >= 26 && x <= 34) return true;
            if (y >=  3 && y <=  8 && x >= 27 && x <= 33) return true;
            if (y >=  0 && y <=  2 && x >= 28 && x <= 32) return true;
            return false;
        }

        private void BuildLowerDeckHull()
        {
            // Paint floor and clear wall for all interior tiles.
            PaintLowerHullSection(56, 59, 29, 31);
            PaintLowerHullSection(50, 55, 27, 33);
            PaintLowerHullSection(41, 49, 25, 35);
            PaintLowerHullSection(30, 40, 24, 36);
            PaintLowerHullSection(17, 29, 24, 36);
            PaintLowerHullSection( 9, 16, 26, 34);
            PaintLowerHullSection( 3,  8, 27, 33);
            PaintLowerHullSection( 0,  2, 28, 32);

            // Re-paint the hull-wall perimeter (interior edge tiles adjacent to void).
            PaintLowerDeckPerimeter();
        }

        private void PaintLowerHullSection(int yMin, int yMax, int xMin, int xMax)
        {
            TileBase floor = GetTileAsset("ship_deck_dark");
            for (int y = yMin; y <= yMax; y++)
            for (int x = xMin; x <= xMax; x++)
            {
                groundTilemap.SetTile(new Vector3Int(x, y, 0), floor);
                wallTilemap.SetTile(new Vector3Int(x, y, 0), null);   // clear void → walkable
            }
        }

        private void PaintLowerDeckPerimeter()
        {
            TileBase wall = GetTileAsset("hold_wall");
            int[] dx = { 1, -1, 0, 0 };
            int[] dy = { 0,  0, 1, -1 };

            for (int y = 0; y < 60; y++)
            for (int x = 0; x < 60; x++)
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

        // ---------------------------------------------------------------
        // BlackwaterArmory  — 20x15
        // ---------------------------------------------------------------

        private void BuildArmory()
        {
            Portal.ApplyPendingSpawn();

            const int W = 20, H = 15;

            FillRect(groundTilemap, 0, 0, W - 1, H - 1, "room_floor");
            FillBorder(wallTilemap, 0, 0, W - 1, H - 1, 2, "room_wall");

            // Exit door on the LEFT wall at x=1 (inner wall row).
            // PlacePortal clears wallTilemap at (1,7) so the door is visible and reachable.
            // Return-spawn on flagship is 2 tiles right of the flagship armory door (x=29).
            PlacePortal("FlagshipArmory", 1, 7, "BlackwaterFlagship",
                        new Vector2(29f, 38f), wallTilemap, "door_marker");
        }

        // ---------------------------------------------------------------
        // BlackwaterMessHall  — 20x15
        // ---------------------------------------------------------------

        private void BuildMessHall()
        {
            Portal.ApplyPendingSpawn();

            const int W = 20, H = 15;

            FillRect(groundTilemap, 0, 0, W - 1, H - 1, "sand_path");
            FillBorder(wallTilemap, 0, 0, W - 1, H - 1, 2, "room_wall");

            // Exit door on the RIGHT wall at x=18 (inner wall column).
            // Return-spawn on flagship is 2 tiles left of the flagship mess door (x=40).
            PlacePortal("FlagshipMess", 18, 7, "BlackwaterFlagship",
                        new Vector2(40f, 38f), wallTilemap, "door_marker");
        }

        // ---------------------------------------------------------------
        // BlackwaterBrig  — 15x15
        // ---------------------------------------------------------------

        private void BuildBrig()
        {
            Portal.ApplyPendingSpawn();

            const int W = 15, H = 15;

            FillRect(groundTilemap, 0, 0, W - 1, H - 1, "camp_wall_dark");
            FillBorder(wallTilemap, 0, 0, W - 1, H - 1, 2, "room_wall");

            // Exit door on the LEFT wall at x=1.
            // Return-spawn on flagship is 2 tiles right of the flagship brig door (x=30).
            PlacePortal("FlagshipBrig", 1, 7, "BlackwaterFlagship",
                        new Vector2(30f, 22f), wallTilemap, "door_marker");
        }

        // ---------------------------------------------------------------
        // BlackwaterNavigationRoom  — 15x15
        // ---------------------------------------------------------------

        private void BuildNavigationRoom()
        {
            Portal.ApplyPendingSpawn();

            const int W = 15, H = 15;

            FillRect(groundTilemap, 0, 0, W - 1, H - 1, "sand_wet");
            FillBorder(wallTilemap, 0, 0, W - 1, H - 1, 2, "room_wall");

            // Exit door on the BOTTOM wall at y=1 (inner wall row).
            // Return-spawn on flagship is 2 tiles south of the flagship nav door (y=61).
            PlacePortal("FlagshipNav", 7, 1, "BlackwaterFlagship",
                        new Vector2(35f, 61f), wallTilemap, "door_marker");
        }

        // ---------------------------------------------------------------
        // BlackwaterCaptainsQuarters  — 25x20
        // ---------------------------------------------------------------

        private void BuildCaptainsQuarters()
        {
            Portal.ApplyPendingSpawn();

            const int W = 25, H = 20;

            // Warm dock-plank floor throughout.
            FillRect(groundTilemap, 0, 0, W - 1, H - 1, "dock_planks");

            // 2-tile wall border using camp_wall stone.
            FillBorder(wallTilemap, 0, 0, W - 1, H - 1, 2, "camp_wall");

            // 3x3 throne / desk accent at (11,16)-(13,18).
            TileBase accent = GetTileAsset("treasure_chest_gold");
            for (int x = 11; x <= 13; x++)
                for (int y = 16; y <= 18; y++)
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), accent);

            // Exit door on the TOP wall at y=18 (inner wall row).
            // Room entered from the stern (bottom of ship) so exit faces up/back.
            // Return-spawn on flagship is 2 tiles north of the flagship captains door (y=7).
            PlacePortal("FlagshipCaptains", 12, 18, "BlackwaterFlagship",
                        new Vector2(35f, 7f), wallTilemap, "door_marker");
        }

        // ===============================================================
        // MAP HELPERS
        // ===============================================================

        /// Fill a rectangle on a Tilemap with a visual-tag tile.
        private void FillRect(Tilemap tm, int x0, int y0, int x1, int y1, string tag)
        {
            TileBase t = GetTileAsset(tag);
            for (int x = x0; x <= x1; x++)
                for (int y = y0; y <= y1; y++)
                    tm.SetTile(new Vector3Int(x, y, 0), t);
        }

        /// Fill 'thickness' rows/columns around the perimeter of a rectangle.
        private void FillBorder(Tilemap tm, int x0, int y0, int x1, int y1,
                                 int thickness, string tag)
        {
            TileBase t = GetTileAsset(tag);
            for (int d = 0; d < thickness; d++)
            {
                int left  = x0 + d, right = x1 - d;
                int bot   = y0 + d, top   = y1 - d;
                if (left > right || bot > top) break;

                for (int x = left; x <= right; x++)
                {
                    tm.SetTile(new Vector3Int(x, bot, 0), t);
                    tm.SetTile(new Vector3Int(x, top, 0), t);
                }
                for (int y = bot + 1; y < top; y++)
                {
                    tm.SetTile(new Vector3Int(left,  y, 0), t);
                    tm.SetTile(new Vector3Int(right, y, 0), t);
                }
            }
        }

        // ===============================================================
        // PORTAL PLACEMENT
        // ===============================================================

        /// <summary>
        /// Places a portal at tile (tx, ty).
        ///
        /// Always clears the wallTilemap tile at that position first so the
        /// door marker on the ground layer is visible, and the wall collider
        /// does not block the player from reaching the trigger.
        /// </summary>
        private void PlacePortal(string id, int tx, int ty,
                                  string targetScene, Vector2 spawnPos,
                                  Tilemap markerTilemap, string markerTag)
        {
            var pos = new Vector3Int(tx, ty, 0);

            // Remove any wall tile (rail / room_wall / hold_wall) at this position
            // so the door is both visible and physically accessible.
            wallTilemap.SetTile(pos, null);

            // Visual marker tile on the ground layer.
            groundTilemap.SetTile(pos, GetTileAsset(markerTag));

            // Portal trigger GameObject.
            var go       = new GameObject("Portal_" + id);
            go.transform.position = new Vector3(tx + 0.5f, ty + 0.5f, 0f);

            var col      = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size      = new Vector2(1f, 1f);

            var portal           = go.AddComponent<Portal>();
            portal.targetScene   = targetScene;
            portal.spawnPosition = spawnPos;
        }

        // ===============================================================
        // TILE ASSET RESOLUTION  (same pattern as IslandSceneBuilder)
        // ===============================================================

        private TileBase GetTileAsset(string tag)
        {
            if (_tileAssets.TryGetValue(tag, out TileBase existing))
                return existing;

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

        // ===============================================================
        // VORONOI CELL TILE GENERATOR  (copied from IslandSceneBuilder)
        // ===============================================================

        private Color[] VoronoiTile(
            int tx, int ty,
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
                                 ? Mathf.PerlinNoise(px*0.18f + tx*0.3f, py*0.18f + ty*0.3f) - 0.5f
                                 : 0f;
                    c = Color.Lerp(cellLight, cellBase, t);
                    c = new Color(
                        Mathf.Clamp01(c.r + noise * noiseStrength),
                        Mathf.Clamp01(c.g + noise * noiseStrength),
                        Mathf.Clamp01(c.b + noise * noiseStrength), 1f);
                }

                int rim = Mathf.Min(px, py, TEX - 1 - px, TEX - 1 - py);
                if (rim == 0) c = Color.Lerp(c, mortar, 0.35f);
                pixels[py * TEX + px] = c;
            }
            return pixels;
        }

        // ===============================================================
        // TEXTURE GENERATOR
        // ===============================================================

        private Texture2D GenerateTexture(string tag, int tx, int ty)
        {
            Color[] px;
            switch (tag)
            {
                // ---- Existing island tags (reused) ----
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
                case "water": case "water_shallow":
                    px = VoronoiTile(tx, ty,
                        new Color(0.18f,0.46f,0.70f), new Color(0.32f,0.64f,0.88f),
                        new Color(0.06f,0.22f,0.44f), 10, 1.8f, 0.10f); break;
                case "camp_wall":
                    px = VoronoiTile(tx, ty,
                        new Color(0.58f,0.54f,0.48f), new Color(0.74f,0.70f,0.64f),
                        new Color(0.22f,0.20f,0.18f), 8, 2.4f, 0.08f); break;
                case "camp_wall_dark":
                    px = VoronoiTile(tx, ty,
                        new Color(0.36f,0.33f,0.29f), new Color(0.50f,0.47f,0.42f),
                        new Color(0.12f,0.10f,0.08f), 8, 2.6f, 0.07f); break;
                case "barrels":
                    px = VoronoiTile(tx, ty,
                        new Color(0.42f,0.28f,0.12f), new Color(0.58f,0.42f,0.22f),
                        new Color(0.20f,0.12f,0.04f), 8, 2.0f, 0.08f); break;
                case "dock_planks": case "dock_secret":
                    px = VoronoiTile(tx, ty,
                        new Color(0.62f,0.44f,0.24f), new Color(0.78f,0.60f,0.38f),
                        new Color(0.30f,0.18f,0.08f), 6, 3.0f, 0.08f); break;
                case "treasure_chest": case "treasure_chest_gold":
                    px = VoronoiTile(tx, ty,
                        new Color(0.64f,0.44f,0.12f), new Color(0.84f,0.66f,0.30f),
                        new Color(0.36f,0.22f,0.04f), 7, 2.2f, 0.10f); break;

                // ---- Blackwater ship deck tags ----

                // Warm brown wooden planks (main deck).
                case "ship_deck":
                    px = VoronoiTile(tx, ty,
                        new Color(0.55f,0.35f,0.15f), new Color(0.72f,0.52f,0.28f),
                        new Color(0.30f,0.18f,0.06f), 8, 2.5f, 0.09f); break;

                // Darker brown planks (lower deck floor).
                case "ship_deck_dark":
                    px = VoronoiTile(tx, ty,
                        new Color(0.38f,0.22f,0.08f), new Color(0.52f,0.34f,0.14f),
                        new Color(0.18f,0.10f,0.02f), 8, 2.5f, 0.09f); break;

                // Dark navy railing / hull perimeter.
                case "ship_rail":
                    px = VoronoiTile(tx, ty,
                        new Color(0.12f,0.10f,0.22f), new Color(0.22f,0.20f,0.35f),
                        new Color(0.04f,0.04f,0.08f), 8, 2.0f, 0.06f); break;

                // Medium stone/wood interior room floor.
                case "room_floor":
                    px = VoronoiTile(tx, ty,
                        new Color(0.58f,0.52f,0.44f), new Color(0.74f,0.68f,0.58f),
                        new Color(0.28f,0.24f,0.18f), 9, 2.2f, 0.08f); break;

                // Grey stone room wall.
                case "room_wall":
                    px = VoronoiTile(tx, ty,
                        new Color(0.44f,0.44f,0.44f), new Color(0.62f,0.62f,0.62f),
                        new Color(0.18f,0.18f,0.18f), 8, 2.4f, 0.07f); break;

                // Bright amber door marker — standard rooms.
                case "door_marker":
                    px = VoronoiTile(tx, ty,
                        new Color(0.90f,0.65f,0.10f), new Color(1.00f,0.85f,0.30f),
                        new Color(0.50f,0.30f,0.02f), 7, 2.0f, 0.08f); break;

                // Red-gold captain's door marker — visually distinct from standard doors.
                case "door_captains":
                    px = VoronoiTile(tx, ty,
                        new Color(0.85f,0.20f,0.10f), new Color(1.00f,0.45f,0.30f),
                        new Color(0.40f,0.05f,0.02f), 7, 2.0f, 0.08f); break;

                // Bright cyan hatch / ladder marker — distinct from doors.
                case "hatch_marker":
                    px = VoronoiTile(tx, ty,
                        new Color(0.10f,0.65f,0.75f), new Color(0.25f,0.82f,0.90f),
                        new Color(0.04f,0.28f,0.35f), 7, 2.0f, 0.08f); break;

                // Near-black void used outside the ship in the lower deck.
                case "hold_void":
                    px = VoronoiTile(tx, ty,
                        new Color(0.08f,0.04f,0.01f), new Color(0.14f,0.08f,0.03f),
                        new Color(0.02f,0.01f,0.00f), 8, 2.0f, 0.03f); break;

                // Dark brown hull wall (inner edge of the lower-deck hull shape).
                case "hold_wall":
                    px = VoronoiTile(tx, ty,
                        new Color(0.28f,0.20f,0.12f), new Color(0.38f,0.28f,0.18f),
                        new Color(0.14f,0.08f,0.04f), 8, 2.2f, 0.07f); break;

                // Thick wooden support beam / pillar in the hold.
                case "support_beam":
                    px = VoronoiTile(tx, ty,
                        new Color(0.50f,0.30f,0.10f), new Color(0.64f,0.44f,0.18f),
                        new Color(0.24f,0.14f,0.04f), 6, 3.0f, 0.06f); break;

                // Ornate dark crimson captain's floor accent (unused in current layout
                // but kept for potential future use).
                case "room_floor_captains":
                    px = VoronoiTile(tx, ty,
                        new Color(0.45f,0.08f,0.08f), new Color(0.62f,0.18f,0.15f),
                        new Color(0.18f,0.02f,0.02f), 8, 2.2f, 0.08f); break;

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
