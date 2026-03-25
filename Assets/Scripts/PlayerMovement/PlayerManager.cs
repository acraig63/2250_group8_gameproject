using UnityEngine;
using DefaultNamespace;

public class PlayerManager : MonoBehaviour
{
    public Player player;

    void Start()
    {
        string characterName = CharacterSelectManager.selectedCharacter;
        player = new Player(characterName, "Pirate");
    
        Debug.Log("Selected sprite: " + CharacterSelectManager.selectedSprite);
        GetComponent<SpriteRenderer>().sprite = CharacterSelectManager.selectedSprite;
    }

    void Update()
    {
        
    }
}