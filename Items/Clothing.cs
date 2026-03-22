namespace DefaultNamespace;

public class Clothing
{
    // Fields
    private ClothingSlot _slot;
    private int _defenseBonus;
    private String _visualTag;

    // Properties
    public ClothingSlot Slot
    {
        get { return _slot; }
        set
        {
            _slot = value;
        }
    }
    
    public int DefenseBonus
    {
        get { return _defenseBonus; }
        set
        {
            if (value > 0) _defenseBonus = value;
            else _defenseBonus = 0;
        }
    }

    /**
     * use() Meethod: Equips the clothing item to the player
     * @param: Player, the player
     */
    public void use(Player player)
    {
        player.equipItem(this);
    }

}