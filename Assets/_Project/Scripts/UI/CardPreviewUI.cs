// Caminho: Assets/_Project/Scripts/UI/CardPreviewUI.cs
// Descrição: Prévia grande premium usando molduras do tema visual. Mantém botões de jogar/fechar.

using CardGame.Battle;
using CardGame.Cards;
using CardGame.Visual;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class CardPreviewUI : MonoBehaviour
    {
        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;

        [Header("Layout")]
        [SerializeField] private Vector2 panelSize = new Vector2(650f, 910f);
        [SerializeField] private Vector2 buttonSize = new Vector2(190f, 58f);

        [Header("Texto")]
        [SerializeField] private int titleFontSize = 26;
        [SerializeField] private int infoFontSize = 15;
        [SerializeField] private int descriptionFontSize = 15;
        [SerializeField] private int buttonFontSize = 18;

        [Header("Cores")]
        [SerializeField] private Color overlayColor = new Color(0f, 0f, 0f, 0.64f);
        [SerializeField] private Color textColor = new Color(0.98f, 0.91f, 0.72f, 1f);
        [SerializeField] private Color subTextColor = new Color(0.74f, 0.82f, 1f, 0.92f);
        [SerializeField] private Color playButtonColor = new Color(0.10f, 0.42f, 0.23f, 0.98f);
        [SerializeField] private Color closeButtonColor = new Color(0.46f, 0.10f, 0.12f, 0.98f);
        [SerializeField] private Color unavailableButtonColor = new Color(0.12f, 0.23f, 0.34f, 0.82f);

        private Canvas canvas;
        private RectTransform root;
        private GameObject overlayObject;
        private GameObject panelObject;
        private Image backgroundImage;
        private Image artworkImage;
        private Image frameImage;
        private Text titleText;
        private Text typeText;
        private Text costText;
        private Text statsText;
        private Text descriptionText;
        private Button playButton;
        private Image playButtonImage;
        private Text playButtonText;
        private Font defaultFont;
        private CardGameArtTheme theme;

        private CardRuntime selectedCard;
        private System.Action<CardRuntime> onConfirmPlay;

        public bool IsOpen => overlayObject != null && overlayObject.activeSelf;

        private void Awake()
        {
            if (battleManager == null) battleManager = FindFirstObjectByType<BattleManager>();

            defaultFont = ResponsiveUIUtility.GetDefaultFont();
            theme = CardGameArtThemeProvider.Current;
            CreateCanvas();
            CreatePreview();
            Hide();
        }

        private void Update()
        {
            if (theme == null) theme = CardGameArtThemeProvider.Current;
        }

        public void Show(CardRuntime card, System.Action<CardRuntime> confirmCallback)
        {
            if (card == null) return;

            selectedCard = card;
            onConfirmPlay = confirmCallback;
            overlayObject.SetActive(true);
            panelObject.SetActive(true);
            RefreshView();
        }

        public void Hide()
        {
            selectedCard = null;
            onConfirmPlay = null;
            if (overlayObject != null) overlayObject.SetActive(false);
            if (panelObject != null) panelObject.SetActive(false);
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
            scaler.referenceResolution = new Vector2(1080f, 2400f);
            scaler.matchWidthOrHeight = 0.65f;

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

            backgroundImage = panelObject.AddComponent<Image>();
            backgroundImage.color = new Color(0.01f, 0.015f, 0.035f, 0.96f);

            artworkImage = CreateImage("Artwork");
            frameImage = CreateImage("Frame");

            titleText = CreateText("Title", titleFontSize, FontStyle.Bold, TextAnchor.MiddleCenter);
            typeText = CreateText("Type", infoFontSize, FontStyle.Italic, TextAnchor.MiddleCenter);
            costText = CreateText("Cost", infoFontSize + 5, FontStyle.Bold, TextAnchor.MiddleCenter);
            statsText = CreateText("Stats", infoFontSize, FontStyle.Bold, TextAnchor.MiddleCenter);
            descriptionText = CreateText("Description", descriptionFontSize, FontStyle.Normal, TextAnchor.UpperLeft);

            CreateButtonRow();
            ApplyStaticLayout();
        }

        private Image CreateImage(string objectName)
        {
            GameObject obj = new GameObject(objectName);
            obj.transform.SetParent(panelObject.transform, false);

            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            Image image = obj.AddComponent<Image>();
            image.color = Color.white;
            image.preserveAspect = true;
            image.raycastTarget = false;
            return image;
        }

        private Text CreateText(string objectName, int fontSize, FontStyle fontStyle, TextAnchor alignment)
        {
            GameObject obj = new GameObject(objectName);
            obj.transform.SetParent(panelObject.transform, false);

            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            Text text = obj.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = Mathf.Clamp(fontSize, 10, 34);
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.color = textColor;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 9;
            text.resizeTextMaxSize = Mathf.Clamp(fontSize, 10, 34);
            text.raycastTarget = false;
            return text;
        }

        private void CreateButtonRow()
        {
            GameObject rowObject = new GameObject("Button Row");
            rowObject.transform.SetParent(panelObject.transform, false);

            RectTransform rowRect = rowObject.AddComponent<RectTransform>();
            rowRect.anchorMin = new Vector2(0.5f, 0.5f);
            rowRect.anchorMax = new Vector2(0.5f, 0.5f);
            rowRect.pivot = new Vector2(0.5f, 0.5f);
            rowRect.sizeDelta = new Vector2(panelSize.x * 0.78f, buttonSize.y);
            rowRect.anchoredPosition = new Vector2(0f, -panelSize.y * 0.415f);

            HorizontalLayoutGroup row = rowObject.AddComponent<HorizontalLayoutGroup>();
            row.spacing = 20f;
            row.childAlignment = TextAnchor.MiddleCenter;
            row.childControlWidth = false;
            row.childControlHeight = false;
            row.childForceExpandWidth = false;
            row.childForceExpandHeight = false;

            CreateButton(rowObject.transform, "FECHAR", closeButtonColor, Hide);
            playButton = CreateButton(rowObject.transform, "JOGAR", playButtonColor, ConfirmPlay);
            playButtonImage = playButton.GetComponent<Image>();
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
            text.fontSize = Mathf.Clamp(buttonFontSize, 12, 24);
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

        private void ApplyStaticLayout()
        {
            RectTransform artworkRect = artworkImage.rectTransform;
            artworkRect.sizeDelta = new Vector2(panelSize.x * 0.68f, panelSize.y * 0.40f);
            artworkRect.anchoredPosition = new Vector2(0f, panelSize.y * 0.165f);

            RectTransform frameRect = frameImage.rectTransform;
            frameRect.sizeDelta = panelSize;
            frameRect.anchoredPosition = Vector2.zero;

            costText.rectTransform.sizeDelta = new Vector2(panelSize.x * 0.15f, panelSize.y * 0.08f);
            costText.rectTransform.anchoredPosition = new Vector2(-panelSize.x * 0.360f, panelSize.y * 0.410f);

            titleText.rectTransform.sizeDelta = new Vector2(panelSize.x * 0.62f, panelSize.y * 0.08f);
            titleText.rectTransform.anchoredPosition = new Vector2(0f, panelSize.y * 0.382f);

            typeText.rectTransform.sizeDelta = new Vector2(panelSize.x * 0.62f, panelSize.y * 0.055f);
            typeText.rectTransform.anchoredPosition = new Vector2(0f, -panelSize.y * 0.215f);

            statsText.rectTransform.sizeDelta = new Vector2(panelSize.x * 0.62f, panelSize.y * 0.105f);
            statsText.rectTransform.anchoredPosition = new Vector2(0f, -panelSize.y * 0.300f);

            descriptionText.rectTransform.sizeDelta = new Vector2(panelSize.x * 0.70f, panelSize.y * 0.140f);
            descriptionText.rectTransform.anchoredPosition = new Vector2(0f, -panelSize.y * 0.385f);
        }

        private void RefreshView()
        {
            if (selectedCard == null) return;
            if (theme == null) theme = CardGameArtThemeProvider.Current;

            frameImage.sprite = GetFrameForCard(selectedCard);
            backgroundImage.color = new Color(0.008f, 0.014f, 0.032f, 0.96f);

            Sprite artwork = selectedCard.Data != null ? selectedCard.Data.Artwork : null;
            artworkImage.gameObject.SetActive(artwork != null);
            artworkImage.sprite = artwork;

            titleText.text = selectedCard.CardName;
            costText.text = selectedCard.Data != null ? selectedCard.Data.Cost.ToString() : "0";
            typeText.text = selectedCard.Data != null ? $"{GetTypeLabel(selectedCard.CardType)} • {selectedCard.Data.Rarity}" : GetTypeLabel(selectedCard.CardType);
            statsText.text = BuildStatsText(selectedCard);
            descriptionText.text = BuildDescriptionText(selectedCard);
            descriptionText.color = subTextColor;

            bool canPlayNow = CanPlaySelectedCard();
            playButton.interactable = canPlayNow;
            if (playButtonImage != null) playButtonImage.color = canPlayNow ? playButtonColor : unavailableButtonColor;
            if (playButtonText != null) playButtonText.text = canPlayNow ? "JOGAR" : "VER";
        }

        private Sprite GetFrameForCard(CardRuntime card)
        {
            if (theme == null) return null;

            return card.CardType switch
            {
                CardType.Creature => theme.frameCreature,
                CardType.Spell => theme.frameSpell,
                CardType.Trap => theme.frameTrap,
                _ => theme.frameCreature
            };
        }

        private bool CanPlaySelectedCard()
        {
            if (selectedCard == null || onConfirmPlay == null || battleManager == null || battleManager.PlayerState == null || battleManager.TurnManager == null)
            {
                return false;
            }

            if (!battleManager.TurnManager.IsPlayerTurn) return false;
            if (!battleManager.PlayerState.HasEnoughEnergy(selectedCard.Data.Cost)) return false;

            return selectedCard.CardType switch
            {
                CardType.Creature => battleManager.PlayerState.Board.HasFreeCreatureSlot(),
                CardType.Trap => battleManager.PlayerState.Board.HasFreeTrapSlot(),
                _ => false
            };
        }

        private string BuildStatsText(CardRuntime card)
        {
            if (card.CardType != CardType.Creature)
            {
                return $"Custo {card.Data.Cost}";
            }

            return $"ATK {card.CurrentAttack}   HP {card.CurrentHealth}\nSPD {card.CurrentSpeed}   DEF {card.CurrentDefense}\nFOC {card.CurrentFocus}   RES {card.CurrentResistance}";
        }

        private string BuildDescriptionText(CardRuntime card)
        {
            string description = card.Data == null || string.IsNullOrWhiteSpace(card.Data.Description) ? "Sem descrição." : card.Data.Description;

            if (card.CardType != CardType.Creature)
            {
                return description;
            }

            string rules = string.Empty;
            if (card.Data.CanAttackDirectly) rules += "\n• Pode atacar diretamente.";
            if (card.Data.HasTaunt) rules += "\n• Provocar.";
            if (card.Data.HasPiercing) rules += "\n• Perfuração.";
            if (card.Data.HasLifeSteal) rules += "\n• Roubo de vida.";
            if (card.Data.HasRetaliation) rules += "\n• Retaliação.";
            if (card.Data.HasShield) rules += "\n• Escudo inicial.";

            if (string.IsNullOrWhiteSpace(rules)) rules = "\n• Sem regra especial ativa.";
            return description + rules;
        }

        private string GetTypeLabel(CardType type)
        {
            return type switch
            {
                CardType.Creature => "CRIATURA",
                CardType.Spell => "MAGIA",
                CardType.Trap => "ARMADILHA",
                CardType.Equipment => "EQUIPAMENTO",
                _ => "CARTA"
            };
        }
    }
}
