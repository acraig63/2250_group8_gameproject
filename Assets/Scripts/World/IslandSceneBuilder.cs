using UnityEngine;

/// <summary>
/// Attach this to an empty GameObject called "IslandSceneBuilder" in your scene.
/// On Play it procedurally builds the Smuggler's Island layout using colored sprites —
/// no tile assets or external packages required.
///
/// What gets built:
///   - Ocean background (solid blue camera background)
///   - Island ground plane (tan — walkable, no collider)
///   - Rock obstacles around the camp (dark gray, BoxCollider2D — blocks movement)
///   - A storm hazard zone (red-tinted, trigger — damages player on contact)
///   - A dock exit marker (yellow — visual only for now)
/// </summary>
public class IslandSceneBuilder : MonoBehaviour
{
    [Header("Island Size")]
    public int islandWidth  = 20;
    public int islandHeight = 20;
    public float tileSize   = 1f;

    [Header("Colors")]
    public Color oceanColor    = new Color(0.10f, 0.30f, 0.70f, 1f);  // deep blue
    public Color groundColor   = new Color(0.76f, 0.70f, 0.50f, 1f);  // sandy tan
    public Color obstacleColor = new Color(0.30f, 0.30f, 0.30f, 1f);  // dark rock
    public Color hazardColor   = new Color(0.80f, 0.15f, 0.15f, 0.5f);// red tint
    public Color dockColor     = new Color(0.90f, 0.80f, 0.20f, 1f);  // yellow

    // -----------------------------------------------------------------------
    // Tile coordinate data — mirrors SmugglersIslandLevel layout
    // -----------------------------------------------------------------------

    // Rock obstacles (impassable)
    private static readonly int[,] RockTiles =
    {
        {5,5},{5,6},{6,5},{6,6},      // NW camp cluster
        {13,5},{14,5},{13,6},          // NE camp cluster
        {5,13},{5,14},{6,13},          // SW camp cluster
        {8,8},{9,8},{8,9}              // Central wall fragment
    };

    // Hazard zone — storm area (damages player)
    private static readonly int[,] HazardTiles =
    {
        {15,12},{16,12},{15,13},{16,13},{15,14},{16,14}
    };

    // Exit dock position
    private static readonly int[] DockTile = { 10, 18 };

    void Start()
    {
        SetOceanBackground();
        BuildGround();
        BuildWaterBorder();
        BuildRockObstacles();
        BuildHazardZone();
        BuildDock();
    }

    // -----------------------------------------------------------------------
    // Ocean background — just set the camera clear color
    // -----------------------------------------------------------------------
    private void SetOceanBackground()
    {
        Camera.main.backgroundColor = oceanColor;
        Camera.main.clearFlags      = CameraClearFlags.SolidColor;
    }

    // -----------------------------------------------------------------------
    // Island ground — one large sandy quad, no collider (player walks on it)
    // -----------------------------------------------------------------------
    private void BuildGround()
    {
        // Slightly inset from the water border
        float w = (islandWidth  - 2) * tileSize;
        float h = (islandHeight - 2) * tileSize;
        float cx = (islandWidth  / 2f - 0.5f) * tileSize;
        float cy = (islandHeight / 2f - 0.5f) * tileSize;

        GameObject ground = CreateColoredQuad("IslandGround", groundColor, new Vector3(cx, cy, 1f), w, h);
        ground.transform.SetParent(transform);
    }

    // -----------------------------------------------------------------------
    // Water border — thin blue quads on all four edges with colliders
    // so the player can't walk off the island
    // -----------------------------------------------------------------------
    private void BuildWaterBorder()
    {
        float w = islandWidth  * tileSize;
        float h = islandHeight * tileSize;
        float cx = (islandWidth  / 2f - 0.5f) * tileSize;
        float cy = (islandHeight / 2f - 0.5f) * tileSize;

        // Bottom, Top, Left, Right border strips
        CreateBorderWall("Border_Bottom", new Vector3(cx,  -0.5f * tileSize, 0f), w,  tileSize);
        CreateBorderWall("Border_Top",    new Vector3(cx, (islandHeight - 0.5f) * tileSize, 0f), w, tileSize);
        CreateBorderWall("Border_Left",   new Vector3(-0.5f * tileSize, cy, 0f), tileSize, h);
        CreateBorderWall("Border_Right",  new Vector3((islandWidth - 0.5f) * tileSize, cy, 0f), tileSize, h);
    }

    private void CreateBorderWall(string wallName, Vector3 pos, float w, float h)
    {
        GameObject wall = CreateColoredQuad(wallName, oceanColor, pos, w, h);
        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.size = new Vector2(w, h);
        wall.transform.SetParent(transform);
    }

    // -----------------------------------------------------------------------
    // Rock obstacles — solid dark gray quads with BoxCollider2D
    // -----------------------------------------------------------------------
    private void BuildRockObstacles()
    {
        GameObject rockParent = new GameObject("Rocks");
        rockParent.transform.SetParent(transform);

        for (int i = 0; i < RockTiles.GetLength(0); i++)
        {
            int tx = RockTiles[i, 0];
            int ty = RockTiles[i, 1];
            Vector3 pos = TileToWorldPos(tx, ty);

            GameObject rock = CreateColoredQuad($"Rock_{tx}_{ty}", obstacleColor, pos, tileSize * 0.95f, tileSize * 0.95f);
            BoxCollider2D col = rock.AddComponent<BoxCollider2D>();
            col.size = new Vector2(tileSize * 0.9f, tileSize * 0.9f);
            rock.transform.SetParent(rockParent.transform);
        }
    }

    // -----------------------------------------------------------------------
    // Hazard zone — red trigger zone, HazardZone.cs handles damage
    // -----------------------------------------------------------------------
    private void BuildHazardZone()
    {
        GameObject hazardParent = new GameObject("HazardZone_Storm");
        hazardParent.transform.SetParent(transform);

        // Calculate bounds of all hazard tiles to make one composite trigger
        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        for (int i = 0; i < HazardTiles.GetLength(0); i++)
        {
            Vector3 p = TileToWorldPos(HazardTiles[i, 0], HazardTiles[i, 1]);
            minX = Mathf.Min(minX, p.x); maxX = Mathf.Max(maxX, p.x);
            minY = Mathf.Min(minY, p.y); maxY = Mathf.Max(maxY, p.y);

            // Visual tile
            GameObject htile = CreateColoredQuad($"Hazard_{i}", hazardColor, p, tileSize * 0.95f, tileSize * 0.95f);
            htile.transform.SetParent(hazardParent.transform);
        }

        // One trigger collider covering the whole zone
        float zw = (maxX - minX) + tileSize;
        float zh = (maxY - minY) + tileSize;
        Vector3 center = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, 0f);

        BoxCollider2D trigger = hazardParent.AddComponent<BoxCollider2D>();
        trigger.isTrigger = true;
        trigger.size = new Vector2(zw, zh);
        trigger.offset = center - hazardParent.transform.position;

        // Attach damage script
        HazardZone hz = hazardParent.AddComponent<HazardZone>();
        hz.damagePerSecond = 5f;
        hz.hazardLabel = "Storm Zone";

        Debug.Log("Hazard zone built at storm area.");
    }

    // -----------------------------------------------------------------------
    // Dock — yellow visual marker at the exit point
    // -----------------------------------------------------------------------
    private void BuildDock()
    {
        Vector3 pos = TileToWorldPos(DockTile[0], DockTile[1]);
        GameObject dock = CreateColoredQuad("Dock_Exit", dockColor, pos, tileSize * 1.5f, tileSize * 0.4f);
        dock.transform.SetParent(transform);
        Debug.Log("Dock exit marker placed.");
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    /// <summary>Converts tile grid coordinates to world position.</summary>
    private Vector3 TileToWorldPos(int tx, int ty)
    {
        return new Vector3(tx * tileSize, ty * tileSize, 0f);
    }

    /// <summary>
    /// Creates a flat colored quad sprite at the given position and size.
    /// Uses a runtime-generated 1x1 white texture tinted with the given color.
    /// </summary>
    private GameObject CreateColoredQuad(string objName, Color color, Vector3 worldPos, float width, float height)
    {
        GameObject obj = new GameObject(objName);
        obj.transform.position = worldPos;
        obj.transform.localScale = new Vector3(width, height, 1f);

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();

        // Create a 1×1 white texture and tint it
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        sr.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        sr.color  = color;

        return obj;
    }
}
