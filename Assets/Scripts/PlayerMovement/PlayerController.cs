using DefaultNamespace;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    public int MaxHealth { get; private set; } = 100;
    private int _currentHealth = 100;
    
    private Animator animator;
    private SpriteRenderer sr;
    
    private InventoryUI inventoryUI;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Inventory inv = new Inventory(5);
        inv.Gold = BattleData.PlayerGold; // restore gold
        FindObjectOfType<InventoryUI>().Initialize(inv);
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        
        // Restore health from BattleData if returning from battle
        if (BattleData.PlayerCurrentHealth > 0)
            _currentHealth = BattleData.PlayerCurrentHealth;
        
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        
        inventoryUI = FindObjectOfType<InventoryUI>();
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        movement = new Vector2(x, y).normalized;
        
        float move = movement.magnitude;
        animator.SetFloat("Speed", move);
        
        if (movement.x > 0)
        {
            sr.flipX = false; // facing right
        }
        else if (movement.x < 0)
        {
            sr.flipX = true; // facing left
        }
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (inventoryUI != null)
            {
                int gold = inventoryUI.GetGold();

                if (gold >= 10)
                {
                    inventoryUI.TrySpendGold(10);
                    SetHealth(GetHealth() + 10);

                    Debug.Log("Spent 10 gold to heal!");
                }
                else
                {
                    Debug.Log("Not enough gold!");
                }
            }
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * speed, movement.y * speed);
    }
    
    public int GetHealth() => _currentHealth;
    
    public void SetHealth(int hp)
    {
        _currentHealth = Mathf.Clamp(hp, 0, MaxHealth);
        BattleData.PlayerCurrentHealth = _currentHealth;

        PlayerManager pm = GetComponent<PlayerManager>();
        if (pm != null && pm.player != null)
            pm.player.SetCurrentHealth(_currentHealth);

        if (_currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        BattleData.DiedInBattle = true;
        BattleData.PlayerCurrentHealth = MaxHealth;
        BattleData.HasReturnPosition = false;
        BattleData.ReturningFromBattle = false;
        BattleData.DefeatedEnemies.Clear();
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    

}