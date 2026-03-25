using UnityEngine;
using DefaultNamespace;

public class PlayerManager : MonoBehaviour
{
    public Player player;

    void Start()
    {
        player = new Player("Rory", "Pirate");
    }

    void Update()
    {
        
    }
}