using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DefaultNamespace
{
    public static class PopupMessage
    {
        private static readonly Queue<(string msg, float dur)> _queue =
            new Queue<(string, float)>();
        private static bool _isShowing = false;
        private static PopupHost _host;

        public static void Show(string text, float duration = 3f)
        {
            // Skip exact duplicates already waiting
            foreach (var (m, _) in _queue)
                if (m == text) return;

            if (_queue.Count >= 5) return;

            _queue.Enqueue((text, duration));
            EnsureHost();
            if (!_isShowing)
                _host.StartCoroutine(_host.ShowNext());
        }

        private static void EnsureHost()
        {
            if (_host != null) return;
            _isShowing = false;
            var go = new GameObject("PopupMessageHost");
            Object.DontDestroyOnLoad(go);
            _host = go.AddComponent<PopupHost>();
        }

        private class PopupHost : MonoBehaviour
        {
            public IEnumerator ShowNext()
            {
                _isShowing = true;
                while (_queue.Count > 0)
                {
                    var (msg, dur) = _queue.Dequeue();

                    GameObject canvasGO = new GameObject("PopupMessageCanvas");
                    Canvas canvas = canvasGO.AddComponent<Canvas>();
                    canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
                    canvas.sortingOrder = 100;
                    canvasGO.AddComponent<CanvasScaler>();
                    canvasGO.AddComponent<GraphicRaycaster>();

                    GameObject textGO = new GameObject("PopupText");
                    textGO.transform.SetParent(canvasGO.transform, false);
                    TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
                    tmp.text         = msg;
                    tmp.fontSize     = 36;
                    tmp.color        = Color.white;
                    tmp.alignment    = TextAlignmentOptions.Center;
                    tmp.outlineWidth = 0.2f;
                    tmp.outlineColor = Color.black;

                    RectTransform rt = textGO.GetComponent<RectTransform>();
                    rt.anchorMin        = new Vector2(0f, 0.5f);
                    rt.anchorMax        = new Vector2(1f, 0.5f);
                    rt.pivot            = new Vector2(0.5f, 0.5f);
                    rt.anchoredPosition = Vector2.zero;
                    rt.sizeDelta        = new Vector2(0f, 120f);

                    yield return new WaitForSeconds(dur);
                    if (canvasGO != null) Destroy(canvasGO);
                    yield return new WaitForSeconds(0.3f);
                }
                _isShowing = false;
            }
        }
    }
}
