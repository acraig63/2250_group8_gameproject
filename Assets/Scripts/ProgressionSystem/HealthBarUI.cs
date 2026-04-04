using UnityEngine;
using UnityEngine.UI;
<<<<<<< HEAD
=======
using TMPro;
>>>>>>> mike-level

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
<<<<<<< HEAD
    public PlayerManager playerManager;

    void Update()
    {
        if (playerManager != null && playerManager.player != null)
        {
            slider.value = playerManager.player.GetHealthNormalized();
        }
=======
    public TMP_Text healthText;

    void Update()
    {
        float normalized = (float)BattleData.PlayerCurrentHealth / BattleData.PlayerMaxHealth;
        slider.value = normalized;

        if (healthText != null)
            healthText.text = $"HP: {BattleData.PlayerCurrentHealth} / {BattleData.PlayerMaxHealth}";
>>>>>>> mike-level
    }
}