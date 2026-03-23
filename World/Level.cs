using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;

/// <summary>
/// Abstract base class for all five playable islands.
/// Defines the shared interface every Level must implement and owns
/// the level's NPC list, item spawns, challenge, and tile map.
/// Does NOT contain player progression logic — delegates to ProgressionSystem.
/// </summary>
public abstract class Level
{
    protected string _levelId;
    protected string _levelName;
    protected List<NPC> _npcs;
    protected List<Item> _itemSpawns;
    protected Challenge _challenge;
    protected bool _isCompleted;
    protected TileMap _mapLayout;

    protected Level(string levelId, string levelName, int mapWidth, int mapHeight)
    {
        _levelId    = levelId;
        _levelName  = levelName;
        _npcs       = new List<NPC>();
        _itemSpawns = new List<Item>();
        _isCompleted = false;
        _mapLayout  = new TileMap(mapWidth, mapHeight);
    }

    public string  LevelId    { get => _levelId; }
    public string  LevelName  { get => _levelName; }
    public bool    IsCompleted { get => _isCompleted; }
    public TileMap MapLayout  { get => _mapLayout; }

    public abstract void Initialize();
    public abstract void Update();
    public abstract void OnLevelComplete(Player player);
    public abstract void SpawnNPCs();

    public virtual void OnPlayerEnter(Player player)
    {
        // Fix: Console.WriteLine → Debug.Log
        Debug.Log($"You have arrived at {_levelName}.");
        SpawnNPCs();
    }

    public Challenge  GetChallenge()   { return _challenge; }
    public List<NPC>  GetNPCs()        { return _npcs; }
    public List<Item> GetItemSpawns()  { return _itemSpawns; }

    protected void AddNPC(NPC npc)      { _npcs.Add(npc); }
    protected void AddItemSpawn(Item i) { _itemSpawns.Add(i); }

    protected void CompleteLevel()
    {
        _isCompleted = true;
        // Fix: Console.WriteLine → Debug.Log
        Debug.Log($"{_levelName} cleared!");
    }
}
