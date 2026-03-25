using UnityEngine;
using DefaultNamespace;

public class PlayerManager : MonoBehaviour
{
    public Player player;

    void Start()
    {
        player = new Player("Rory", "Pirate");
    }

    void Update()
    {
        // TEST: press space to take damage
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.TakeDamage(10);
            Debug.Log("Player took damage");
        }

        // TEST: press X to gain XP
        if (Input.GetKeyDown(KeyCode.X))
        {
            player.Progression.AddXP(10);
            Debug.Log("Gained XP");
        }
    }
}