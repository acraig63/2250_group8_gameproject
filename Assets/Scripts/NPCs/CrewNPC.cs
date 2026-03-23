using System.Collections.Generic;
using UnityEngine;
 
using DefaultNamespace;
public class CrewNPC : NPC
{
    // ── Attributes ────────────────────────────────────────────────
    private string crewRole;        // e.g. navigator, gunner, cook
    private string crewBonus;       // passive bonus description shown to player
    private List<Item> itemsForSale;
    private bool questActive;
 
    // ── Constructor ───────────────────────────────────────────────
    public CrewNPC(
        string npcId,
        string npcName,
        int health,
        Point position,
        string crewRole,
        string crewBonus,
        List<Item> itemsForSale = null,
        bool questActive = false,
        List<string> dialogue = null)
        : base(npcId, npcName, health, position, dialogue)
    {
        this.crewRole     = crewRole;
        this.crewBonus    = crewBonus ?? string.Empty;
        this.itemsForSale = itemsForSale ?? new List<Item>();
        this.questActive  = questActive;
    }
 
    // ── Methods ───────────────────────────────────────────────────

    public void OfferTrade(Player player)
    {
        if (itemsForSale == null || itemsForSale.Count == 0)
        {
            Debug.Log($"{npcName} has nothing to sell right now.");
            return;
        }
 
        Debug.Log($"=== {npcName}'s Shop ===");
        for (int i = 0; i < itemsForSale.Count; i++)
        {
            Item item = itemsForSale[i];
            Debug.Log($"  [{i + 1}] {item.Name} — {item.GoldValue} gold");
        }
 
        // Actual purchase selection is handled by the UI layer
        Debug.Log("(Select an item to purchase — handled by the UI layer.)");
    }
    
    public void GiveQuest(Player player)
    {
        if (!questActive)
        {
            Debug.Log($"{npcName} has no quest available at the moment.");
            return;
        }
 
        Debug.Log($"{npcName}: \"I have a task for you, {player.GetName()}. " +
                  $"Complete it and I'll reward you handsomely!\"");
 
        questActive = false;
 
        // Quest details and tracking are managed by the Challenge subsystem
        Debug.Log($"(Quest from {npcName} is now active — tracked by Challenge subsystem.)");
    }

    public void ApplyCrewBonus(Player player)
    {
        if (string.IsNullOrWhiteSpace(crewBonus))
        {
            Debug.Log($"{npcName} has no passive bonus to apply.");
            return;
        }
 
        Debug.Log($"{npcName} joins your crew and grants: {crewBonus}");
 
        // Concrete stat changes are applied through the Player subsystem
        Debug.Log($"(Bonus \"{crewBonus}\" applied to {player.GetName()} via Player subsystem.)");
    }
 
    public override void Interact(Player player)
    {
        string line = GetDialogue();
        if (!string.IsNullOrEmpty(line))
            Debug.Log($"{npcName}: \"{line}\"");
        else
            Debug.Log($"{npcName} greets you warmly.");
 
        if (itemsForSale != null && itemsForSale.Count > 0)
            OfferTrade(player);
 
        if (questActive)
            GiveQuest(player);
    }
 
    // ── Accessors ─────────────────────────────────────────────────
    public string     CrewRole     => crewRole;
    public string     CrewBonus    => crewBonus;
    public List<Item> ItemsForSale => new List<Item>(itemsForSale); // defensive copy
    public bool       QuestActive
    {
        get => questActive;
        set => questActive = value;
    }
}

 