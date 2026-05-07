// Caminho: Assets/_Project/Scripts/UI/MythicActivationBannerUI.cs
// Descrição: Mostra uma faixa dramática temporária quando um Mítico é ativado, capturando automaticamente mensagens do Debug.Log.

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class MythicActivationBannerUI : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private Vector2 panelSize = new Vector2(760f, 150f);
        [SerializeField] private Vector2 anchorPosition = Vector2.zero;

        [Header("Tempo")]
        [SerializeField] private float visibleDuration = 1.8f;

        [Header("Texto")]
        [SerializeField] private int titleFontSize = 34;
        [SerializeField] private int subtitleFontSize = 18;

        [Header("Cores")]
        [SerializeField] private Color panelColor = new Color(0.10f, 0.04f, 0.18f, 0.92f);
        [SerializeField] private Color titleColor = new Color(1f, 0.85f, 0.22f, 1f);
        [SerializeField] private Color subtitleColor = Color.white;

        private Canvas canvas;
        private RectTransform root;
        private GameObject panelObject;
        private Text titleText;
        private Text subtitleText;
        private Font defaultFont;
        private Coroutine hideRoutine;

        private void Awake()
        {
            defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            if (defaultFont == null)
            {
                defaultFont = Font.CreateDynamicFontFromOSFont("Arial", 16);
            }

            CreateCanvas();
            CreatePanel();
            HideInstant();
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLogMessage;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLogMessage;
        }

        private void HandleLogMessage(string condition, string stackTrace, LogType type)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                return;
            }

            if (!condition.Contains("MÍTICO ATIVADO") && !condition.Contains("MITICO ATIVADO"))
            {
                return;
            }

            Show(condition);
        }

        private void CreateCanvas()
        {
            GameObject canvasObject = new GameObject("Mythic Activation Banner Canvas");
            canvasObject.transform.SetParent(transform, false);

            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 180;

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

        private void CreatePanel()
        {
            panelObject = new GameObject("Mythic Activation Banner");
            panelObject.transform.SetParent(root, false);

            RectTransform panelRect = panelObject.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = anchorPosition;
            panelRect.sizeDelta = panelSize;

            Image background = panelObject.AddComponent<Image>();
            background.color = panelColor;

            Outline outline = panelObject.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 0.85f, 0.22f, 0.55f);
            outline.effectDistance = new Vector2(3f, -3f);

            VerticalLayoutGroup vertical = panelObject.AddComponent<VerticalLayoutGroup>();
            vertical.padding = new RectOffset(20, 20, 24, 18);
            vertical.spacing = 8;
            vertical.childAlignment = TextAnchor.MiddleCenter;
            vertical.childControlWidth = true;
            vertical.childControlHeight = false;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;

            titleText = CreateText("Title", titleFontSize, FontStyle.Bold, titleColor, 58f);
            subtitleText = CreateText("Subtitle", subtitleFontSize, FontStyle.Italic, subtitleColor, 44f);
        }

        private Text CreateText(string objectName, int fontSize, FontStyle fontStyle, Color color, float height)
        {
            GameObject textObject = new GameObject(objectName);
            textObject.transform.SetParent(panelObject.transform, false);

            Text text = textObject.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 10;
            text.resizeTextMaxSize = fontSize;

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(panelSize.x - 40f, height);

            return text;
        }

        private void Show(string logMessage)
        {
            panelObject.SetActive(true);

            titleText.text = "MÍTICO ATIVADO";
            subtitleText.text = ExtractMythicName(logMessage);

            if (hideRoutine != null)
            {
                StopCoroutine(hideRoutine);
            }

            hideRoutine = StartCoroutine(HideAfterDelay());
        }

        private string ExtractMythicName(string logMessage)
        {
            int separatorIndex = logMessage.LastIndexOf(':');

            if (separatorIndex >= 0 && separatorIndex + 1 < logMessage.Length)
            {
                return logMessage.Substring(separatorIndex + 1).Trim();
            }

            return logMessage.Trim();
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(visibleDuration);
            HideInstant();
        }

        private void HideInstant()
        {
            if (panelObject != null)
            {
                panelObject.SetActive(false);
            }
        }
    }
}
