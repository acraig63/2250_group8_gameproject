using System.Collections.Generic;
using UnityEngine;
 
using DefaultNamespace;
public class EnemyNPC : NPC
{
    // ── Attributes ────────────────────────────────────────────────
    private int attackPower;
    private Item lootDrop;
    private bool dropsKey;
    private bool dropsMap;
    private CodingQuestion combatQuestion;
 
    // ── Constructor ───────────────────────────────────────────────

    public EnemyNPC(
        string npcId,
        string npcName,
        int health,
        Point position,
        int attackPower,
        CodingQuestion combatQuestion,
        Item lootDrop = null,
        bool dropsKey = false,
        bool dropsMap = false,
        List<string> dialogue = null)
        : base(npcId, npcName, health, position, dialogue)
    {
        this.attackPower    = attackPower;
        this.combatQuestion = combatQuestion;
        this.lootDrop       = lootDrop;
        this.dropsKey       = dropsKey;
        this.dropsMap       = dropsMap;
    }
 
    // ── Methods ───────────────────────────────────────────────────

    public CodingQuestion GetCombatQuestion()
    {
        return combatQuestion;
    }
    
    public Item DropLoot()
    {
        if (!IsDefeated())
        {
            Debug.Log($"{npcName} has not been defeated yet — no loot to drop.");
            return null;
        }
 
        if (lootDrop != null)
            Debug.Log($"{npcName} dropped: {lootDrop.Name}");
        else
            Debug.Log($"{npcName} dropped no item.");
 
        return lootDrop;
    }
    
    public override void Interact(Player player)
    {
        if (IsDefeated())
        {
            Debug.Log($"{npcName} has already been defeated.");
            return;
        }
 
        string line = GetDialogue();
        if (!string.IsNullOrEmpty(line))
            Debug.Log($"{npcName}: \"{line}\"");
 
        Debug.Log($"{npcName} challenges {player.getName()} to combat!");
 
        // Combat flow is delegated to Player and AttackMove
        player.Attack(this);
    }
    
    public void AttackPlayer(Player player)
    {
        if (IsDefeated())
            return;
 
        Debug.Log($"{npcName} attacks {player.getName()} for {attackPower} damage!");
        player.TakeDamage(attackPower);
    }
 
    // ── Accessors ─────────────────────────────────────────────────
    public int  AttackPower => attackPower;
    public bool DropsKey    => dropsKey;
    public bool DropsMap    => dropsMap;
}
 