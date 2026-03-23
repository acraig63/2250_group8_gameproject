namespace DefaultNamespace
{
    public class Clothing : Item
    {
    // Fields
    private ClothingSlot _slot;
    private int _defenseBonus;
    private string _visualTag;
    
    // Constructor
    public Clothing(string id, string name, string description, int goldValue,
        Rarity rarity, ClothingSlot slot, int defenseBonus, string visualTag = "")
        : base(id, name, description, goldValue, rarity, ItemType.Clothing)
    {
        _slot = slot;
        _defenseBonus = defenseBonus;
        _visualTag = visualTag;
    }

    // Properties
    public ClothingSlot Slot
    {
        get { return _slot; }
        set { _slot = value; }
    }

    public int DefenseBonus
    {
        get { return _defenseBonus; }
        set
        {
            if (value >= 0) _defenseBonus = value;
            else _defenseBonus = 0;
        }
    }
    
    public string VisualTag
    {
        get { return _visualTag; }
        set { _visualTag = value; }
    }



    /**
     * use() Method: Equips the clothing item to the player
     * @param: Player, the player
     */
    public override void use(Player player)
    {
        player.equipItem(this);
    }

    }
}