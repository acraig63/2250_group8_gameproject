using UnityEngine;
using UnityEngine.SceneManagement;

public class Key : MonoBehaviour
{
    [SerializeField] private string sceneLocation;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            SceneManager.LoadScene(sceneLocation);
        }
    }
}