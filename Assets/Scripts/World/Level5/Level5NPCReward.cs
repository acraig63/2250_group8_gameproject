using UnityEngine;

namespace DefaultNamespace
{
    public class Level5NPCReward : MonoBehaviour
    {
        public string npcName;
        public bool   grantsSpeedBoots = false;
        public bool   dropsKeyPiece    = true;

        private bool _rewardGiven = false;

        private void OnDestroy()
        {
            if (_rewardGiven) return;
            if (!Application.isPlaying) return;
            if (BlackwaterState.defeatedNPCs.Contains(npcName)) return;
            if (!BattleData.ReturningFromBattle || !BattleData.PlayerWon) return;
            if (BattleData.EnemyName != npcName) return;

            _rewardGiven = true;
            BlackwaterState.defeatedNPCs.Add(npcName);

            if (dropsKeyPiece)
                KeyPiecePickup.SpawnKeyPiece(transform.position);

            if (grantsSpeedBoots)
            {
                BlackwaterState.hasSpeedBoots = true;
                PopupMessage.Show("Speed Boots acquired! You can now survive the gauntlet.", 4f);
            }
        }
    }
}
