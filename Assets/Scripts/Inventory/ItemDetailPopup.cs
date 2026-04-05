using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DefaultNamespace;


public class ItemDetailPopup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescriptionText;
    [SerializeField] private TMP_Text itemStatsText;
    [SerializeField] private Button   dropButton;
    [SerializeField] private Button   closeButton;

    [Header("Drop Settings")]
    [SerializeField] private GameObject itemSpritePrefab; // prefab to spawn in world on drop

    private Item _currentItem;
    private Inventory _inventory;
    private InventoryUI _inventoryUI;
    private Transform _playerTransform;
    private RectTransform _dropButtonRect;
    private RectTransform _closeButtonRect;

    void Start()
    {
        _dropButtonRect  = dropButton.GetComponent<RectTransform>();
        _closeButtonRect = closeButton.GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (!gameObject.activeSelf) return;
    
        if (Input.GetMouseButtonDown(0))
        {
            if (_closeButtonRect != null && 
                RectTransformUtility.RectangleContainsScreenPoint(_closeButtonRect, Input.mousePosition))
            {
                Debug.Log("Close clicked");
                Hide();
                return;
            }
        
            if (_dropButtonRect != null && 
                RectTransformUtility.RectangleContainsScreenPoint(_dropButtonRect, Input.mousePosition))
            {
                Debug.Log("Drop clicked");
                DropItem();
                return;
            }
        }
    }

    /// <summary>
    /// Call this from InventoryUI when a slot is clicked.
    /// </summary>
    public void Show(Item item, Inventory inventory, InventoryUI inventoryUI, Transform playerTransform)
    {
        _currentItem      = item;
        _inventory        = inventory;
        _inventoryUI      = inventoryUI;
        _playerTransform  = playerTransform;

        // Fill in the details
        itemNameText.text        = item.Name;
        itemDescriptionText.text = item.Description;
        itemStatsText.text       = GetStatsText(item);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _currentItem = null;
    }

    private void DropItem()
    {
        if (_currentItem == null || _inventory == null) return;

        // Remove from inventory
        _inventory.RemoveItem(_currentItem);

        // Spawn sprite back in the world at the player's position
        if (itemSpritePrefab != null && _playerTransform != null)
        {
            Vector3 dropPosition = _playerTransform.position + new Vector3(1f, 0f, 0f);
            GameObject dropped = Instantiate(itemSpritePrefab, dropPosition, Quaternion.identity);

            // Copy item data back onto the ItemPickup script
            ItemPickup pickup = dropped.GetComponent<ItemPickup>();
            if (pickup != null)
                pickup.SetItemData(_currentItem, _currentItem.WorldSprite);
        }

        // Refresh inventory UI and close popup
        _inventoryUI.RefreshUI();
        Hide();
    }

    private string GetStatsText(Item item)
    {
        if (item is Weapon weapon)
            return $"Damage: {weapon.calculateDamage()}\nType: {weapon.WeaponType}\nEffect: {weapon.SpecialEffect}";
        if (item is Clothing clothing)
            return $"Defense: {clothing.DefenseBonus}\nSlot: {clothing.Slot}";
        return $"Value: {item.GoldValue} gold";
    }
}