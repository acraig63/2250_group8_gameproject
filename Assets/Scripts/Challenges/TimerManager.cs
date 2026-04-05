using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;
    public float timeRemaining;
    private float countdownTime = 120f;
    private bool isRunning = false;
    public Transform playerTransform;

    void Awake() { Instance = this; }

    public void StartTimer()
    {
        timeRemaining = countdownTime;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetElapsed()
    {
        return timeRemaining;
    }

    void Update()
    {
        if (isRunning)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isRunning = false;
                playerTransform.position = new Vector2(11, -11);
            }
        }
    }
}
