// Caminho: Assets/_Project/Scripts/UI/PlayerHandUI.cs
// Descrição: Mão do jogador em leque, maior, com molduras corretas, textos mais legíveis
// e integração com o preview da carta.

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
        [SerializeField] private CardPreviewUI cardPreviewUI;

        [Header("Atualização")]
        [SerializeField] private bool updateLayoutEveryFrame = true;

        [Header("Layout")]
        [SerializeField] private Vector2 cardSize = new Vector2(224f, 330f);
        [SerializeField] private float cardSpacing = -102f;
        [SerializeField] private Vector2 anchorPosition = new Vector2(0f, 54f);
        [SerializeField] private float maxFanAngle = 15f;
        [SerializeField] private float selectedLift = 58f;
        [SerializeField] private float fanCurve = 44f;

        [Header("Texto")]
        [SerializeField] private int costFontSize = 15;
        [SerializeField] private int nameFontSize = 15;
        [SerializeField] private int statsFontSize = 11;

        private RectTransform root;
        private RectTransform handRoot;
        private Font defaultFont;
        private readonly List<HandCardUI> spawnedCards = new();
        private int lastHandCount = -1;
        private int highlightedIndex = -1;

        private void Awake()
        {
            if (battleManager == null) battleManager = FindFirstObjectByType<BattleManager>();
            if (cardPreviewUI == null) cardPreviewUI = FindFirstObjectByType<CardPreviewUI>();

            defaultFont = ResponsiveUIUtility.GetDefaultFont();
            CreateCanvasBinding();
            CreateHandRoot();
        }

        private void Update()
        {
            if (cardPreviewUI == null) cardPreviewUI = FindFirstObjectByType<CardPreviewUI>();
            RefreshIfNeeded();

            if (updateLayoutEveryFrame)
            {
                ApplyRuntimeLayout();
            }

            RefreshCardTransforms();
        }

        private void CreateCanvasBinding()
        {
            BattleScreenLayoutUI layout = BattleScreenLayoutUI.GetOrCreate();
            root = layout.GetZone(BattleScreenZone.PlayerHand);
        }

        private void CreateHandRoot()
        {
            GameObject rowObject = new GameObject("Player Hand Fan Root");
            rowObject.transform.SetParent(root, false);

            handRoot = rowObject.AddComponent<RectTransform>();
            handRoot.anchorMin = new Vector2(0.5f, 0f);
            handRoot.anchorMax = new Vector2(0.5f, 0f);
            handRoot.pivot = new Vector2(0.5f, 0f);
            handRoot.anchoredPosition = anchorPosition;
            handRoot.sizeDelta = new Vector2(980f, 380f);
        }

        private void ApplyRuntimeLayout()
        {
            if (handRoot == null || root == null) return;

            Canvas.ForceUpdateCanvases();
            handRoot.anchoredPosition = anchorPosition;
            handRoot.sizeDelta = new Vector2(Mathf.Max(760f, root.rect.width), Mathf.Max(320f, root.rect.height + 170f));

            Vector2 size = EffectiveCardSize();
            for (int i = 0; i < spawnedCards.Count; i++)
            {
                spawnedCards[i].SetSize(size);
            }
        }

        private Vector2 EffectiveCardSize()
        {
            return new Vector2(Mathf.Max(202f, cardSize.x), Mathf.Max(300f, cardSize.y));
        }

        private float EffectiveSpacing()
        {
            return Mathf.Min(cardSpacing, -74f);
        }

        private int SafeFont(int value, int min, int max)
        {
            if (value <= 0 || value > 60) return Mathf.Clamp(max - 2, min, max);
            return Mathf.Clamp(value, min, max);
        }

        private void RefreshIfNeeded()
        {
            if (battleManager == null || battleManager.PlayerState == null || battleManager.PlayerState.Hand == null) return;

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
                spawnedCards.Add(CreateCardUI(cards[i], cardIndex));
            }

            ApplyRuntimeLayout();
            RefreshCardTransforms();
        }

        private void ClearHand()
        {
            if (handRoot == null) return;

            for (int i = handRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(handRoot.GetChild(i).gameObject);
            }

            spawnedCards.Clear();
            highlightedIndex = -1;
        }

        private HandCardUI CreateCardUI(CardRuntime card, int handIndex)
        {
            GameObject cardObject = new GameObject($"Hand Card {handIndex + 1}");
            cardObject.transform.SetParent(handRoot, false);

            RectTransform rect = cardObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = EffectiveCardSize();

            Image clickTarget = cardObject.AddComponent<Image>();
            clickTarget.color = new Color(0f, 0f, 0f, 0.01f);

            Button button = cardObject.AddComponent<Button>();
            button.targetGraphic = clickTarget;
            button.transition = Selectable.Transition.None;
            button.onClick.AddListener(() => HandleCardClicked(handIndex));

            Image shadow = CreateChildImage(cardObject.transform, "Shadow");
            shadow.color = new Color(0f, 0f, 0f, 0.30f);
            Image artwork = CreateChildImage(cardObject.transform, "Artwork");
            Image frame = CreateChildImage(cardObject.transform, "Frame");

            Text cost = CreateText(cardObject.transform, "Cost", SafeFont(costFontSize, 10, 18), FontStyle.Bold, TextAnchor.MiddleCenter);
            Text name = CreateText(cardObject.transform, "Name", SafeFont(nameFontSize, 10, 18), FontStyle.Bold, TextAnchor.MiddleCenter);
            Text stats = CreateText(cardObject.transform, "Stats", SafeFont(statsFontSize, 8, 13), FontStyle.Bold, TextAnchor.MiddleCenter);

            HandCardUI cardUI = new HandCardUI(rect, clickTarget, shadow, artwork, frame, cost, name, stats);
            cardUI.SetCard(card);
            cardUI.SetSize(EffectiveCardSize());
            return cardUI;
        }

        private Image CreateChildImage(Transform parent, string objectName)
        {
            GameObject obj = new GameObject(objectName);
            obj.transform.SetParent(parent, false);

            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);

            Image image = obj.AddComponent<Image>();
            image.color = Color.white;
            image.preserveAspect = true;
            image.raycastTarget = false;
            return image;
        }

        private Text CreateText(Transform parent, string objectName, int fontSize, FontStyle fontStyle, TextAnchor alignment)
        {
            GameObject obj = new GameObject(objectName);
            obj.transform.SetParent(parent, false);

            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);

            Text text = obj.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.color = new Color(0.98f, 0.91f, 0.72f, 1f);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(7, fontSize - 5);
            text.resizeTextMaxSize = fontSize;
            text.raycastTarget = false;
            return text;
        }

        private void RefreshCardTransforms()
        {
            int count = spawnedCards.Count;
            if (count <= 0) return;

            Vector2 size = EffectiveCardSize();
            float step = size.x + EffectiveSpacing();

            for (int i = 0; i < count; i++)
            {
                float centeredIndex = i - (count - 1) * 0.5f;
                float normalized = count <= 1 ? 0f : centeredIndex / ((count - 1) * 0.5f);

                float x = centeredIndex * step;
                float curveY = -Mathf.Abs(normalized) * Mathf.Max(28f, fanCurve);
                float rotation = -normalized * Mathf.Clamp(maxFanAngle, 4f, 18f);

                if (i == highlightedIndex)
                {
                    curveY += Mathf.Max(42f, selectedLift);
                    rotation *= 0.35f;
                }

                spawnedCards[i].SetTransform(new Vector2(x, curveY), rotation, i);
            }
        }

        private void HandleCardClicked(int handIndex)
        {
            if (battleManager == null || battleManager.PlayerState == null || battleManager.PlayerState.Hand == null) return;

            CardRuntime clickedCard = battleManager.PlayerState.Hand.GetCardAt(handIndex);
            if (clickedCard == null) return;

            highlightedIndex = handIndex;
            RefreshCardTransforms();

            if (cardPreviewUI == null) cardPreviewUI = FindFirstObjectByType<CardPreviewUI>();
            if (cardPreviewUI == null) return;

            cardPreviewUI.Show(clickedCard, TryPlayConfirmedCard);
        }

        private void TryPlayConfirmedCard(CardRuntime card)
        {
            if (card == null || battleManager == null || battleManager.PlayerState == null) return;
            if (battleManager.TurnManager == null || !battleManager.TurnManager.IsPlayerTurn) return;

            switch (card.CardType)
            {
                case CardType.Creature:
                    TryPlayCreature(card);
                    break;
                case CardType.Trap:
                    TrySetTrap(card);
                    break;
                case CardType.Spell:
                    Debug.Log($"Sistema de magia com alvo ainda será implementado. {card.CardName} não foi jogada.");
                    break;
                case CardType.Equipment:
                    Debug.Log($"Sistema de equipamento ainda será implementado. {card.CardName} não foi jogada.");
                    break;
            }
        }

        private void TryPlayCreature(CardRuntime card)
        {
            if (battleManager.PlayerState.Board == null || !battleManager.PlayerState.Board.HasFreeCreatureSlot()) return;

            if (battleManager.PlayerState.TryPlayCreatureInFirstFreeSlot(card, out int slotIndex))
            {
                Debug.Log($"Jogador confirmou e colocou {card.CardName} no slot {slotIndex + 1}.");
                ForceRefreshHand();
            }
        }

        private void TrySetTrap(CardRuntime card)
        {
            if (battleManager.PlayerState.Board == null || !battleManager.PlayerState.Board.HasFreeTrapSlot()) return;

            if (battleManager.PlayerState.TrySetTrapInFirstFreeSlot(card, out int slotIndex))
            {
                Debug.Log($"Jogador confirmou e preparou armadilha {card.CardName} no slot {slotIndex + 1}.");
                ForceRefreshHand();
            }
        }

        private void ForceRefreshHand()
        {
            lastHandCount = -1;
            highlightedIndex = -1;
            RefreshIfNeeded();
        }

        private void RefreshCardTextsOnly()
        {
            if (battleManager == null || battleManager.PlayerState == null || battleManager.PlayerState.Hand == null) return;

            IReadOnlyList<CardRuntime> cards = battleManager.PlayerState.Hand.Cards;
            for (int i = 0; i < spawnedCards.Count && i < cards.Count; i++)
            {
                spawnedCards[i].SetCard(cards[i]);
            }
        }

        private sealed class HandCardUI
        {
            private readonly RectTransform rectTransform;
            private readonly Image clickTarget;
            private readonly Image shadowImage;
            private readonly Image artworkImage;
            private readonly Image frameImage;
            private readonly Text costText;
            private readonly Text nameText;
            private readonly Text statsText;

            public HandCardUI(RectTransform rectTransform, Image clickTarget, Image shadowImage, Image artworkImage, Image frameImage, Text costText, Text nameText, Text statsText)
            {
                this.rectTransform = rectTransform;
                this.clickTarget = clickTarget;
                this.shadowImage = shadowImage;
                this.artworkImage = artworkImage;
                this.frameImage = frameImage;
                this.costText = costText;
                this.nameText = nameText;
                this.statsText = statsText;
            }

            public void SetSize(Vector2 size)
            {
                rectTransform.sizeDelta = size;

                shadowImage.rectTransform.sizeDelta = new Vector2(size.x * 0.92f, size.y * 0.96f);
                shadowImage.rectTransform.anchoredPosition = new Vector2(0f, size.y * 0.02f);

                artworkImage.rectTransform.sizeDelta = new Vector2(size.x * 0.70f, size.y * 0.43f);
                artworkImage.rectTransform.anchoredPosition = new Vector2(0f, size.y * 0.16f);

                frameImage.rectTransform.anchorMin = Vector2.zero;
                frameImage.rectTransform.anchorMax = Vector2.one;
                frameImage.rectTransform.offsetMin = Vector2.zero;
                frameImage.rectTransform.offsetMax = Vector2.zero;

                costText.rectTransform.sizeDelta = new Vector2(size.x * 0.18f, size.y * 0.10f);
                costText.rectTransform.anchoredPosition = new Vector2(-size.x * 0.335f, size.y * 0.405f);

                nameText.rectTransform.sizeDelta = new Vector2(size.x * 0.78f, size.y * 0.12f);
                nameText.rectTransform.anchoredPosition = new Vector2(0f, -size.y * 0.18f);

                statsText.rectTransform.sizeDelta = new Vector2(size.x * 0.80f, size.y * 0.19f);
                statsText.rectTransform.anchoredPosition = new Vector2(0f, -size.y * 0.35f);
            }

            public void SetTransform(Vector2 anchoredPosition, float rotationZ, int siblingIndex)
            {
                rectTransform.anchoredPosition = anchoredPosition;
                rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
                rectTransform.SetSiblingIndex(siblingIndex);
            }

            public void SetCard(CardRuntime card)
            {
                if (card == null)
                {
                    artworkImage.gameObject.SetActive(false);
                    frameImage.sprite = null;
                    costText.text = string.Empty;
                    nameText.text = string.Empty;
                    statsText.text = string.Empty;
                    return;
                }

                Sprite artwork = card.Data != null ? card.Data.Artwork : null;
                artworkImage.gameObject.SetActive(artwork != null);
                artworkImage.sprite = artwork;

                frameImage.sprite = CardGameArtResolver.GetFrame(card.CardType);
                frameImage.color = Color.white;
                clickTarget.color = new Color(0f, 0f, 0f, 0.01f);

                costText.text = card.Data != null ? card.Data.Cost.ToString() : "0";
                nameText.text = ShortName(card.CardName);
                statsText.text = card.CardType == CardType.Creature
                    ? $"ATK {card.CurrentAttack}  HP {card.CurrentHealth}\nSPD {card.CurrentSpeed}  DEF {card.CurrentDefense}"
                    : ShortDescription(card.Data != null ? card.Data.Description : string.Empty);
            }

            private static string ShortName(string value)
            {
                if (string.IsNullOrWhiteSpace(value)) return "Carta";
                return value.Length <= 16 ? value : value.Substring(0, 16) + "...";
            }

            private static string ShortDescription(string value)
            {
                if (string.IsNullOrWhiteSpace(value)) return "Toque para ver.";
                return value.Length <= 42 ? value : value.Substring(0, 42) + "...";
            }
        }
    }
}
