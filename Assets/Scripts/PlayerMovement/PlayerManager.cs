using UnityEngine;
using DefaultNamespace;

public class PlayerManager : MonoBehaviour
{
    public Player player;
    private SpriteRenderer spriteRenderer;

    public static string playerType = "Archer";


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
        if (BattleData.HasReturnPosition)
        {
            transform.position = BattleData.ReturnPlayerPosition;
            BattleData.HasReturnPosition = false;
        }

        Debug.Log("Spawned player: " + selectedName);
    }
}