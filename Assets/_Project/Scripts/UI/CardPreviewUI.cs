// Caminho: Assets/_Project/Scripts/UI/CardPreviewUI.cs
// Descrição: Mostra uma prévia grande da carta selecionada, com botões para confirmar a jogada ou fechar sem jogar.

using CardGame.Battle;
using CardGame.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class CardPreviewUI : MonoBehaviour
    {
        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;

        [Header("Layout")]
        [SerializeField] private Vector2 panelSize = new Vector2(520f, 720f);
        [SerializeField] private Vector2 buttonSize = new Vector2(190f, 58f);

        [Header("Texto")]
        [SerializeField] private int titleFontSize = 30;
        [SerializeField] private int infoFontSize = 18;
        [SerializeField] private int descriptionFontSize = 17;
        [SerializeField] private int buttonFontSize = 20;

        [Header("Cores")]
        [SerializeField] private Color overlayColor = new Color(0f, 0f, 0f, 0.58f);
        [SerializeField] private Color creatureColor = new Color(0.10f, 0.28f, 0.62f, 0.98f);
        [SerializeField] private Color spellColor = new Color(0.35f, 0.16f, 0.62f, 0.98f);
        [SerializeField] private Color trapColor = new Color(0.68f, 0.35f, 0.10f, 0.98f);
        [SerializeField] private Color equipmentColor = new Color(0.42f, 0.42f, 0.42f, 0.98f);
        [SerializeField] private Color playButtonColor = new Color(0.12f, 0.48f, 0.22f, 0.98f);
        [SerializeField] private Color closeButtonColor = new Color(0.45f, 0.12f, 0.12f, 0.98f);

        private Canvas canvas;
        private RectTransform root;
        private GameObject overlayObject;
        private GameObject panelObject;
        private Image panelBackground;
        private Image artworkImage;
        private Text titleText;
        private Text typeText;
        private Text statsText;
        private Text descriptionText;
        private Button playButton;
        private Text playButtonText;
        private Font defaultFont;

        private CardRuntime selectedCard;
        private System.Action<CardRuntime> onConfirmPlay;

        public bool IsOpen => overlayObject != null && overlayObject.activeSelf;

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
            CreatePreview();
            Hide();
        }

        public void Show(CardRuntime card, System.Action<CardRuntime> confirmCallback)
        {
            if (card == null)
            {
                return;
            }

            selectedCard = card;
            onConfirmPlay = confirmCallback;

            overlayObject.SetActive(true);
            panelObject.SetActive(true);

            RefreshTexts();
        }

        public void Hide()
        {
            selectedCard = null;
            onConfirmPlay = null;

            if (overlayObject != null)
            {
                overlayObject.SetActive(false);
            }

            if (panelObject != null)
            {
                panelObject.SetActive(false);
            }
        }

        private void ConfirmPlay()
        {
            if (selectedCard == null)
            {
                Hide();
                return;
            }

            CardRuntime cardToPlay = selectedCard;
            System.Action<CardRuntime> callback = onConfirmPlay;

            Hide();

            callback?.Invoke(cardToPlay);
        }

        private void CreateCanvas()
        {
            GameObject canvasObject = new GameObject("Card Preview UI Canvas");
            canvasObject.transform.SetParent(transform, false);

            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 160;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();

            root = canvasObject.GetComponent<RectTransform>();
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;
        }

        private void CreatePreview()
        {
            CreateOverlay();
            CreatePanel();
        }

        private void CreateOverlay()
        {
            overlayObject = new GameObject("Card Preview Overlay");
            overlayObject.transform.SetParent(root, false);

            RectTransform rect = overlayObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = overlayObject.AddComponent<Image>();
            image.color = overlayColor;

            Button closeByBackground = overlayObject.AddComponent<Button>();
            closeByBackground.targetGraphic = image;
            closeByBackground.onClick.AddListener(Hide);
        }

        private void CreatePanel()
        {
            panelObject = new GameObject("Card Preview Panel");
            panelObject.transform.SetParent(root, false);

            RectTransform panelRect = panelObject.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = panelSize;

            panelBackground = panelObject.AddComponent<Image>();
            panelBackground.color = creatureColor;

            Outline outline = panelObject.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.32f);
            outline.effectDistance = new Vector2(3f, -3f);

            VerticalLayoutGroup vertical = panelObject.AddComponent<VerticalLayoutGroup>();
            vertical.padding = new RectOffset(26, 26, 30, 24);
            vertical.spacing = 14;
            vertical.childAlignment = TextAnchor.UpperCenter;
            vertical.childControlWidth = true;
            vertical.childControlHeight = false;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;

            titleText = CreateText("Title", titleFontSize, FontStyle.Bold, 78f, TextAnchor.MiddleCenter);
            typeText = CreateText("Type", infoFontSize, FontStyle.Italic, 42f, TextAnchor.MiddleCenter);
            artworkImage = CreateArtwork("Artwork", panelSize.x - 52f, Mathf.Clamp(panelSize.y * 0.27f, 220f, 330f));
            statsText = CreateText("Stats", infoFontSize, FontStyle.Bold, 170f, TextAnchor.MiddleCenter);
            descriptionText = CreateText("Description", descriptionFontSize, FontStyle.Normal, 230f, TextAnchor.UpperLeft);

            CreateButtonRow();
        }

        private Image CreateArtwork(string objectName, float width, float height)
        {
            GameObject artworkObject = new GameObject(objectName);
            artworkObject.transform.SetParent(panelObject.transform, false);

            RectTransform rect = artworkObject.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, height);

            Image image = artworkObject.AddComponent<Image>();
            image.color = Color.white;
            image.preserveAspect = true;
            image.raycastTarget = false;
            artworkObject.SetActive(false);

            Outline outline = artworkObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.55f);
            outline.effectDistance = new Vector2(2f, -2f);

            return image;
        }

        private Text CreateText(string objectName, int fontSize, FontStyle fontStyle, float height, TextAnchor alignment)
        {
            GameObject textObject = new GameObject(objectName);
            textObject.transform.SetParent(panelObject.transform, false);

            Text text = textObject.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 10;
            text.resizeTextMaxSize = fontSize;

            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(panelSize.x - 52f, height);

            return text;
        }

        private void CreateButtonRow()
        {
            GameObject rowObject = new GameObject("Button Row");
            rowObject.transform.SetParent(panelObject.transform, false);

            RectTransform rowRect = rowObject.AddComponent<RectTransform>();
            rowRect.sizeDelta = new Vector2(panelSize.x - 52f, buttonSize.y);

            HorizontalLayoutGroup row = rowObject.AddComponent<HorizontalLayoutGroup>();
            row.spacing = 18f;
            row.childAlignment = TextAnchor.MiddleCenter;
            row.childControlWidth = false;
            row.childControlHeight = false;
            row.childForceExpandWidth = false;
            row.childForceExpandHeight = false;

            CreateButton(rowObject.transform, "FECHAR", closeButtonColor, Hide);
            playButton = CreateButton(rowObject.transform, "JOGAR", playButtonColor, ConfirmPlay);
            playButtonText = playButton.GetComponentInChildren<Text>();
        }

        private Button CreateButton(Transform parent, string label, Color color, UnityEngine.Events.UnityAction action)
        {
            GameObject buttonObject = new GameObject(label + " Button");
            buttonObject.transform.SetParent(parent, false);

            RectTransform rect = buttonObject.AddComponent<RectTransform>();
            rect.sizeDelta = buttonSize;

            LayoutElement layout = buttonObject.AddComponent<LayoutElement>();
            layout.preferredWidth = buttonSize.x;
            layout.preferredHeight = buttonSize.y;

            Image background = buttonObject.AddComponent<Image>();
            background.color = color;

            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = background;
            button.onClick.AddListener(action);

            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(buttonObject.transform, false);

            Text text = textObject.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = buttonFontSize;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.text = label;

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            return button;
        }

        private void RefreshTexts()
        {
            if (selectedCard == null)
            {
                return;
            }

            panelBackground.color = GetColorByCardType(selectedCard.CardType);

            Sprite artwork = selectedCard.Data != null ? selectedCard.Data.Artwork : null;
            artworkImage.gameObject.SetActive(artwork != null);
            artworkImage.sprite = artwork;

            titleText.text = selectedCard.CardName;
            typeText.text = $"{selectedCard.CardType} • {selectedCard.Data.Rarity} • Custo {selectedCard.Data.Cost}";
            statsText.text = BuildStatsText(selectedCard);
            descriptionText.text = BuildDescriptionText(selectedCard);

            bool canPlayNow = CanPlaySelectedCard();
            playButton.interactable = canPlayNow;

            if (playButtonText != null)
            {
                playButtonText.text = canPlayNow ? "JOGAR" : "INDISPONÍVEL";
            }
        }

        private bool CanPlaySelectedCard()
        {
            if (selectedCard == null || battleManager == null || battleManager.PlayerState == null || battleManager.TurnManager == null)
            {
                return false;
            }

            if (!battleManager.TurnManager.IsPlayerTurn)
            {
                return false;
            }

            if (!battleManager.PlayerState.HasEnoughEnergy(selectedCard.Data.Cost))
            {
                return false;
            }

            if (selectedCard.CardType == CardType.Creature)
            {
                return battleManager.PlayerState.Board.HasFreeCreatureSlot();
            }

            if (selectedCard.CardType == CardType.Trap)
            {
                return battleManager.PlayerState.Board.HasFreeTrapSlot();
            }

            if (selectedCard.CardType == CardType.Spell)
            {
                return false;
            }

            if (selectedCard.CardType == CardType.Equipment)
            {
                return false;
            }

            return false;
        }

        private string BuildStatsText(CardRuntime card)
        {
            if (card.CardType != CardType.Creature)
            {
                return $"Custo: {card.Data.Cost}";
            }

            return
                $"ATK {card.CurrentAttack}   HP {card.CurrentHealth}\n" +
                $"SPD {card.CurrentSpeed}   DEF {card.CurrentDefense}\n" +
                $"FOC {card.CurrentFocus}   RES {card.CurrentResistance}";
        }

        private string BuildDescriptionText(CardRuntime card)
        {
            string description = string.IsNullOrWhiteSpace(card.Data.Description)
                ? "Sem descrição."
                : card.Data.Description;

            if (card.CardType != CardType.Creature)
            {
                return description + "\n\nObs: magias e arenas ainda vão receber sistema de alvo/efeito completo.";
            }

            string specialRules = "";

            if (card.Data.CanAttackDirectly)
            {
                specialRules += "\n• Pode atacar diretamente.";
            }

            if (card.Data.HasTaunt)
            {
                specialRules += "\n• Provocar.";
            }

            if (card.Data.HasPiercing)
            {
                specialRules += "\n• Perfuração.";
            }

            if (card.Data.HasLifeSteal)
            {
                specialRules += "\n• Roubo de vida.";
            }

            if (card.Data.HasRetaliation)
            {
                specialRules += "\n• Retaliação.";
            }

            if (card.Data.HasShield)
            {
                specialRules += "\n• Escudo inicial.";
            }

            if (string.IsNullOrWhiteSpace(specialRules))
            {
                specialRules = "\n• Sem regra especial ativa no protótipo.";
            }

            return description + "\n" + specialRules;
        }

        private Color GetColorByCardType(CardType cardType)
        {
            return cardType switch
            {
                CardType.Creature => creatureColor,
                CardType.Spell => spellColor,
                CardType.Trap => trapColor,
                CardType.Equipment => equipmentColor,
                _ => creatureColor
            };
        }
    }
}
