// Caminho: Assets/_Project/Scripts/UI/DeckCounterUI.cs
// Descrição: Contadores de deck/mão/cemitério dentro das zonas EnemyDeck e PlayerDeck, com proteção contra estado nulo.

using CardGame.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class DeckCounterUI : MonoBehaviour
    {
        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;

        [Header("Texto")]
        [SerializeField] private int labelFontSize = 10;
        [SerializeField] private int valueFontSize = 16;

        [Header("Cores")]
        [SerializeField] private Color playerColor = new Color(0.03f, 0.09f, 0.20f, 0.88f);
        [SerializeField] private Color enemyColor = new Color(0.18f, 0.04f, 0.07f, 0.88f);

        private Font defaultFont;
        private CounterPanel playerPanel;
        private CounterPanel enemyPanel;
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

            enemyPanel = CreatePanel("Enemy Deck Panel", layout.GetZone(BattleScreenZone.EnemyDeck), enemyColor);
            playerPanel = CreatePanel("Player Deck Panel", layout.GetZone(BattleScreenZone.PlayerDeck), playerColor);

            built = true;
        }

        private CounterPanel CreatePanel(string objectName, RectTransform parent, Color backgroundColor)
        {
            GameObject panelObject = new GameObject(objectName);
            panelObject.transform.SetParent(parent, false);

            RectTransform rect = panelObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image background = panelObject.AddComponent<Image>();
            background.color = backgroundColor;

            Outline outline = panelObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.65f);
            outline.effectDistance = new Vector2(2f, -2f);

            VerticalLayoutGroup vertical = panelObject.AddComponent<VerticalLayoutGroup>();
            vertical.padding = new RectOffset(4, 4, 6, 6);
            vertical.spacing = 0;
            vertical.childAlignment = TextAnchor.MiddleCenter;
            vertical.childControlWidth = true;
            vertical.childControlHeight = false;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;

            Text deck = CreateLine(panelObject.transform, "DECK", "0");
            Text hand = CreateLine(panelObject.transform, "MÃO", "0");
            Text graveyard = CreateLine(panelObject.transform, "CEM", "0");
            Text energy = CreateLine(panelObject.transform, "EN", "0/0");

            return new CounterPanel(deck, hand, graveyard, energy);
        }

        private Text CreateLine(Transform parent, string label, string value)
        {
            GameObject lineObject = new GameObject(label);
            lineObject.transform.SetParent(parent, false);

            RectTransform rect = lineObject.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(10f, 31f);

            VerticalLayoutGroup vertical = lineObject.AddComponent<VerticalLayoutGroup>();
            vertical.spacing = -2;
            vertical.childAlignment = TextAnchor.MiddleCenter;
            vertical.childControlWidth = true;
            vertical.childControlHeight = false;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;

            GameObject labelObject = new GameObject("Label");
            labelObject.transform.SetParent(lineObject.transform, false);

            Text labelText = labelObject.AddComponent<Text>();
            labelText.font = defaultFont;
            labelText.fontSize = labelFontSize;
            labelText.fontStyle = FontStyle.Bold;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.color = new Color(1f, 1f, 1f, 0.75f);
            labelText.text = label;
            labelText.resizeTextForBestFit = true;
            labelText.resizeTextMinSize = 6;
            labelText.resizeTextMaxSize = labelFontSize;

            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.sizeDelta = new Vector2(10f, 11f);

            GameObject valueObject = new GameObject("Value");
            valueObject.transform.SetParent(lineObject.transform, false);

            Text valueText = valueObject.AddComponent<Text>();
            valueText.font = defaultFont;
            valueText.fontSize = valueFontSize;
            valueText.fontStyle = FontStyle.Bold;
            valueText.alignment = TextAnchor.MiddleCenter;
            valueText.color = Color.white;
            valueText.text = value;
            valueText.resizeTextForBestFit = true;
            valueText.resizeTextMinSize = 8;
            valueText.resizeTextMaxSize = valueFontSize;

            RectTransform valueRect = valueObject.GetComponent<RectTransform>();
            valueRect.sizeDelta = new Vector2(10f, 16f);

            return valueText;
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

            int enemyDeck = battleManager.EnemyState.Deck != null ? battleManager.EnemyState.Deck.CardsRemaining : 0;
            int enemyHand = battleManager.EnemyState.Hand != null ? battleManager.EnemyState.Hand.Count : 0;

            int playerDeck = battleManager.PlayerState.Deck != null ? battleManager.PlayerState.Deck.CardsRemaining : 0;
            int playerHand = battleManager.PlayerState.Hand != null ? battleManager.PlayerState.Hand.Count : 0;

            enemyPanel.SetValues(enemyDeck, enemyHand, 0, $"{battleManager.EnemyState.CurrentEnergy}/{Mathf.Max(1, battleManager.EnemyState.MaxEnergy)}");
            playerPanel.SetValues(playerDeck, playerHand, 0, $"{battleManager.PlayerState.CurrentEnergy}/{Mathf.Max(1, battleManager.PlayerState.MaxEnergy)}");
        }

        private sealed class CounterPanel
        {
            private readonly Text deck;
            private readonly Text hand;
            private readonly Text graveyard;
            private readonly Text energy;

            public CounterPanel(Text deck, Text hand, Text graveyard, Text energy)
            {
                this.deck = deck;
                this.hand = hand;
                this.graveyard = graveyard;
                this.energy = energy;
            }

            public void SetValues(int deckValue, int handValue, int graveyardValue, string energyValue)
            {
                deck.text = deckValue.ToString();
                hand.text = handValue.ToString();
                graveyard.text = graveyardValue.ToString();
                energy.text = energyValue;
            }
        }
    }
}
