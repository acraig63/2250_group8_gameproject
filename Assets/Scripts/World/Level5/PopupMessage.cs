using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DefaultNamespace
{
    public static class PopupMessage
    {
        public static void Show(string text, float duration = 3f)
        {
            GameObject canvasGO = new GameObject("PopupMessageCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            GameObject textGO = new GameObject("PopupText");
            textGO.transform.SetParent(canvasGO.transform, false);

            TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text      = text;
            tmp.fontSize  = 36;
            tmp.color     = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;

            RectTransform rt = textGO.GetComponent<RectTransform>();
            rt.anchorMin        = new Vector2(0f, 0.5f);
            rt.anchorMax        = new Vector2(1f, 0.5f);
            rt.pivot            = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta        = new Vector2(0f, 120f);

            tmp.outlineWidth = 0.2f;
            tmp.outlineColor = Color.black;

            // Auto-destroy timer
            PopupTimer timer = canvasGO.AddComponent<PopupTimer>();
            timer.duration = duration;
        }

        private class PopupTimer : MonoBehaviour
        {
            public float duration;
            private float _elapsed;

            private void Update()
            {
                _elapsed += Time.deltaTime;
                if (_elapsed >= duration)
                    Destroy(gameObject);
            }
        }
    }
}
