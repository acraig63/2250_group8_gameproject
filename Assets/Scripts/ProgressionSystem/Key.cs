using UnityEngine;
using UnityEngine.SceneManagement;

public class Key : MonoBehaviour
{
    [SerializeField] private string sceneLocation;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
            if (inventoryUI != null)
            {
                BattleData.PlayerGold = inventoryUI.GetInventory().Gold;
                inventoryUI.SaveItemsToData();
            }

            Destroy(gameObject);
            SceneManager.LoadScene(sceneLocation);
        }
    }
}