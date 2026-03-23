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
            _minimapCamera = GetComponent<Camera>();

            if (_minimapCamera == null)
            {
                Debug.LogError("MinimapCamera: No Camera component found on this GameObject.");
                return;
            }

            // Force orthographic — minimap should never have perspective distortion
            _minimapCamera.orthographic = true;
            _minimapCamera.orthographicSize = orthographicSize;

            // Render on top of everything else but after the main camera
            _minimapCamera.depth = 1;
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
            if (playerTransform == null) return;

            // Follow the player's X and Z position, stay at a fixed height above them
            transform.position = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y + heightAbovePlayer,
                playerTransform.position.z
            );
        }
    }
}
