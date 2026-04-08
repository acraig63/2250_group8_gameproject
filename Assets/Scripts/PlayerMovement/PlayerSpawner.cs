using System.Collections;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;

    void Start()
    {
        Vector3 spawnPos = spawnPoint.position;

        GameObject player = Instantiate(
            CharacterSelectManager.selectedCharacterPrefab,
            spawnPos,
            Quaternion.identity
        );

        // If returning from won battle, move player after one frame
        // (one frame delay lets all Start() methods finish first)
        if (BattleData.HasReturnPosition)
            StartCoroutine(MovePlayerNextFrame(player));

        CameraFollow cam = FindObjectOfType<CameraFollow>();
        if (cam != null)
            cam.target = player.transform;
    }

    private IEnumerator MovePlayerNextFrame(GameObject player)
    {
        yield return null;
        
        Debug.Log($"PlayerSpawner moving player to {BattleData.ReturnPlayerPosition}");
        player.transform.position = BattleData.ReturnPlayerPosition;
        BattleData.HasReturnPosition = false;
    }
}