
using UnityEngine;

public static class BattleData
{
    // Overworld → Battle

    // Which island scene to return to after the battle
    public static string ReturnScene = "StormbreakerIsland";

    // Question difficulty for this enemy (1–5)
    public static int QuestionLevel = 1;

    // Enemy display name shown in battle UI
    public static string EnemyName = "Pirate Guard";

    // Enemy stats
    public static int EnemyMaxHealth   = 50;
    public static int EnemyAttackPower = 10;

    // Player stats going into battle
    public static int PlayerMaxHealth   = 100;
    public static int PlayerAttackPower = 20;
    
    // Sprites
    public static Sprite PlayerSprite = null;
    public static Sprite EnemySprite  = null;

    // Battle → Overworld 

    // Set by BattleManager before returning to overworld
    public static bool PlayerWon           = false;
    public static int  PlayerCurrentHealth = 100;
}