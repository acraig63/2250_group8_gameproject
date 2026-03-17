namespace DefaultNamespace;

public class Weapon
{
    // Fields and properties
    private int _baseDamage;
    public int BaseDamage
    {
        get { return _baseDamage; }
        set
        {
            calculateDamage();
        }
    }

    private String _weaponType;
    public String WeaponType WeaponType
    {
        get {
            return _weaponType; 
        }
        set
        {
            _weaponType = value;
        }
    }

    // private string _specialEffect;
    // public string SpecialEffect
    // {
    //     get { return _specialEffect; }
    //     set { _specialEffect = value; }
    // }

    // public void applyEffect(NPC target)
    // {
    //     
    // }

 
    /**
     * calculareDamage() Method : Calcualtes the amount of damage the weapon will deal
     * @return: Integer, damage value
     */
    public int calculateDamage()
    {
        
    }

    /**
     * use() Meethod: Equips the item to the player
     * @param: Player, the player
     */
    public void use(Player player)
    {
        player.equipItem(this)
    }
    
    