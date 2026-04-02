using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Map Bounds (world units)")]
    // Tilemap origin is (0,0,0); MAP_WIDTH=80, MAP_HEIGHT=60 tiles at 1 unit each.
    public float mapMinX = 0f;
    public float mapMaxX = 80f;
    public float mapMinY = 0f;
    public float mapMaxY = 60f;

    private Camera _cam;

    void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    void Start()
    {
        // Compute bounds once at startup and log so they can be verified in Console.
        float halfH  = _cam != null ? _cam.orthographicSize : 5f;
        float aspect = (_cam != null && _cam.aspect > 0f && !float.IsNaN(_cam.aspect))
                       ? _cam.aspect
                       : (Screen.height > 0 ? Screen.width / (float)Screen.height : 16f / 9f);
        float halfW  = halfH * aspect;
        Debug.Log($"[CameraFollow] orthoSize={halfH:F2}  aspect={aspect:F3}  " +
                  $"halfW={halfW:F2}  halfH={halfH:F2}  " +
                  $"clampX=[{mapMinX + halfW:F2}, {mapMaxX - halfW:F2}]  " +
                  $"clampY=[{mapMinY + halfH:F2}, {mapMaxY - halfH:F2}]");
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Step 1: Move camera to follow player exactly.
        Vector3 pos = new Vector3(target.position.x, target.position.y, transform.position.z);

        // Step 2: Clamp so the camera never shows outside the tilemap.
        float halfH  = _cam != null ? _cam.orthographicSize : 5f;
        float aspect = (_cam != null && _cam.aspect > 0f && !float.IsNaN(_cam.aspect))
                       ? _cam.aspect
                       : (Screen.height > 0 ? Screen.width / (float)Screen.height : 16f / 9f);
        float halfW  = halfH * aspect;

        if (halfH > (mapMaxY - mapMinY) * 0.5f)
            Debug.LogWarning($"[CameraFollow] orthoSize {halfH} exceeds half the map height — bounds will invert.");

        pos.x = Mathf.Clamp(pos.x, mapMinX + halfW, mapMaxX - halfW);
        pos.y = Mathf.Clamp(pos.y, mapMinY + halfH, mapMaxY - halfH);
        transform.position = pos;
    }
}