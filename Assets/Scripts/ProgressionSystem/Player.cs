using System.Collections.Generic;

namespace DefaultNamespace
{
    public class Player
    {
        //private attributes for player class
        private string name;
        private string characterClass;

        private HealthBar health;
        private Inventory inventory;
        private ProgressionSystem progression;

        private Weapon equippedWeapon;
        private Dictionary<ClothingSlot, Clothing> equippedOutfit;

        private Point position;
        private List<AttackMove> attackMoves;

        //constructor
        public Player(string name, string characterClass)
        {
            this.name = name;
            this.characterClass = characterClass;

            //these can be changed later
            health = new HealthBar(100);
            inventory = new Inventory(20);
            progression = new ProgressionSystem();

            equippedOutfit = new Dictionary<ClothingSlot, Clothing>();
            attackMoves = new List<AttackMove>();

            position = new Point(0, 0);
            equippedWeapon = null;
        }

        public void Move(Direction direction)
        {
            position = position.Move(direction);
        }

        public AttackMove Attack(NPC target)
        {
            if (equippedWeapon == null)
            {
                return new AttackMove(false, 0);
            }

            return equippedWeapon.Use(target);
        }

        public void PickUpItem(Item item)
        {
            inventory.AddItem(item);
        }

        public void EquipItem(Item item)
        {
            if (item is Weapon weapon)
            {
                equippedWeapon = weapon;
            }
            else if (item is Clothing clothing)
            {
                equippedOutfit[clothing.Slot] = clothing;
            }
        }

        public void TakeDamage(int amount)
        {
            health.TakeDamage(amount);
        }

        public bool IsAlive()
        {
            return !health.IsDead();
        }

        public PlayerStats GetStats()
        {
            return new PlayerStats(
                health.GetCurrentHP(),
                progression.GetLevel(),
                equippedWeapon
            );
        }

}