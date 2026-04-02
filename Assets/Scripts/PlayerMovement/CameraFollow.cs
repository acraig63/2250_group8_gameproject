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

        if (halfH > (mapMaxY - mapMinY) * 0.5f)
            Debug.LogWarning($"[CameraFollow] orthoSize {halfH} exceeds half the map height — clamp bounds will invert.");

        Debug.Log($"[CameraFollow] bounds ready — orthoSize={halfH:F2} aspect={aspect:F3} " +
                  $"halfW={halfW:F2} clampX=[{_minX:F2},{_maxX:F2}] clampY=[{_minY:F2},{_maxY:F2}]");
    }

    void LateUpdate()
    {
        if (target == null || !_boundsReady) return;

        // Step 1: Move camera to player position.
        Vector3 pos = new Vector3(target.position.x, target.position.y, transform.position.z);

        // Step 2: Clamp using precomputed limits so camera never shows outside the map.
        pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
        pos.y = Mathf.Clamp(pos.y, _minY, _maxY);

        transform.position = pos;
        // Proof log — fires once per 300 frames (~5 s at 60 fps).
        if (Time.frameCount % 300 == 1)
            Debug.Log($">>> CAMERAFOLLOW CLAMPING ACTIVE: target=({target.position.x:F1},{target.position.y:F1}) "
                      + $"clamped=({pos.x:F2},{pos.y:F2}) bounds=[{_minX:F1},{_maxX:F1}]x[{_minY:F1},{_maxY:F1}]");
    }
}