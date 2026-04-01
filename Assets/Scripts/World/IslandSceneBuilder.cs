using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    // =========================================================================
    // TileSpriteEntry — one row in the Inspector sprite override table
    // =========================================================================

    [System.Serializable]
    public class TileSpriteEntry
    {
        [Tooltip("Visual tag string exactly matching one used in SmugglersIslandLevel.")]
        public string visualTag;

        [Tooltip("Assign a sprite here to override the procedural art for this tag.")]
        public Sprite sprite;
    }

    // =========================================================================
    // IslandSceneBuilder — MonoBehaviour (Tilemap version)
    // =========================================================================

    /// <summary>
    /// Attach to any empty GameObject in the SmugglersIsland scene.
    ///
    /// Uses Unity's Tilemap system for performance — all tiles are batched into
    /// a single renderer instead of thousands of individual GameObjects.
    ///
    /// Scene setup required (do once):
    ///   1. Create a Grid GameObject (GameObject -> 2D Object -> Tilemap)
    ///      This auto-creates a Grid with one child Tilemap.
    ///   2. Rename that child Tilemap to "Ground"
    ///   3. Add two more child Tilemaps to the Grid: "Obstacles" and "Hazards"
    ///   4. On the "Obstacles" Tilemap, add a TilemapCollider2D component
    ///      and a CompositeCollider2D — set Rigidbody2D Body Type to Static
    ///   5. On the "Hazards" Tilemap, add a TilemapCollider2D set to trigger
    ///   6. Drag all three Tilemap references into this script's Inspector fields
    ///   7. Press Play — the map renders instantly with no per-tile overhead
    ///
    /// Sprite assignment:
    ///   Populate Sprite Entries in the Inspector to use real art.
    ///   Any unmapped tag uses procedural Voronoi pixel art as fallback.
    /// </summary>
    public class IslandSceneBuilder : MonoBehaviour
    {
        [Header("Tilemap References (drag from Hierarchy)")]
        [Tooltip("The ground layer tilemap — sand, water, paths, jungle floor.")]
        public Tilemap groundTilemap;

        [Tooltip("The obstacle layer tilemap — rocks, walls, trees, tents.")]
        public Tilemap obstacleTilemap;

        [Tooltip("The hazard layer tilemap — quicksand. Needs TilemapCollider2D as trigger.")]
        public Tilemap hazardTilemap;

        [Header("Sprite Overrides (leave blank to use procedural art)")]
        [SerializeField] private TileSpriteEntry[] spriteEntries;

        [Header("Hazard Settings")]
        [Tooltip("HP dealt per tick when player stands on a hazard tile.")]
        [SerializeField] private int hazardDamagePerTick = 5;
        [SerializeField] private float hazardDamageInterval = 0.5f;

        // Sprite lookup
        private Dictionary<string, Sprite> _overrides;
        private Dictionary<string, Sprite> _cache;

        // Tile asset cache — one TileBase per visual tag
        private Dictionary<string, TileBase> _tileAssets;

        [SerializeField] private string levelType;

        private Level _level;

        private const int TEX = 32;

        void Start()
        {
            _overrides  = new Dictionary<string, Sprite>();
            _cache      = new Dictionary<string, Sprite>();
            _tileAssets = new Dictionary<string, TileBase>();

            // Load sprite overrides from Inspector
            if (spriteEntries != null)
                foreach (var e in spriteEntries)
                    if (e?.sprite != null && !string.IsNullOrEmpty(e.visualTag))
                        _overrides[e.visualTag] = e.sprite;

            // Validate tilemap references
            if (groundTilemap == null || obstacleTilemap == null || hazardTilemap == null)
            {
                Debug.LogError("IslandSceneBuilder: One or more Tilemap references are missing. " +
                               "Drag Ground, Obstacles, and Hazards tilemaps into the Inspector.");
                return;
            }

            // Build level data
            if (levelType == "jungle")
            {
                _level = new JungleRuinsIsland();
            }
            else
            {
                _level = new SmugglersIslandLevel();
            }
            _level.Initialize();

            // Paint all tiles
            PaintMap(_level.MapLayout);

            // Add hazard detector (single component, not per-tile)
            AddHazardDetector();

            Debug.Log($"IslandSceneBuilder: Map painted via Tilemap — {_level.LevelName}.");
        }

        // ------------------------------------------------------------------
        // Map painting
        // ------------------------------------------------------------------

        private void PaintMap(TileMap map)
        {
            for (int x = 0; x < map.Width;  x++)
            for (int y = 0; y < map.Height; y++)
            {
                Tile tile = map.GetTile(x, y);
                if (tile == null) continue;

                TileBase tileAsset = GetTileAsset(tile.VisualTag);
                Vector3Int pos     = new Vector3Int(x, y, 0);

                switch (tile.TileType)
                {
                    case TileType.Obstacle:
                    case TileType.Water:
                        obstacleTilemap.SetTile(pos, tileAsset);
                        break;

                    case TileType.Hazard:
                        // Paint ground underneath hazard so it doesn't look empty
                        groundTilemap.SetTile(pos, GetTileAsset("sand"));
                        hazardTilemap.SetTile(pos, tileAsset);
                        break;

                    default:
                        // Walkable, NPCSpawn, TreasureSpot, Exit all go on ground layer
                        groundTilemap.SetTile(pos, tileAsset);
                        break;
                }
            }

            // Refresh all tilemaps after bulk painting
            groundTilemap.RefreshAllTiles();
            obstacleTilemap.RefreshAllTiles();
            hazardTilemap.RefreshAllTiles();
        }

        // ------------------------------------------------------------------
        // Hazard detector — one trigger on the hazard tilemap
        // ------------------------------------------------------------------

        private void AddHazardDetector()
        {
            // HazardZone is added to the hazard Tilemap GameObject itself.
            // The TilemapCollider2D (trigger) on that object fires OnTriggerEnter2D.
            HazardZone hz = hazardTilemap.gameObject.GetComponent<HazardZone>();
            if (hz == null)
                hz = hazardTilemap.gameObject.AddComponent<HazardZone>();

            hz.SetDamagePerTick(hazardDamagePerTick);
        }

        // ------------------------------------------------------------------
        // Tile asset resolution
        // ------------------------------------------------------------------

        private TileBase GetTileAsset(string tag)
        {
            if (_tileAssets.TryGetValue(tag, out TileBase existing))
                return existing;

            // Create a Unity Tile asset with the resolved sprite
            var tile    = ScriptableObject.CreateInstance<UnityEngine.Tilemaps.Tile>();
            tile.sprite = GetSprite(tag);
            tile.flags  = TileFlags.LockColor;

            _tileAssets[tag] = tile;
            return tile;
        }

        private Sprite GetSprite(string tag)
        {
            if (_overrides.TryGetValue(tag, out Sprite ov)) return ov;

            if (!_cache.TryGetValue(tag, out Sprite sp))
            {
                sp = SpriteFrom(GenerateTexture(tag, 0, 0));
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
        // VORONOI CELL TILE GENERATOR (unchanged from previous version)
        // ==================================================================

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
                    if (d < d1)      { d2 = d1; d1 = d; }
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

                int rim = Mathf.Min(px, py, TEX-1-px, TEX-1-py);
                if (rim == 0) c = Color.Lerp(c, mortar, 0.35f);
                pixels[py * TEX + px] = c;
            }
            return pixels;
        }

        private Texture2D GenerateTexture(string tag, int tx, int ty)
        {
            Color[] px;
            switch (tag)
            {
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
                case "quicksand":
                    px = VoronoiTile(tx, ty,
                        new Color(0.74f,0.62f,0.20f), new Color(0.88f,0.78f,0.38f),
                        new Color(0.38f,0.28f,0.06f), 9, 2.8f, 0.12f); break;
                case "water": case "water_shallow":
                    px = VoronoiTile(tx, ty,
                        new Color(0.18f,0.46f,0.70f), new Color(0.32f,0.64f,0.88f),
                        new Color(0.06f,0.22f,0.44f), 10, 1.8f, 0.10f); break;
                case "rock_tide":
                    px = VoronoiTile(tx, ty,
                        new Color(0.54f,0.52f,0.50f), new Color(0.72f,0.70f,0.68f),
                        new Color(0.24f,0.22f,0.20f), 9, 2.2f, 0.10f); break;
                case "camp_wall":
                    px = VoronoiTile(tx, ty,
                        new Color(0.58f,0.54f,0.48f), new Color(0.74f,0.70f,0.64f),
                        new Color(0.22f,0.20f,0.18f), 8, 2.4f, 0.08f); break;
                case "camp_wall_dark":
                    px = VoronoiTile(tx, ty,
                        new Color(0.36f,0.33f,0.29f), new Color(0.50f,0.47f,0.42f),
                        new Color(0.12f,0.10f,0.08f), 8, 2.6f, 0.07f); break;
                case "jungle_tree":
                    px = VoronoiTile(tx, ty,
                        new Color(0.10f,0.42f,0.10f), new Color(0.26f,0.62f,0.18f),
                        new Color(0.04f,0.16f,0.04f), 12, 1.6f, 0.12f); break;
                case "jungle_floor":
                    px = VoronoiTile(tx, ty,
                        new Color(0.22f,0.55f,0.20f), new Color(0.40f,0.74f,0.32f),
                        new Color(0.08f,0.24f,0.06f), 11, 1.8f, 0.10f); break;
                case "driftwood":
                    px = VoronoiTile(tx, ty,
                        new Color(0.52f,0.36f,0.18f), new Color(0.68f,0.52f,0.30f),
                        new Color(0.26f,0.16f,0.06f), 8, 2.0f, 0.10f); break;
                case "barrels":
                    px = VoronoiTile(tx, ty,
                        new Color(0.42f,0.28f,0.12f), new Color(0.58f,0.42f,0.22f),
                        new Color(0.20f,0.12f,0.04f), 8, 2.0f, 0.08f); break;
                case "tent_red": case "tent_large":
                    px = VoronoiTile(tx, ty,
                        new Color(0.76f,0.18f,0.14f), new Color(0.92f,0.40f,0.30f),
                        new Color(0.40f,0.08f,0.06f), 9, 1.6f, 0.08f); break;
                case "treasure_chest": case "treasure_chest_gold":
                    px = VoronoiTile(tx, ty,
                        new Color(0.64f,0.44f,0.12f), new Color(0.84f,0.66f,0.30f),
                        new Color(0.36f,0.22f,0.04f), 7, 2.2f, 0.10f); break;
                case "dock_planks": case "dock_secret":
                    px = VoronoiTile(tx, ty,
                        new Color(0.62f,0.44f,0.24f), new Color(0.78f,0.60f,0.38f),
                        new Color(0.30f,0.18f,0.08f), 6, 3.0f, 0.08f); break;
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
