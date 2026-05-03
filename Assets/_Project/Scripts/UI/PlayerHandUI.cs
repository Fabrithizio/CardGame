// Caminho: Assets/_Project/Scripts/UI/PlayerHandUI.cs
// Descrição: Mostra as cartas na mão do jogador e permite clicar em criatura para colocar no campo ou armadilha para preparar no slot de armadilha.

using System.Collections.Generic;
using CardGame.Battle;
using CardGame.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class PlayerHandUI : MonoBehaviour
    {
        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;

        [Header("Layout")]
        [SerializeField] private Vector2 cardSize = new Vector2(95f, 135f);
        [SerializeField] private float cardSpacing = 6f;
        [SerializeField] private Vector2 anchorPosition = new Vector2(0f, 20f);

        [Header("Texto")]
        [SerializeField] private int nameFontSize = 12;
        [SerializeField] private int statsFontSize = 10;
        [SerializeField] private int typeFontSize = 9;

        private Canvas canvas;
        private RectTransform root;
        private RectTransform handRow;
        private Font defaultFont;

        private readonly List<HandCardUI> spawnedCards = new();

        private int lastHandCount = -1;

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
            CreateHandRow();
        }

        private void Update()
        {
            RefreshIfNeeded();
        }

        private void CreateCanvas()
        {
            GameObject canvasObject = new GameObject("Player Hand UI Canvas");
            canvasObject.transform.SetParent(transform, false);

            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 40;

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

        private void CreateHandRow()
        {
            GameObject rowObject = new GameObject("Player Hand Row");
            rowObject.transform.SetParent(root, false);

            handRow = rowObject.AddComponent<RectTransform>();
            handRow.anchorMin = new Vector2(0.5f, 0f);
            handRow.anchorMax = new Vector2(0.5f, 0f);
            handRow.pivot = new Vector2(0.5f, 0f);
            handRow.anchoredPosition = anchorPosition;
            handRow.sizeDelta = new Vector2(1200f, cardSize.y);

            HorizontalLayoutGroup layout = rowObject.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = cardSpacing;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }

        private void RefreshIfNeeded()
        {
            if (battleManager == null || battleManager.PlayerState == null)
            {
                return;
            }

            int currentHandCount = battleManager.PlayerState.Hand.Count;

            if (currentHandCount == lastHandCount)
            {
                RefreshCardTextsOnly();
                return;
            }

            lastHandCount = currentHandCount;
            RebuildHand();
        }

        private void RebuildHand()
        {
            ClearHand();

            IReadOnlyList<CardRuntime> cards = battleManager.PlayerState.Hand.Cards;

            for (int i = 0; i < cards.Count; i++)
            {
                int cardIndex = i;
                HandCardUI cardUI = CreateCardUI(cards[i], cardIndex);
                spawnedCards.Add(cardUI);
            }
        }

        private void ClearHand()
        {
            for (int i = handRow.childCount - 1; i >= 0; i--)
            {
                Destroy(handRow.GetChild(i).gameObject);
            }

            spawnedCards.Clear();
        }

        private HandCardUI CreateCardUI(CardRuntime card, int handIndex)
        {
            GameObject cardObject = new GameObject("Hand Card");
            cardObject.transform.SetParent(handRow, false);

            RectTransform rect = cardObject.AddComponent<RectTransform>();
            rect.sizeDelta = cardSize;

            LayoutElement layout = cardObject.AddComponent<LayoutElement>();
            layout.preferredWidth = cardSize.x;
            layout.preferredHeight = cardSize.y;

            Image background = cardObject.AddComponent<Image>();
            background.color = GetColorByCardType(card);

            Button button = cardObject.AddComponent<Button>();
            button.targetGraphic = background;
            button.onClick.AddListener(() => HandleCardClicked(handIndex));

            VerticalLayoutGroup vertical = cardObject.AddComponent<VerticalLayoutGroup>();
            vertical.padding = new RectOffset(7, 7, 8, 7);
            vertical.spacing = 4;
            vertical.childAlignment = TextAnchor.MiddleCenter;
            vertical.childControlWidth = true;
            vertical.childControlHeight = false;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;

            Text nameText = CreateText(cardObject.transform, "Name", nameFontSize, FontStyle.Bold, 36f);
            Text typeText = CreateText(cardObject.transform, "Type", typeFontSize, FontStyle.Italic, 24f);
            Text statsText = CreateText(cardObject.transform, "Stats", statsFontSize, FontStyle.Normal, 54f);

            HandCardUI cardUI = new HandCardUI(background, nameText, typeText, statsText);
            cardUI.SetCard(card);

            return cardUI;
        }

        private Text CreateText(
            Transform parent,
            string objectName,
            int fontSize,
            FontStyle fontStyle,
            float height)
        {
            GameObject textObject = new GameObject(objectName);
            textObject.transform.SetParent(parent, false);

            Text text = textObject.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 7;
            text.resizeTextMaxSize = fontSize;
            text.text = string.Empty;

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(cardSize.x - 14f, height);

            return text;
        }

        private void HandleCardClicked(int handIndex)
        {
            if (battleManager == null || battleManager.PlayerState == null)
            {
                return;
            }

            if (battleManager.TurnManager == null || !battleManager.TurnManager.IsPlayerTurn)
            {
                Debug.Log("Você só pode jogar cartas da sua mão no seu turno.");
                return;
            }

            CardRuntime clickedCard = battleManager.PlayerState.Hand.GetCardAt(handIndex);

            if (clickedCard == null)
            {
                Debug.Log("Carta inválida na mão.");
                return;
            }

            switch (clickedCard.CardType)
            {
                case CardType.Creature:
                    TryPlayCreature(clickedCard);
                    break;

                case CardType.Trap:
                    TrySetTrap(clickedCard);
                    break;

                case CardType.Spell:
                    Debug.Log($"Clique em {clickedCard.CardName}: sistema de magia ainda será implementado.");
                    break;

                case CardType.Equipment:
                    Debug.Log($"Clique em {clickedCard.CardName}: sistema de equipamento ainda será implementado.");
                    break;

                default:
                    Debug.Log($"Tipo de carta não suportado: {clickedCard.CardType}.");
                    break;
            }
        }

        private void TryPlayCreature(CardRuntime card)
        {
            if (!battleManager.PlayerState.Board.HasFreeCreatureSlot())
            {
                Debug.Log("Não há slot livre para colocar criatura.");
                return;
            }

            if (battleManager.PlayerState.TryPlayCreatureInFirstFreeSlot(card, out int slotIndex))
            {
                Debug.Log($"Jogador colocou {card.CardName} no slot {slotIndex + 1} clicando na mão.");
                ForceRefreshHand();
                return;
            }

            Debug.Log($"Não foi possível jogar {card.CardName}.");
        }

        private void TrySetTrap(CardRuntime card)
        {
            if (!battleManager.PlayerState.Board.HasFreeTrapSlot())
            {
                Debug.Log("Não há slot livre para preparar armadilha.");
                return;
            }

            if (battleManager.PlayerState.TrySetTrapInFirstFreeSlot(card, out int slotIndex))
            {
                Debug.Log($"Jogador preparou armadilha {card.CardName} no slot {slotIndex + 1}.");
                ForceRefreshHand();
                return;
            }

            Debug.Log($"Não foi possível preparar {card.CardName}.");
        }

        private void ForceRefreshHand()
        {
            lastHandCount = -1;
            RefreshIfNeeded();
        }

        private void RefreshCardTextsOnly()
        {
            IReadOnlyList<CardRuntime> cards = battleManager.PlayerState.Hand.Cards;

            for (int i = 0; i < spawnedCards.Count && i < cards.Count; i++)
            {
                spawnedCards[i].SetCard(cards[i]);
            }
        }

        private Color GetColorByCardType(CardRuntime card)
        {
            if (card == null)
            {
                return new Color(0.18f, 0.18f, 0.18f, 0.86f);
            }

            return card.CardType switch
            {
                CardType.Creature => new Color(0.12f, 0.30f, 0.68f, 0.90f),
                CardType.Spell => new Color(0.32f, 0.16f, 0.62f, 0.90f),
                CardType.Trap => new Color(0.62f, 0.32f, 0.10f, 0.90f),
                CardType.Equipment => new Color(0.42f, 0.42f, 0.42f, 0.90f),
                _ => new Color(0.18f, 0.18f, 0.18f, 0.86f)
            };
        }

        private sealed class HandCardUI
        {
            private readonly Image background;
            private readonly Text nameText;
            private readonly Text typeText;
            private readonly Text statsText;

            public HandCardUI(Image background, Text nameText, Text typeText, Text statsText)
            {
                this.background = background;
                this.nameText = nameText;
                this.typeText = typeText;
                this.statsText = statsText;
            }

            public void SetCard(CardRuntime card)
            {
                if (card == null)
                {
                    background.color = new Color(0.18f, 0.18f, 0.18f, 0.86f);
                    nameText.text = "Vazio";
                    typeText.text = string.Empty;
                    statsText.text = string.Empty;
                    return;
                }

                nameText.text = ShortName(card.CardName);
                typeText.text = card.CardType.ToString();

                if (card.CardType == CardType.Creature)
                {
                    statsText.text =
                        $"Custo {card.Data.Cost}\n" +
                        $"ATK {card.CurrentAttack} HP {card.CurrentHealth}\n" +
                        $"SPD {card.CurrentSpeed} DEF {card.CurrentDefense}";
                }
                else
                {
                    statsText.text = $"Custo {card.Data.Cost}";
                }
            }

            private string ShortName(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return "Carta";
                }

                if (value.Length <= 16)
                {
                    return value;
                }

                return value.Substring(0, 16) + "...";
            }
        }
    }
}