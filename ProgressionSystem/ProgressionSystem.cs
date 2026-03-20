namespace DefaultNamespace;

public class ProgressionSystem
{
    private int _currentXP;
    private int _playerLevel;
    private List<String> _keysCollected;
    private List<String> _unlockedAbilities;
    private List<String> _completedLevels;

    public void AddXP(int amount)
    {
        _currentXP = _currentXP + amount;
    }

    public void CheckLevelUp()
    {
        
    }

    public void AddKey(String levelId)
    {
        _keysCollected.add(levelId);
    }

    public bool HasAllKeys()
    {
        if(_keysCollected.length == 5)
    }
}