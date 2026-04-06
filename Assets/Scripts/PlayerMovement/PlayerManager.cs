using System.Collections;
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

        Debug.Log($"HasReturnPosition: {BattleData.HasReturnPosition}, ReturningFromBattle: {BattleData.ReturningFromBattle}, Position: {BattleData.ReturnPlayerPosition}");
        if (BattleData.HasReturnPosition) //  && BattleData.ReturningFromBattle) 
        {
            //transform.position = BattleData.ReturnPlayerPosition;
            StartCoroutine(RestoreReturnPositionNextFrame());
            BattleData.HasReturnPosition = false;
            BattleData.ReturningFromBattle = false;
        }

        Debug.Log("Spawned player: " + selectedName);
    }
    
    private IEnumerator RestoreReturnPositionNextFrame()
    {
        yield return null;

        Debug.Log("Restoring player to return position: " + BattleData.ReturnPlayerPosition);

        transform.position = BattleData.ReturnPlayerPosition;
        BattleData.HasReturnPosition = false;
        BattleData.ReturningFromBattle = false;
    }
}