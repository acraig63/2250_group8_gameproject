using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class BattleAnimationController : MonoBehaviour
{
    [Header("Animation Frames")]
    [Tooltip("Drag the sliced frames from Archer_Idle.png in order")]
    [SerializeField] private Sprite[] idleFrames;

    [Tooltip("Drag the sliced frames from Archer_Shoot.png in order")]
    [SerializeField] private Sprite[] attackFrames;

    [Header("Settings")]
    [SerializeField] private int fps = 8;

    // ── Internal state ────────────────────────────────────────────────
    private Image       _image;
    private bool        _isAttacking = false;
    private bool        _isDead      = false;
    private int         _idleFrame   = 0;
    private float       _frameTimer  = 0f;

    private Vector3    _startPos;
    private Quaternion _startRot;

    void Awake()
    {
        _image    = GetComponent<Image>();
        _startPos = transform.localPosition;
        _startRot = transform.localRotation;
    }

    void Start()
    {
        // Start idle loop
        if (idleFrames != null && idleFrames.Length > 0)
            _image.sprite = idleFrames[0];
    }

    void Update()
    {
        if (_isDead || _isAttacking) return;
        if (idleFrames == null || idleFrames.Length == 0) return;

        _frameTimer += Time.deltaTime;
        float frameInterval = 1f / fps;

        if (_frameTimer >= frameInterval)
        {
            _frameTimer = 0f;
            _idleFrame  = (_idleFrame + 1) % idleFrames.Length;
            _image.sprite = idleFrames[_idleFrame];
        }
    }

    // ── Public methods called by BattleManager ────────────────────────

    /// <summary>Plays the attack animation once then returns to idle.</summary>
    public IEnumerator PlayAttack()
    {
        if (attackFrames == null || attackFrames.Length == 0) yield break;

        _isAttacking = true;
        float frameInterval = 1f / fps;

        foreach (Sprite frame in attackFrames)
        {
            _image.sprite = frame;
            yield return new WaitForSeconds(frameInterval);
        }

        _isAttacking  = false;
        _idleFrame    = 0;
        _frameTimer   = 0f;
        if (idleFrames != null && idleFrames.Length > 0)
            _image.sprite = idleFrames[0];
    }

    /// <summary>Jerks the sprite backwards to show it was hit.</summary>
    public IEnumerator PlayHit(bool isPlayer)
    {
        float direction = isPlayer ? -30f : 30f;
        Vector3 hitPos  = _startPos + new Vector3(direction, 0f, 0f);
        float   duration = 0.08f;
        float   elapsed  = 0f;

        // Jerk backwards
        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(_startPos, hitPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return
        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(hitPos, _startPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = _startPos;
    }

    /// <summary>Slowly rotates the sprite to simulate falling over on death.</summary>
    public IEnumerator PlayDeath()
    {
        _isDead = true;
        float    duration  = 1.0f;
        float    elapsed   = 0f;
        Quaternion deadRot = Quaternion.Euler(0f, 0f, -90f);

        while (elapsed < duration)
        {
            transform.localRotation = Quaternion.Lerp(_startRot, deadRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = deadRot;
    }

    /// <summary>
    /// Call this to override the idle frames at runtime — used to set the
    /// correct colour variant based on the selected character.
    /// </summary>
    public void SetFrames(Sprite[] idle, Sprite[] attack)
    {
        idleFrames   = idle;
        attackFrames = attack;
        _idleFrame   = 0;
        _frameTimer  = 0f;
        if (_image != null && idleFrames != null && idleFrames.Length > 0)
            _image.sprite = idleFrames[0];
    }
}