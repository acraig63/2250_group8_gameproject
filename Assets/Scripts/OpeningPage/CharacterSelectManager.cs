using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    public static GameObject selectedCharacterPrefab;
    public static string selectedCharacter;
    public static string selectedCharacterType;

    public void SelectCharacter(GameObject prefab)
    {
        selectedCharacterPrefab = prefab;

        selectedCharacter = prefab.name;              // MUST match sprite data
        selectedCharacterType = prefab.name;          // same here

        PlayerManager.playerType = selectedCharacterType;

        Debug.Log("Selected: " + selectedCharacter);

        SceneManager.LoadScene("StormbreakerIsland");
    }
}