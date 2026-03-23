namespace DefaultNamespace
{
    public class Weapon: Item
    {
        // Fields 
        private int _baseDamage;
        private WeaponType _weaponType;
        private string _specialEffect;
        
        // Constructor
        public Weapon(string id, string name, string description, int goldValue,
            Rarity rarity, int baseDamage, WeaponType weaponType, string specialEffect = "none")
            : base(id, name, description, goldValue, rarity, ItemType.Weapon)
        {
            _baseDamage = baseDamage;
            _weaponType = weaponType;
            _specialEffect = specialEffect;
        }
    
        // Properties
        public int BaseDamage
        {
            get { return _baseDamage; }
            set
            {
                _baseDamage = value;
            }
        }
    
        public  WeaponType WeaponType
        {
            get {
                return _weaponType; 
            }
            set
            {
                _weaponType = value;
            }
        }
        public string SpecialEffect
        {
            get { return _specialEffect; }
            set { _specialEffect = value; }
        }

        // public void applyEffect(NPC target)
        // {
        //     TO UPDATE
        // }

 
        /**
         * calculareDamage() Method : Calcualtes the amount of damage the weapon will deal
         * @return: Integer, damage value
         */
        public int calculateDamage()
        {
            float multiplier = Rarity switch
            {
                Rarity.Common    => 1.0f,
                Rarity.Uncommon  => 1.2f,
                Rarity.Rare      => 1.5f,
                Rarity.Legendary => 2.0f,
                _                => 1.0f
            };
            return (int)(_baseDamage * multiplier);
        }

        /**
         * use() Meethod: Equips the weapon item to the player
         * @param: Player, the player
         */
        public override void use(Player player)
        {
            player.equipItem(this);
        }


}