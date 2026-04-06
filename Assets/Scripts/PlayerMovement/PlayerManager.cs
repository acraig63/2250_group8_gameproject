using System.Collections;
using UnityEngine;
using DefaultNamespace;

public class PlayerManager : MonoBehaviour
{
    public Player player;

    public static string playerType = "Archer";

    void Start()
    {
        string selectedName = CharacterSelectManager.selectedCharacter;

        player = new Player(selectedName, "Pirate");

        Debug.Log($"HasReturnPosition: {BattleData.HasReturnPosition}, ReturningFromBattle: {BattleData.ReturningFromBattle}, Position: {BattleData.ReturnPlayerPosition}");

        if (BattleData.HasReturnPosition)
        {
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