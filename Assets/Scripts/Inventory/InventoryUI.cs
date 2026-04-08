using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DefaultNamespace;


public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Transform  itemSlotsContainer;
    [SerializeField] private TMP_Text   goldText;
    [SerializeField] private TMP_Text   fullText;
    [SerializeField] private ItemDetailPopup itemDetailPopup;
    [SerializeField] private Transform playerTransform;

    private Inventory _inventory;
    private bool      _isOpen = false;
    public Inventory GetInventory() => _inventory;

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

    private List<(RectTransform rect, Item item)> _slotRects = new List<(RectTransform, Item)>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _isOpen = !_isOpen;
            inventoryPanel.SetActive(_isOpen);
            if (_isOpen) RefreshUI();
            
            if (!_isOpen && itemDetailPopup != null)
                itemDetailPopup.Hide();
        }

        if (_isOpen && Input.GetMouseButtonDown(0))
        {
            foreach (var (rect, item) in _slotRects)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition))
                {
                    Debug.Log($"Clicked slot: {item.Name}");
                    if (itemDetailPopup != null)
                    {
                        Transform pt = playerTransform;
                        if (pt == null)
                        {
                            GameObject p = GameObject.FindWithTag("Player");
                            if (p != null) pt = p.transform;
                        }
                        itemDetailPopup.Show(item, _inventory, this, pt);
                    }
                        
                        //itemDetailPopup.Show(item, _inventory, this, playerTransform);
                    break;
                }
            }
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
        
        
        _slotRects.Clear();

        foreach (Item item in _inventory.Items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemSlotsContainer);

            TMP_Text label = slot.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = $"<b><size=14>{item.Name}</size></b>\n<size=8>{item.Rarity} {item.Type}</size>";

            Image slotImage = slot.GetComponent<Image>();
            if (slotImage != null)
                slotImage.color = RarityColor(item.Rarity);

            // Register slot rect for manual hit detection
            RectTransform rect = slot.GetComponent<RectTransform>();
            if (rect != null)
                _slotRects.Add((rect, item));
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
    
    public int GetGold()
    {
        return _inventory.Gold;
    }
    
    public bool TrySpendGold(int amount)
    {
        if (_inventory.Gold < amount)
            return false;

        _inventory.Gold -= amount;
        RefreshUI();
        return true;
    }
}