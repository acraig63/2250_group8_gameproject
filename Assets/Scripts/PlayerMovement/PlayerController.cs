using DefaultNamespace;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FindObjectOfType<InventoryUI>().Initialize(new Inventory(5));
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        movement = new Vector2(x, y).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * speed, movement.y * speed);
    }
}