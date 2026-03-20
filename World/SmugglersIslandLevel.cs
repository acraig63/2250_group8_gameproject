namespace DefaultNamespace;

/// <summary>
/// Level 1 — Smuggler's Island (Tutorial Level)
/// Owner: Toby (world construction) + Rory (combat/question logic)
///
/// Concept: Sandy cove with pirate camps. Introduces the full combat loop
/// with MultipleChoiceQuestions. No environmental hazards — low-pressure entry.
///
/// Win condition: Defeat the Pirate Camp Leader → receive Key 1 + Map 1.
/// </summary>
public class SmugglersIslandLevel : Level
{
    // Map dimensions for the tutorial island
    private const int MAP_WIDTH  = 20;
    private const int MAP_HEIGHT = 20;

    // Track whether the boss has been handled to avoid double-triggering completion
    private bool _bossDefeatedHandled;

    public SmugglersIslandLevel() : base("level_1", "Smuggler's Island", MAP_WIDTH, MAP_HEIGHT)
    {
        _bossDefeatedHandled = false;
    }

    // -------------------------------------------------------------------
    // Initialization
    // -------------------------------------------------------------------

    /// <summary>
    /// Builds the island tile layout, sets up the MultipleChoice challenge,
    /// spawns NPCs, and places starter items.
    /// </summary>
    public override void Initialize()
    {
        Console.WriteLine("Initializing Smuggler's Island...");
        BuildMapLayout();
        SetupChallenge();
        SpawnNPCs();
        SpawnItems();
        Console.WriteLine("Smuggler's Island ready.\n");
    }

    /// <summary>
    /// Builds the 20×20 tile map for the tutorial island.
    /// Layout: water border → rocky obstacles → treasure spots → NPC positions → exit dock.
    /// </summary>
    private void BuildMapLayout()
    {
        // --- Water border (impassable ocean ring around the island) ---
        for (int x = 0; x < MAP_WIDTH; x++)
        {
            _mapLayout.SetTile(x, 0, TileType.Water, "water");
            _mapLayout.SetTile(x, MAP_HEIGHT - 1, TileType.Water, "water");
        }
        for (int y = 0; y < MAP_HEIGHT; y++)
        {
            _mapLayout.SetTile(0, y, TileType.Water, "water");
            _mapLayout.SetTile(MAP_WIDTH - 1, y, TileType.Water, "water");
        }

        // --- Rocky obstacles forming the pirate camp walls ---
        int[,] obstacleCoords =
        {
            { 5, 5 }, { 5, 6 }, { 6, 5 }, { 6, 6 },   // NW camp cluster
            { 13, 5 }, { 14, 5 }, { 13, 6 },            // NE camp cluster
            { 5, 13 }, { 5, 14 }, { 6, 13 },            // SW camp cluster
            { 8, 8 }, { 9, 8 }, { 8, 9 }                // Central wall fragment
        };
        for (int i = 0; i < obstacleCoords.GetLength(0); i++)
            _mapLayout.SetTile(obstacleCoords[i, 0], obstacleCoords[i, 1], TileType.Obstacle, "rock");

        // --- Treasure chest spawn spots ---
        _mapLayout.SetTile(3, 3, TileType.TreasureSpot, "treasure_chest");  // NW corner
        _mapLayout.SetTile(16, 3, TileType.TreasureSpot, "treasure_chest"); // NE corner
        _mapLayout.SetTile(10, 16, TileType.TreasureSpot, "treasure_chest");// Central south

        // --- NPC starting positions ---
        _mapLayout.SetTile(10, 10, TileType.NPCSpawn, "sand"); // Boss: Camp Leader
        _mapLayout.SetTile(3, 16, TileType.NPCSpawn, "sand");  // Friendly: Old Salt Pete

        // --- Exit dock (return to ship after boss is defeated) ---
        _mapLayout.SetTile(10, 18, TileType.Exit, "dock");

        Console.WriteLine("Map layout built for Smuggler's Island.");
    }

    /// <summary>
    /// Creates the Level 1 Challenge with two MultipleChoiceQuestions (syntax topics).
    /// Richard's Challenge and MultipleChoiceQuestion classes are used here.
    /// </summary>
    private void SetupChallenge()
    {
        // Challenge: no time limit (0 = unlimited), 100 XP on full completion
        _challenge = new Challenge("challenge_lv1", xpReward: 100, timeLimit: 0);

        MultipleChoiceQuestion q1 = new MultipleChoiceQuestion(
            questionText:  "What is the correct way to declare an integer variable in C#?",
            correctAnswer: "A",
            difficulty:    1,
            xpValue:       25,
            options: new List<string>
            {
                "A) int x = 5;",
                "B) integer x = 5;",
                "C) var x = 5.0;",
                "D) Int x = 5;"
            }
        );

        MultipleChoiceQuestion q2 = new MultipleChoiceQuestion(
            questionText:  "Which keyword is used to create a class in C#?",
            correctAnswer: "B",
            difficulty:    1,
            xpValue:       25,
            options: new List<string>
            {
                "A) define",
                "B) class",
                "C) struct",
                "D) object"
            }
        );

        MultipleChoiceQuestion q3 = new MultipleChoiceQuestion(
            questionText:  "What does the 'new' keyword do in C#?",
            correctAnswer: "C",
            difficulty:    1,
            xpValue:       25,
            options: new List<string>
            {
                "A) Declares a method",
                "B) Imports a namespace",
                "C) Creates a new instance of a class",
                "D) Defines an interface"
            }
        );

        _challenge.AddQuestion(q1);
        _challenge.AddQuestion(q2);
        _challenge.AddQuestion(q3);

        Console.WriteLine("Level 1 challenge configured (3 multiple-choice questions).");
    }

    // -------------------------------------------------------------------
    // NPC Spawning
    // -------------------------------------------------------------------

    /// <summary>
    /// Spawns the Pirate Camp Leader (boss) and Old Salt Pete (friendly shop NPC).
    /// Kabi's EnemyNPC and CrewNPC classes are used here.
    /// </summary>
    public override void SpawnNPCs()
    {
        // Boss enemy — defeating him awards Key 1 + Map 1
        EnemyNPC campLeader = new EnemyNPC(
            npcId:       "npc_lv1_boss",
            name:        "Pirate Camp Leader",
            health:      50,
            position:    new Point(10, 10),
            attackPower: 10,
            dropsKey:    true,
            dropsMap:    true
        );

        // Friendly NPC — offers hints and a small starter shop
        CrewNPC oldSaltPete = new CrewNPC(
            npcId:     "npc_lv1_pete",
            name:      "Old Salt Pete",
            health:    100,
            position:  new Point(3, 16),
            crewRole:  "navigator",
            crewBonus: "Reveals one free combat hint per island"
        );

        AddNPC(campLeader);
        AddNPC(oldSaltPete);

        Console.WriteLine($"{_npcs.Count} NPC(s) spawned on Smuggler's Island.");
    }

    // -------------------------------------------------------------------
    // Item Spawning
    // -------------------------------------------------------------------

    /// <summary>
    /// Places starter items at treasure spots. Michael's Item subclasses are used here.
    /// </summary>
    private void SpawnItems()
    {
        Weapon rustyСutlass = new Weapon(
            id:           "item_weapon_001",
            name:         "Rusty Cutlass",
            goldValue:    10,
            rarity:       Rarity.Common,
            baseDamage:   8,
            weaponType:   "cutlass",
            specialEffect: "none"
        );

        Clothing tatteredHat = new Clothing(
            id:          "item_cloth_001",
            name:        "Tattered Pirate Hat",
            goldValue:   5,
            rarity:      Rarity.Common,
            slot:        ClothingSlot.Head,
            defenseBonus: 2,
            visualTag:   "hat_tattered"
        );

        Clothing raggedJacket = new Clothing(
            id:          "item_cloth_002",
            name:        "Ragged Jacket",
            goldValue:   8,
            rarity:      Rarity.Common,
            slot:        ClothingSlot.Body,
            defenseBonus: 3,
            visualTag:   "jacket_ragged"
        );

        AddItemSpawn(rustyСutlass);
        AddItemSpawn(tatteredHat);
        AddItemSpawn(raggedJacket);

        Console.WriteLine($"{_itemSpawns.Count} item(s) spawned on Smuggler's Island.");
    }

    // -------------------------------------------------------------------
    // Game Loop
    // -------------------------------------------------------------------

    /// <summary>
    /// Called every game tick by GameManager.
    /// Watches for boss defeat to trigger level completion.
    /// </summary>
    public override void Update()
    {
        if (_isCompleted || _bossDefeatedHandled) return;

        foreach (NPC npc in _npcs)
        {
            if (npc is EnemyNPC enemy
                && enemy.Name == "Pirate Camp Leader"
                && enemy.IsDefeated())
            {
                _bossDefeatedHandled = true;
                Console.WriteLine("\nThe Pirate Camp Leader has been defeated!");
                Console.WriteLine("Key 1 and Map 1 recovered. Head to the docks to return to your ship.");
                CompleteLevel();
                break;
            }
        }
    }

    /// <summary>
    /// Finalizes level completion: awards XP, registers the key, logs the transition.
    /// Called by LevelManager after Update() marks the level complete.
    /// </summary>
    public override void OnLevelComplete(Player player)
    {
        player.Progression.AddXP(_challenge.XpReward);
        player.Progression.AddKey(_levelId);
        Console.WriteLine("Smuggler's Island cleared! The Jungle Ruins await...");
    }
}
