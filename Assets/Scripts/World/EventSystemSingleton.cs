using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    /// <summary>
    /// Automatically removes duplicate EventSystem objects after every scene load.
    ///
    /// Unity adds one EventSystem per scene. When scenes are loaded in sequence
    /// (OpeningScreen → CharacterCustomization → SmugglersIsland) the default
    /// non-additive LoadScene destroys the previous scene, but DontDestroyOnLoad
    /// objects can occasionally carry stale EventSystems across. This static
    /// bootstrap runs after every scene load and destroys all but the first
    /// EventSystem it finds, keeping exactly one active at all times.
    ///
    /// No GameObject or component setup required — the [RuntimeInitializeOnLoadMethod]
    /// attribute makes Unity invoke this automatically.
    /// </summary>
    public static class EventSystemSingleton
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void DeduplicateEventSystems()
        {
            EventSystem[] all = Object.FindObjectsOfType<EventSystem>();
            // Keep the first one; destroy every extra instance
            for (int i = 1; i < all.Length; i++)
            {
                Debug.LogWarning("[EventSystemSingleton] Destroying duplicate EventSystem: "
                    + all[i].gameObject.name);
                Object.Destroy(all[i].gameObject);
            }
        }
    }
}
