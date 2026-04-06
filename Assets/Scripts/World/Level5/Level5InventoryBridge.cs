using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace DefaultNamespace
{
    /// <summary>
    /// Ensures the inventory UI canvas persists across all Blackwater scenes
    /// and is hidden during the Battle scene so it doesn't block quiz buttons.
    /// Call Level5InventoryBridge.EnsureInventoryExists() from BlackwaterSceneBuilder.
    /// </summary>
    public class Level5InventoryBridge : MonoBehaviour
    {
        private static GameObject _inventoryCanvas;

        /// <summary>
        /// Direct reference to the InventoryUI we constructed.
        /// Used by Level5EquipManager to avoid FindObjectOfType returning a
        /// different InventoryUI (e.g. one in the Battle scene) or returning null
        /// when the canvas is inactive.
        /// </summary>
        public static InventoryUI KnownInventoryUI { get; private set; }

        // ── Public entry point ─────────────────────────────────────────────────

        public static void EnsureInventoryExists()
        {
            // Already exists?
            if (UnityEngine.Object.FindObjectOfType<InventoryUI>() != null) return;
            if (_inventoryCanvas != null) return;

            _inventoryCanvas = BuildInventoryCanvas();
            DontDestroyOnLoad(_inventoryCanvas);

            // Add the bridge MonoBehaviour so it can listen to scene changes
            Level5InventoryBridge bridge = _inventoryCanvas.AddComponent<Level5InventoryBridge>();
            SceneManager.sceneLoaded += bridge.OnSceneLoaded;
        }

        // ── Scene visibility ───────────────────────────────────────────────────

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_inventoryCanvas == null) return;

            bool isBattle = scene.name == "Battle" || scene.name == "pirateBattleScene";
            _inventoryCanvas.SetActive(!isBattle);

            // Refresh the playerTransform reference each non-battle scene so the
            // ItemDetailPopup drop-position is always the current player.
            if (!isBattle)
            {
                InventoryUI ui = _inventoryCanvas.GetComponent<InventoryUI>();
                if (ui != null)
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player == null)
                    {
                        PlayerController pc = UnityEngine.Object.FindObjectOfType<PlayerController>();
                        if (pc != null) player = pc.gameObject;
                    }
                    if (player != null)
                    {
                        BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;
                        typeof(InventoryUI).GetField("playerTransform", bf)
                            ?.SetValue(ui, player.transform);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // ── Canvas construction ────────────────────────────────────────────────

        private static GameObject BuildInventoryCanvas()
        {
            // ── Root Canvas ───────────────────────────────────────────────────
            GameObject canvasGO = new GameObject("InventoryCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.scaleFactor = 1f;

            canvasGO.AddComponent<GraphicRaycaster>();

            InventoryUI inventoryUI = canvasGO.AddComponent<InventoryUI>();
            KnownInventoryUI = inventoryUI;   // cache for Level5EquipManager

            // ── InventoryPanel ────────────────────────────────────────────────
            GameObject panelGO = new GameObject("InventoryPanel");
            panelGO.transform.SetParent(canvasGO.transform, false);
            RectTransform panelRect = panelGO.AddComponent<RectTransform>();
            SetRect(panelRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    Vector2.zero, new Vector2(200f, 400f), new Vector2(0.5f, 0.5f));
            Image panelImage = panelGO.AddComponent<Image>();
            panelImage.color = new Color(1f, 1f, 1f, 0.392f);

            // ── GoldText ──────────────────────────────────────────────────────
            TextMeshProUGUI goldText = MakeTMPText(panelGO.transform, "GoldText",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(5f, 70f), new Vector2(200f, 220f), new Vector2(0.5f, 0.5f),
                "Gold: 0", 24, Color.white);

            // ── FullText ──────────────────────────────────────────────────────
            TextMeshProUGUI fullText = MakeTMPText(panelGO.transform, "FullText",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(5f, 45f), new Vector2(200f, 220f), new Vector2(0.5f, 0.5f),
                "Inventory Full!", 24, Color.white);

            // ── ItemSlotsContainer ────────────────────────────────────────────
            GameObject containerGO = new GameObject("ItemSlotsContainer");
            containerGO.transform.SetParent(panelGO.transform, false);
            RectTransform containerRect = containerGO.AddComponent<RectTransform>();
            SetRect(containerRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    Vector2.zero, new Vector2(100f, 240f), new Vector2(0.5f, 0.5f));
            VerticalLayoutGroup vlg = containerGO.AddComponent<VerticalLayoutGroup>();
            vlg.childControlHeight  = false;
            vlg.childControlWidth   = false;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth  = false;

            // ── Slot template (inactive parent keeps it hidden) ───────────────
            GameObject templateHolder = new GameObject("_SlotTemplate");
            templateHolder.transform.SetParent(canvasGO.transform, false);
            templateHolder.SetActive(false);

            GameObject slotTemplate = new GameObject("ItemSlot");
            slotTemplate.transform.SetParent(templateHolder.transform, false);
            Image slotImg = slotTemplate.AddComponent<Image>();
            slotImg.color = Color.white;
            RectTransform slotRect = slotTemplate.GetComponent<RectTransform>();
            SetRect(slotRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    Vector2.zero, new Vector2(100f, 100f), new Vector2(0.5f, 0.5f));

            GameObject slotTextGO = new GameObject("Text (TMP)");
            slotTextGO.transform.SetParent(slotTemplate.transform, false);
            TextMeshProUGUI slotText = slotTextGO.AddComponent<TextMeshProUGUI>();
            slotText.fontSize = 36;
            slotText.color    = Color.white;
            RectTransform slotTextRect = slotTextGO.GetComponent<RectTransform>();
            SetRect(slotTextRect, Vector2.zero, Vector2.one,
                    Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

            // ── ItemDetailPopup ───────────────────────────────────────────────
            GameObject popupGO = new GameObject("ItemDetailPopup");
            popupGO.transform.SetParent(canvasGO.transform, false);
            RectTransform popupRect = popupGO.AddComponent<RectTransform>();
            SetRect(popupRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    Vector2.zero, new Vector2(200f, 400f), new Vector2(0.5f, 0.5f));
            Image popupImage = popupGO.AddComponent<Image>();
            popupImage.color = new Color(0.114f, 0.123f, 0.122f, 1f);
            popupGO.SetActive(false);

            ItemDetailPopup detailPopup = popupGO.AddComponent<ItemDetailPopup>();

            // ItemNameText
            TextMeshProUGUI itemNameText = MakeTMPText(popupGO.transform, "ItemNameText",
                new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(10f, -30f), new Vector2(200f, 50f), new Vector2(0f, 1f),
                "", 16, new Color(0.973f, 0.898f, 0.318f));

            // ItemDescriptionText
            TextMeshProUGUI itemDescText = MakeTMPText(popupGO.transform, "ItemDescriptionText",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(10f, 0f), new Vector2(200f, 125f), new Vector2(0.5f, 0.5f),
                "", 14, Color.white);

            // ItemStatsText
            TextMeshProUGUI itemStatsText = MakeTMPText(popupGO.transform, "ItemStatsText",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(10f, 0f), new Vector2(200f, 75f), new Vector2(0.5f, 0.5f),
                "", 12, Color.white);

            // DropButton
            Button dropButton = MakeButton(popupGO.transform, "DropButton",
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 10f), new Vector2(100f, 20f), new Vector2(0.5f, 0f),
                new Color(0.8f, 0.2f, 0.2f), "Drop");

            // CloseButton
            Button closeButton = MakeButton(popupGO.transform, "CloseButton",
                new Vector2(1f, 1f), new Vector2(1f, 1f),
                Vector2.zero, new Vector2(30f, 30f), new Vector2(1f, 1f),
                new Color(0.962f, 0.059f, 0.059f), "X");

            // ── Wire fields via reflection ─────────────────────────────────────
            BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;
            Type uiType = typeof(InventoryUI);
            uiType.GetField("inventoryPanel",    bf).SetValue(inventoryUI, panelGO);
            uiType.GetField("itemSlotPrefab",    bf).SetValue(inventoryUI, slotTemplate);
            uiType.GetField("itemSlotsContainer",bf).SetValue(inventoryUI, (Transform)containerRect);
            uiType.GetField("goldText",          bf).SetValue(inventoryUI, goldText);
            uiType.GetField("fullText",          bf).SetValue(inventoryUI, fullText);
            uiType.GetField("itemDetailPopup",   bf).SetValue(inventoryUI, detailPopup);
            // playerTransform is set per-scene by OnSceneLoaded

            Type popupType = typeof(ItemDetailPopup);
            popupType.GetField("itemNameText",       bf).SetValue(detailPopup, itemNameText);
            popupType.GetField("itemDescriptionText", bf).SetValue(detailPopup, itemDescText);
            popupType.GetField("itemStatsText",      bf).SetValue(detailPopup, itemStatsText);
            popupType.GetField("dropButton",         bf).SetValue(detailPopup, dropButton);
            popupType.GetField("closeButton",        bf).SetValue(detailPopup, closeButton);

            // ── Dropped-item template ────────────────────────────────────────────
            // ItemDetailPopup.DropItem() instantiates this prefab at the player's
            // feet to create a world pickup. Without it the item just vanishes.
            GameObject dropTemplate = BuildDropTemplate();
            popupType.GetField("itemSpritePrefab", bf)?.SetValue(detailPopup, dropTemplate);

            inventoryUI.Initialize(new Inventory(5));

            return canvasGO;
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private static void SetRect(RectTransform rt,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 anchoredPos, Vector2 sizeDelta, Vector2 pivot)
        {
            rt.anchorMin        = anchorMin;
            rt.anchorMax        = anchorMax;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta        = sizeDelta;
            rt.pivot            = pivot;
        }

        private static TextMeshProUGUI MakeTMPText(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 anchoredPos, Vector2 sizeDelta, Vector2 pivot,
            string text, float fontSize, Color color)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text      = text;
            tmp.fontSize  = fontSize;
            tmp.color     = color;
            tmp.alignment = TextAlignmentOptions.TopLeft;
            RectTransform rt = go.GetComponent<RectTransform>();
            SetRect(rt, anchorMin, anchorMax, anchoredPos, sizeDelta, pivot);
            return tmp;
        }

        private static Button MakeButton(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 anchoredPos, Vector2 sizeDelta, Vector2 pivot,
            Color bgColor, string label)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Image img = go.AddComponent<Image>();
            img.color = bgColor;
            Button btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            RectTransform rt = go.GetComponent<RectTransform>();
            SetRect(rt, anchorMin, anchorMax, anchoredPos, sizeDelta, pivot);

            // Label child
            GameObject labelGO = new GameObject("Text (TMP)");
            labelGO.transform.SetParent(go.transform, false);
            TextMeshProUGUI tmp = labelGO.AddComponent<TextMeshProUGUI>();
            tmp.text      = label;
            tmp.fontSize  = 20;
            tmp.color     = new Color(0.196f, 0.196f, 0.196f);
            tmp.alignment = TextAlignmentOptions.Center;
            RectTransform labelRect = labelGO.GetComponent<RectTransform>();
            SetRect(labelRect, Vector2.zero, Vector2.one,
                    Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

            return btn;
        }

        // ── Dropped-item template ──────────────────────────────────────────────
        // Creates an inactive GameObject that acts as a prefab for
        // ItemDetailPopup.DropItem(). When an item is dropped from inventory,
        // ItemDetailPopup.Instantiate(dropTemplate) clones this object, then
        // calls SetItemData() to configure the clone for the specific item.
        private static GameObject BuildDropTemplate()
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            Sprite spr = Sprite.Create(tex, new Rect(0, 0, 1, 1),
                                       new Vector2(0.5f, 0.5f), 1f);

            GameObject go = new GameObject("DroppedItemTemplate");
            go.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            go.SetActive(false);  // inactive = acts as a prefab
            DontDestroyOnLoad(go);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = spr;
            sr.color        = Color.white;
            sr.sortingOrder = 5;

            CircleCollider2D col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius    = 0.5f;

            go.AddComponent<ItemPickup>();
            return go;
        }
    }
}
