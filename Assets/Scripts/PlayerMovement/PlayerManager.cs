using UnityEngine;
using DefaultNamespace;

public class PlayerManager : MonoBehaviour
{
    public Player player;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        string selectedName = CharacterSelectManager.selectedCharacter;
        Sprite selectedSprite = CharacterSelectManager.selectedSprite;

        player = new Player(selectedName, "Pirate");

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null && selectedSprite != null)
        {
            spriteRenderer.sprite = selectedSprite;
        }

        Debug.Log("Spawned player: " + selectedName);
    }
}