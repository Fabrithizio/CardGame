// Caminho: Assets/_Project/Scripts/UI/BattleCombatCenterUI.cs
// Descrição: Painel cinematográfico central para exibir eventos importantes da batalha: dano, ataque, cura, destruição, magia e Mítico.

using System.Collections.Generic;
using CardGame.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class BattleCombatCenterUI : MonoBehaviour
    {
        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;

        [Header("Layout")]
        [SerializeField] private Vector2 panelSize = new Vector2(860f, 170f);
        [SerializeField] private Vector2 anchorPosition = new Vector2(0f, -12f);

        [Header("Texto")]
        [SerializeField] private int titleFontSize = 36;
        [SerializeField] private int subtitleFontSize = 22;
        [SerializeField] private int damageFontSize = 46;

        [Header("Tempo")]
        [SerializeField] private float visibleDuration = 1.55f;
        [SerializeField] private float fadeDuration = 0.25f;

        [Header("Cores")]
        [SerializeField] private Color panelColor = new Color(0.015f, 0.018f, 0.035f, 0.76f);
        [SerializeField] private Color titleColor = new Color(1f, 0.92f, 0.72f, 1f);
        [SerializeField] private Color subtitleColor = new Color(0.90f, 0.94f, 1f, 0.92f);
        [SerializeField] private Color damageColor = new Color(1f, 0.24f, 0.24f, 1f);
        [SerializeField] private Color healColor = new Color(0.28f, 1f, 0.45f, 1f);
        [SerializeField] private Color mythicColor = new Color(0.94f, 0.42f, 1f, 1f);

        private CanvasGroup canvasGroup;
        private Image panelImage;
        private Text titleText;
        private Text subtitleText;
        private Text impactText;
        private Font defaultFont;

        private float timer;
        private bool visible;

        private readonly Queue<CombatMessage> pendingMessages = new();

        private void Awake()
        {
            if (battleManager == null)
            {
                battleManager = FindFirstObjectByType<BattleManager>();
            }

            defaultFont = ResponsiveUIUtility.GetDefaultFont();

            CreateUI();
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

        private void Update()
        {
            if (!visible && pendingMessages.Count > 0)
            {
                ShowMessage(pendingMessages.Dequeue());
            }

            if (!visible)
            {
                return;
            }

            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                HideInstant();
                return;
            }

            if (timer < fadeDuration)
            {
                canvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
            }
        }

        public void PushMessage(string title, string subtitle = "", string impact = "", CombatMessageKind kind = CombatMessageKind.Neutral)
        {
            pendingMessages.Enqueue(new CombatMessage(title, subtitle, impact, kind));
        }

        private void HandleLogMessage(string condition, string stackTrace, LogType type)
        {
            if (string.IsNullOrWhiteSpace(condition))
            {
                return;
            }

            if (type == LogType.Warning || type == LogType.Error || type == LogType.Exception)
            {
                return;
            }

            if (condition.Contains("[Estado]") || condition.Contains("Fase atual") || condition.Contains("Turno iniciado") || condition.Contains("Turno encerrado"))
            {
                return;
            }

            CombatMessage message;

            if (TryParseCombatMessage(condition, out message))
            {
                pendingMessages.Enqueue(message);
            }
        }

        private bool TryParseCombatMessage(string log, out CombatMessage message)
        {
            message = default;

            string clean = RemoveTimestamp(log);

            if (clean.Contains("MÍTICO") || clean.Contains("Mítico") || clean.Contains("mítico"))
            {
                message = new CombatMessage("MÍTICO DESPERTADO", clean, "", CombatMessageKind.Mythic);
                return true;
            }

            if (clean.Contains("VITÓRIA") || clean.Contains("DERROTA") || clean.Contains("venceu") || clean.Contains("perdeu"))
            {
                message = new CombatMessage("FIM DA PARTIDA", clean, "", CombatMessageKind.Neutral);
                return true;
            }

            if (clean.Contains("destruído") || clean.Contains("destruida") || clean.Contains("destruída"))
            {
                message = new CombatMessage("CRIATURA DESTRUÍDA", clean, "", CombatMessageKind.Damage);
                return true;
            }

            if (clean.Contains("curou") || clean.Contains("cura") || clean.Contains("recuperou"))
            {
                string number = ExtractSignedNumber(clean, "+");
                message = new CombatMessage("CURA", clean, number, CombatMessageKind.Heal);
                return true;
            }

            if (clean.Contains("causou") || clean.Contains("dano") || clean.Contains("atacou"))
            {
                string number = ExtractSignedNumber(clean, "-");
                message = new CombatMessage("IMPACTO", clean, number, CombatMessageKind.Damage);
                return true;
            }

            if (clean.Contains("Escudo") || clean.Contains("escudo"))
            {
                message = new CombatMessage("ESCUDO", clean, "", CombatMessageKind.Neutral);
                return true;
            }

            return false;
        }

        private string RemoveTimestamp(string value)
        {
            int bracketIndex = value.IndexOf(']');

            if (value.StartsWith("[") && bracketIndex >= 0 && bracketIndex + 1 < value.Length)
            {
                return value.Substring(bracketIndex + 1).Trim();
            }

            return value.Trim();
        }

        private string ExtractSignedNumber(string value, string sign)
        {
            string[] parts = value.Split(' ');

            for (int i = 0; i < parts.Length; i++)
            {
                string token = parts[i].Trim();

                if (int.TryParse(token, out int number) && number > 0)
                {
                    return $"{sign}{number}";
                }
            }

            return string.Empty;
        }

        private void CreateUI()
        {
            Canvas canvas = ResponsiveUIUtility.CreateOverlayCanvas(transform, "Battle Combat Center UI Canvas", 58);
            RectTransform root = canvas.GetComponent<RectTransform>();

            GameObject panelObject = new GameObject("Combat Center Panel");
            panelObject.transform.SetParent(root, false);

            RectTransform panelRect = panelObject.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = anchorPosition;
            panelRect.sizeDelta = panelSize;

            panelImage = panelObject.AddComponent<Image>();
            panelImage.color = panelColor;

            Outline outline = panelObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.70f);
            outline.effectDistance = new Vector2(3f, -3f);

            canvasGroup = panelObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            VerticalLayoutGroup vertical = panelObject.AddComponent<VerticalLayoutGroup>();
            vertical.padding = new RectOffset(20, 20, 12, 12);
            vertical.spacing = 2;
            vertical.childAlignment = TextAnchor.MiddleCenter;
            vertical.childControlWidth = true;
            vertical.childControlHeight = false;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;

            titleText = CreateText(panelObject.transform, "Title", titleFontSize, FontStyle.Bold, 48f, titleColor);
            impactText = CreateText(panelObject.transform, "Impact", damageFontSize, FontStyle.Bold, 54f, damageColor);
            subtitleText = CreateText(panelObject.transform, "Subtitle", subtitleFontSize, FontStyle.Italic, 44f, subtitleColor);
        }

        private Text CreateText(Transform parent, string objectName, int fontSize, FontStyle style, float height, Color color)
        {
            GameObject textObject = new GameObject(objectName);
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(panelSize.x - 40f, height);

            Text text = textObject.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 10;
            text.resizeTextMaxSize = fontSize;

            return text;
        }

        private void ShowMessage(CombatMessage message)
        {
            visible = true;
            timer = visibleDuration;
            canvasGroup.alpha = 1f;

            titleText.text = message.Title;
            subtitleText.text = message.Subtitle;
            impactText.text = message.Impact;

            switch (message.Kind)
            {
                case CombatMessageKind.Damage:
                    impactText.color = damageColor;
                    titleText.color = damageColor;
                    break;

                case CombatMessageKind.Heal:
                    impactText.color = healColor;
                    titleText.color = healColor;
                    break;

                case CombatMessageKind.Mythic:
                    impactText.color = mythicColor;
                    titleText.color = mythicColor;
                    break;

                default:
                    impactText.color = titleColor;
                    titleText.color = titleColor;
                    break;
            }
        }

        private void HideInstant()
        {
            visible = false;
            timer = 0f;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }

        public enum CombatMessageKind
        {
            Neutral,
            Damage,
            Heal,
            Mythic
        }

        private readonly struct CombatMessage
        {
            public readonly string Title;
            public readonly string Subtitle;
            public readonly string Impact;
            public readonly CombatMessageKind Kind;

            public CombatMessage(string title, string subtitle, string impact, CombatMessageKind kind)
            {
                Title = title;
                Subtitle = subtitle;
                Impact = impact;
                Kind = kind;
            }
        }
    }
}
