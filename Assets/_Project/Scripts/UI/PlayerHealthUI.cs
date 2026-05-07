// Caminho: Assets/_Project/Scripts/UI/PlayerHealthUI.cs
// Descrição: Barras de vida/energia dentro das zonas EnemyHealth e PlayerHealth, com proteção contra estado nulo.

using CardGame.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;

        [Header("Texto")]
        [SerializeField] private int nameFontSize = 14;
        [SerializeField] private int valueFontSize = 12;

        [Header("Cores")]
        [SerializeField] private Color playerPanelColor = new Color(0.02f, 0.08f, 0.18f, 0.88f);
        [SerializeField] private Color enemyPanelColor = new Color(0.16f, 0.04f, 0.08f, 0.88f);
        [SerializeField] private Color healthColor = new Color(0.84f, 0.08f, 0.10f, 0.96f);
        [SerializeField] private Color energyColor = new Color(0.10f, 0.46f, 1f, 0.96f);
        [SerializeField] private Color emptyBarColor = new Color(0.03f, 0.03f, 0.04f, 0.72f);

        private Font defaultFont;
        private PlayerHudPanel playerPanel;
        private PlayerHudPanel enemyPanel;
        private bool built;

        private void Awake()
        {
            if (battleManager == null)
            {
                battleManager = FindFirstObjectByType<BattleManager>();
            }

            defaultFont = ResponsiveUIUtility.GetDefaultFont();
            CreatePanels();
        }

        private void Update()
        {
            if (!built)
            {
                CreatePanels();
            }

            Refresh();
        }

        private void CreatePanels()
        {
            if (built)
            {
                return;
            }

            BattleScreenLayoutUI layout = BattleScreenLayoutUI.GetOrCreate();

            enemyPanel = CreatePanel("Enemy Bars", layout.GetZone(BattleScreenZone.EnemyHealth), enemyPanelColor);
            playerPanel = CreatePanel("Player Bars", layout.GetZone(BattleScreenZone.PlayerHealth), playerPanelColor);

            built = true;
        }

        private PlayerHudPanel CreatePanel(string objectName, RectTransform parent, Color panelColor)
        {
            GameObject panelObject = new GameObject(objectName);
            panelObject.transform.SetParent(parent, false);

            RectTransform rect = panelObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image background = panelObject.AddComponent<Image>();
            background.color = panelColor;

            Outline outline = panelObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.65f);
            outline.effectDistance = new Vector2(2f, -2f);

            VerticalLayoutGroup vertical = panelObject.AddComponent<VerticalLayoutGroup>();
            vertical.padding = new RectOffset(8, 8, 6, 6);
            vertical.spacing = 4;
            vertical.childControlWidth = true;
            vertical.childControlHeight = false;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;

            Text nameText = CreateText(panelObject.transform, "Name", nameFontSize, FontStyle.Bold, 22f, TextAnchor.MiddleLeft);
            BarUI healthBar = CreateBar(panelObject.transform, "Health", healthColor);
            BarUI energyBar = CreateBar(panelObject.transform, "Energy", energyColor);

            return new PlayerHudPanel(nameText, healthBar, energyBar);
        }

        private Text CreateText(Transform parent, string objectName, int fontSize, FontStyle fontStyle, float height, TextAnchor anchor)
        {
            GameObject textObject = new GameObject(objectName);
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(10f, height);

            Text text = textObject.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = anchor;
            text.color = Color.white;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 8;
            text.resizeTextMaxSize = fontSize;

            return text;
        }

        private BarUI CreateBar(Transform parent, string objectName, Color fillColor)
        {
            GameObject barObject = new GameObject(objectName);
            barObject.transform.SetParent(parent, false);

            RectTransform rect = barObject.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(10f, 18f);

            Image background = barObject.AddComponent<Image>();
            background.color = emptyBarColor;

            GameObject fillObject = new GameObject("Fill");
            fillObject.transform.SetParent(barObject.transform, false);

            RectTransform fillRect = fillObject.AddComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0f, 0f);
            fillRect.anchorMax = new Vector2(1f, 1f);
            fillRect.pivot = new Vector2(0f, 0.5f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            Image fillImage = fillObject.AddComponent<Image>();
            fillImage.color = fillColor;

            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(barObject.transform, false);

            RectTransform textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text text = textObject.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = valueFontSize;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 7;
            text.resizeTextMaxSize = valueFontSize;

            return new BarUI(fillRect, text);
        }

        private void Refresh()
        {
            if (battleManager == null || playerPanel == null || enemyPanel == null)
            {
                return;
            }

            if (battleManager.PlayerState == null || battleManager.EnemyState == null)
            {
                return;
            }

            enemyPanel.SetValues(
                "Inimigo",
                battleManager.EnemyState.CurrentHealth,
                battleManager.EnemyState.MaxHealth,
                battleManager.EnemyState.CurrentEnergy,
                battleManager.EnemyState.MaxEnergy);

            playerPanel.SetValues(
                "Jogador",
                battleManager.PlayerState.CurrentHealth,
                battleManager.PlayerState.MaxHealth,
                battleManager.PlayerState.CurrentEnergy,
                battleManager.PlayerState.MaxEnergy);
        }

        private sealed class PlayerHudPanel
        {
            private readonly Text nameText;
            private readonly BarUI healthBar;
            private readonly BarUI energyBar;

            public PlayerHudPanel(Text nameText, BarUI healthBar, BarUI energyBar)
            {
                this.nameText = nameText;
                this.healthBar = healthBar;
                this.energyBar = energyBar;
            }

            public void SetValues(string label, int currentHealth, int maxHealth, int currentEnergy, int maxEnergy)
            {
                int safeMaxHealth = Mathf.Max(1, maxHealth);
                int safeMaxEnergy = Mathf.Max(1, maxEnergy);

                nameText.text = label;
                healthBar.SetValue(currentHealth, safeMaxHealth, $"HP {currentHealth}/{safeMaxHealth}");
                energyBar.SetValue(currentEnergy, safeMaxEnergy, $"EN {currentEnergy}/{safeMaxEnergy}");
            }
        }

        private sealed class BarUI
        {
            private readonly RectTransform fillRect;
            private readonly Text valueText;

            public BarUI(RectTransform fillRect, Text valueText)
            {
                this.fillRect = fillRect;
                this.valueText = valueText;
            }

            public void SetValue(int current, int max, string text)
            {
                float ratio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;
                fillRect.anchorMax = new Vector2(ratio, 1f);
                valueText.text = text;
            }
        }
    }
}
