using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public string characterName;
    public Sprite characterSprite;

    private CharacterSelectManager manager;

    void Start()
    {
        manager = FindObjectOfType<CharacterSelectManager>();
        GetComponent<Button>().onClick.AddListener(() => 
            manager.SelectCharacter(characterName, characterSprite));
    }
}