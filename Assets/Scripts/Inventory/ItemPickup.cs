using UnityEngine;
using DefaultNamespace;

public class ItemPickup : MonoBehaviour
{
    [Header("Item Identity")]
    [SerializeField] private string   itemId          = "item_001";
    [SerializeField] private string   itemName        = "Basic Sword";
    [SerializeField] private string   itemDescription = "A worn cutlass.";
    [SerializeField] private int      goldValue       = 10;
    [SerializeField] private Rarity   rarity          = Rarity.Common;
    [SerializeField] private ItemType itemType        = ItemType.Weapon;

    [Header("Weapon Stats (only used if ItemType = Weapon)")]
    [SerializeField] private int        baseDamage = 5;
    [SerializeField] private WeaponType weaponType = WeaponType.Sword;

    [Header("Clothing Stats (only used if ItemType = Clothing)")]
    [SerializeField] private ClothingSlot clothingSlot = ClothingSlot.Torso;
    [SerializeField] private int          defenseBonus = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI == null)
        {
            Debug.LogWarning("ItemPickup: No InventoryUI found in scene.");
            return;
        }

        Item item = BuildItem();

        // If this is treasure / coin loot, add to gold only
        if (item.Type == ItemType.Treasure)
        {
            inventoryUI.AddGold(item.GoldValue);
            Debug.Log($"Picked up {item.GoldValue} gold.");
            Destroy(gameObject);
            return;
        }

        bool added = inventoryUI.TryAddItem(item);

        if (added)
        {
            Debug.Log($"Picked up: {itemName}");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory full, could not pick up item.");
        }
    }

    private Item BuildItem()
    {
        return itemType switch
        {
            ItemType.Weapon   => new Weapon(itemId, itemName, itemDescription,
                                            goldValue, rarity, baseDamage, weaponType),
            ItemType.Clothing => new Clothing(itemId, itemName, itemDescription,
                                              goldValue, rarity, clothingSlot, defenseBonus),
            _                 => new TreasureItem(itemId, itemName, itemDescription,
                                                  goldValue, rarity, itemType)
        };
    }
}