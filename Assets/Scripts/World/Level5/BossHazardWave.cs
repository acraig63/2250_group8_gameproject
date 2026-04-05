using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Attached to the Captain Blackwater NPC. After the player returns from
    /// a battle quiz round (detected via BattleData.ReturningFromBattle), spawns
    /// a ring of hazard objects that expands outward.
    ///
    /// DETECTION APPROACH:
    /// Using _playerWasNull to detect "player just appeared" is not viable here
    /// because the scene reloads after each battle, creating a fresh instance of
    /// this component with _playerWasNull = false. Instead we use
    /// BattleData.ReturningFromBattle, which is set to true for exactly one frame
    /// by BattleResultHandler.Start() when the player returns victorious.
    /// Combined with BattleData.EnemyName == "Captain Blackwater" this reliably
    /// identifies the return from a boss battle round.
    ///
    /// Result: a final hazard wave spawns when the player wins the boss fight,
    /// before Level5NPCReward.OnDestroy fires in the next frame.
    /// </summary>
    public class BossHazardWave : MonoBehaviour
    {
        private float   _cooldown     = 0f;
        private bool    _waveThisReturn = false;
        private Vector3 _bossPosition;

        private void Start()
        {
            _bossPosition = transform.position;
        }

        private void Update()
        {
            if (_cooldown > 0f)
            {
                _cooldown -= Time.deltaTime;
                return;
            }

            // Detect player returning victorious from Captain Blackwater battle
            if (!_waveThisReturn &&
                BattleData.ReturningFromBattle &&
                BattleData.EnemyName == "Captain Blackwater")
            {
                _waveThisReturn = true;
                StartCoroutine(SpawnWave());
                _cooldown = 5f;
            }

            // Reset the per-return guard once ReturningFromBattle clears
            if (!BattleData.ReturningFromBattle)
                _waveThisReturn = false;
        }

        private IEnumerator SpawnWave()
        {
            const int   pieceCount = 8;
            const float startRadius = 3f;
            const float endRadius   = 8f;
            const float duration    = 3.5f;

            GameObject waveParent = new GameObject("HazardWave");

            // Build hazard pieces in a circle
            GameObject[] pieces = new GameObject[pieceCount];
            for (int i = 0; i < pieceCount; i++)
            {
                float angle = i * (2f * Mathf.PI / pieceCount);
                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * startRadius,
                    Mathf.Sin(angle) * startRadius,
                    0f);

                Texture2D tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, Color.white);
                tex.Apply();
                Sprite spr = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

                GameObject piece = new GameObject("BossHazard_" + i);
                piece.transform.SetParent(waveParent.transform);
                piece.transform.position   = _bossPosition + offset;
                piece.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

                SpriteRenderer sr = piece.AddComponent<SpriteRenderer>();
                sr.sprite       = spr;
                sr.color        = new Color(1f, 0.3f, 0f, 0.5f);   // semi-transparent red-orange
                sr.sortingOrder = 4;

                BoxCollider2D col = piece.AddComponent<BoxCollider2D>();
                col.isTrigger = true;

                piece.AddComponent<BossHazardPiece>();
                pieces[i] = piece;
            }

            // Expand ring over duration, then destroy
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t      = elapsed / duration;
                float radius = Mathf.Lerp(startRadius, endRadius, t);

                for (int i = 0; i < pieceCount; i++)
                {
                    if (pieces[i] == null) continue;
                    float angle = i * (2f * Mathf.PI / pieceCount);
                    pieces[i].transform.position = _bossPosition + new Vector3(
                        Mathf.Cos(angle) * radius,
                        Mathf.Sin(angle) * radius,
                        0f);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (waveParent != null)
                Destroy(waveParent);
        }
    }

    // ────────────────────────────────────────────────────────────────────────────
    // Individual hazard piece — sends damage to any player that stays inside it.
    // Calls SendMessage("TakeDamage", int) on the player, matching HazardZone's
    // exact signature. Intercepted by PlayerHazardShield if the player is immune.
    // ────────────────────────────────────────────────────────────────────────────
    public class BossHazardPiece : MonoBehaviour
    {
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player") && other.GetComponent<PlayerController>() == null)
                return;

            other.gameObject.SendMessage("TakeDamage", 10, SendMessageOptions.DontRequireReceiver);
        }
    }
}
