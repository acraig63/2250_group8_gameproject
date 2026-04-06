using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class Level5EndGameHost : MonoBehaviour
    {
        private StoryIntroUI _intro;

        public static void Show()
        {
            if (Object.FindObjectOfType<Level5EndGameHost>() != null) return;
            GameObject go = new GameObject("Level5EndGame");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<Level5EndGameHost>();
        }

        private void Awake()
        {
            // StoryIntroUI requires a Canvas on the same GameObject
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;
            gameObject.AddComponent<CanvasScaler>();
            gameObject.AddComponent<GraphicRaycaster>();

            // Clear ReturningFromBattle so StoryIntroUI.Start() doesn't skip the intro
            BattleData.ReturningFromBattle = false;

            _intro = gameObject.AddComponent<StoryIntroUI>();

            // Set end game text before StoryIntroUI.Start() runs next frame
            FieldInfo fi = typeof(StoryIntroUI).GetField("introText",
                BindingFlags.NonPublic | BindingFlags.Instance);
            string text =
                "Victory!\n\n" +
                "You have defeated Captain Blackwater and rescued your crew from the Blackwater Flagship.\n\n" +
                "The seas are safe once more.\n\n" +
                "Congratulations, you have completed the game!";
            fi?.SetValue(_intro, text);
        }

        private void Update()
        {
            if (_intro != null && !_intro.IsRunning())
            {
                Destroy(gameObject);
                SceneManager.LoadScene("SmugglersIsland");
            }
        }
    }
}
