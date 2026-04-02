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
        float halfW = (_cam != null) ? halfH * _cam.aspect   : halfH;

        float x = Mathf.Clamp(target.position.x, mapMinX + halfW, mapMaxX - halfW);
        float y = Mathf.Clamp(target.position.y, mapMinY + halfH, mapMaxY - halfH);

        transform.position = new Vector3(x, y, transform.position.z);
    }
}