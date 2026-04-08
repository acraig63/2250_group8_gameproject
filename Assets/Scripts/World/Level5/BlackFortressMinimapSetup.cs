using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public static class BlackFortressMinimapSetup
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

            if (SceneManager.GetActiveScene().name == "BlackFortress")
                CreateMinimap();
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "BlackFortress")
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
