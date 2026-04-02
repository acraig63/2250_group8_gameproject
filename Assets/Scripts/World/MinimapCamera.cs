using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    /// <summary>
    /// Attach to the MinimapCamera GameObject in SmugglersIsland.
    /// Follows the player from above and renders to a RenderTexture.
    /// Start the GameObject INACTIVE — StoryIntroUI activates it after the intro.
    ///
    /// Setup (do once in Unity Editor):
    ///   1. Create Camera GameObject, name it "MinimapCamera"
    ///   2. Attach this script
    ///   3. Create a RenderTexture asset (256×256), name it "MinimapRenderTexture"
    ///   4. Drag the RenderTexture into the Camera component's "Target Texture" field
    ///   5. Also drag it into the "renderTexture" field on THIS script
    ///   6. Set the GameObject inactive in the Hierarchy
    /// </summary>
    public class MinimapCamera : MonoBehaviour
    {
        public static MinimapCamera Instance;

        [Header("Render Texture")]
        [Tooltip("Assign MinimapRenderTexture here. Must also be set on the Camera component.")]
        public RenderTexture renderTexture;

        [Header("Camera Settings")]
        [Tooltip("Z distance above the map plane.")]
        public float heightAboveMap = 20f;

        [Tooltip("Orthographic size — 10 shows ~20 world units of height around the player.")]
        public float orthographicSize = 10f;

        private Camera _cam;

        void Awake()
        {
            // Only valid in SmugglersIsland — destroy immediately in any other scene.
            if (SceneManager.GetActiveScene().name != "SmugglersIsland")
            {
                Destroy(gameObject);
                return;
            }

            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _cam = GetComponent<Camera>();
            if (_cam == null)
            {
                Debug.LogError("MinimapCamera: no Camera component found.");
                return;
            }

            // Always create the RenderTexture at runtime to avoid serialization
            // timing issues when the GameObject starts inactive.
            renderTexture = new RenderTexture(256, 256, 16);
            renderTexture.name = "MinimapRT_Runtime";
            renderTexture.Create();
            _cam.targetTexture = renderTexture;

            // Silence any extra audio listener.
            AudioListener al = GetComponent<AudioListener>();
            if (al != null) al.enabled = false;

            _cam.orthographic     = true;
            _cam.orthographicSize = orthographicSize;
            _cam.depth            = -2;           // render before main camera (depth -1)
            _cam.cullingMask      = ~(1 << 5);    // exclude UI layer (layer 5)
            _cam.clearFlags       = CameraClearFlags.SolidColor;
            _cam.backgroundColor  = new Color(0.05f, 0.05f, 0.08f, 1f);
            _cam.nearClipPlane    = 0.1f;
            _cam.farClipPlane     = heightAboveMap + 50f;
        }

        void Start()
        {
            FollowPlayer();
        }

        void LateUpdate()
        {
            FollowPlayer();
        }

        private Transform _playerTransform;

        private void FollowPlayer()
        {
            if (_playerTransform == null)
            {
                GameObject p = GameObject.FindWithTag("Player");
                if (p != null) _playerTransform = p.transform;
            }

            if (_playerTransform != null)
                transform.position = new Vector3(
                    _playerTransform.position.x,
                    _playerTransform.position.y,
                    -heightAboveMap);
        }
    }
}
