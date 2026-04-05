using TMPro;
using UnityEngine;

public class TimerDisplay : MonoBehaviour
{
    private TextMeshProUGUI timerText;

    void Awake()
    {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        timerText.text = "Time: " + TimerManager.Instance.GetElapsed().ToString("F2") + "s";
    }
}