namespace DefaultNamespace;

public abstract class Item
{

    // Fields
    private string _id;
    private string _name;
    private string _description;
    private int _goldValue;
    private Rarity  _rarity;
    private ItemType _type;

    // Properties
    public string Id
    {
        get => _id;
        set => _id = value;
    }

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    public string Description
    {
        get => _description;
        set => _description = value;
    }

    public int GoldValue
    {
        get => _goldValue;
        set
        {
            if (value >= 0) _goldValue = value;
            else Console.WriteLine("Invalid gold value");
        }
    }

    public Rairity Rarity
    {
        get => _rarity;
        set => _rarity = value;
    }

    public ItemType Type
    {
        get => _type;
        set => _type = value;
    }
    
    // Abstract method for using the item
    public abstract void use(Player player)
    {
        
    }
    

}