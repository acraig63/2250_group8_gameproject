using UnityEngine;

namespace DefaultNamespace
{
    // ============================================================
    // HazardImmunityManager — static state manager
    // ============================================================

    /// <summary>
    /// Tracks whether the player has hazard immunity (granted by GauntletReward).
    /// Reset() is called by BlackwaterState.Reset() on death.
    /// </summary>
    public static class HazardImmunityManager
    {
        private static bool _isImmune = false;

        public static bool IsImmune     => _isImmune;
        public static void SetImmune(bool value) => _isImmune = value;
        public static void Reset()               => _isImmune = false;
    }

    // ============================================================
    // PlayerHazardShield — MonoBehaviour on the player
    // ============================================================

    /// <summary>
    /// Attached to the player to intercept HazardZone's SendMessage("TakeDamage", int).
    ///
    /// APPROACH CHOSEN:
    /// PlayerController does NOT have a TakeDamage(int) method (confirmed by reading
    /// PlayerController.cs). Therefore PlayerHazardShield is the sole handler for
    /// SendMessage("TakeDamage"), with no conflict or double-call risk.
    ///
    /// HazardZone sends:
    ///   _playerObject.SendMessage("TakeDamage", damagePerTick, DontRequireReceiver)
    /// where damagePerTick is int (confirmed from HazardZone.cs).
    ///
    /// Interception logic:
    ///   - If immune: absorb all damage (return without modifying health).
    ///   - Otherwise: apply DEF bonus and call SetHealth with the reduced value.
    ///
    /// Defense floor rule: actualDamage = Mathf.Max(0, damage - DEF). Never negative.
    /// </summary>
    public class PlayerHazardShield : MonoBehaviour
    {
        // Called by HazardZone.SendMessage("TakeDamage", int)
        // Parameter type matches HazardZone.damagePerTick (int).
        private void TakeDamage(int damage)
        {
            if (HazardImmunityManager.IsImmune) return;

            PlayerController pc = GetComponent<PlayerController>();
            if (pc == null) return;

            int def          = Level5EquipManager.GetTotalBonusDEF();
            int actualDamage = Mathf.Max(0, damage - def);

            pc.SetHealth(pc.GetHealth() - actualDamage);
        }

        public static void AttachToPlayer(GameObject player)
        {
            if (player.GetComponent<PlayerHazardShield>() != null) return;
            player.AddComponent<PlayerHazardShield>();
        }
    }
}
