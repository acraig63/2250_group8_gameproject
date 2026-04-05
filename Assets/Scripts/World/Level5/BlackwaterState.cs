using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public static class BlackwaterState
    {
        public static HashSet<string> defeatedNPCs      = new HashSet<string>();
        public static HashSet<string> collectedKeyPieces = new HashSet<string>();
        public static HashSet<string> collectedItems     = new HashSet<string>();

        public static bool  hasSpeedBoots      = false;
        public static bool  hasHazardImmunity  = false;
        public static int   savedHealth        = -1;
        public static float savedSpeed         = -1f;
        public static bool  hasSavedState      = false;

        public static bool HasAllKeyPieces() => collectedKeyPieces.Count >= 5;

        public static void SavePlayerState()
        {
            PlayerController pc = Object.FindObjectOfType<PlayerController>();
            if (pc == null) return;

            savedHealth   = pc.GetHealth();
            savedSpeed    = pc.speed;
            hasSavedState = true;
        }

        public static void LoadPlayerState()
        {
            if (!hasSavedState) return;

            PlayerController pc = Object.FindObjectOfType<PlayerController>();
            if (pc == null) return;

            pc.SetHealth(savedHealth);
            pc.speed = savedSpeed;
        }

        public static void Reset()
        {
            defeatedNPCs.Clear();
            collectedKeyPieces.Clear();
            collectedItems.Clear();

            hasSpeedBoots     = false;
            hasHazardImmunity = false;
            savedHealth       = -1;
            savedSpeed        = -1f;
            hasSavedState     = false;
        }
    }
}
