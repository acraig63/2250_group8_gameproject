using System.Collections.Generic;
using System.Linq;

namespace DefaultNamespace;

public class Inventory
{
    //private inventory data fields
    private List<Item> _items;
    private int _maxCapacity;
    private int _gold;

    //constructor
    public Inventory(int maxCapacity)
    {
        this._maxCapacity = maxCapacity;
        _items = new List<Item>();
        gold = 0;
    }

    //adds item to inventory and checks first to see if inventory is full
    public bool AddItem(Item item)
    {
        if (IsFull())
        {
            return false;
        }

        _items.Add(item);
        return true;
    }

    //removes specified item from inventory
    public void RemoveItem(Item item)
    {
        _items.Remove(item);
    }

    //returns all items in inventory that match the specified type
    public List<Item> GetItemsByType(ItemType type)
    {
        return _items.Where(item => item.GetItemType() == type).ToList();
    }

    //adds specified amount to gold count
    public void AddGold(int amount)
    {
        gold += amount;
    }

    //spends specified gold amount if allowed
    public bool SpendGold(int amount)
    {
        if (gold < amount)
        {
            return false;
        }

        gold -= amount;
        return true;
    }

    //checks if inventory is greater than the max inventory count
    public bool IsFull()
    {
        return items.Count >= maxCapacity;
    }
}