using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    public static string selectedCharacter;
    public static Sprite selectedSprite;

    public void SelectCharacter(string characterName, Sprite sprite)
    {
        selectedCharacter = characterName;
        selectedSprite = sprite;
        SceneManager.LoadScene("SmugglersIsland");
    }
}