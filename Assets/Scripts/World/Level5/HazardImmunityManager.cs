using UnityEngine;

namespace DefaultNamespace
{
    public static class HazardImmunityManager
    {
        private static bool _isImmune = false;

        public static bool IsImmune     => _isImmune;
        public static void SetImmune(bool value) => _isImmune = value;
        public static void Reset()               => _isImmune = false;
    }

    public class PlayerHazardShield : MonoBehaviour
    {
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
