using System.Collections;
using UnityEngine;

public class MonkHealerNPC : MonoBehaviour
{
    [Header("Healing")]
    [SerializeField] private int   healAmount     = 20;
    [SerializeField] private float displayDuration = 4f;
    [SerializeField] private float cooldown        = 10f;

    private bool _isOnCooldown = false;

    private readonly string[] _dialogueLines = new string[]
    {
        "Arrr, ye look like ye've been dragged through a storm backwards!\nRest a moment, I'll patch ye up — free of charge, ye scallywag. (+20 HP)",
        "By Davy Jones' medicine cabinet! Ye need healin'!\nHold still, this won't hurt a bit... probably. (+20 HP)",
        "I've seen healthier-lookin' barnacles than you!\nCome 'ere, let the monk work his magic. No gold needed. (+20 HP)",
        "Shiver me timbers, you're a mess!\nThe seas are rough enough — let me mend those wounds before ye sink. (+20 HP)"
    };

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (_isOnCooldown) return;
        StartCoroutine(HealAndShow(other.gameObject));
    }

    private IEnumerator HealAndShow(GameObject player)
    {
        _isOnCooldown = true;

        // Show dialogue via shared panel
        string line = _dialogueLines[Random.Range(0, _dialogueLines.Length)];
        NPCDialoguePanel.Instance?.Show(line, displayDuration);

        // Heal the player
        PlayerController pc = player.GetComponent<PlayerController>();
        PlayerManager    pm = player.GetComponent<PlayerManager>();

        if (pc != null)
        {
            int newHP = Mathf.Min(pc.GetHealth() + healAmount, pc.MaxHealth);
            pc.SetHealth(newHP);
            BattleData.PlayerCurrentHealth = newHP;

            if (pm != null && pm.player != null)
                pm.player.SetCurrentHealth(newHP);

            Debug.Log($"Monk healed player for {healAmount} HP. New HP: {newHP}");
        }

        yield return new WaitForSeconds(displayDuration);
        yield return new WaitForSeconds(cooldown);
        _isOnCooldown = false;
    }
}