using UnityEngine;

/// <summary>
/// Attach to any GameObject with a trigger BoxCollider2D to create a hazard zone.
/// When the player walks into the trigger, they take damage per second.
/// IslandSceneBuilder attaches this automatically to the storm hazard zone.
///
/// Manual setup (optional):
///   1. Create an empty GameObject, add BoxCollider2D (set Is Trigger = true)
///   2. Attach this script
///   3. Set damagePerSecond in the Inspector
/// </summary>
public class HazardZone : MonoBehaviour
{
    [Tooltip("Damage dealt to player per second while inside the zone.")]
    public float damagePerSecond = 5f;

    [Tooltip("Label shown in the debug log when player enters.")]
    public string hazardLabel = "Hazard Zone";

    private bool _playerInside = false;
    private GameObject _player;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _playerInside = true;
        _player = other.gameObject;
        Debug.Log($"Player entered {hazardLabel}! Taking {damagePerSecond} damage per second.");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        _playerInside = false;
        _player = null;
        Debug.Log($"Player left {hazardLabel}.");
    }

    void Update()
    {
        if (!_playerInside || _player == null) return;

        // Apply damage via PlayerController's health reference
        // For now, log the damage tick — wire to HealthBar once Player is exposed
        Debug.Log($"{hazardLabel}: dealing {damagePerSecond * Time.deltaTime:F2} damage this frame.");

        // Uncomment once HealthBar is accessible on the player GameObject:
        // HealthBar hb = _player.GetComponent<HealthBar>();
        // if (hb != null) hb.TakeDamage((int)(damagePerSecond * Time.deltaTime));
    }
}
