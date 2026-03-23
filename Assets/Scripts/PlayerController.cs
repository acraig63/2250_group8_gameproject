using DefaultNamespace;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    private Player player;
    private Rigidbody2D rb;
    private Vector2 movement;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = new Player();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        movement = new Vector2(x, y);
        
        player.Move(movement);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}