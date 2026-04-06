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

            // Guard: only give rewards when returning from a WON battle against this NPC.
            //
            // PROBLEM THIS SOLVES:
            // When the player touches the NPC, EnemySpawner loads the Battle scene.
            // This causes the current scene (e.g. BlackwaterBrig) to unload, which
            // destroys all objects in it — including this NPC — BEFORE the battle is won.
            // Without this guard, OnDestroy would trigger speed boots / key-piece rewards
            // on the scene unload, not on actual defeat.
            //
            // HOW REWARDS NOW FIRE (correct path):
            // 1. Player wins battle → BattleData.PlayerWon=true, ReturningFromBattle=true
            // 2. BattleResultHandler adds enemy to BattleData.DefeatedEnemies
            // 3. Return scene reloads → EnemySpawner.Start() sees enemy in DefeatedEnemies
            //    → calls Destroy(gameObject) on the freshly-spawned NPC
            // 4. OnDestroy fires here with all BattleData flags set correctly → rewards given
            if (!BattleData.ReturningFromBattle || !BattleData.PlayerWon) return;
            if (BattleData.EnemyName != npcName) return;

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
