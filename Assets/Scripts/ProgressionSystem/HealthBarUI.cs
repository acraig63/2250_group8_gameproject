using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
    public PlayerManager playerManager;
    public TMP_Text healthText;

    void Update()
    {
        if (playerManager != null && playerManager.player != null)
        {
            slider.value = playerManager.player.GetHealthNormalized();
        }
    }
}