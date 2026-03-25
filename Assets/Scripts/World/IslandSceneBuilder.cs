using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    // =========================================================================
    // TileSpriteEntry — one row in the Inspector sprite override table
    // =========================================================================

    [System.Serializable]
    public class TileSpriteEntry
    {
        [Tooltip("Visual tag string exactly matching one used in SmugglersIslandLevel.\n" +
                 "Known tags: water, water_shallow, sand, sand_wet, sand_path,\n" +
                 "  rock_tide, driftwood, barrels, quicksand,\n" +
                 "  camp_wall, camp_wall_dark, tent_red, tent_large,\n" +
                 "  jungle_tree, jungle_floor,\n" +
                 "  treasure_chest, treasure_chest_gold,\n" +
                 "  dock_planks, dock_secret")]
        public string visualTag;

        [Tooltip("Real sprite to override the procedural art for this tag.")]
        public Sprite sprite;
    }

    // =========================================================================
    // IslandSceneBuilder — MonoBehaviour
    // =========================================================================

    /// <summary>
    /// Attach to any empty GameObject in the SmugglersIsland scene.
    ///
    /// On Start it:
    ///   1. Instantiates SmugglersIslandLevel and calls Initialize().
    ///   2. Reads the 80×60 TileMap.
    ///   3. Spawns one child GameObject per tile with a SpriteRenderer.
    ///   4. Generates a 32×32 procedural sprite per visual tag using a
    ///      Voronoi cell algorithm — the same "chunky cobblestone/grass cell"
    ///      style seen in classic top-down 2D RPG tile sets.
    ///      • Each tile has ~10 rounded cells with dark borders and a
    ///        lighter highlight near each cell centre.
    ///      • Tile type only changes the colour palette — the underlying
    ///        cell structure is consistent across all ground tiles.
    ///      • Water tiles animate (scrolling cell pattern) via Update().
    ///   5. Adds physics colliders and special components per TileType.
    ///
    /// Inspector:
    ///   • Sprite Entries — override any tag with a real imported sprite.
    ///   • Tile Size      — world-space width/height of one tile (default 1).
    ///   • Tile Parent    — optional parent transform for the tile hierarchy.
    /// </summary>
    public class IslandSceneBuilder : MonoBehaviour
    {
        // ------------------------------------------------------------------
        // Inspector
        // ------------------------------------------------------------------

        [Header("Sprite Overrides (leave blank entries to use procedural art)")]
        [SerializeField] private TileSpriteEntry[] spriteEntries;

        [Header("World")]
        [Tooltip("World-space side length of one tile in Unity units.")]
        [SerializeField] private float tileSize = 1f;

        [Tooltip("Optional parent transform for all tile GameObjects.")]
        [SerializeField] private Transform tileParent;

        [Header("Sorting Layers")]
        [SerializeField] private string groundLayer   = "Default";
        [SerializeField] private string obstacleLayer = "Default";

        // ------------------------------------------------------------------
        // Internal
        // ------------------------------------------------------------------

        private Dictionary<string, Sprite> _overrides;
        private Dictionary<string, Sprite> _cache;             // shared per-tag sprites
        private Dictionary<string, Sprite> _waterCache;        // per-tile water (animated)
        private List<(SpriteRenderer sr, int tx, int ty, bool shallow)> _waterRenderers;

        private SmugglersIslandLevel _level;
        public  SmugglersIslandLevel Level => _level;

        private const int TEX = 32;   // texture side in pixels

        // ------------------------------------------------------------------
        // Unity lifecycle
        // ------------------------------------------------------------------

        void Start()
        {
            _overrides       = new Dictionary<string, Sprite>();
            _cache           = new Dictionary<string, Sprite>();
            _waterCache      = new Dictionary<string, Sprite>();
            _waterRenderers  = new List<(SpriteRenderer, int, int, bool)>();

            if (spriteEntries != null)
                foreach (var e in spriteEntries)
                    if (e?.sprite != null && !string.IsNullOrEmpty(e.visualTag))
                        _overrides[e.visualTag] = e.sprite;

            _level = new SmugglersIslandLevel();
            _level.Initialize();

            Transform parent = tileParent != null ? tileParent : transform;
            TileMap   map    = _level.MapLayout;
            int       n      = 0;

            for (int x = 0; x < map.Width;  x++)
            for (int y = 0; y < map.Height; y++)
            {
                Tile t = map.GetTile(x, y);
                if (t != null) { SpawnTile(t, parent); n++; }
            }

            Debug.Log($"IslandSceneBuilder: {n} tiles spawned — {_level.LevelName}.");
        }

        // Animate water every other frame
        void Update()
        {
            if (Time.frameCount % 2 != 0) return;
            float t = Time.time * 0.35f;
            foreach (var (sr, tx, ty, shallow) in _waterRenderers)
            {
                if (sr == null) continue;
                sr.sprite = SpriteFrom(DrawWater(tx, ty, t, shallow));
            }
        }

        // ------------------------------------------------------------------
        // Tile spawning
        // ------------------------------------------------------------------

        private void SpawnTile(Tile tile, Transform parent)
        {
            var go = new GameObject($"T_{tile.X}_{tile.Y}_{tile.VisualTag}");
            go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(tile.X * tileSize, tile.Y * tileSize, 0f);

            var sr          = go.AddComponent<SpriteRenderer>();
            sr.sprite       = ResolveSprite(tile.VisualTag, tile.X, tile.Y);

            bool isGround   = tile.TileType == TileType.Walkable
                           || tile.TileType == TileType.Water
                           || tile.TileType == TileType.NPCSpawn
                           || tile.TileType == TileType.Hazard;

            sr.sortingLayerName = isGround ? groundLayer : obstacleLayer;
            sr.sortingOrder     = isGround ? 0 : 1;

            switch (tile.TileType)
            {
                case TileType.Obstacle:
                case TileType.Water:
                    AddCol(go, false); break;
                case TileType.Hazard:
                    AddCol(go, true);
                    go.AddComponent<HazardZone>(); break;
                case TileType.Exit:
                    AddCol(go, true); SetTag(go, "ExitZone"); break;
                case TileType.TreasureSpot:
                    AddCol(go, true); SetTag(go, "TreasureSpot"); break;
            }

            // Register water for animation
            bool isWater = tile.VisualTag == "water" || tile.VisualTag == "water_shallow";
            if (isWater && !_overrides.ContainsKey(tile.VisualTag))
                _waterRenderers.Add((sr, tile.X, tile.Y, tile.VisualTag == "water_shallow"));
        }

        private void AddCol(GameObject go, bool trigger)
        {
            var c = go.AddComponent<BoxCollider2D>();
            c.isTrigger = trigger;
            c.size      = Vector2.one * tileSize;
        }

        private void SetTag(GameObject go, string tag)
        {
            try   { go.tag = tag; }
            catch { Debug.LogWarning($"IslandSceneBuilder: tag '{tag}' not found in Tags & Layers."); }
        }

        // ------------------------------------------------------------------
        // Sprite resolution
        // ------------------------------------------------------------------

        private Sprite ResolveSprite(string tag, int tx, int ty)
        {
            if (_overrides.TryGetValue(tag, out Sprite ov)) return ov;

            // Water: unique per-tile entry so each can hold its own animated texture
            if (tag == "water" || tag == "water_shallow")
            {
                string key = $"water_{tx}_{ty}";
                if (!_waterCache.TryGetValue(key, out Sprite ws))
                {
                    ws = SpriteFrom(DrawWater(tx, ty, 0f, tag == "water_shallow"));
                    _waterCache[key] = ws;
                }
                return ws;
            }

            // All other tags: one shared cached sprite per tag
            if (!_cache.TryGetValue(tag, out Sprite sp))
            {
                sp = SpriteFrom(GenerateTexture(tag, tx, ty));
                _cache[tag] = sp;
            }
            return sp;
        }

        // ------------------------------------------------------------------
        // Sprite utility
        // ------------------------------------------------------------------

        private Sprite SpriteFrom(Texture2D tex)
            => Sprite.Create(tex, new Rect(0, 0, TEX, TEX), new Vector2(0.5f, 0.5f), TEX);

        // ==================================================================
        // ===            VORONOI CELL TILE ENGINE                        ===
        // ==================================================================
        //
        // Core idea (matches the style in the screenshot):
        //   1. Scatter N random cell centres across the 32×32 texture.
        //   2. For each pixel, find distance to its nearest centre (d1)
        //      and second-nearest (d2).
        //   3. The "edge distance" is (d2 - d1):
        //        • Near 0  → pixel is on a cell border → paint dark mortar.
        //        • Larger  → pixel is deep in a cell interior.
        //   4. Inside a cell: lerp from a bright highlight (near centre)
        //      to the base cell colour (near edge) using d1.
        //   5. Apply a subtle rim darken on the outer 1-2 pixels so tiles
        //      visually separate from their neighbours.
        //
        // Every ground tile type calls VoronoiTile() with a colour palette.
        // Structure tiles (tents, chests, dock, barrels) add a shape mask
        // on top of a Voronoi base.
        // ==================================================================

        // ------------------------------------------------------------------
        // CORE: Voronoi cell generator
        // ------------------------------------------------------------------

        /// <summary>
        /// Produces a TEX×TEX pixel array with the chunky cobblestone/grass-cell
        /// look from the screenshot.
        /// </summary>
        /// <param name="tx">Tile world X (used as random seed so neighbours vary).</param>
        /// <param name="ty">Tile world Y.</param>
        /// <param name="cellBase">Main colour of each cell interior.</param>
        /// <param name="cellLight">Highlight colour at each cell centre.</param>
        /// <param name="mortar">Dark border colour between cells.</param>
        /// <param name="numCells">How many Voronoi cells to scatter (8–14 works well).</param>
        /// <param name="borderWidth">How thick the dark border stripe is (pixels).</param>
        /// <param name="noiseStrength">0–1 amount of Perlin colour variation per cell.</param>
        private Color[] VoronoiTile(
            int    tx, int ty,
            Color  cellBase, Color cellLight, Color mortar,
            int    numCells    = 10,
            float  borderWidth = 1.8f,
            float  noiseStrength = 0.10f)
        {
            // Seed the RNG with the tile position so every tile is deterministically
            // different from its neighbours but reproducible.
            var rng = new System.Random(tx * 73856093 ^ ty * 19349663);

            // Scatter cell centres. We tile three copies (a 3×1 strip offset by –TEX,
            // 0, +TEX in both axes) so cells that straddle tile edges are handled
            // correctly and the border doesn't cluster at corners.
            int total = numCells * 9;
            var cx    = new float[total];
            var cy    = new float[total];
            int idx   = 0;
            for (int oy = -1; oy <= 1; oy++)
            for (int ox = -1; ox <= 1; ox++)
            for (int i = 0; i < numCells; i++)
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
                    float ddx = px - cx[i];
                    float ddy = py - cy[i];
                    float d   = ddx * ddx + ddy * ddy;
                    if (d < d1)      { d2 = d1; d1 = d; }
                    else if (d < d2) { d2 = d; }
                }

                d1 = Mathf.Sqrt(d1);
                d2 = Mathf.Sqrt(d2);
                float edge = d2 - d1;   // 0 at cell borders, grows toward cell centres

                Color c;
                if (edge < borderWidth)
                {
                    // Border: blend from mortar at the centre-line to base colour at edge
                    float t = Mathf.Clamp01(edge / borderWidth);
                    c = Color.Lerp(mortar, cellBase, t * t);  // quadratic so border stays crisp
                }
                else
                {
                    // Interior: lerp highlight (d1 small = near centre) → base (d1 large = near edge)
                    // Max expected d1 for a well-filled cell ≈ half the average cell radius
                    float cellRadius = TEX / Mathf.Sqrt(numCells) * 0.55f;
                    float t          = Mathf.Clamp01(d1 / cellRadius);

                    // Optional Perlin variation so each cell has a slightly different shade
                    float noise      = noiseStrength > 0f
                                     ? Mathf.PerlinNoise(px * 0.18f + tx * 0.3f,
                                                         py * 0.18f + ty * 0.3f) - 0.5f
                                     : 0f;

                    c = Color.Lerp(cellLight, cellBase, t);
                    c = new Color(
                        Mathf.Clamp01(c.r + noise * noiseStrength),
                        Mathf.Clamp01(c.g + noise * noiseStrength),
                        Mathf.Clamp01(c.b + noise * noiseStrength),
                        1f);
                }

                // Outer rim darkening (1 px) so tiles separate visually from neighbours
                int rim = Mathf.Min(px, py, TEX - 1 - px, TEX - 1 - py);
                if (rim == 0) c = Color.Lerp(c, mortar, 0.35f);

                pixels[py * TEX + px] = c;
            }

            return pixels;
        }

        // ------------------------------------------------------------------
        // TEXTURE DISPATCHER — maps visual tag → Draw* method
        // ------------------------------------------------------------------

        private Texture2D GenerateTexture(string tag, int tx, int ty)
        {
            Color[] px;
            switch (tag)
            {
                // ---- Ground tiles ----
                case "sand":
                    px = VoronoiTile(tx, ty,
                        cellBase:  new Color(0.88f, 0.76f, 0.50f),   // warm tan
                        cellLight: new Color(0.98f, 0.90f, 0.70f),   // cream highlight
                        mortar:    new Color(0.54f, 0.44f, 0.26f),   // dark sand border
                        numCells: 11, borderWidth: 1.6f, noiseStrength: 0.08f);
                    break;

                case "sand_wet":
                    px = VoronoiTile(tx, ty,
                        cellBase:  new Color(0.70f, 0.60f, 0.40f),
                        cellLight: new Color(0.82f, 0.72f, 0.52f),
                        mortar:    new Color(0.38f, 0.30f, 0.18f),
                        numCells: 11, borderWidth: 1.8f, noiseStrength: 0.07f);
                    break;

                case "sand_path":
                    // Slightly more yellow-gold, like the bottom-right yellow tiles
                    px = VoronoiTile(tx, ty,
                        cellBase:  new Color(0.80f, 0.68f, 0.36f),
                        cellLight: new Color(0.94f, 0.84f, 0.56f),
                        mortar:    new Color(0.46f, 0.36f, 0.14f),
                        numCells: 10, borderWidth: 2.0f, noiseStrength: 0.09f);
                    break;

                case "quicksand":
                    // Murky yellow-brown with extra-thick borders (looks dense/dangerous)
                    px = VoronoiTile(tx, ty,
                        cellBase:  new Color(0.74f, 0.62f, 0.20f),
                        cellLight: new Color(0.88f, 0.78f, 0.38f),
                        mortar:    new Color(0.38f, 0.28f, 0.06f),
                        numCells: 9, borderWidth: 2.8f, noiseStrength: 0.12f);
                    break;

                // ---- Rock / wall tiles ----
                case "rock_tide":
                    px = VoronoiTile(tx, ty,
                        cellBase:  new Color(0.54f, 0.52f, 0.50f),
                        cellLight: new Color(0.72f, 0.70f, 0.68f),
                        mortar:    new Color(0.24f, 0.22f, 0.20f),
                        numCells: 9, borderWidth: 2.2f, noiseStrength: 0.10f);
                    break;

                case "camp_wall":
                    px = VoronoiTile(tx, ty,
                        cellBase:  new Color(0.58f, 0.54f, 0.48f),
                        cellLight: new Color(0.74f, 0.70f, 0.64f),
                        mortar:    new Color(0.22f, 0.20f, 0.18f),
                        numCells: 8, borderWidth: 2.4f, noiseStrength: 0.08f);
                    break;

                case "camp_wall_dark":
                    px = VoronoiTile(tx, ty,
                        cellBase:  new Color(0.36f, 0.33f, 0.29f),
                        cellLight: new Color(0.50f, 0.47f, 0.42f),
                        mortar:    new Color(0.12f, 0.10f, 0.08f),
                        numCells: 8, borderWidth: 2.6f, noiseStrength: 0.07f);
                    break;

                // ---- Jungle tiles ----
                case "jungle_tree":
                    // Dark green — like the grass tiles in the screenshot but richer/darker
                    px = VoronoiTile(tx, ty,
                        cellBase:  new Color(0.10f, 0.42f, 0.10f),
                        cellLight: new Color(0.26f, 0.62f, 0.18f),
                        mortar:    new Color(0.04f, 0.16f, 0.04f),
                        numCells: 12, borderWidth: 1.6f, noiseStrength: 0.12f);
                    break;

                case "jungle_floor":
                    // Medium green — lighter than tree canopy, like the main grass in screenshot
                    px = VoronoiTile(tx, ty,
                        cellBase:  new Color(0.22f, 0.55f, 0.20f),
                        cellLight: new Color(0.40f, 0.74f, 0.32f),
                        mortar:    new Color(0.08f, 0.24f, 0.06f),
                        numCells: 11, borderWidth: 1.8f, noiseStrength: 0.10f);
                    break;

                // ---- Special decoration tiles ----
                case "driftwood":
                    px = DrawDriftwood(tx, ty);
                    break;

                case "barrels":
                    px = DrawBarrels(tx, ty);
                    break;

                case "tent_red":
                    px = DrawTent(tx, ty,
                        new Color(0.80f, 0.18f, 0.14f),
                        new Color(0.52f, 0.10f, 0.08f));
                    break;

                case "tent_large":
                    px = DrawTent(tx, ty,
                        new Color(0.58f, 0.12f, 0.10f),
                        new Color(0.34f, 0.06f, 0.05f));
                    break;

                case "treasure_chest":
                    px = DrawTreasureChest(tx, ty, false);
                    break;

                case "treasure_chest_gold":
                    px = DrawTreasureChest(tx, ty, true);
                    break;

                case "dock_planks":
                    px = DrawDock(tx, ty, new Color(0.62f, 0.44f, 0.24f), false);
                    break;

                case "dock_secret":
                    px = DrawDock(tx, ty, new Color(0.34f, 0.24f, 0.12f), true);
                    break;

                default:
                    // Fallback: generic sandy cell
                    px = VoronoiTile(tx, ty,
                        new Color(0.88f, 0.76f, 0.50f),
                        new Color(0.98f, 0.90f, 0.70f),
                        new Color(0.54f, 0.44f, 0.26f));
                    break;
            }

            return MakeTex(px);
        }

        // ------------------------------------------------------------------
        // ANIMATED WATER
        // ------------------------------------------------------------------

        /// <summary>
        /// Water gets the Voronoi cell treatment too (matches the teal tiles
        /// in the top-right of the screenshot), but uses a time offset to
        /// gently shift the cell pattern each frame.
        /// </summary>
        private Texture2D DrawWater(int tx, int ty, float time, bool shallow)
        {
            // Colour palette: teal-blue (deep) or lighter teal (shallow)
            Color cellBase  = shallow ? new Color(0.34f, 0.68f, 0.78f) : new Color(0.18f, 0.46f, 0.70f);
            Color cellLight = shallow ? new Color(0.58f, 0.86f, 0.92f) : new Color(0.32f, 0.64f, 0.88f);
            Color mortar    = shallow ? new Color(0.10f, 0.36f, 0.52f) : new Color(0.06f, 0.22f, 0.44f);

            // Animate by shifting cell centres over time
            var rng   = new System.Random(tx * 73856093 ^ ty * 19349663);
            int  numCells = 10;
            int  total    = numCells * 9;
            var  ccx      = new float[total];
            var  ccy      = new float[total];
            int  idx      = 0;

            for (int oy = -1; oy <= 1; oy++)
            for (int ox = -1; ox <= 1; ox++)
            for (int i  = 0; i < numCells; i++)
            {
                float baseX = (float)rng.NextDouble() * TEX + ox * TEX;
                float baseY = (float)rng.NextDouble() * TEX + oy * TEX;
                // Each centre drifts slowly in a tiny ellipse
                float angle = (float)rng.NextDouble() * Mathf.PI * 2f;
                ccx[idx] = baseX + Mathf.Cos(angle + time) * 1.2f;
                ccy[idx] = baseY + Mathf.Sin(angle + time * 0.7f) * 0.8f;
                idx++;
            }

            var pixels = new Color[TEX * TEX];
            float bw   = 1.8f;

            for (int py = 0; py < TEX; py++)
            for (int px = 0; px < TEX; px++)
            {
                float d1 = float.MaxValue, d2 = float.MaxValue;
                for (int i = 0; i < total; i++)
                {
                    float ddx = px - ccx[i];
                    float ddy = py - ccy[i];
                    float d   = ddx*ddx + ddy*ddy;
                    if (d < d1)      { d2 = d1; d1 = d; }
                    else if (d < d2) { d2 = d; }
                }
                d1 = Mathf.Sqrt(d1); d2 = Mathf.Sqrt(d2);
                float edge = d2 - d1;

                Color c;
                if (edge < bw)
                {
                    float t = Mathf.Clamp01(edge / bw);
                    c = Color.Lerp(mortar, cellBase, t * t);
                }
                else
                {
                    float radius = TEX / Mathf.Sqrt(numCells) * 0.55f;
                    float t      = Mathf.Clamp01(d1 / radius);

                    // Subtle shimmer ripple on cell surface
                    float shimmer = Mathf.PerlinNoise(
                        px * 0.22f + tx * 0.3f + time * 0.6f,
                        py * 0.22f + ty * 0.3f + time * 0.4f) - 0.5f;

                    c = Color.Lerp(cellLight, cellBase, t);
                    c = new Color(
                        Mathf.Clamp01(c.r + shimmer * 0.08f),
                        Mathf.Clamp01(c.g + shimmer * 0.08f),
                        Mathf.Clamp01(c.b + shimmer * 0.10f), 1f);
                }

                int rim = Mathf.Min(px, py, TEX-1-px, TEX-1-py);
                if (rim == 0) c = Color.Lerp(c, mortar, 0.30f);

                pixels[py * TEX + px] = c;
            }

            return MakeTex(pixels);
        }

        // ==================================================================
        // STRUCTURE TILES (shape-masked, Voronoi base underneath)
        // ==================================================================

        // ------------------------------------------------------------------
        // DRIFTWOOD — plank-grain over a brownish Voronoi base
        // ------------------------------------------------------------------
        private Color[] DrawDriftwood(int tx, int ty)
        {
            // Start from a wood-coloured Voronoi base
            Color[] px = VoronoiTile(tx, ty,
                cellBase:  new Color(0.52f, 0.36f, 0.18f),
                cellLight: new Color(0.68f, 0.52f, 0.30f),
                mortar:    new Color(0.26f, 0.16f, 0.06f),
                numCells: 8, borderWidth: 2.0f, noiseStrength: 0.10f);

            // Overlay horizontal grain lines
            for (int y = 0; y < TEX; y++)
            for (int x = 0; x < TEX; x++)
            {
                float grain = Mathf.Sin(y * 1.1f + Mathf.PerlinNoise(x * 0.3f + tx, y * 0.1f + ty) * 2f);
                if (grain > 0.65f)
                {
                    int i = y * TEX + x;
                    px[i] = Color.Lerp(px[i], new Color(0.68f, 0.52f, 0.28f), 0.25f);
                }
            }
            return px;
        }

        // ------------------------------------------------------------------
        // BARRELS — circular shape over a wooden Voronoi cell base
        // ------------------------------------------------------------------
        private Color[] DrawBarrels(int tx, int ty)
        {
            Color[] px = VoronoiTile(tx, ty,
                cellBase:  new Color(0.42f, 0.28f, 0.12f),
                cellLight: new Color(0.58f, 0.42f, 0.22f),
                mortar:    new Color(0.20f, 0.12f, 0.04f),
                numCells: 8, borderWidth: 2.0f, noiseStrength: 0.08f);

            // Metal hoop bands (horizontal, every TEX/3 rows)
            Color hoop = new Color(0.35f, 0.35f, 0.38f);
            for (int y = 0; y < TEX; y++)
            for (int x = 0; x < TEX; x++)
            {
                if (y % (TEX / 3) <= 1) px[y * TEX + x] = hoop;
            }

            // Circular mask — transparent outside the barrel circle
            int cx = TEX / 2, cy = TEX / 2;
            for (int y = 0; y < TEX; y++)
            for (int x = 0; x < TEX; x++)
            {
                float dx = x - cx, dy = y - cy;
                if (dx*dx + dy*dy > 13f*13f)
                    px[y * TEX + x] = new Color(0,0,0,0);
            }
            return px;
        }

        // ------------------------------------------------------------------
        // TENT — triangle canvas shape over a fabric Voronoi base
        // ------------------------------------------------------------------
        private Color[] DrawTent(int tx, int ty, Color main, Color shadow)
        {
            // Fabric base — Voronoi cells in the tent colours
            Color[] px = VoronoiTile(tx, ty,
                cellBase:  main,
                cellLight: new Color(
                    Mathf.Min(1f, main.r + 0.22f),
                    Mathf.Min(1f, main.g + 0.10f),
                    Mathf.Min(1f, main.b + 0.10f)),
                mortar: shadow,
                numCells: 9, borderWidth: 1.6f, noiseStrength: 0.08f);

            // Tent triangle mask: apex at (cx, TEX-1), base across y=0
            // Pixels above the slope lines are transparent (sky)
            float cx2 = TEX / 2f;
            for (int y = 0; y < TEX; y++)
            for (int x = 0; x < TEX; x++)
            {
                float slope = Mathf.Abs(x - cx2) / cx2;
                float threshold = (1f - slope) * TEX * 0.60f;
                if (y > threshold)                       // above the canvas triangle
                    px[y * TEX + x] = new Color(0,0,0,0);
                else if (x >= cx2 - 1 && x <= cx2 + 1)  // centre pole
                    px[y * TEX + x] = new Color(0.42f, 0.28f, 0.10f);
            }
            return px;
        }

        // ------------------------------------------------------------------
        // TREASURE CHEST — box silhouette over a wooden Voronoi base
        // ------------------------------------------------------------------
        private Color[] DrawTreasureChest(int tx, int ty, bool gold)
        {
            Color woodBase  = gold ? new Color(0.64f, 0.44f, 0.12f) : new Color(0.46f, 0.30f, 0.12f);
            Color woodLight = gold ? new Color(0.84f, 0.66f, 0.30f) : new Color(0.62f, 0.46f, 0.26f);
            Color woodDark  = gold ? new Color(0.36f, 0.22f, 0.04f) : new Color(0.24f, 0.14f, 0.04f);
            Color metal     = gold ? new Color(1.00f, 0.84f, 0.10f) : new Color(0.50f, 0.50f, 0.54f);
            Color lockC     = new Color(0.90f, 0.78f, 0.10f);

            Color[] px = VoronoiTile(tx, ty,
                cellBase:  woodBase,
                cellLight: woodLight,
                mortar:    woodDark,
                numCells: 7, borderWidth: 2.2f, noiseStrength: 0.10f);

            // Chest silhouette: occupy x 5..26, y 7..25
            int x0 = 5, x1 = 26, y0 = 7, y1 = 25;
            int midY = (y0 + y1) / 2;

            for (int y = 0; y < TEX; y++)
            for (int x = 0; x < TEX; x++)
            {
                int i = y * TEX + x;
                // Outside chest bounds → transparent
                if (x < x0 || x > x1 || y < y0 || y > y1)
                { px[i] = new Color(0,0,0,0); continue; }

                // Metal corner bands
                bool onBorder = x <= x0+1 || x >= x1-1 || y <= y0+1 || y >= y1-1;
                if (onBorder) { px[i] = metal; continue; }

                // Lid divider
                if (y == midY || y == midY+1) { px[i] = metal; continue; }

                // Lock (centre of lid)
                bool inLid  = y > midY;
                int  lockCx = (x0 + x1) / 2;
                if (inLid && x >= lockCx-2 && x <= lockCx+2 && y >= midY+2 && y <= midY+5)
                { px[i] = lockC; continue; }

                // Lid face slightly lighter than base
                if (inLid) px[i] = Color.Lerp(px[i], woodLight, 0.18f);
            }
            return px;
        }

        // ------------------------------------------------------------------
        // DOCK PLANKS — horizontal planks with nail heads
        // ------------------------------------------------------------------
        private Color[] DrawDock(int tx, int ty, Color plank, bool worn)
        {
            Color dark = new Color(plank.r*0.55f, plank.g*0.55f, plank.b*0.55f);
            Color nail = new Color(0.38f, 0.38f, 0.40f);

            // Voronoi base gives the plank boards their chunky wood grain
            Color[] px = VoronoiTile(tx, ty,
                cellBase:  plank,
                cellLight: new Color(
                    Mathf.Min(1f, plank.r + 0.18f),
                    Mathf.Min(1f, plank.g + 0.12f),
                    Mathf.Min(1f, plank.b + 0.08f)),
                mortar: dark,
                numCells: 6, borderWidth: 3.0f, noiseStrength: 0.08f);

            // Plank gap lines every 6px horizontally
            Color gap = new Color(dark.r*0.7f, dark.g*0.7f, dark.b*0.7f);
            for (int y = 0; y < TEX; y++)
            for (int x = 0; x < TEX; x++)
            {
                if (y % 6 < 2) px[y * TEX + x] = gap;
            }

            // Nail heads at crossing points
            for (int y = 3; y < TEX; y += 6)
            for (int x = 4; x < TEX; x += 8)
            {
                if (x < TEX && y < TEX) px[y * TEX + x] = nail;
            }

            // Extra worn patches
            if (worn)
            {
                var rng = new System.Random(tx * 7 + ty * 13);
                for (int y = 0; y < TEX; y++)
                for (int x = 0; x < TEX; x++)
                {
                    if (rng.NextDouble() < 0.04f)
                        px[y * TEX + x] = Color.Lerp(px[y*TEX+x], gap, 0.45f);
                }
            }

            return px;
        }

        // ------------------------------------------------------------------
        // Texture factory
        // ------------------------------------------------------------------

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
