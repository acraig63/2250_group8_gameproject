namespace DefaultNamespace
{
    public class Weapon
    {
        // Fields and properties
        private int _baseDamage;
        private string _weaponType;
        private string _specialEffect;
    
        public int BaseDamage
        {
            get { return _baseDamage; }
            set
            {
                calculateDamage();
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
        
        }

        /**
         * use() Meethod: Equips the weapon item to the player
         * @param: Player, the player
         */
        public void use(Player player)
        {
            player.equipItem(this)
        }


}