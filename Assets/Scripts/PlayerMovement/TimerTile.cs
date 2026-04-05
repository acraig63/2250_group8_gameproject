using UnityEngine;

public class TimerTile : MonoBehaviour
{
    public bool isStartTile;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something entered: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit tile, isStartTile: " + isStartTile);
            if (isStartTile)
                TimerManager.Instance.StartTimer();
            else
                TimerManager.Instance.StopTimer();
        }
    }
}
