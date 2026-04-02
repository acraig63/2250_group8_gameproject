using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Attach this to a dedicated Minimap Camera GameObject.
    /// The camera follows the player from above and renders to a RenderTexture
    /// which is displayed in the MinimapUI panel in the top-left corner.
    ///
    /// Setup steps (do once in Unity Editor):
    ///   1. Create a new Camera GameObject — name it "MinimapCamera"
    ///   2. Attach this script to it
    ///   3. Drag your Player GameObject into the "playerTransform" field in the Inspector
    ///   4. Create a RenderTexture asset (Project panel → right-click → Create → Render Texture)
    ///      Set size to 256x256. Name it "MinimapRenderTexture"
    ///   5. Drag that RenderTexture into the "targetTexture" field of the MinimapCamera's
    ///      Camera component (not this script — the Camera component itself)
    ///   6. Set the MinimapCamera's Culling Mask to whatever layers you want shown
    ///   7. Also drag the same RenderTexture into the "minimapRenderTexture" field on this script
    /// </summary>
    public class MinimapCamera : MonoBehaviour
    {
        public static MinimapCamera Instance;

        [Header("Target")]
        [Tooltip("Drag your Player GameObject here.")]
        public Transform playerTransform;

        [Header("Camera Settings")]
        [Tooltip("How high above the player the minimap camera sits (orthographic size controls zoom).")]
        public float heightAbovePlayer = 20f;

        [Tooltip("Orthographic size — smaller = more zoomed in. 10 is a good starting point.")]
        public float orthographicSize = 10f;

        private Camera _minimapCamera;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _minimapCamera = GetComponent<Camera>();

            if (_minimapCamera == null)
            {
                Debug.LogError("MinimapCamera: No Camera component found on this GameObject.");
                return;
            }

            // Disable any AudioListener — the main camera already has one and Unity
            // will warn about duplicates if this camera also has one active.
            AudioListener al = GetComponent<AudioListener>();
            if (al != null) al.enabled = false;

            // Force orthographic — minimap should never have perspective distortion
            _minimapCamera.orthographic = true;
            _minimapCamera.orthographicSize = orthographicSize;

            // Render on top of everything else but after the main camera
            _minimapCamera.depth = 1;

            // Include all layers so Default-layer tilemaps are always visible.
            // Restrict this in the Inspector if you need to hide specific layers.
            _minimapCamera.cullingMask = ~0;

            // In Unity 2D the camera sits at a negative Z and everything rendered
            // is at Z = 0.  The default near clip (0.3) is fine, but make sure
            // the far clip reaches well past the Z offset we apply in LateUpdate.
            _minimapCamera.nearClipPlane = 0.1f;
            _minimapCamera.farClipPlane  = heightAbovePlayer + 50f;
        }

        void Start()
        {
            if (playerTransform == null)
            {
                // Attempt to find the player automatically by tag as a fallback
                GameObject playerObj = GameObject.FindWithTag("Player");
                if (playerObj != null)
                {
                    playerTransform = playerObj.transform;
                    Debug.Log("MinimapCamera: Player found automatically via 'Player' tag.");
                }
                else
                {
                    Debug.LogWarning("MinimapCamera: No player assigned and none found with tag 'Player'. " +
                                     "Drag the Player into the playerTransform field in the Inspector.");
                }
            }
        }

        // LateUpdate runs after all movement scripts — ensures camera never lags behind player
        void LateUpdate()
        {
            // Re-find player after scene transitions (DontDestroyOnLoad means Start won't re-run)
            if (playerTransform == null)
            {
                GameObject playerObj = GameObject.FindWithTag("Player");
                if (playerObj != null)
                    playerTransform = playerObj.transform;
                return;
            }

            // Unity 2D: the scene sits at Z = 0 and the camera must look down the
            // negative-Z axis.  heightAbovePlayer becomes a Z offset, NOT a Y offset.
            transform.position = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y,
                playerTransform.position.z - heightAbovePlayer
            );
        }
    }
}
