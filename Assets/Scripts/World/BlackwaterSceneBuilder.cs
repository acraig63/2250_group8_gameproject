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
    ///   BlackwaterLowerDeck       — 60x60 below-deck hold
    ///   BlackwaterArmory          — 20x15 armory room
    ///   BlackwaterMessHall        — 20x15 mess hall
    ///   BlackwaterBrig            — 15x15 brig / prison cell
    ///   BlackwaterNavigationRoom  — 15x15 navigation room
    ///   BlackwaterCaptainsQuarters — 25x20 captain's quarters
    ///
    /// Place this on a GameObject in each scene with the Ground and Walls
    /// Tilemap references assigned.  The script reads the active scene name
    /// and builds the appropriate layout at runtime.
    ///
    /// Portal GameObjects are spawned procedurally — no manual wiring needed.
    /// </summary>
    public class BlackwaterSceneBuilder : MonoBehaviour
    {
        [Header("Tilemap References (drag from Hierarchy)")]
        [Tooltip("Ground layer — ship deck, room floors, water.")]
        public Tilemap groundTilemap;

        [Tooltip("Wall/marker layer — railings, room walls, door and hatch markers.")]
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

            groundTilemap.RefreshAllTiles();
            wallTilemap.RefreshAllTiles();

            Debug.Log($"[BlackwaterSceneBuilder] Built scene: {SceneManager.GetActiveScene().name}");
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

            // Fill entire grid with water
            FillRect(groundTilemap, 0, 0, W - 1, H - 1, "water");

            // Ship hull — painted as ship_deck on groundTilemap
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

            // Ship rail — perimeter of the hull on wallTilemap
            PaintHullPerimeter();

            // ---- Portals / doors ----

            // Door → Armory (left side mid)
            PlacePortal("Armory",    27, 38, "BlackwaterArmory",
                        new Vector2(18f, 7f), wallTilemap, "door_marker");

            // Door → MessHall (right side mid)
            PlacePortal("MessHall",  42, 38, "BlackwaterMessHall",
                        new Vector2(1f, 7f), wallTilemap, "door_marker");

            // Door → Brig (left side stern-ward)
            PlacePortal("Brig",      28, 22, "BlackwaterBrig",
                        new Vector2(13f, 7f), wallTilemap, "door_marker");

            // Door → NavigationRoom (near bow)
            PlacePortal("Navigation", 35, 63, "BlackwaterNavigationRoom",
                        new Vector2(7f, 1f), wallTilemap, "door_marker");

            // Door → CaptainsQuarters (stern center) — ornate marker
            PlacePortal("Captains",  35, 5,  "BlackwaterCaptainsQuarters",
                        new Vector2(12f, 1f), wallTilemap, "door_captains");

            // Hatch → LowerDeck (center of ship)
            PlacePortal("LowerDeck", 35, 35, "BlackwaterLowerDeck",
                        new Vector2(30f, 30f), wallTilemap, "hatch_marker");
        }

        // Fills a horizontal band of the hull with ship_deck tile
        private void PaintHullRow(Tilemap tm, int yMin, int yMax, int xMin, int xMax)
        {
            TileBase t = GetTileAsset("ship_deck");
            for (int y = yMin; y <= yMax; y++)
                for (int x = xMin; x <= xMax; x++)
                    tm.SetTile(new Vector3Int(x, y, 0), t);
        }

        // Paints ship_rail on wallTilemap at the outer edge of each hull row
        private void PaintHullPerimeter()
        {
            TileBase rail = GetTileAsset("ship_rail");

            // Helper: mark the border of a rectangular band
            void Rail(int yMin, int yMax, int xMin, int xMax)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    wallTilemap.SetTile(new Vector3Int(xMin, y, 0), rail);
                    wallTilemap.SetTile(new Vector3Int(xMax, y, 0), rail);
                }
                for (int x = xMin; x <= xMax; x++)
                {
                    wallTilemap.SetTile(new Vector3Int(x, yMin, 0), rail);
                    wallTilemap.SetTile(new Vector3Int(x, yMax, 0), rail);
                }
            }

            Rail(65, 69, 33, 36);
            Rail(58, 64, 31, 38);
            Rail(48, 57, 29, 40);
            Rail(35, 47, 27, 42);
            Rail(20, 34, 28, 41);
            Rail(10, 19, 30, 39);
            Rail(3,  9,  31, 38);
            Rail(0,  2,  32, 37);
        }

        // ---------------------------------------------------------------
        // BlackwaterLowerDeck  — 60x60
        // ---------------------------------------------------------------

        private void BuildLowerDeck()
        {
            Portal.ApplyPendingSpawn();

            const int W = 60, H = 60;

            // Dark wooden deck floor
            FillRect(groundTilemap, 0, 0, W - 1, H - 1, "ship_deck_dark");

            // 2-tile wall border
            FillBorder(wallTilemap, 0, 0, W - 1, H - 1, 2, "room_wall");

            // Scatter barrel/crate clusters for visual interest
            int[] bx = { 8, 9, 20, 21, 38, 39, 50, 51, 8, 51 };
            int[] by = { 8, 8, 10, 10,  8,  8, 10, 10, 50, 50 };
            TileBase barrel = GetTileAsset("barrels");
            for (int i = 0; i < bx.Length; i++)
                wallTilemap.SetTile(new Vector3Int(bx[i], by[i], 0), barrel);

            // Hatch back to main deck
            PlacePortal("Flagship", 30, 30, "BlackwaterFlagship",
                        new Vector2(35f, 35f), wallTilemap, "hatch_marker");
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

            // Exit door on the left wall, center row
            PlacePortal("FlagshipArmory", 1, 7, "BlackwaterFlagship",
                        new Vector2(27f, 38f), wallTilemap, "door_marker");
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

            // Exit door on the right wall, center row
            PlacePortal("FlagshipMess", 18, 7, "BlackwaterFlagship",
                        new Vector2(42f, 38f), wallTilemap, "door_marker");
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

            // Exit door on the left wall, center row
            PlacePortal("FlagshipBrig", 1, 7, "BlackwaterFlagship",
                        new Vector2(28f, 22f), wallTilemap, "door_marker");
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

            // Exit door on the bottom wall, center column
            PlacePortal("FlagshipNav", 7, 1, "BlackwaterFlagship",
                        new Vector2(35f, 63f), wallTilemap, "door_marker");
        }

        // ---------------------------------------------------------------
        // BlackwaterCaptainsQuarters  — 25x20
        // ---------------------------------------------------------------

        private void BuildCaptainsQuarters()
        {
            Portal.ApplyPendingSpawn();

            const int W = 25, H = 20;

            // Warm dock-plank floor throughout
            FillRect(groundTilemap, 0, 0, W - 1, H - 1, "dock_planks");

            // 2-tile wall border using camp_wall stone
            FillBorder(wallTilemap, 0, 0, W - 1, H - 1, 2, "camp_wall");

            // 3x3 throne / desk accent at (11,16)-(13,18)
            TileBase accent = GetTileAsset("treasure_chest_gold");
            for (int x = 11; x <= 13; x++)
                for (int y = 16; y <= 18; y++)
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), accent);

            // Exit door on the top wall (near bow of ship → stern marker)
            PlacePortal("FlagshipCaptains", 12, 18, "BlackwaterFlagship",
                        new Vector2(35f, 5f), wallTilemap, "door_marker");
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

        private void PlacePortal(string id, int tx, int ty,
                                  string targetScene, Vector2 spawnPos,
                                  Tilemap markerTilemap, string markerTag)
        {
            // Visual marker tile
            markerTilemap.SetTile(new Vector3Int(tx, ty, 0), GetTileAsset(markerTag));

            // Portal GameObject
            var go  = new GameObject("Portal_" + id);
            go.transform.position = new Vector3(tx + 0.5f, ty + 0.5f, 0f);

            var col      = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size      = new Vector2(1f, 1f);

            var portal         = go.AddComponent<Portal>();
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
        // TEXTURE GENERATOR  (extends IslandSceneBuilder tags + new ship tags)
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

                // ---- New Blackwater ship tags ----

                // Warm brown wooden planks (main deck)
                case "ship_deck":
                    px = VoronoiTile(tx, ty,
                        new Color(0.55f,0.35f,0.15f), new Color(0.72f,0.52f,0.28f),
                        new Color(0.30f,0.18f,0.06f), 8, 2.5f, 0.09f); break;

                // Darker brown planks (lower deck)
                case "ship_deck_dark":
                    px = VoronoiTile(tx, ty,
                        new Color(0.38f,0.22f,0.08f), new Color(0.52f,0.34f,0.14f),
                        new Color(0.18f,0.10f,0.02f), 8, 2.5f, 0.09f); break;

                // Dark navy railing/border
                case "ship_rail":
                    px = VoronoiTile(tx, ty,
                        new Color(0.12f,0.10f,0.22f), new Color(0.22f,0.20f,0.35f),
                        new Color(0.04f,0.04f,0.08f), 8, 2.0f, 0.06f); break;

                // Medium stone/wood interior room floor
                case "room_floor":
                    px = VoronoiTile(tx, ty,
                        new Color(0.58f,0.52f,0.44f), new Color(0.74f,0.68f,0.58f),
                        new Color(0.28f,0.24f,0.18f), 9, 2.2f, 0.08f); break;

                // Grey stone room wall
                case "room_wall":
                    px = VoronoiTile(tx, ty,
                        new Color(0.44f,0.44f,0.44f), new Color(0.62f,0.62f,0.62f),
                        new Color(0.18f,0.18f,0.18f), 8, 2.4f, 0.07f); break;

                // Bright amber door marker
                case "door_marker":
                    px = VoronoiTile(tx, ty,
                        new Color(0.90f,0.65f,0.10f), new Color(1.00f,0.85f,0.30f),
                        new Color(0.50f,0.30f,0.02f), 7, 2.0f, 0.08f); break;

                // Red-gold captain's door marker
                case "door_captains":
                    px = VoronoiTile(tx, ty,
                        new Color(0.85f,0.20f,0.10f), new Color(1.00f,0.45f,0.30f),
                        new Color(0.40f,0.05f,0.02f), 7, 2.0f, 0.08f); break;

                // Cyan hatch/ladder marker
                case "hatch_marker":
                    px = VoronoiTile(tx, ty,
                        new Color(0.10f,0.65f,0.75f), new Color(0.25f,0.82f,0.90f),
                        new Color(0.04f,0.28f,0.35f), 7, 2.0f, 0.08f); break;

                // Ornate dark crimson captain's floor accent
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
