using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Map Bounds (world units)")]
    public float mapMinX = 0f;
    public float mapMaxX = 80f;
    public float mapMinY = 0f;
    public float mapMaxY = 60f;

    private Camera _cam;

    void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        float halfH = (_cam != null) ? _cam.orthographicSize : 5f;
        float aspect = (_cam != null) ? _cam.aspect : 0f;
        // Fallback: compute aspect from screen size if camera reports 0 or NaN.
        if (aspect <= 0f || float.IsNaN(aspect))
            aspect = Screen.width / (float)Screen.height;
        float halfW = halfH * aspect;

        float mapH = mapMaxY - mapMinY;
        if (halfH > mapH * 0.5f)
            Debug.LogWarning($"CameraFollow: orthoSize {halfH} is larger than half the map height {mapH * 0.5f} — camera cannot fit within bounds.");

        float x = Mathf.Clamp(target.position.x, mapMinX + halfW, mapMaxX - halfW);
        float y = Mathf.Clamp(target.position.y, mapMinY + halfH, mapMaxY - halfH);

        transform.position = new Vector3(x, y, transform.position.z);
    }
}