using System;
    using System.Collections.Generic;
    using UnityEngine;

    using DefaultNamespace;

    public abstract class NPC
    {
        // ── Attributes ────────────────────────────────────────────────
        protected string npcId;
        protected string npcName; // renamed from 'name' to avoid conflict with UnityEngine.Object.name
        protected int health;
        protected Point position;
        protected List<string> dialogue;

        // ── Constructor ───────────────────────────────────────────────

        protected NPC(string npcId, string npcName, int health, Point position,
            List<string> dialogue = null)
        {
            this.npcId = npcId;
            this.npcName = npcName;
            this.health = health;
            this.position = position;
            this.dialogue = dialogue ?? new List<string>();
        }

        // ── Abstract Methods ──────────────────────────────────────────

        public abstract void Interact(Player player);

        // ── Concrete Methods ──────────────────────────────────────────
        public void TakeDamage(int amount)
        {
            health = Mathf.Max(0, health - amount);
            Debug.Log($"{npcName} took {amount} damage. Remaining HP: {health}");
        }

        public bool IsDefeated()
        {
            return health <= 0;
        }

        // Returns a random line of dialogue from the list.
        // Returns an empty string when no dialogue has been set.

        public string GetDialogue()
        {
            if (dialogue == null || dialogue.Count == 0)
                return string.Empty;

            // Use System.Random explicitly to avoid conflict with UnityEngine.Random
            int index = new System.Random().Next(dialogue.Count);
            return dialogue[index];
        }

        // ── Accessors ─────────────────────────────────────────────────
        public string NpcId => npcId;
        public string NpcName => npcName;
        public int Health => health;

        public Point Position
        {
            get => position;
            set => position = value;
        }
    }

 