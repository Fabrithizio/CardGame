// Caminho: Assets/_Project/Scripts/UI/BattleResultUI.cs
// Descrição: Mostra uma tela simples de vitória ou derrota quando a batalha termina, com botão para reiniciar a cena atual.

using CardGame.Battle;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class BattleResultUI : MonoBehaviour
    {
        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;

        [Header("Visual")]
        [SerializeField] private Vector2 panelSize = new Vector2(520f, 300f);
        [SerializeField] private int titleFontSize = 46;
        [SerializeField] private int subtitleFontSize = 20;
        [SerializeField] private int buttonFontSize = 22;

        [Header("Cores")]
        [SerializeField] private Color victoryColor = new Color(0.10f, 0.48f, 0.20f, 0.96f);
        [SerializeField] private Color defeatColor = new Color(0.58f, 0.10f, 0.12f, 0.96f);
        [SerializeField] private Color buttonColor = new Color(0.08f, 0.20f, 0.36f, 0.98f);

        private Canvas canvas;
        private RectTransform root;
        private GameObject panelObject;
        private Image panelBackground;
        private Text titleText;
        private Text subtitleText;
        private Button restartButton;
        private Text restartButtonText;
        private Font defaultFont;

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
            CreateResultPanel();
            Hide();
        }

        private void Update()
        {
            Refresh();
        }

        private void CreateCanvas()
        {
            GameObject canvasObject = new GameObject("Battle Result UI Canvas");
            canvasObject.transform.SetParent(transform, false);

            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;

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

        private void CreateResultPanel()
        {
            panelObject = new GameObject("Battle Result Panel");
            panelObject.transform.SetParent(root, false);

            RectTransform panelRect = panelObject.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = panelSize;

            panelBackground = panelObject.AddComponent<Image>();
            panelBackground.color = victoryColor;

            VerticalLayoutGroup vertical = panelObject.AddComponent<VerticalLayoutGroup>();
            vertical.padding = new RectOffset(24, 24, 28, 24);
            vertical.spacing = 14;
            vertical.childAlignment = TextAnchor.MiddleCenter;
            vertical.childControlWidth = true;
            vertical.childControlHeight = false;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;

            titleText = CreateText("Title", titleFontSize, FontStyle.Bold, 72f);
            subtitleText = CreateText("Subtitle", subtitleFontSize, FontStyle.Normal, 70f);

            CreateRestartButton();
        }

        private Text CreateText(string objectName, int fontSize, FontStyle fontStyle, float height)
        {
            GameObject textObject = new GameObject(objectName);
            textObject.transform.SetParent(panelObject.transform, false);

            Text text = textObject.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 10;
            text.resizeTextMaxSize = fontSize;

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(panelSize.x - 48f, height);

            return text;
        }

        private void CreateRestartButton()
        {
            GameObject buttonObject = new GameObject("Restart Button");
            buttonObject.transform.SetParent(panelObject.transform, false);

            RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(260f, 58f);

            LayoutElement layout = buttonObject.AddComponent<LayoutElement>();
            layout.preferredWidth = 260f;
            layout.preferredHeight = 58f;

            Image background = buttonObject.AddComponent<Image>();
            background.color = buttonColor;

            restartButton = buttonObject.AddComponent<Button>();
            restartButton.targetGraphic = background;
            restartButton.onClick.AddListener(RestartBattleScene);

            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(buttonObject.transform, false);

            restartButtonText = textObject.AddComponent<Text>();
            restartButtonText.font = defaultFont;
            restartButtonText.fontSize = buttonFontSize;
            restartButtonText.fontStyle = FontStyle.Bold;
            restartButtonText.alignment = TextAnchor.MiddleCenter;
            restartButtonText.color = Color.white;
            restartButtonText.text = "REINICIAR";

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }

        private void Refresh()
        {
            if (battleManager == null ||
                battleManager.PlayerState == null ||
                battleManager.EnemyState == null ||
                battleManager.TurnManager == null)
            {
                return;
            }

            if (battleManager.TurnManager.CurrentPhase != BattlePhase.Finished)
            {
                Hide();
                return;
            }

            if (battleManager.EnemyState.IsDefeated)
            {
                ShowVictory();
                return;
            }

            if (battleManager.PlayerState.IsDefeated)
            {
                ShowDefeat();
                return;
            }

            ShowFinishedWithoutWinner();
        }

        private void ShowVictory()
        {
            panelObject.SetActive(true);
            panelBackground.color = victoryColor;
            titleText.text = "VITÓRIA";
            subtitleText.text = "O inimigo ficou sem vida.";
            restartButtonText.text = "REINICIAR";
        }

        private void ShowDefeat()
        {
            panelObject.SetActive(true);
            panelBackground.color = defeatColor;
            titleText.text = "DERROTA";
            subtitleText.text = "Você ficou sem vida.";
            restartButtonText.text = "TENTAR DE NOVO";
        }

        private void ShowFinishedWithoutWinner()
        {
            panelObject.SetActive(true);
            panelBackground.color = new Color(0.18f, 0.18f, 0.18f, 0.96f);
            titleText.text = "FIM";
            subtitleText.text = "A batalha terminou.";
            restartButtonText.text = "REINICIAR";
        }

        private void Hide()
        {
            if (panelObject != null)
            {
                panelObject.SetActive(false);
            }
        }

        private void RestartBattleScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}