using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Attach this to a dedicated Minimap Camera GameObject.
    /// The camera is fixed above the centre of the map and renders the entire
    /// island to a RenderTexture displayed in the MinimapUI panel.
    ///
    /// Setup steps (do once in Unity Editor):
    ///   1. Create a new Camera GameObject — name it "MinimapCamera"
    ///   2. Attach this script to it
    ///   3. Create a RenderTexture asset (right-click in Project → Create → Render Texture)
    ///      Set size to 256x256. Name it "MinimapRenderTexture"
    ///   4. Drag that RenderTexture into the "renderTexture" field on THIS script
    ///   5. Also drag it into the "targetTexture" field of the Camera component itself
    ///      (both assignments ensure it works across DontDestroyOnLoad transitions)
    ///   6. Set orthographicSize to cover half the map height (40 for an 80x60 map)
    /// </summary>
    public class MinimapCamera : MonoBehaviour
    {
        public static MinimapCamera Instance;

        [Header("Render Texture")]
        [Tooltip("Assign the MinimapRenderTexture asset here. The camera will explicitly render into this texture.")]
        public RenderTexture renderTexture;

        [Header("Map Centre")]
        [Tooltip("World-space X of the map centre.")]
        public float mapCenterX = 40f;

        [Tooltip("World-space Y of the map centre.")]
        public float mapCenterY = 30f;

        [Header("Camera Settings")]
        [Tooltip("How far above the map (Z offset in 2D) the minimap camera sits.")]
        public float heightAboveMap = 20f;

        [Tooltip("Orthographic size — set to half the map's longest dimension. 40 covers an 80-unit-wide map.")]
        public float orthographicSize = 40f;

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

            // Explicitly assign target texture in code so it survives DontDestroyOnLoad
            // and is not lost if the serialized reference on the Camera component drops.
            if (renderTexture != null)
                _minimapCamera.targetTexture = renderTexture;
            else
                Debug.LogWarning("MinimapCamera: No RenderTexture assigned — camera will render to screen! " +
                                 "Drag MinimapRenderTexture into the renderTexture field.");

            // Disable any AudioListener — the main camera already has one.
            AudioListener al = GetComponent<AudioListener>();
            if (al != null) al.enabled = false;

            // Force orthographic — minimap should never have perspective distortion.
            _minimapCamera.orthographic = true;
            _minimapCamera.orthographicSize = orthographicSize;

            // Render after the main camera (which is at depth -1).
            _minimapCamera.depth = 1;

            // Exclude the UI layer (layer 5) so minimap UI elements don't render
            // inside the minimap texture itself.
            _minimapCamera.cullingMask = ~(1 << 5);

            // Use a solid dark background so areas outside the tilemap look clean.
            _minimapCamera.clearFlags = CameraClearFlags.SolidColor;
            _minimapCamera.backgroundColor = new Color(0.05f, 0.05f, 0.08f, 1f);

            _minimapCamera.nearClipPlane = 0.1f;
            _minimapCamera.farClipPlane  = heightAboveMap + 50f;

            // Fix the camera above the map centre — it never moves.
            transform.position = new Vector3(mapCenterX, mapCenterY, -heightAboveMap);
        }
    }
}
