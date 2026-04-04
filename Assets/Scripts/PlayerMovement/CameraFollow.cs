using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Map Bounds (world units)")]
    // Tilemap origin (0,0,0); MAP_WIDTH=80, MAP_HEIGHT=60 tiles × 1 unit/tile.
    public float mapMinX = 0f;
    public float mapMaxX = 80f;
    public float mapMinY = 0f;
    public float mapMaxY = 60f;

    private Camera _cam;

    // Clamp limits precomputed once in Start() — never recomputed per-frame.
    private float _minX, _maxX, _minY, _maxY;
    private bool  _boundsReady;

    void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    void Start()
    {
        float halfH  = _cam != null ? _cam.orthographicSize : 5f;
        float aspect = (_cam != null && _cam.aspect > 0f && !float.IsNaN(_cam.aspect))
                       ? _cam.aspect
                       : (Screen.height > 0 ? Screen.width / (float)Screen.height : 16f / 9f);
        float halfW  = halfH * aspect;

        _minX = mapMinX + halfW;
        _maxX = mapMaxX - halfW;
        _minY = mapMinY + halfH;
        _maxY = mapMaxY - halfH;
        _boundsReady = true;

        if (_cam != null)
        {
            // HDR in URP 2D causes a washed-out grey appearance when no
            // tone mapping is applied. Disable it, same as MinimapCamera.
            _cam.allowHDR = false;

            // Match background to sand so any sub-pixel gap at the map
            // edge blends in rather than showing the default grey/blue.
            _cam.clearFlags      = CameraClearFlags.SolidColor;
            _cam.backgroundColor = new Color(0.88f, 0.76f, 0.50f, 1f);
        }

        if (halfH > (mapMaxY - mapMinY) * 0.5f)
            Debug.LogWarning($"[CameraFollow] orthoSize {halfH} exceeds half the map height — clamp bounds will invert.");

        Debug.Log($"[CameraFollow] bounds ready — orthoSize={halfH:F2} aspect={aspect:F3} " +
                  $"halfW={halfW:F2} clampX=[{_minX:F2},{_maxX:F2}] clampY=[{_minY:F2},{_maxY:F2}]");
    }

<<<<<<< HEAD
    /// <summary>
    /// Override the map bounds at runtime (called by scene builders for non-standard map sizes).
    /// Recomputes clamp limits immediately using the current camera state.
    /// </summary>
    public void SetBounds(float minX, float maxX, float minY, float maxY)
    {
        mapMinX = minX;
        mapMaxX = maxX;
        mapMinY = minY;
        mapMaxY = maxY;

        float halfH  = _cam != null ? _cam.orthographicSize : 5f;
        float aspect = (_cam != null && _cam.aspect > 0f && !float.IsNaN(_cam.aspect))
                       ? _cam.aspect
                       : (Screen.height > 0 ? Screen.width / (float)Screen.height : 16f / 9f);
        float halfW  = halfH * aspect;

        _minX = mapMinX + halfW;
        _maxX = mapMaxX - halfW;
        _minY = mapMinY + halfH;
        _maxY = mapMaxY - halfH;
        _boundsReady = true;
    }

=======
>>>>>>> mike-level
    void LateUpdate()
    {
        if (target == null || !_boundsReady) return;

        // Step 1: Move camera to player position.
        Vector3 pos = new Vector3(target.position.x, target.position.y, transform.position.z);

        // Step 2: Clamp using precomputed limits so camera never shows outside the map.
        pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
        pos.y = Mathf.Clamp(pos.y, _minY, _maxY);

        transform.position = pos;
    }
}