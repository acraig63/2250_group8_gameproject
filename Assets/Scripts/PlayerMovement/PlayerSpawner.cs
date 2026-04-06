using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;

    void Start()
    {
        GameObject player = Instantiate(
            CharacterSelectManager.selectedCharacterPrefab,
            spawnPoint.position,
            Quaternion.identity
        );
        
        CameraFollow cam = FindObjectOfType<CameraFollow>();
        if (cam != null)
        {
            cam.target = player.transform;
        }
    }
}