using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Attached to each NPC GameObject. Fires rewards when BattleResultHandler calls
    /// EnemySpawner.OnPlayerWon(), which ultimately calls Destroy(gameObject).
    /// </summary>
    public class Level5NPCReward : MonoBehaviour
    {
        public string npcName;
        public bool   grantsSpeedBoots = false;
        public bool   dropsKeyPiece    = true;

        private bool _rewardGiven = false;

        private void OnDestroy()
        {
            // Guard: already gave rewards this session
            if (_rewardGiven) return;

            // Guard: prevents false triggers during scene teardown in the editor
            if (!Application.isPlaying) return;

            // Guard: NPC was already defeated in a previous scene visit — just cleaning up
            if (BlackwaterState.defeatedNPCs.Contains(npcName)) return;

            _rewardGiven = true;
            BlackwaterState.defeatedNPCs.Add(npcName);

            if (dropsKeyPiece)
            {
                KeyPiecePickup.SpawnKeyPiece(transform.position);
                int count = BlackwaterState.collectedKeyPieces.Count;
                PopupMessage.Show(npcName + " defeated! Key piece acquired. (" + count + "/5)", 3f);
            }
            else
            {
                PopupMessage.Show(npcName + " defeated!", 3f);
            }

            if (grantsSpeedBoots)
            {
                BlackwaterState.hasSpeedBoots = true;
                PopupMessage.Show("Speed Boots acquired! You can now survive the gauntlet.", 4f);
            }
        }
    }
}
