namespace ProgressionSystem {
public class ProgressionSystem
{
    private int _currentXP;
    private int _playerLevel;
    private List<string> _keysCollected;
    private List<string> _unlockedAbilities;
    private List<string> _completedLevels;

    public void AddXP(int amount)
    {
        _currentXP = _currentXP + amount;
    }

    public void CheckLevelUp()
    {
        
    }

    public void AddKey(string levelId)
    {
        _keysCollected.add(levelId);
    }

    public bool HasAllKeys()
    {
        if (_keysCollected.length == 5) ;
    }
}
}