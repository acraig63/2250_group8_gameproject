using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public static class CharacterDisableManager
    {
        private static readonly HashSet<string> _disabled = new HashSet<string>();
        private static int _totalCharacters = 0;

        // Called by CharacterSelectEnforcer after scanning CharacterButton objects
        public static void RegisterTotal(int total) => _totalCharacters = total;

        public static void DisableCharacter(string characterName) =>
            _disabled.Add(characterName);

        public static bool IsDisabled(string characterName) =>
            _disabled.Contains(characterName);

        public static bool AllDisabled() =>
            _totalCharacters > 0 && _disabled.Count >= _totalCharacters;

        public static int RemainingCharacters() =>
            Mathf.Max(0, _totalCharacters - _disabled.Count);

        public static void ResetAll() => _disabled.Clear();
    }
}
