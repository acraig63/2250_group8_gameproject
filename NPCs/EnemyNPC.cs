using System;
using System.Collections.Generic;

namespace DefaultNamespace;

public class EnemyNPC : NPC
    {
        //Attributes
        private int            attackPower;
        private Item           lootDrop;
        private bool           dropsKey;
        private bool           dropsMap;
        private CodingQuestion combatQuestion;
 
        //Constructor
        public EnemyNPC(
            string         npcId,
            string         name,
            int            health,
            Point          position,
            int            attackPower,
            CodingQuestion combatQuestion,
            Item           lootDrop   = null,
            bool           dropsKey   = false,
            bool           dropsMap   = false,
            List<string>   dialogue   = null) : base(npcId, name, health, position, dialogue)
        {
            if (attackPower < 0)
                throw new ArgumentOutOfRangeException(nameof(attackPower),
                    "Attack power cannot be negative.");
            if (combatQuestion == null)
                throw new ArgumentNullException(nameof(combatQuestion),
                    "EnemyNPC must have a combat question.");
 
            this.attackPower    = attackPower;
            this.combatQuestion = combatQuestion;
            this.lootDrop       = lootDrop;
            this.dropsKey       = dropsKey;
            this.dropsMap       = dropsMap;
        }
 
        //Methods
        public CodingQuestion GetCombatQuestion()
        {
            return combatQuestion;
        }
        
        public Item DropLoot()
        {
            if (!IsDefeated())
            {
                Console.WriteLine($"{name} has not been defeated yet — no loot to drop.");
                return null;
            }
 
            if (lootDrop != null)
                Console.WriteLine($"{name} dropped: {lootDrop.Name}");
            else
                Console.WriteLine($"{name} dropped no item.");
 
            return lootDrop;
        }
        
        public override void Interact(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
 
            if (IsDefeated())
            {
                Console.WriteLine($"{name} has already been defeated.");
                return;
            }
 
            //display pre-combat dialogue
            string line = GetDialogue();
            if (!string.IsNullOrEmpty(line))
                Console.WriteLine($"{name}: \"{line}\"");
 
            Console.WriteLine($"{name} challenges {player.Name} to combat!");
 
            //combat flow is delegated to Player and AttackMove.
            //this call signals the intent; the game loop picks it up.
            player.Attack(this);
        }
 

        public void AttackPlayer(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
 
            if (IsDefeated())
                return; //a defeated enemy cannot retaliate
 
            Console.WriteLine($"{name} attacks {player.Name} for {attackPower} damage!");
            player.TakeDamage(attackPower);
        }
 
        //Accessors
        public int  AttackPower => attackPower;
        public bool DropsKey    => dropsKey;
        public bool DropsMap    => dropsMap;
    }
 
