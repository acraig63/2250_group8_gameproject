using System.Collections.Generic;
using UnityEngine;

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
        public Player()
        {

        }

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

        public void Move(Vector2 direction)
        {
            position = new Point(position.X + direction.x, position.Y + direction.y);
        }

        public AttackMove Attack(EnemyNPC target)
        {
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
    }
}