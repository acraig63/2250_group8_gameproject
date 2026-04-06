using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    public class BlackwaterSceneBuilder : MonoBehaviour
    {
        [Header("Tilemap References (drag from Hierarchy)")]
        public Tilemap groundTilemap;
        public Tilemap wallTilemap;

        private Dictionary<string, TileBase> _tileAssets;
        private Dictionary<string, Sprite>   _spriteCache;
        private const int TEX = 32;

        private static float _originalSpeed = -1f;

        private const int FLAGSHIP_W  = 90;
        private const int FLAGSHIP_H  = 90;
        private const int LOWERDECK_W = 80;
        private const int LOWERDECK_H = 80;

        void Awake()
        {
            string awakeScene = SceneManager.GetActiveScene().name;
            if (awakeScene.StartsWith("Blackwater"))
                Level5NPCSetup.HandlePostBattleReward(awakeScene);
        }

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
            SetupWallCollision();

            groundTilemap.RefreshAllTiles();
            wallTilemap.RefreshAllTiles();

            string scene = SceneManager.GetActiveScene().name;
            switch (scene)
            {
                case "BlackwaterFlagship":
                    SetupCameraBounds(FLAGSHIP_W, FLAGSHIP_H);
                    SetupMinimap(40f);
                    break;
                case "BlackwaterLowerDeck":
                    SetupCameraBounds(LOWERDECK_W, LOWERDECK_H);
                    SetupMinimap(40f);
                    break;
                case "BlackwaterArmory":
                case "BlackwaterMessHall":
                    SetupCameraBounds(20f, 15f);
                    SetupMinimap(10f);
                    break;
                case "BlackwaterBrig":
                case "BlackwaterNavigationRoom":
                    SetupCameraBounds(15f, 15f);
                    SetupMinimap(8f);
                    break;
                case "BlackwaterCaptainsQuarters":
                    SetupCameraBounds(25f, 20f);
                    SetupMinimap(13f);
                    break;
            }

            Debug.Log($"[BlackwaterSceneBuilder] Built {scene}");

            Level5HealthBar.EnsureExists();
            Level5InventoryBridge.EnsureInventoryExists();

            Level5NPCSetup.SetupRoomNPCs(scene);
            if (scene == "BlackwaterFlagship")
                Level5NPCSetup.SpawnDeckItems();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                PlayerController pcSearch = Object.FindObjectOfType<PlayerController>();
                if (pcSearch != null) player = pcSearch.gameObject;
            }
            if (player != null)
                PlayerHazardShield.AttachToPlayer(player);
            else
                Debug.LogWarning("[BlackwaterSceneBuilder] Player not found — PlayerHazardShield not attached.");

            Level5EquipManagerMono.EnsureExists();

            if (player != null)
            {
                PlayerController pc = player.GetComponent<PlayerController>();
                if (pc != null)
                {
                    if (_originalSpeed < 0f) _originalSpeed = pc.speed;
                    pc.speed = BlackwaterState.hasSpeedBoots ? _originalSpeed * 1.5f : _originalSpeed;
                }
            }

            if (scene.StartsWith("Blackwater") && player != null)
            {
                if (BlackwaterState.hasSavedState)
                    BlackwaterState.LoadPlayerState();
                BlackwaterState.SavePlayerState();
            }

            if (scene == "BlackwaterCaptainsQuarters")
            {
                GameObject boss = GameObject.Find("Captain Blackwater");
                if (boss != null && boss.GetComponent<BossHazardWave>() == null)
                    boss.AddComponent<BossHazardWave>();
            }

            Level5DeathHandler.EnsureExists();
            Level5ItemDropHandler.EnsureExists();
        }

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

        private GameObject CreateWall(float x, float y, float width, float height)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            Sprite spr = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

            GameObject wall = new GameObject("MazeWall");
            wall.transform.position   = new Vector3(x, y, 0f);
            wall.transform.localScale = new Vector3(width, height, 1f);

            SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
            sr.sprite       = spr;
            sr.color        = new Color(0.3f, 0.25f, 0.2f);   // dark brown
            sr.sortingOrder = 2;

            Rigidbody2D rb = wall.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;

            wall.AddComponent<BoxCollider2D>();
            return wall;
        }

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

        private void BuildFlagship()
        {
            Portal.ApplyPendingSpawn();

            FillRect(groundTilemap, 0, 0, FLAGSHIP_W-1, FLAGSHIP_H-1, "water");

            // quarterdeck
            PaintDeckRow(7,  12, 33, 56, "ship_deck_aft");
            PaintDeckRow(13, 18, 31, 58, "ship_deck_aft");
            PaintDeckRow(19, 24, 29, 60, "ship_deck_aft");

            // main deck
            PaintDeckRow(25, 30, 27, 62, "ship_deck");
            PaintDeckRow(31, 55, 25, 64, "ship_deck");
            PaintDeckRow(56, 60, 27, 62, "ship_deck");

            // forecastle
            PaintDeckRow(61, 65, 29, 60, "ship_deck_fore");
            PaintDeckRow(66, 70, 31, 58, "ship_deck_fore");
            PaintDeckRow(71, 74, 33, 56, "ship_deck_fore");
            PaintDeckRow(75, 78, 36, 53, "ship_deck_fore");
            PaintDeckRow(79, 80, 39, 50, "ship_deck_fore");
            PaintDeckRow(81, 82, 42, 47, "ship_deck_fore");

            PaintHullPerimeterAuto();

            // deck zone trim strips
            for (int x = 27; x <= 62; x++)
                groundTilemap.SetTile(new Vector3Int(x, 25, 0), GetTileAsset("dock_planks"));
            for (int x = 27; x <= 62; x++)
                groundTilemap.SetTile(new Vector3Int(x, 60, 0), GetTileAsset("dock_planks"));

            // mast bases
            PlaceRect(wallTilemap, 43, 26, 45, 28, "mast_base"); // Mizzenmast
            PlaceRect(wallTilemap, 43, 41, 45, 43, "mast_base"); // Mainmast
            PlaceRect(wallTilemap, 43, 52, 45, 54, "mast_base"); // Foremast

            PlaceObstacles("barrels", new int[]
            {
                34, 22,  35, 22,
                54, 22,  55, 22,
                29, 35,  30, 35,
                58, 35,  59, 35,
                29, 47,  30, 47,
                58, 47,  59, 47,
                36, 63,  37, 63,
                51, 63,  52, 63,
            });

            PlaceRect(groundTilemap, 43, 11, 45, 13, "dock_planks");

            // hatches / portals
            wallTilemap.SetTile(new Vector3Int(44, 12, 0), null);
            groundTilemap.SetTile(new Vector3Int(44, 12, 0), GetTileAsset("door_marker"));
            LockedPortal.Create(new Vector3(44.5f, 12.5f, 0f),
                                "BlackwaterCaptainsQuarters", new Vector2(12f, 10f));

            PlacePortal("Brig",       32, 35, "BlackwaterBrig",
                        new Vector2(7f, 7f), "door_marker");
            PlacePortal("LowerDeck",  44, 38, "BlackwaterLowerDeck",
                        new Vector2(39f, 40f), "door_marker");
            PlacePortal("Armory",     32, 46, "BlackwaterArmory",
                        new Vector2(10f, 7f), "door_marker");
            PlacePortal("MessHall",   57, 46, "BlackwaterMessHall",
                        new Vector2(10f, 7f), "door_marker");
            PlacePortal("Navigation", 44, 65, "BlackwaterNavigationRoom",
                        new Vector2(7f, 7f), "door_marker");
        }

        private void PaintDeckRow(int yMin, int yMax, int xMin, int xMax, string deckTag)
        {
            TileBase t = GetTileAsset(deckTag);
            for (int y = yMin; y <= yMax; y++)
                for (int x = xMin; x <= xMax; x++)
                    groundTilemap.SetTile(new Vector3Int(x, y, 0), t);
        }

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

        private void BuildLowerDeck()
        {
            Portal.ApplyPendingSpawn();

            FillRect(groundTilemap, 0, 0, LOWERDECK_W-1, LOWERDECK_H-1, "hold_void");
            FillRect(wallTilemap,   0, 0, LOWERDECK_W-1, LOWERDECK_H-1, "hold_void");

            BuildLowerDeckHull();

            // support beam pillars
            PlaceObstaclesIf("support_beam", new int[]
            {
                28, 37,  50, 37,
                28, 43,  50, 43,
                28, 22,  50, 22,
                28, 50,  50, 50,
            });

            PlaceObstaclesIf("barrels", new int[]
            {
                25, 33,  26, 33,  52, 33,  53, 33,
                25, 48,  26, 48,  52, 48,  53, 48,
                34, 63,  35, 63,  43, 63,  44, 63,
            });

            PlacePortal("Flagship", 39, 38, "BlackwaterFlagship",
                        new Vector2(44f, 36f), "door_marker");

            // maze walls (vertical)
            CreateWall(29f, 41.5f, 1.5f, 4f);
            CreateWall(39f, 45.5f, 1.5f, 4f);
            CreateWall(39f, 49.5f, 1.5f, 4f);
            CreateWall(44f, 37.5f, 1.5f, 4f);
            CreateWall(44f, 45.5f, 1.5f, 4f);
            CreateWall(44f, 49.5f, 1.5f, 4f);
            CreateWall(49f, 49.5f, 1.5f, 4f);

            // maze walls (horizontal)
            CreateWall(31f, 40f, 5f, 1.5f);
            CreateWall(36f, 40f, 5f, 1.5f);
            CreateWall(41f, 40f, 5f, 1.5f);
            CreateWall(51f, 40f, 5f, 1.5f);
            CreateWall(31f, 44f, 5f, 1.5f);
            CreateWall(36f, 44f, 5f, 1.5f);
            CreateWall(46f, 44f, 5f, 1.5f);
            CreateWall(31f, 48f, 5f, 1.5f);
            CreateWall(36f, 48f, 5f, 1.5f);
            CreateWall(46f, 48f, 5f, 1.5f);
            CreateWall(31f, 52f, 5f, 1.5f);
            CreateWall(36f, 52f, 5f, 1.5f);
            CreateWall(41f, 52f, 5f, 1.5f);

            // maze-to-gauntlet separator
            CreateWall(30.5f, 35f, 17f, 1.5f);
            CreateWall(47.5f, 35f, 17f, 1.5f);

            MazeKeyDoor.Create(new Vector3(39f, 35f, 0f));

            // lava tiles every 2 units
            for (int gx = 24; gx <= 54; gx += 2)
                GauntletLavaHazard.Create(new Vector3(gx, 33f, 0f));

            GauntletReward.Create(new Vector3(55f, 33f, 0f));
        }

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
            PaintLowerSection( 7, 12, 31, 48);
            PaintLowerSection(13, 18, 29, 50);
            PaintLowerSection(19, 24, 27, 52);
            PaintLowerSection(25, 30, 25, 54);
            PaintLowerSection(31, 55, 23, 56);
            PaintLowerSection(56, 60, 25, 54);
            PaintLowerSection(61, 65, 27, 52);
            PaintLowerSection(66, 70, 29, 50);
            PaintLowerSection(71, 74, 31, 48);
            PaintLowerDeckPerimeter();
        }

        private void PaintLowerSection(int yMin, int yMax, int xMin, int xMax)
        {
            TileBase floor = GetTileAsset("ship_deck_dark");
            for (int y = yMin; y <= yMax; y++)
            for (int x = xMin; x <= xMax; x++)
            {
                groundTilemap.SetTile(new Vector3Int(x, y, 0), floor);
                wallTilemap.SetTile(new Vector3Int(x, y, 0), null);
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

        private void BuildArmory()
        {
            Portal.ApplyPendingSpawn();
            const int W = 20, H = 15;

            FillRect(groundTilemap, 0, 0, W-1, H-1, "room_floor");
            FillBorder(wallTilemap, 0, 0, W-1, H-1, 2, "room_wall");

            PlaceObstacles("barrels", new int[] { 3,4, 4,4, 3,9, 4,9, 15,4, 15,9 });
            PlacePortal("FlagshipArmory", 10, 13, "BlackwaterFlagship",
                        new Vector2(32f, 44f), "door_marker");
        }

        private void BuildMessHall()
        {
            Portal.ApplyPendingSpawn();
            const int W = 20, H = 15;

            FillRect(groundTilemap, 0, 0, W-1, H-1, "sand_path");
            FillBorder(wallTilemap, 0, 0, W-1, H-1, 2, "room_wall");

            PlaceObstacles("barrels", new int[] { 4,4, 5,4, 14,4, 15,4, 4,9, 5,9 });
            PlacePortal("FlagshipMess", 10, 13, "BlackwaterFlagship",
                        new Vector2(57f, 44f), "door_marker");
        }

        private void BuildBrig()
        {
            Portal.ApplyPendingSpawn();
            const int W = 15, H = 15;

            FillRect(groundTilemap, 0, 0, W-1, H-1, "camp_wall_dark");
            FillBorder(wallTilemap, 0, 0, W-1, H-1, 2, "room_wall");

            PlaceObstacles("room_wall", new int[] { 4,5, 9,5, 4,9, 9,9 });
            PlacePortal("FlagshipBrig", 7, 13, "BlackwaterFlagship",
                        new Vector2(32f, 33f), "door_marker");
        }

        private void BuildNavigationRoom()
        {
            Portal.ApplyPendingSpawn();
            const int W = 15, H = 15;

            FillRect(groundTilemap, 0, 0, W-1, H-1, "sand_wet");
            FillBorder(wallTilemap, 0, 0, W-1, H-1, 2, "room_wall");

            PlaceObstacles("dock_planks", new int[] { 4,5, 5,5, 9,5, 10,5, 4,9, 9,9 });
            PlacePortal("FlagshipNav", 7, 13, "BlackwaterFlagship",
                        new Vector2(44f, 63f), "door_marker");
        }

        private void BuildCaptainsQuarters()
        {
            Portal.ApplyPendingSpawn();
            const int W = 25, H = 20;

            FillRect(groundTilemap, 0, 0, W-1, H-1, "dock_planks");
            FillBorder(wallTilemap, 0, 0, W-1, H-1, 2, "camp_wall");

            PlaceRect(groundTilemap, 11, 14, 13, 16, "treasure_chest_gold");
            PlaceObstacles("barrels", new int[] { 3,4, 4,4, 20,4, 21,4, 3,13, 20,13 });
            PlacePortal("FlagshipCaptains", 12, 18, "BlackwaterFlagship",
                        new Vector2(44f, 14f), "door_marker");
        }

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

        private void PlaceRect(Tilemap tm, int x0, int y0, int x1, int y1, string tag)
        {
            TileBase t = GetTileAsset(tag);
            for (int x = x0; x <= x1; x++)
                for (int y = y0; y <= y1; y++)
                    tm.SetTile(new Vector3Int(x, y, 0), t);
        }

        private void PlaceObstacles(string tag, int[] coords)
        {
            TileBase t = GetTileAsset(tag);
            for (int i = 0; i < coords.Length - 1; i += 2)
                wallTilemap.SetTile(new Vector3Int(coords[i], coords[i+1], 0), t);
        }

        private void PlaceObstaclesIf(string tag, int[] coords)
        {
            TileBase t = GetTileAsset(tag);
            for (int i = 0; i < coords.Length - 1; i += 2)
                if (IsLowerDeckInterior(coords[i], coords[i+1]))
                    wallTilemap.SetTile(new Vector3Int(coords[i], coords[i+1], 0), t);
        }

        private void PlacePortal(string id, int tx, int ty,
                                  string targetScene, Vector2 spawnPos,
                                  string markerTag)
        {
            var pos = new Vector3Int(tx, ty, 0);
            wallTilemap.SetTile(pos, null);
            groundTilemap.SetTile(pos, GetTileAsset(markerTag));

            var go = new GameObject("Portal_" + id);
            go.transform.position = new Vector3(tx + 0.5f, ty + 0.5f, 0f);

            var rb2d      = go.AddComponent<Rigidbody2D>();
            rb2d.bodyType = RigidbodyType2D.Kinematic;

            var col       = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size      = new Vector2(1f, 1f);

            var portal           = go.AddComponent<Portal>();
            portal.targetScene   = targetScene;
            portal.spawnPosition = spawnPos;
        }

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

        private Texture2D GenerateTexture(string tag, int tx, int ty)
        {
            Color[] px;
            switch (tag)
            {
                case "water": case "water_shallow":
                    px = VoronoiTile(tx, ty,
                        new Color(0.18f,0.46f,0.70f), new Color(0.32f,0.64f,0.88f),
                        new Color(0.06f,0.22f,0.44f), 10, 1.8f, 0.10f); break;
                case "ship_deck":
                    px = VoronoiTile(tx, ty,
                        new Color(0.55f,0.35f,0.15f), new Color(0.72f,0.52f,0.28f),
                        new Color(0.30f,0.18f,0.06f), 8, 2.5f, 0.09f); break;
                case "ship_deck_aft":
                    px = VoronoiTile(tx, ty,
                        new Color(0.62f,0.38f,0.12f), new Color(0.80f,0.58f,0.28f),
                        new Color(0.34f,0.18f,0.04f), 8, 2.6f, 0.09f); break;
                case "ship_deck_fore":
                    px = VoronoiTile(tx, ty,
                        new Color(0.68f,0.52f,0.28f), new Color(0.84f,0.70f,0.46f),
                        new Color(0.38f,0.26f,0.10f), 8, 2.4f, 0.09f); break;
                case "ship_deck_dark":
                    px = VoronoiTile(tx, ty,
                        new Color(0.38f,0.22f,0.08f), new Color(0.52f,0.34f,0.14f),
                        new Color(0.18f,0.10f,0.02f), 8, 2.5f, 0.09f); break;
                case "ship_rail":
                    px = VoronoiTile(tx, ty,
                        new Color(0.12f,0.10f,0.22f), new Color(0.22f,0.20f,0.35f),
                        new Color(0.04f,0.04f,0.08f), 8, 2.0f, 0.06f); break;
                case "dock_planks": case "dock_secret":
                    px = VoronoiTile(tx, ty,
                        new Color(0.62f,0.44f,0.24f), new Color(0.78f,0.60f,0.38f),
                        new Color(0.30f,0.18f,0.08f), 6, 3.0f, 0.08f); break;
                case "mast_base":
                    px = VoronoiTile(tx, ty,
                        new Color(0.18f,0.10f,0.04f), new Color(0.28f,0.18f,0.08f),
                        new Color(0.06f,0.03f,0.01f), 6, 2.8f, 0.05f); break;
                case "room_floor":
                    px = VoronoiTile(tx, ty,
                        new Color(0.58f,0.52f,0.44f), new Color(0.74f,0.68f,0.58f),
                        new Color(0.28f,0.24f,0.18f), 9, 2.2f, 0.08f); break;
                case "room_wall":
                    px = VoronoiTile(tx, ty,
                        new Color(0.44f,0.44f,0.44f), new Color(0.62f,0.62f,0.62f),
                        new Color(0.18f,0.18f,0.18f), 8, 2.4f, 0.07f); break;
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
                case "treasure_chest": case "treasure_chest_gold":
                    px = VoronoiTile(tx, ty,
                        new Color(0.64f,0.44f,0.12f), new Color(0.84f,0.66f,0.30f),
                        new Color(0.36f,0.22f,0.04f), 7, 2.2f, 0.10f); break;
                case "hatch_marker":
                    px = VoronoiTile(tx, ty,
                        new Color(0.10f,0.65f,0.75f), new Color(0.25f,0.82f,0.90f),
                        new Color(0.04f,0.28f,0.35f), 7, 2.0f, 0.08f); break;
                case "door_marker":
                    px = VoronoiTile(tx, ty,
                        new Color(0.90f,0.65f,0.10f), new Color(1.00f,0.85f,0.30f),
                        new Color(0.50f,0.30f,0.02f), 7, 2.0f, 0.08f); break;
                case "door_captains":
                    px = VoronoiTile(tx, ty,
                        new Color(0.85f,0.20f,0.10f), new Color(1.00f,0.45f,0.30f),
                        new Color(0.40f,0.05f,0.02f), 7, 2.0f, 0.08f); break;
                case "hold_void":
                    px = VoronoiTile(tx, ty,
                        new Color(0.08f,0.04f,0.01f), new Color(0.14f,0.08f,0.03f),
                        new Color(0.02f,0.01f,0.00f), 8, 2.0f, 0.03f); break;
                case "hold_wall":
                    px = VoronoiTile(tx, ty,
                        new Color(0.28f,0.20f,0.12f), new Color(0.38f,0.28f,0.18f),
                        new Color(0.14f,0.08f,0.04f), 8, 2.2f, 0.07f); break;
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
