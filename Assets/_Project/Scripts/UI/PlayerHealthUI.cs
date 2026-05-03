// Caminho: Assets/_Project/Scripts/UI/PlayerHealthUI.cs
// Descrição: Mostra a vida e energia do jogador e do inimigo na tela durante a batalha.

using CardGame.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;

        [Header("Layout")]
        [SerializeField] private Vector2 panelSize = new Vector2(230f, 66f);
        [SerializeField] private Vector2 playerAnchorPosition = new Vector2(22f, 22f);
        [SerializeField] private Vector2 enemyAnchorPosition = new Vector2(22f, -22f);

        [Header("Texto")]
        [SerializeField] private int fontSize = 16;

        [Header("Cores")]
        [SerializeField] private Color playerColor = new Color(0.10f, 0.32f, 0.72f, 0.92f);
        [SerializeField] private Color enemyColor = new Color(0.62f, 0.16f, 0.18f, 0.92f);

        private Canvas canvas;
        private RectTransform root;
        private Font defaultFont;

        private Text playerHealthText;
        private Text enemyHealthText;

        private void Awake()
        {
            if (battleManager == null)
            {
                battleManager = FindFirstObjectByType<BattleManager>();
            }

            defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            if (defaultFont == null)
            {
                defaultFont = Font.CreateDynamicFontFromOSFont("Arial", 16);
            }

            CreateCanvas();
            CreateHealthPanels();
        }

        private void Update()
        {
            Refresh();
        }

        private void CreateCanvas()
        {
            GameObject canvasObject = new GameObject("Player Health UI Canvas");
            canvasObject.transform.SetParent(transform, false);

            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 45;

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

        private void CreateHealthPanels()
        {
            enemyHealthText = CreatePanel(
                "Enemy Health",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                enemyAnchorPosition,
                enemyColor,
                "Inimigo"
            );

            playerHealthText = CreatePanel(
                "Player Health",
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                playerAnchorPosition,
                playerColor,
                "Jogador"
            );
        }

        private Text CreatePanel(
            string objectName,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 anchoredPosition,
            Color backgroundColor,
            string label)
        {
            GameObject panelObject = new GameObject(objectName);
            panelObject.transform.SetParent(root, false);

            RectTransform rect = panelObject.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = anchorMin;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = panelSize;

            Image background = panelObject.AddComponent<Image>();
            background.color = backgroundColor;

            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(panelObject.transform, false);

            Text text = textObject.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = fontSize;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.text = $"{label}: --/--";

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            return text;
        }

        private void Refresh()
        {
            if (battleManager == null || battleManager.PlayerState == null || battleManager.EnemyState == null)
            {
                return;
            }

            playerHealthText.text =
                $"Jogador: {battleManager.PlayerState.CurrentHealth}/{battleManager.PlayerState.MaxHealth}\n" +
                $"Energia: {battleManager.PlayerState.CurrentEnergy}/{battleManager.PlayerState.MaxEnergy}";

            enemyHealthText.text =
                $"Inimigo: {battleManager.EnemyState.CurrentHealth}/{battleManager.EnemyState.MaxHealth}\n" +
                $"Energia: {battleManager.EnemyState.CurrentEnergy}/{battleManager.EnemyState.MaxEnergy}";
        }
    }
}