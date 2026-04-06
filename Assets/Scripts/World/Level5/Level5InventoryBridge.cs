using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace DefaultNamespace
{
    public class Level5InventoryBridge : MonoBehaviour
    {
        private static GameObject _inventoryCanvas;

        public static InventoryUI KnownInventoryUI { get; private set; }

        private static Inventory _savedInventory;

        private static readonly FieldInfo _inventoryField =
            typeof(InventoryUI).GetField("_inventory",
                BindingFlags.NonPublic | BindingFlags.Instance);

        public static void EnsureInventoryExists()
        {
            if (UnityEngine.Object.FindObjectOfType<InventoryUI>() != null) return;
            if (_inventoryCanvas != null) return;

            _inventoryCanvas = BuildInventoryCanvas();
            DontDestroyOnLoad(_inventoryCanvas);

            Level5InventoryBridge bridge = _inventoryCanvas.AddComponent<Level5InventoryBridge>();
            SceneManager.sceneLoaded += bridge.OnSceneLoaded;
        }

        private void Start()
        {
            // Bind player transform on initial scene load (sceneLoaded doesn't fire for first load)
            if (_inventoryCanvas == null) return;
            string sceneName = SceneManager.GetActiveScene().name;
            bool isBattle = sceneName == "Battle" || sceneName == "pirateBattleScene";
            if (isBattle) return;

            InventoryUI ui = _inventoryCanvas.GetComponent<InventoryUI>();
            if (ui == null) return;

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

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_inventoryCanvas == null) return;

            bool isBattle = scene.name == "Battle" || scene.name == "pirateBattleScene";
            _inventoryCanvas.SetActive(!isBattle);

            if (!isBattle)
            {
                StartCoroutine(RestoreInventoryAfterDelay());

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

        private IEnumerator RestoreInventoryAfterDelay()
        {
            // wait for PlayerController.Start() to wipe inventory
            yield return null;
            yield return null;

            if (KnownInventoryUI == null || _savedInventory == null) yield break;
            if (_inventoryField == null) yield break;

            _inventoryField.SetValue(KnownInventoryUI, _savedInventory);
            KnownInventoryUI.RefreshUI();

            Debug.Log("[Level5InventoryBridge] Restored inventory (" +
                      _savedInventory.Items.Count + " items) after scene load.");
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private static GameObject BuildInventoryCanvas()
        {
            GameObject canvasGO = new GameObject("InventoryCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;

            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            scaler.scaleFactor = 1f;

            canvasGO.AddComponent<GraphicRaycaster>();

            InventoryUI inventoryUI = canvasGO.AddComponent<InventoryUI>();
            KnownInventoryUI = inventoryUI;

            GameObject panelGO = new GameObject("InventoryPanel");
            panelGO.transform.SetParent(canvasGO.transform, false);
            RectTransform panelRect = panelGO.AddComponent<RectTransform>();
            SetRect(panelRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    Vector2.zero, new Vector2(200f, 400f), new Vector2(0.5f, 0.5f));
            Image panelImage = panelGO.AddComponent<Image>();
            panelImage.color = new Color(1f, 1f, 1f, 0.392f);
            panelGO.AddComponent<RectMask2D>();

            TextMeshProUGUI goldText = MakeTMPText(panelGO.transform, "GoldText",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(5f, 70f), new Vector2(200f, 220f), new Vector2(0.5f, 0.5f),
                "Gold: 0", 24, Color.white);

            TextMeshProUGUI fullText = MakeTMPText(panelGO.transform, "FullText",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(5f, 45f), new Vector2(200f, 220f), new Vector2(0.5f, 0.5f),
                "Inventory Full!", 24, Color.white);

            GameObject containerGO = new GameObject("ItemSlotsContainer");
            containerGO.transform.SetParent(panelGO.transform, false);
            RectTransform containerRect = containerGO.AddComponent<RectTransform>();
            SetRect(containerRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    Vector2.zero, new Vector2(180f, 240f), new Vector2(0.5f, 0.5f));
            VerticalLayoutGroup vlg = containerGO.AddComponent<VerticalLayoutGroup>();
            vlg.childControlHeight     = false;
            vlg.childControlWidth      = true;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth  = true;
            vlg.spacing = 5f;

            GameObject templateHolder = new GameObject("_SlotTemplate");
            templateHolder.transform.SetParent(canvasGO.transform, false);
            templateHolder.SetActive(false);

            GameObject slotTemplate = new GameObject("ItemSlot");
            slotTemplate.transform.SetParent(templateHolder.transform, false);
            Image slotImg = slotTemplate.AddComponent<Image>();
            slotImg.color = Color.white;
            RectTransform slotRect = slotTemplate.GetComponent<RectTransform>();
            SetRect(slotRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    Vector2.zero, new Vector2(180f, 55f), new Vector2(0.5f, 0.5f));

            GameObject slotTextGO = new GameObject("Text (TMP)");
            slotTextGO.transform.SetParent(slotTemplate.transform, false);
            TextMeshProUGUI slotText = slotTextGO.AddComponent<TextMeshProUGUI>();
            slotText.fontSize = 36;
            slotText.color    = Color.white;
            RectTransform slotTextRect = slotTextGO.GetComponent<RectTransform>();
            SetRect(slotTextRect, Vector2.zero, Vector2.one,
                    Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));

            GameObject popupGO = new GameObject("ItemDetailPopup");
            popupGO.transform.SetParent(canvasGO.transform, false);
            RectTransform popupRect = popupGO.AddComponent<RectTransform>();
            SetRect(popupRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                    Vector2.zero, new Vector2(200f, 400f), new Vector2(0.5f, 0.5f));
            Image popupImage = popupGO.AddComponent<Image>();
            popupImage.color = new Color(0.114f, 0.123f, 0.122f, 1f);
            popupGO.SetActive(false);

            ItemDetailPopup detailPopup = popupGO.AddComponent<ItemDetailPopup>();

            TextMeshProUGUI itemNameText = MakeTMPText(popupGO.transform, "ItemNameText",
                new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(10f, -30f), new Vector2(200f, 50f), new Vector2(0f, 1f),
                "", 16, new Color(0.973f, 0.898f, 0.318f));

            TextMeshProUGUI itemDescText = MakeTMPText(popupGO.transform, "ItemDescriptionText",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(10f, 0f), new Vector2(200f, 125f), new Vector2(0.5f, 0.5f),
                "", 14, Color.white);

            TextMeshProUGUI itemStatsText = MakeTMPText(popupGO.transform, "ItemStatsText",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(10f, 0f), new Vector2(200f, 75f), new Vector2(0.5f, 0.5f),
                "", 12, Color.white);

            Button dropButton = MakeButton(popupGO.transform, "DropButton",
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 10f), new Vector2(100f, 20f), new Vector2(0.5f, 0f),
                new Color(0.8f, 0.2f, 0.2f), "Drop");

            Button closeButton = MakeButton(popupGO.transform, "CloseButton",
                new Vector2(1f, 1f), new Vector2(1f, 1f),
                Vector2.zero, new Vector2(30f, 30f), new Vector2(1f, 1f),
                new Color(0.962f, 0.059f, 0.059f), "X");

            BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;
            Type uiType = typeof(InventoryUI);
            uiType.GetField("inventoryPanel",    bf).SetValue(inventoryUI, panelGO);
            uiType.GetField("itemSlotPrefab",    bf).SetValue(inventoryUI, slotTemplate);
            uiType.GetField("itemSlotsContainer",bf).SetValue(inventoryUI, (Transform)containerRect);
            uiType.GetField("goldText",          bf).SetValue(inventoryUI, goldText);
            uiType.GetField("fullText",          bf).SetValue(inventoryUI, fullText);
            uiType.GetField("itemDetailPopup",   bf).SetValue(inventoryUI, detailPopup);

            Type popupType = typeof(ItemDetailPopup);
            popupType.GetField("itemNameText",       bf).SetValue(detailPopup, itemNameText);
            popupType.GetField("itemDescriptionText", bf).SetValue(detailPopup, itemDescText);
            popupType.GetField("itemStatsText",      bf).SetValue(detailPopup, itemStatsText);
            popupType.GetField("dropButton",         bf).SetValue(detailPopup, dropButton);
            popupType.GetField("closeButton",        bf).SetValue(detailPopup, closeButton);

            GameObject dropTemplate = BuildDropTemplate();
            popupType.GetField("itemSpritePrefab", bf)?.SetValue(detailPopup, dropTemplate);

            _savedInventory = new Inventory(4);
            inventoryUI.Initialize(_savedInventory);

            return canvasGO;
        }

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

        // keep ACTIVE so Instantiate clones are also active
        private static GameObject BuildDropTemplate()
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            Sprite spr = Sprite.Create(tex, new Rect(0, 0, 1, 1),
                                       new Vector2(0.5f, 0.5f), 1f);

            GameObject go = new GameObject("DroppedItemTemplate");
            go.transform.localScale  = new Vector3(1.5f, 1.5f, 1f);
            go.transform.position = new Vector3(-9999f, -9999f, 0f);
            DontDestroyOnLoad(go);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = spr;
            sr.color        = Color.white;
            sr.sortingOrder = 5;

            CircleCollider2D col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius    = 0.5f;

            go.AddComponent<ItemPickup>();
            go.AddComponent<DropPickupCooldown>();
            return go;
        }
    }
}
