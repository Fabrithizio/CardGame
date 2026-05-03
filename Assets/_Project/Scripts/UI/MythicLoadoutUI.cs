// Caminho: Assets/_Project/Scripts/UI/MythicLoadoutUI.cs
// Descrição: Mostra os Míticos do jogador e do inimigo como bolinhas empilhadas na lateral direita. Aceso = disponível, apagado = usado, fraco = vazio.

using CardGame.Battle;
using CardGame.Mythics;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class MythicLoadoutUI : MonoBehaviour
    {
        private const int MythicSlotCount = 3;

        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;

        [Header("Layout")]
        [SerializeField] private Vector2 dotSize = new Vector2(28f, 28f);
        [SerializeField] private float dotSpacing = 10f;

        [Header("Posição")]
        [SerializeField] private Vector2 enemyAnchorPosition = new Vector2(-22f, -170f);
        [SerializeField] private Vector2 playerAnchorPosition = new Vector2(-22f, 170f);

        [Header("Cores do Jogador")]
        [SerializeField] private Color playerAvailableColor = new Color(1f, 0.84f, 0.20f, 0.95f);
        [SerializeField] private Color playerUsedColor = new Color(0.12f, 0.12f, 0.12f, 0.75f);
        [SerializeField] private Color playerEmptyColor = new Color(0.10f, 0.10f, 0.10f, 0.30f);

        [Header("Cores do Inimigo")]
        [SerializeField] private Color enemyAvailableColor = new Color(0.95f, 0.20f, 0.28f, 0.95f);
        [SerializeField] private Color enemyUsedColor = new Color(0.12f, 0.12f, 0.12f, 0.75f);
        [SerializeField] private Color enemyEmptyColor = new Color(0.10f, 0.10f, 0.10f, 0.30f);

        private Canvas canvas;
        private RectTransform root;

        private Image[] enemyDotImages;
        private Image[] playerDotImages;

        private Sprite circleSprite;

        private void Awake()
        {
            if (battleManager == null)
            {
                battleManager = FindFirstObjectByType<BattleManager>();
            }

            circleSprite = CreateCircleSprite();

            CreateCanvas();

            enemyDotImages = CreateDotsColumn(
                "Enemy Mythic Dots",
                new Vector2(1f, 1f),
                enemyAnchorPosition
            );

            playerDotImages = CreateDotsColumn(
                "Player Mythic Dots",
                new Vector2(1f, 0f),
                playerAnchorPosition
            );
        }

        private void Update()
        {
            Refresh();
        }

        private void CreateCanvas()
        {
            GameObject canvasObject = new GameObject("Mythic Dots Canvas");
            canvasObject.transform.SetParent(transform, false);

            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 30;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            root = canvasObject.GetComponent<RectTransform>();
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;
        }

        private Image[] CreateDotsColumn(string objectName, Vector2 anchor, Vector2 anchoredPosition)
        {
            GameObject columnObject = new GameObject(objectName);
            columnObject.transform.SetParent(root, false);

            RectTransform columnRect = columnObject.AddComponent<RectTransform>();
            columnRect.anchorMin = anchor;
            columnRect.anchorMax = anchor;
            columnRect.pivot = new Vector2(1f, 0.5f);
            columnRect.anchoredPosition = anchoredPosition;
            columnRect.sizeDelta = new Vector2(
                dotSize.x,
                MythicSlotCount * dotSize.y + (MythicSlotCount - 1) * dotSpacing
            );

            VerticalLayoutGroup layout = columnObject.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = dotSpacing;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            Image[] dots = new Image[MythicSlotCount];

            for (int i = 0; i < MythicSlotCount; i++)
            {
                dots[i] = CreateDot(columnRect);
            }

            return dots;
        }

        private Image CreateDot(RectTransform parent)
        {
            GameObject dotObject = new GameObject("Mythic Dot");
            dotObject.transform.SetParent(parent, false);

            RectTransform rect = dotObject.AddComponent<RectTransform>();
            rect.sizeDelta = dotSize;

            LayoutElement layout = dotObject.AddComponent<LayoutElement>();
            layout.preferredWidth = dotSize.x;
            layout.preferredHeight = dotSize.y;

            Image image = dotObject.AddComponent<Image>();
            image.sprite = circleSprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = true;

            Outline outline = dotObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.45f);
            outline.effectDistance = new Vector2(1f, -1f);

            return image;
        }

        private void Refresh()
        {
            if (battleManager == null || battleManager.PlayerState == null || battleManager.EnemyState == null)
            {
                return;
            }

            RefreshDots(
                playerDotImages,
                battleManager.PlayerState.MythicLoadout,
                playerAvailableColor,
                playerUsedColor,
                playerEmptyColor
            );

            RefreshDots(
                enemyDotImages,
                battleManager.EnemyState.MythicLoadout,
                enemyAvailableColor,
                enemyUsedColor,
                enemyEmptyColor
            );
        }

        private void RefreshDots(
            Image[] dots,
            MythicLoadoutRuntime loadout,
            Color availableColor,
            Color usedColor,
            Color emptyColor)
        {
            if (dots == null || loadout == null)
            {
                return;
            }

            for (int i = 0; i < dots.Length; i++)
            {
                MythicRuntime mythic = loadout.GetMythicAt(i);
                RefreshDot(dots[i], mythic, availableColor, usedColor, emptyColor);
            }
        }

        private void RefreshDot(
            Image dotImage,
            MythicRuntime mythic,
            Color availableColor,
            Color usedColor,
            Color emptyColor)
        {
            if (dotImage == null)
            {
                return;
            }

            if (mythic == null)
            {
                dotImage.color = emptyColor;
                return;
            }

            dotImage.color = mythic.IsUsed ? usedColor : availableColor;
        }

        private Sprite CreateCircleSprite()
        {
            const int size = 64;

            Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Bilinear;

            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size * 0.42f;
            float radiusSquared = radius * radius;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 point = new Vector2(x, y);
                    float distanceSquared = (point - center).sqrMagnitude;

                    texture.SetPixel(
                        x,
                        y,
                        distanceSquared <= radiusSquared ? Color.white : Color.clear
                    );
                }
            }

            texture.Apply();

            return Sprite.Create(
                texture,
                new Rect(0f, 0f, size, size),
                new Vector2(0.5f, 0.5f),
                size
            );
        }
    }
}