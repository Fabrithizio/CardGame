// Caminho: Assets/_Project/Scripts/UI/PlayerHandUI.cs
// Descrição: Mão do jogador em leque mais alta, mais legível e atualizada em tempo real.

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

        [Header("Atualização em Tempo Real")]
        [SerializeField] private bool updateLayoutEveryFrame = true;

        [Header("Layout")]
        [SerializeField] private Vector2 cardSize = new Vector2(136f, 198f);
        [SerializeField] private float cardSpacing = -54f;
        [SerializeField] private Vector2 anchorPosition = new Vector2(0f, 30f);
        [SerializeField] private float maxFanAngle = 12f;
        [SerializeField] private float selectedLift = 38f;
        [SerializeField] private float fanCurve = 24f;

        [Header("Texto")]
        [SerializeField] private int nameFontSize = 15;
        [SerializeField] private int statsFontSize = 11;
        [SerializeField] private int typeFontSize = 10;

        private RectTransform root;
        private RectTransform handRoot;
        private Font defaultFont;

        private readonly List<HandCardUI> spawnedCards = new();

        private int lastHandCount = -1;
        private int lastHoveredIndex = -1;

        private void Awake()
        {
            if (battleManager == null)
            {
                battleManager = FindFirstObjectByType<BattleManager>();
            }

            if (cardPreviewUI == null)
            {
                cardPreviewUI = FindFirstObjectByType<CardPreviewUI>();
            }

            defaultFont = ResponsiveUIUtility.GetDefaultFont();

            CreateCanvas();
            CreateHandRoot();
        }

        private void Update()
        {
            RefreshIfNeeded();

            if (updateLayoutEveryFrame)
            {
                ApplyRuntimeLayout();
            }

            RefreshCardTransforms();
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            ApplyRuntimeLayout();
            RefreshCardTransforms();
        }

        private void CreateCanvas()
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
            handRoot.sizeDelta = new Vector2(900f, 300f);
        }

        private void ApplyRuntimeLayout()
        {
            if (handRoot == null || root == null)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();

            handRoot.anchoredPosition = anchorPosition;
            handRoot.sizeDelta = new Vector2(Mathf.Max(600f, root.rect.width), Mathf.Max(240f, root.rect.height + 130f));

            for (int i = 0; i < spawnedCards.Count; i++)
            {
                spawnedCards[i].SetSize(cardSize);
            }
        }

        private void RefreshIfNeeded()
        {
            if (battleManager == null || battleManager.PlayerState == null || battleManager.PlayerState.Hand == null)
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

            ApplyRuntimeLayout();
            RefreshCardTransforms();
        }

        private void ClearHand()
        {
            if (handRoot == null)
            {
                return;
            }

            for (int i = handRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(handRoot.GetChild(i).gameObject);
            }

            spawnedCards.Clear();
            lastHoveredIndex = -1;
        }

        private HandCardUI CreateCardUI(CardRuntime card, int handIndex)
        {
            GameObject cardObject = new GameObject("Hand Card");
            cardObject.transform.SetParent(handRoot, false);

            RectTransform rect = cardObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = cardSize;

            Image background = cardObject.AddComponent<Image>();
            background.color = GetColorByCardType(card);

            Outline outline = cardObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.72f);
            outline.effectDistance = new Vector2(3f, -3f);

            Shadow shadow = cardObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.58f);
            shadow.effectDistance = new Vector2(0f, -5f);

            Button button = cardObject.AddComponent<Button>();
            button.targetGraphic = background;
            button.onClick.AddListener(() => HandleCardClicked(handIndex));

            VerticalLayoutGroup vertical = cardObject.AddComponent<VerticalLayoutGroup>();
            vertical.padding = new RectOffset(8, 8, 7, 8);
            vertical.spacing = 3;
            vertical.childAlignment = TextAnchor.MiddleCenter;
            vertical.childControlWidth = true;
            vertical.childControlHeight = false;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;

            Text costText = CreateText(cardObject.transform, "Cost", typeFontSize + 3, FontStyle.Bold, 18f);
            Image artworkImage = CreateArtwork(cardObject.transform, "Artwork", cardSize.x - 16f, 72f);
            Text nameText = CreateText(cardObject.transform, "Name", nameFontSize, FontStyle.Bold, 34f);
            Text typeText = CreateText(cardObject.transform, "Type", typeFontSize, FontStyle.Italic, 17f);
            Text statsText = CreateText(cardObject.transform, "Stats", statsFontSize, FontStyle.Normal, 38f);

            HandCardUI cardUI = new HandCardUI(rect, background, artworkImage, costText, nameText, typeText, statsText);
            cardUI.SetCard(card);
            cardUI.SetSize(cardSize);

            return cardUI;
        }

        private Text CreateText(Transform parent, string objectName, int fontSize, FontStyle fontStyle, float height)
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
            rect.sizeDelta = new Vector2(cardSize.x - 20f, height);

            return text;
        }

        private Image CreateArtwork(Transform parent, string objectName, float width, float height)
        {
            GameObject artworkObject = new GameObject(objectName);
            artworkObject.transform.SetParent(parent, false);

            RectTransform rect = artworkObject.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, height);

            Image image = artworkObject.AddComponent<Image>();
            image.color = Color.white;
            image.preserveAspect = true;
            image.raycastTarget = false;
            artworkObject.SetActive(false);

            return image;
        }

        private void RefreshCardTransforms()
        {
            int count = spawnedCards.Count;

            if (count <= 0)
            {
                return;
            }

            float step = cardSize.x + cardSpacing;

            for (int i = 0; i < count; i++)
            {
                float centeredIndex = i - (count - 1) * 0.5f;
                float normalized = count <= 1 ? 0f : centeredIndex / ((count - 1) * 0.5f);

                float x = centeredIndex * step;
                float curveY = -Mathf.Abs(normalized) * fanCurve;
                float rotation = -normalized * maxFanAngle;

                if (i == lastHoveredIndex)
                {
                    curveY += selectedLift;
                    rotation *= 0.35f;
                }

                spawnedCards[i].SetTransform(new Vector2(x, curveY), rotation, i);
            }
        }

        private void HandleCardClicked(int handIndex)
        {
            if (battleManager == null || battleManager.PlayerState == null || battleManager.PlayerState.Hand == null)
            {
                return;
            }

            CardRuntime clickedCard = battleManager.PlayerState.Hand.GetCardAt(handIndex);

            if (clickedCard == null)
            {
                Debug.Log("Carta inválida na mão.");
                return;
            }

            lastHoveredIndex = handIndex;
            RefreshCardTransforms();

            if (cardPreviewUI == null)
            {
                Debug.LogWarning("CardPreviewUI não encontrado na cena. Jogada bloqueada para evitar clique acidental.");
                return;
            }

            cardPreviewUI.Show(clickedCard, TryPlayConfirmedCard);
        }

        private void TryPlayConfirmedCard(CardRuntime card)
        {
            if (card == null || battleManager == null || battleManager.PlayerState == null)
            {
                return;
            }

            if (battleManager.TurnManager == null || !battleManager.TurnManager.IsPlayerTurn)
            {
                Debug.Log("Você só pode jogar cartas da sua mão no seu turno.");
                return;
            }

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
                    Debug.Log($"Sistema de equipamento ainda está guardado na gaveta. {card.CardName} não foi jogada.");
                    break;

                default:
                    Debug.Log($"Tipo de carta não suportado: {card.CardType}.");
                    break;
            }
        }

        private void TryPlayCreature(CardRuntime card)
        {
            if (battleManager.PlayerState.Board == null || !battleManager.PlayerState.Board.HasFreeCreatureSlot())
            {
                Debug.Log("Não há slot livre para colocar criatura.");
                return;
            }

            if (battleManager.PlayerState.TryPlayCreatureInFirstFreeSlot(card, out int slotIndex))
            {
                Debug.Log($"Jogador confirmou e colocou {card.CardName} no slot {slotIndex + 1}.");
                ForceRefreshHand();
                return;
            }

            Debug.Log($"Não foi possível jogar {card.CardName}.");
        }

        private void TrySetTrap(CardRuntime card)
        {
            if (battleManager.PlayerState.Board == null || !battleManager.PlayerState.Board.HasFreeTrapSlot())
            {
                Debug.Log("Não há slot livre para preparar armadilha.");
                return;
            }

            if (battleManager.PlayerState.TrySetTrapInFirstFreeSlot(card, out int slotIndex))
            {
                Debug.Log($"Jogador confirmou e preparou armadilha {card.CardName} no slot {slotIndex + 1}.");
                ForceRefreshHand();
                return;
            }

            Debug.Log($"Não foi possível preparar {card.CardName}.");
        }

        private void ForceRefreshHand()
        {
            lastHandCount = -1;
            lastHoveredIndex = -1;
            RefreshIfNeeded();
        }

        private void RefreshCardTextsOnly()
        {
            if (battleManager == null || battleManager.PlayerState == null || battleManager.PlayerState.Hand == null)
            {
                return;
            }

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
                CardType.Creature => new Color(0.08f, 0.26f, 0.65f, 0.96f),
                CardType.Spell => new Color(0.33f, 0.12f, 0.68f, 0.96f),
                CardType.Trap => new Color(0.72f, 0.34f, 0.10f, 0.96f),
                CardType.Equipment => new Color(0.42f, 0.42f, 0.42f, 0.96f),
                _ => new Color(0.18f, 0.18f, 0.18f, 0.86f)
            };
        }

        private sealed class HandCardUI
        {
            private readonly RectTransform rectTransform;
            private readonly Image background;
            private readonly Image artworkImage;
            private readonly Text costText;
            private readonly Text nameText;
            private readonly Text typeText;
            private readonly Text statsText;

            public HandCardUI(RectTransform rectTransform, Image background, Image artworkImage, Text costText, Text nameText, Text typeText, Text statsText)
            {
                this.rectTransform = rectTransform;
                this.background = background;
                this.artworkImage = artworkImage;
                this.costText = costText;
                this.nameText = nameText;
                this.typeText = typeText;
                this.statsText = statsText;
            }

            public void SetSize(Vector2 size)
            {
                rectTransform.sizeDelta = size;

                float innerWidth = Mathf.Max(12f, size.x - 16f);
                costText.rectTransform.sizeDelta = new Vector2(innerWidth, Mathf.Clamp(size.y * 0.10f, 16f, 22f));
                artworkImage.rectTransform.sizeDelta = new Vector2(innerWidth, Mathf.Clamp(size.y * 0.36f, 56f, 82f));
                nameText.rectTransform.sizeDelta = new Vector2(innerWidth, Mathf.Clamp(size.y * 0.17f, 26f, 38f));
                typeText.rectTransform.sizeDelta = new Vector2(innerWidth, Mathf.Clamp(size.y * 0.09f, 14f, 20f));
                statsText.rectTransform.sizeDelta = new Vector2(innerWidth, Mathf.Clamp(size.y * 0.19f, 30f, 44f));
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
                    background.color = new Color(0.18f, 0.18f, 0.18f, 0.86f);
                    artworkImage.gameObject.SetActive(false);
                    artworkImage.sprite = null;
                    costText.text = string.Empty;
                    nameText.text = "Vazio";
                    typeText.text = string.Empty;
                    statsText.text = string.Empty;
                    return;
                }

                Sprite artwork = card.Data != null ? card.Data.Artwork : null;
                artworkImage.gameObject.SetActive(artwork != null);
                artworkImage.sprite = artwork;
                artworkImage.color = Color.white;

                background.color = GetColorByCardType(card);
                costText.text = $"CUSTO {card.Data.Cost}";
                nameText.text = ShortName(card.CardName);
                typeText.text = GetTypeLabel(card.CardType);

                if (card.CardType == CardType.Creature)
                {
                    statsText.text =
                        $"ATK {card.CurrentAttack}  HP {card.CurrentHealth}\n" +
                        $"SPD {card.CurrentSpeed}  DEF {card.CurrentDefense}\n" +
                        $"FOC {card.CurrentFocus}  RES {card.CurrentResistance}";
                }
                else
                {
                    statsText.text = ShortDescription(card.Data.Description);
                }
            }

            private string ShortName(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return "Carta";
                }

                if (value.Length <= 18)
                {
                    return value;
                }

                return value.Substring(0, 18) + "...";
            }

            private string ShortDescription(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return "Toque para ver.";
                }

                if (value.Length <= 45)
                {
                    return value;
                }

                return value.Substring(0, 45) + "...";
            }

            private Color GetColorByCardType(CardRuntime card)
            {
                return card.CardType switch
                {
                    CardType.Creature => new Color(0.06f, 0.18f, 0.45f, 0.96f),
                    CardType.Spell => new Color(0.28f, 0.10f, 0.48f, 0.96f),
                    CardType.Trap => new Color(0.54f, 0.24f, 0.08f, 0.96f),
                    CardType.Equipment => new Color(0.34f, 0.34f, 0.36f, 0.96f),
                    _ => new Color(0.18f, 0.18f, 0.18f, 0.86f)
                };
            }

            private string GetTypeLabel(CardType cardType)
            {
                return cardType switch
                {
                    CardType.Creature => "CRIATURA",
                    CardType.Spell => "MAGIA",
                    CardType.Trap => "ARMADILHA",
                    CardType.Equipment => "EQUIP.",
                    _ => "CARTA"
                };
            }
        }
    }
}
