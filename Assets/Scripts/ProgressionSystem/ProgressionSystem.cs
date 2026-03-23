using System.Collections.Generic;

namespace DefaultNamespace {
public class ProgressionSystem
{
    private int _currentXP;
    private int _playerLevel;
    private List<string> _keysCollected;
    private List<string> _unlockedAbilities;
    private List<string> _completedLevels;

    public ProgressionSystem()
    {
        
    }
    
    public void AddXP(int amount)
    {
        _currentXP = _currentXP + amount;
    }

    public void CheckLevelUp()
    {
        
    }

    public void AddKey(string levelId)
    {
        _keysCollected.Add(levelId);
    }
    
    public bool IsLevelUnlocked(string levelId)
    {
        return _completedLevels.Contains(levelId);
    }

    public bool HasAllKeys()
    {
        if (_keysCollected.Count == 5)
            return true;
        else
            return false;
    }
}
}