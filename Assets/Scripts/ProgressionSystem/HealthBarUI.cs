using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
    public PlayerManager playerManager;

    void Update()
    {
        if (playerManager != null && playerManager.player != null)
        {
            slider.value = playerManager.player.GetHealthNormalized();
        }
    }
}