using UnityEngine;
using UnityEngine.SceneManagement;

public class Key : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            SceneManager.LoadScene("StormbreakerIsland");
        }
    }
}