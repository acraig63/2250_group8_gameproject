using DefaultNamespace;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    public int MaxHealth { get; private set; } = 100;
    private int _currentHealth = 100;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FindObjectOfType<InventoryUI>().Initialize(new Inventory(5));
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        
        // if (BattleData.HasReturnPosition)
        // {
        //     transform.position = BattleData.ReturnPlayerPosition;
        //     BattleData.HasReturnPosition = false;
        // }
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

    public void SetHealth(int hp)
    {
        _currentHealth = Mathf.Clamp(hp, 0, MaxHealth);

        PlayerManager pm = GetComponent<PlayerManager>();
        Debug.Log($"SetHealth called: hp={hp}, pm={pm}, player={pm?.player}");
        if (pm != null && pm.player != null)
        {
            pm.player.SetCurrentHealth(_currentHealth);
            Debug.Log($"SetCurrentHealth called with {_currentHealth}");
        }
    }
    public int GetHealth() => _currentHealth;
    
    

}