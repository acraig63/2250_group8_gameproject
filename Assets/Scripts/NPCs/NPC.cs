using System;
using System.Collections.Generic;
using UnityEngine; 

using DefaultNamespace;

    public abstract class NPC
    {
        //Attributes
        protected string _npcId;
        protected string _name;
        protected int _health;
        protected Point _position;
        protected List<string> _dialogue;
 
        //Constructor 

        protected NPC(string npcId, string name, int health, Point position,
                      List<string>? dialogue = null)   //nullable annotation silences the warning
        {
            if (string.IsNullOrWhiteSpace(npcId))
                throw new ArgumentException("NPC ID cannot be null or empty.", nameof(npcId));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("NPC name cannot be null or empty.", nameof(name));
            if (health <= 0)
                throw new ArgumentOutOfRangeException(nameof(health), "Health must be positive.");
 
            _npcId    = npcId;
            _name     = name;
            _health   = health;
            _position = position;
            _dialogue = dialogue ?? new List<string>(); //always a valid list, never null
        }
 
        //Abstract Methods

        public abstract void Interact(Player player);
 
        //Concrete Methods

        public void TakeDamage(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Damage amount cannot be negative.");
 
            _health = Math.Max(0, _health - amount);
            Console.WriteLine($"{_name} took {amount} damage. Remaining HP: {_health}");
        }
        
        //returns true when this NPC's health has reached zero.

        public bool IsDefeated()
        {
            return _health <= 0;
        }
        
        //returns a random line of dialogue from the list.
        //returns an empty string when no dialogue has been set.

        public string GetDialogue()
        {
            if (_dialogue.Count == 0)
                return string.Empty;
 
            int index = new Random().Next(_dialogue.Count);
            return _dialogue[index];
        }
 
        //accessors 
        public string NpcId    => _npcId;
        public string Name     => _name;
        public int    Health   => _health;
        public Point  Position
        {
            get => _position;
            set => _position = value;
        }
    }
 