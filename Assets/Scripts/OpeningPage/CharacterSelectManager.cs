using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    public static string selectedCharacter;
    public static Sprite selectedSprite;
    public static string selectedCharacterType;

    public void SelectCharacter(string characterName, Sprite sprite)
    {
        selectedCharacter = characterName;
        selectedSprite = sprite;
        SceneManager.LoadScene("StormbreakerIsland");
        Debug.unityLogger.Log("selectedCharacter: " + sprite.name);
        selectedCharacterType = sprite.name.Split('_')[0];
        PlayerManager.playerType = selectedCharacterType;
        Debug.unityLogger.Log("selectedCharacterType: " + selectedCharacterType);
    }
}