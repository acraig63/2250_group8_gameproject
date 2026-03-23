using System;
using System.Collections.Generic;
using DefaultNamespace;

    public class CrewNPC : NPC
    {
        //Attributes
        private string       crewRole;       //e.g. navigator, gunner, cook
        private string       crewBonus;      //passive bonus description shown to player
        private List<Item>   itemsForSale;
        private bool         questActive;
 
        //Constructor

        public CrewNPC(
            string       npcId,
            string       name,
            int          health,
            Point        position,
            string       crewRole,
            string       crewBonus,
            List<Item>   itemsForSale = null,
            bool         questActive  = false,
            List<string> dialogue     = null) : base(npcId, name, health, position, dialogue)
        {
            if (string.IsNullOrWhiteSpace(crewRole))
                throw new ArgumentException("Crew role cannot be null or empty.", nameof(crewRole));
 
            this.crewRole     = crewRole;
            this.crewBonus    = crewBonus ?? string.Empty;
            this.itemsForSale = itemsForSale ?? new List<Item>();
            this.questActive  = questActive;
        }
 
        //Methods
        public void OfferTrade(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
 
            if (itemsForSale == null || itemsForSale.Count == 0)
            {
                Console.WriteLine($"{name} has nothing to sell right now.");
                return;
            }
 
            Console.WriteLine($"\n=== {name}'s Shop ===");
            for (int i = 0; i < itemsForSale.Count; i++)
            {
                Item item = itemsForSale[i];
                Console.WriteLine($"  [{i + 1}] {item.Name} — {item.GoldValue} gold");
            }
 
            //actual purchase logic is driven by the UI layer;
            //this method exposes the catalogue and delegates selection upward.
            Console.WriteLine("(Select an item to purchase — handled by the UI layer.)");
        }
        
        public void GiveQuest(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
 
            if (!questActive)
            {
                Console.WriteLine($"{name} has no quest available at the moment.");
                return;
            }
 
            Console.WriteLine($"{name}: \"I have a task for you, {player.Name}. " +
                              $"Complete it and I'll reward you handsomely!\"");
 
            //quest details and tracking are managed by the Challenge subsystem.
            //CrewNPC simply initiates the handoff and disables repeat offers.
            questActive = false;
 
            //the concrete quest object would be retrieved from a QuestRegistry
            //and passed to the player here in the full implementation.
            Console.WriteLine($"(Quest from {name} is now active — tracked by Challenge subsystem.)");
        }
        
        public void ApplyCrewBonus(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
 
            if (string.IsNullOrWhiteSpace(crewBonus))
            {
                Console.WriteLine($"{name} has no passive bonus to apply.");
                return;
            }
 
            Console.WriteLine($"{name} joins your crew and grants: {crewBonus}");
 
            //concrete stat changes are applied through Player's public interface.
            //for example, a navigator might boost speed; a gunner boosts attack.
            //those calls live here in the full implementation once the stat
            //system is finalised 
            Console.WriteLine($"(Bonus \"{crewBonus}\" applied to {player.Name} via Player subsystem.)");
        }
        
        public override void Interact(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
 
            //greet the player
            string line = GetDialogue();
            if (!string.IsNullOrEmpty(line))
                Console.WriteLine($"{name}: \"{line}\"");
            else
                Console.WriteLine($"{name} greets you warmly.");
 
            //offer trade if the NPC has a shop
            if (itemsForSale != null && itemsForSale.Count > 0)
                OfferTrade(player);
 
            //offer quest if one is waiting
            if (questActive)
                GiveQuest(player);
        }
 
        //Accessors
        public string     CrewRole     => crewRole;
        public string     CrewBonus    => crewBonus;
        public List<Item> ItemsForSale => new List<Item>(itemsForSale); // defensive copy
        public bool       QuestActive
        {
            get => questActive;
            set => questActive = value;
        }
    }