using UnityEngine;

[System.Serializable]
public class CharacterSpriteEntry
{
    [Tooltip("Must match CharacterSelectManager.selectedCharacter exactly")]
    public string characterName;
    public Sprite[] idleFrames;
    public Sprite[] attackFrames;
}

public class CharacterSpriteData : MonoBehaviour
{
    [SerializeField] private CharacterSpriteEntry[] entries;

    /// <summary>
    /// Returns the idle and attack frames for the given character name.
    /// Returns null if not found.
    /// </summary>
    public bool TryGetFrames(string characterName, out Sprite[] idle, out Sprite[] attack)
    {
        foreach (CharacterSpriteEntry entry in entries)
        {
            if (entry.characterName == characterName)
            {
                idle   = entry.idleFrames;
                attack = entry.attackFrames;
                return true;
            }
        }
        idle   = null;
        attack = null;
        return false;
    }
}