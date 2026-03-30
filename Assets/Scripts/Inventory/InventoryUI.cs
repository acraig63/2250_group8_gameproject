using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DefaultNamespace;

/// <summary>
/// Unity UI controller for the player's inventory panel.
/// Attach this to the InventoryPanel GameObject in your Canvas.
///
/// SETUP IN INSPECTOR:
///   inventoryPanel     — the Panel GameObject to show/hide (can be this GameObject)
///   itemSlotPrefab     — prefab with an Image + TMP_Text child for the item name
///   itemSlotsContainer — GridLayoutGroup parent that holds all slots
///   goldText           — TMP_Text showing "Gold: 0"
///   fullText           — TMP_Text shown when inventory is full (can leave empty)
///
/// CONTROLS:
///   Press Tab to open/close the inventory panel.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Transform  itemSlotsContainer;
    [SerializeField] private TMP_Text   goldText;
    [SerializeField] private TMP_Text   fullText;

    private Inventory _inventory;
    private bool      _isOpen = false;

    /// <summary>
    /// Call this from Player.Start() after creating the Inventory instance.
    /// e.g. FindObjectOfType&lt;InventoryUI&gt;().Initialize(new Inventory(20));
    /// </summary>
    public void Initialize(Inventory inventory)
    {
        _inventory = inventory;
        inventoryPanel.SetActive(false);
        if (fullText != null) fullText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _isOpen = !_isOpen;
            inventoryPanel.SetActive(_isOpen);
            if (_isOpen) RefreshUI();
        }
    }

    /// <summary>
    /// Clears and redraws all slots from current inventory state.
    /// Called automatically on open and after any pickup.
    /// </summary>
    public void RefreshUI()
    {
        if (_inventory == null) return;

        // Clear existing slots
        foreach (Transform child in itemSlotsContainer)
            Destroy(child.gameObject);

        // Rebuild from inventory
        foreach (Item item in _inventory.Items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemSlotsContainer);

            TMP_Text label = slot.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = $"<b><size=14>{item.Name}</size></b>\n<size=8>{item.Rarity} {item.Type}</size>";

            // Colour slot by rarity
            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
                slotImage.color = RarityColor(item.Rarity);
        }

        if (goldText != null)
            goldText.text = $"Gold: {_inventory.Gold}";

        if (fullText != null)
            fullText.gameObject.SetActive(_inventory.IsFull());
    }

    /// <summary>
    /// Called by ItemPickup when the player walks over a world item.
    /// Returns true if successfully added to inventory.
    /// </summary>
    public bool TryAddItem(Item item)
    {
        if (_inventory == null) return false;
        bool added = _inventory.AddItem(item);
        if (fullText != null) fullText.gameObject.SetActive(_inventory.IsFull());
        RefreshUI();
        return added;
    }
    
    public void AddGold(int amount)
    {
        if (_inventory == null) return;

        _inventory.Gold += amount;
        RefreshUI();
    }

    private Color RarityColor(Rarity rarity) => rarity switch
    {
        Rarity.Common    => new Color(0.75f, 0.75f, 0.75f), // grey
        Rarity.Uncommon  => new Color(0.4f,  0.8f,  0.4f),  // green
        Rarity.Rare      => new Color(0.4f,  0.6f,  1.0f),  // blue
        Rarity.Legendary => new Color(1.0f,  0.75f, 0.0f),  // gold
        _                => Color.white
    };
}