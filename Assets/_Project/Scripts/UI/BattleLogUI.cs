// Caminho: Assets/_Project/Scripts/UI/BattleLogUI.cs
// Descrição: Log curto dentro da zona BattleLog. Só aparece quando existir mensagem útil.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class BattleLogUI : MonoBehaviour
    {
        [Header("Texto")]
        [SerializeField] private int fontSize = 11;
        [SerializeField] private int maxVisibleLines = 3;

        [Header("Cores")]
        [SerializeField] private Color panelColor = new Color(0.03f, 0.05f, 0.10f, 0.82f);
        [SerializeField] private Color textColor = new Color(0.92f, 0.96f, 1f, 0.96f);

        private GameObject panelObject;
        private Text logText;
        private Font defaultFont;
        private static BattleLogUI instance;
        private readonly Queue<string> messages = new();

        private void Awake()
        {
            instance = this;
            defaultFont = ResponsiveUIUtility.GetDefaultFont();
            CreatePanel();
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        public static void AddMessage(string message)
        {
            if (instance == null)
            {
                Debug.Log(message);
                return;
            }

            instance.AddMessageInternal(message);
        }

        private void AddMessageInternal(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            string cleaned = CleanMessage(message);

            if (string.IsNullOrWhiteSpace(cleaned))
            {
                return;
            }

            messages.Enqueue(cleaned);

            while (messages.Count > maxVisibleLines)
            {
                messages.Dequeue();
            }

            RefreshText();
        }

        private string CleanMessage(string message)
        {
            if (message.Contains("UnityEngine.Debug:Log"))
            {
                return string.Empty;
            }

            if (message.Contains("[Estado]"))
            {
                return string.Empty;
            }

            if (message.Contains("Turno iniciado") || message.Contains("Turno encerrado"))
            {
                return string.Empty;
            }

            int bracketIndex = message.IndexOf(']');

            if (message.StartsWith("[") && bracketIndex >= 0 && bracketIndex + 1 < message.Length)
            {
                return message[(bracketIndex + 1)..].Trim();
            }

            return message.Trim();
        }

        private void RefreshText()
        {
            bool hasMessages = messages.Count > 0;

            if (panelObject != null)
            {
                panelObject.SetActive(hasMessages);
            }

            logText.text = string.Join("\n", messages);
        }

        private void CreatePanel()
        {
            BattleScreenLayoutUI layout = BattleScreenLayoutUI.GetOrCreate();

            panelObject = new GameObject("Battle Log Panel");
            panelObject.transform.SetParent(layout.GetZone(BattleScreenZone.BattleLog), false);

            RectTransform rect = panelObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image background = panelObject.AddComponent<Image>();
            background.color = panelColor;

            Outline outline = panelObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.60f);
            outline.effectDistance = new Vector2(2f, -2f);

            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(panelObject.transform, false);

            logText = textObject.AddComponent<Text>();
            logText.font = defaultFont;
            logText.fontSize = fontSize;
            logText.alignment = TextAnchor.MiddleLeft;
            logText.color = textColor;
            logText.horizontalOverflow = HorizontalWrapMode.Wrap;
            logText.verticalOverflow = VerticalWrapMode.Truncate;
            logText.resizeTextForBestFit = true;
            logText.resizeTextMinSize = 8;
            logText.resizeTextMaxSize = fontSize;
            logText.text = string.Empty;

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10f, 8f);
            textRect.offsetMax = new Vector2(-10f, -8f);

            panelObject.SetActive(false);
        }
    }
}
