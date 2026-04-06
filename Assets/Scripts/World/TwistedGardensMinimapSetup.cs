using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    /// <summary>
    /// Creates a minimap for the TwistedGardens scene.
    /// TimerManager.cs (a teammate file) does not set up a minimap, so we use
    /// RuntimeInitializeOnLoadMethod to hook into scene loads independently.
    ///
    /// Mirrors the exact pattern used by BlackwaterSceneBuilder.SetupMinimap().
    /// </summary>
    public static class TwistedGardensMinimapSetup
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Handle the case where the game started directly in TwistedGardens.
            if (SceneManager.GetActiveScene().name == "TwistedGardens")
                CreateMinimap();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "TwistedGardens")
                CreateMinimap();
        }

        private static void CreateMinimap()
        {
            if (MinimapCamera.Instance == null)
            {
                var camGO = new GameObject("MinimapCamera");
                camGO.AddComponent<Camera>();
                var mc = camGO.AddComponent<MinimapCamera>();
                mc.orthographicSize = 20f;
                // allowHDR set to false inside MinimapCamera.Awake() before RT creation.
                camGO.GetComponent<Camera>().orthographicSize = 20f;
            }

            if (MinimapUI.Instance == null)
            {
                var canvasGO = new GameObject("MinimapCanvas");
                var canvas   = canvasGO.AddComponent<Canvas>();
                canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 10;
                canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                canvasGO.AddComponent<MinimapUI>();
            }
        }
    }
}
