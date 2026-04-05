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

        [Tooltip("Orthographic size — 20 shows ~40 world units of height around the player (~half the map).")]
        public float orthographicSize = 20f;

        private Camera _cam;


        // Scenes that use the minimap system.
        private static bool IsMinimapScene(string s)
            => s == "SmugglersIsland" || s.StartsWith("Blackwater");

        // void Awake()
        // {
        //     // Only valid in minimap-enabled scenes — destroy immediately elsewhere.
        //     if (!IsMinimapScene(SceneManager.GetActiveScene().name))

        void Awake()
        {
            // Only valid in minimap-enabled scenes — destroy immediately elsewhere.
            if (!IsMinimapScene(SceneManager.GetActiveScene().name))
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

            // Disable HDR on the minimap camera before creating the RT — HDR mode
            // causes URP to use a floating-point RT internally, which produces a
            // format mismatch with a standard ARGB32 RT and prevents output from
            // being written. Must be set BEFORE assigning targetTexture.
            _cam.allowHDR = false;

            // Create the RenderTexture at runtime with an explicit ARGB32 format so
            // URP writes alpha correctly. Depth=16 is sufficient for a 2D scene.
            renderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
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
            // Use transparent clear so areas with no tiles are see-through rather
            // than a solid colour that bleeds through the UI RawImage.
            _cam.backgroundColor  = Color.clear;
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
