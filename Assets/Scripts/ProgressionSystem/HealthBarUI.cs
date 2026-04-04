using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    public Slider slider;
    public TMP_Text healthText;

    void Update()
    {
        float normalized = (float)BattleData.PlayerCurrentHealth / BattleData.PlayerMaxHealth;
        slider.value = normalized;

        if (healthText != null)
            healthText.text = $"HP: {BattleData.PlayerCurrentHealth} / {BattleData.PlayerMaxHealth}";
    }
}