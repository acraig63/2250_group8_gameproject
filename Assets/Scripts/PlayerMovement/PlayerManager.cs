using System.Collections;
using UnityEngine;
using DefaultNamespace;

public class PlayerManager : MonoBehaviour
{
    public Player player;
    private GameObject _spawnedPlayer;

    public static string playerType = "Archer";

    void Start()
    {
        string selectedName = CharacterSelectManager.selectedCharacter;

        player = new Player(selectedName, "Pirate");

        Debug.Log("Spawned player: " + selectedName);
    }
}