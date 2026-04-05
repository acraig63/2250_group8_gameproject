using UnityEngine;

public class CactusHazard : MonoBehaviour
{
    public int damage = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Cactus hit: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit cactus, health before: " + other.GetComponent<PlayerController>().GetHealth());
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
                pc.SetHealth(pc.GetHealth() - damage);
        }
    }
}