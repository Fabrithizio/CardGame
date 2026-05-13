// Caminho: Assets/_Project/Scripts/UI/BattleBoardUI.cs
// Descrição: Campo de batalha visual. Cria slots simétricos, usa molduras do tema,
// aproxima criaturas do centro, coloca traps atrás e permite preview/ataque por clique.

using System.Collections.Generic;
using CardGame.Battle;
using CardGame.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class BattleBoardUI : MonoBehaviour
    {
        private const int CreatureSlotCount = BoardRuntime.MaxCreatureSlots;
        private const int TrapSlotCount = BoardRuntime.MaxTrapSlots;

        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private CardPreviewUI cardPreviewUI;

        [Header("Atualização")]
        [SerializeField] private bool updateLayoutEveryFrame = true;

        [Header("Criaturas")]
        [SerializeField] private Vector2 creatureSize = new Vector2(210f, 318f);
        [SerializeField] private float creatureSpacing = -10f;
        [SerializeField] [Range(0.50f, 1f)] private float creatureHeightUse = 0.94f;

        [Header("Armadilhas")]
        [SerializeField] private Vector2 trapSize = new Vector2(124f, 84f);
        [SerializeField] private float trapSpacing = 4f;
        [SerializeField] [Range(0.45f, 1f)] private float trapHeightUse = 0.88f;

        [Header("Artwork nas Molduras")]
        [SerializeField] [Range(0.40f, 0.95f)] private float creatureArtworkWidthPercent = 0.70f;
        [SerializeField] [Range(0.25f, 0.85f)] private float creatureArtworkHeightPercent = 0.45f;
        [SerializeField] private Vector2 creatureArtworkPositionPercent = new Vector2(0f, 0.145f);
        [SerializeField] [Range(0.40f, 0.95f)] private float trapArtworkWidthPercent = 0.82f;
        [SerializeField] [Range(0.20f, 0.85f)] private float trapArtworkHeightPercent = 0.70f;
        [SerializeField] private Vector2 trapArtworkPositionPercent = Vector2.zero;

        [Header("Texto")]
        [SerializeField] private int nameFontSize = 14;
        [SerializeField] private int statsFontSize = 12;
        [SerializeField] private int statusFontSize = 10;
        [SerializeField] private int trapFontSize = 10;
        [SerializeField] private bool showEmptyLabels = false;
        [SerializeField] private bool hideEnemyTrapNames = true;

        [Header("Cores")]
        [SerializeField] private Color emptySlotTint = new Color(1f, 1f, 1f, 0.72f);
        [SerializeField] private Color occupiedSlotTint = new Color(1f, 1f, 1f, 0.26f);
        [SerializeField] private Color selectedTint = new Color(0.40f, 0.94f, 1f, 1f);
        [SerializeField] private Color textColor = new Color(0.97f, 0.91f, 0.73f, 1f);
        [SerializeField] private Color mutedTextColor = new Color(0.97f, 0.91f, 0.73f, 0.20f);

        [Header("Limpeza")]
        [SerializeField] private bool disableLegacyBoardVisual = true;

        private Font defaultFont;
        private readonly List<CardSlotUI> enemyCreatureSlots = new();
        private readonly List<CardSlotUI> playerCreatureSlots = new();
        private readonly List<TrapSlotUI> enemyTrapSlots = new();
        private readonly List<TrapSlotUI> playerTrapSlots = new();

        private RectTransform enemyCreatureZone;
        private RectTransform playerCreatureZone;
        private RectTransform enemyTrapZone;
        private RectTransform playerTrapZone;
        private int selectedPlayerSlotIndex = -1;
        private bool built;
        private bool legacyDisabled;

        private void Awake()
        {
            if (battleManager == null) battleManager = FindFirstObjectByType<BattleManager>();
            if (cardPreviewUI == null) cardPreviewUI = FindFirstObjectByType<CardPreviewUI>();

            defaultFont = ResponsiveUIUtility.GetDefaultFont();
            CreateBoard();
            DisableLegacyBoardVisualIfNeeded();
        }

        private void Update()
        {
            if (!built) CreateBoard();
            if (cardPreviewUI == null) cardPreviewUI = FindFirstObjectByType<CardPreviewUI>();
            if (updateLayoutEveryFrame) ApplyRuntimeLayout();
            DisableLegacyBoardVisualIfNeeded();
            Refresh();
        }

        private void CreateBoard()
        {
            if (built) return;

            BattleScreenLayoutUI layout = BattleScreenLayoutUI.GetOrCreate();
            enemyCreatureZone = CreateContainer(layout.GetZone(BattleScreenZone.EnemyCreatureRow), "Enemy Creature Row");
            playerCreatureZone = CreateContainer(layout.GetZone(BattleScreenZone.PlayerCreatureRow), "Player Creature Row");
            enemyTrapZone = CreateContainer(layout.GetZone(BattleScreenZone.EnemyTrapRow), "Enemy Trap Row");
            playerTrapZone = CreateContainer(layout.GetZone(BattleScreenZone.PlayerTrapRow), "Player Trap Row");

            for (int i = 0; i < CreatureSlotCount; i++)
            {
                int index = i;
                enemyCreatureSlots.Add(CreateCreatureSlot(enemyCreatureZone, false, index));
                playerCreatureSlots.Add(CreateCreatureSlot(playerCreatureZone, true, index));
            }

            for (int i = 0; i < TrapSlotCount; i++)
            {
                int index = i;
                enemyTrapSlots.Add(CreateTrapSlot(enemyTrapZone, false, index));
                playerTrapSlots.Add(CreateTrapSlot(playerTrapZone, true, index));
            }

            built = true;
            ApplyRuntimeLayout();
        }

        private RectTransform CreateContainer(RectTransform zone, string objectName)
        {
            GameObject obj = new GameObject(objectName);
            obj.transform.SetParent(zone, false);

            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return rect;
        }

        private CardSlotUI CreateCreatureSlot(RectTransform parent, bool isPlayerSlot, int slotIndex)
        {
            GameObject slotObject = new GameObject(isPlayerSlot ? $"Player Creature Slot {slotIndex + 1}" : $"Enemy Creature Slot {slotIndex + 1}");
            slotObject.transform.SetParent(parent, false);

            RectTransform rect = slotObject.AddComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);

            Image background = slotObject.AddComponent<Image>();
            background.preserveAspect = true;
            background.type = Image.Type.Simple;
            background.color = emptySlotTint;

            Button button = slotObject.AddComponent<Button>();
            button.targetGraphic = background;
            button.transition = Selectable.Transition.None;
            button.onClick.AddListener(isPlayerSlot ? () => HandlePlayerCreatureClicked(slotIndex) : () => HandleEnemyCreatureClicked(slotIndex));

            Image artwork = CreateChildImage(slotObject.transform, "Artwork");
            Image frame = CreateChildImage(slotObject.transform, "Frame");
            frame.raycastTarget = false;

            Text name = CreateAnchoredText(slotObject.transform, "Name", SafeFont(nameFontSize, 10, 18), FontStyle.Bold, TextAnchor.MiddleCenter);
            Text stats = CreateAnchoredText(slotObject.transform, "Stats", SafeFont(statsFontSize, 9, 16), FontStyle.Bold, TextAnchor.MiddleCenter);
            Text status = CreateAnchoredText(slotObject.transform, "Status", SafeFont(statusFontSize, 8, 14), FontStyle.Italic, TextAnchor.MiddleCenter);

            return new CardSlotUI(rect, background, artwork, frame, name, stats, status, isPlayerSlot, showEmptyLabels,
                textColor, mutedTextColor, emptySlotTint, occupiedSlotTint, CardGameArtResolver.CreatureSlot,
                CardGameArtResolver.FrameCreature, creatureArtworkWidthPercent, creatureArtworkHeightPercent, creatureArtworkPositionPercent);
        }

        private TrapSlotUI CreateTrapSlot(RectTransform parent, bool isPlayerSlot, int slotIndex)
        {
            GameObject slotObject = new GameObject(isPlayerSlot ? $"Player Trap Slot {slotIndex + 1}" : $"Enemy Trap Slot {slotIndex + 1}");
            slotObject.transform.SetParent(parent, false);

            RectTransform rect = slotObject.AddComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);

            Image background = slotObject.AddComponent<Image>();
            background.preserveAspect = true;
            background.type = Image.Type.Simple;
            background.color = emptySlotTint;

            Button button = slotObject.AddComponent<Button>();
            button.targetGraphic = background;
            button.transition = Selectable.Transition.None;
            button.onClick.AddListener(() => HandleTrapClicked(isPlayerSlot, slotIndex));

            Image artwork = CreateChildImage(slotObject.transform, "Artwork");
            Image frame = CreateChildImage(slotObject.transform, "Frame");
            Text label = CreateAnchoredText(slotObject.transform, "Trap Text", SafeFont(trapFontSize, 8, 13), FontStyle.Bold, TextAnchor.MiddleCenter);

            return new TrapSlotUI(rect, background, artwork, frame, label, isPlayerSlot, showEmptyLabels, hideEnemyTrapNames,
                textColor, mutedTextColor, emptySlotTint, occupiedSlotTint, CardGameArtResolver.TrapSlot,
                CardGameArtResolver.FrameTrap, CardGameArtResolver.CardBack, trapArtworkWidthPercent, trapArtworkHeightPercent, trapArtworkPositionPercent);
        }

        private Image CreateChildImage(Transform parent, string objectName)
        {
            GameObject imageObject = new GameObject(objectName);
            imageObject.transform.SetParent(parent, false);

            RectTransform rect = imageObject.AddComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);

            Image image = imageObject.AddComponent<Image>();
            image.color = Color.white;
            image.preserveAspect = true;
            image.raycastTarget = false;
            return image;
        }

        private Text CreateAnchoredText(Transform parent, string objectName, int fontSize, FontStyle style, TextAnchor alignment)
        {
            GameObject textObject = new GameObject(objectName);
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0.5f, 0.5f);

            Text text = textObject.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = textColor;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = Mathf.Max(7, fontSize - 4);
            text.resizeTextMaxSize = fontSize;
            text.raycastTarget = false;
            return text;
        }

        private int SafeFont(int value, int min, int max)
        {
            if (value <= 0 || value > 60) return Mathf.Clamp(max - 2, min, max);
            return Mathf.Clamp(value, min, max);
        }

        private void ApplyRuntimeLayout()
        {
            if (!built) return;
            Canvas.ForceUpdateCanvases();
            ApplyCreatureRowLayout(enemyCreatureSlots, enemyCreatureZone);
            ApplyCreatureRowLayout(playerCreatureSlots, playerCreatureZone);
            ApplyTrapRowLayout(enemyTrapSlots, enemyTrapZone);
            ApplyTrapRowLayout(playerTrapSlots, playerTrapZone);
        }

        private void ApplyCreatureRowLayout(List<CardSlotUI> slots, RectTransform zone)
        {
            if (slots == null || zone == null || slots.Count == 0) return;

            float rowWidth = Mathf.Max(1f, zone.rect.width);
            float rowHeight = Mathf.Max(1f, zone.rect.height);
            Vector2 effectiveSize = new Vector2(Mathf.Max(190f, creatureSize.x), Mathf.Max(286f, creatureSize.y));
            float spacing = creatureSpacing;
            float widthByZone = (rowWidth - spacing * (slots.Count - 1)) / slots.Count;
            float slotWidth = Mathf.Clamp(Mathf.Min(effectiveSize.x, widthByZone), 128f, effectiveSize.x);
            float slotHeight = Mathf.Clamp(Mathf.Min(effectiveSize.y, rowHeight * creatureHeightUse), 188f, effectiveSize.y);

            float totalWidth = slotWidth * slots.Count + spacing * (slots.Count - 1);
            float startX = -totalWidth * 0.5f + slotWidth * 0.5f;

            for (int i = 0; i < slots.Count; i++)
            {
                RectTransform rect = slots[i].RectTransform;
                rect.sizeDelta = new Vector2(slotWidth, slotHeight);
                rect.anchoredPosition = new Vector2(startX + i * (slotWidth + spacing), 0f);
                rect.localScale = Vector3.one;
                slots[i].ApplySize(slotWidth, slotHeight);
            }
        }

        private void ApplyTrapRowLayout(List<TrapSlotUI> slots, RectTransform zone)
        {
            if (slots == null || zone == null || slots.Count == 0) return;

            float rowWidth = Mathf.Max(1f, zone.rect.width);
            float rowHeight = Mathf.Max(1f, zone.rect.height);
            Vector2 effectiveSize = new Vector2(Mathf.Max(110f, trapSize.x), Mathf.Max(72f, trapSize.y));
            float spacing = trapSpacing;
            float widthByZone = (rowWidth - spacing * (slots.Count - 1)) / slots.Count;
            float slotWidth = Mathf.Clamp(Mathf.Min(effectiveSize.x, widthByZone), 64f, effectiveSize.x);
            float slotHeight = Mathf.Clamp(Mathf.Min(effectiveSize.y, rowHeight * trapHeightUse), 38f, effectiveSize.y);

            float totalWidth = slotWidth * slots.Count + spacing * (slots.Count - 1);
            float startX = -totalWidth * 0.5f + slotWidth * 0.5f;

            for (int i = 0; i < slots.Count; i++)
            {
                RectTransform rect = slots[i].RectTransform;
                rect.sizeDelta = new Vector2(slotWidth, slotHeight);
                rect.anchoredPosition = new Vector2(startX + i * (slotWidth + spacing), 0f);
                rect.localScale = Vector3.one;
                slots[i].ApplySize(slotWidth, slotHeight);
            }
        }

        private void DisableLegacyBoardVisualIfNeeded()
        {
            if (legacyDisabled || !disableLegacyBoardVisual) return;
            GameObject old = GameObject.Find("BattleBoardVisual");
            if (old != null && old != gameObject) old.SetActive(false);
            legacyDisabled = true;
        }

        private void HandlePlayerCreatureClicked(int slotIndex)
        {
            CardRuntime card = battleManager != null && battleManager.PlayerState?.Board != null
                ? battleManager.PlayerState.Board.GetCreatureAt(slotIndex)
                : null;

            if (card == null || !card.IsAlive)
            {
                selectedPlayerSlotIndex = -1;
                return;
            }

            ShowFieldPreview(card);

            if (battleManager?.TurnManager == null || !battleManager.TurnManager.IsPlayerTurn) return;
            if (card.HasAttackedThisTurn) return;

            selectedPlayerSlotIndex = slotIndex;
            Debug.Log($"{card.CardName} foi selecionada para atacar.");
        }

        private void HandleEnemyCreatureClicked(int slotIndex)
        {
            if (battleManager == null || battleManager.PlayerState == null || battleManager.EnemyState == null) return;

            CardRuntime defender = battleManager.EnemyState.Board.GetCreatureAt(slotIndex);

            if (battleManager.TurnManager == null || !battleManager.TurnManager.IsPlayerTurn)
            {
                if (defender != null) ShowFieldPreview(defender);
                return;
            }

            if (selectedPlayerSlotIndex < 0)
            {
                if (defender != null) ShowFieldPreview(defender);
                return;
            }

            CardRuntime attacker = battleManager.PlayerState.Board.GetCreatureAt(selectedPlayerSlotIndex);
            if (attacker == null || !attacker.IsAlive)
            {
                selectedPlayerSlotIndex = -1;
                return;
            }

            if (defender != null && defender.IsAlive)
            {
                battleManager.ResolveCreatureAttack(attacker, defender);
                selectedPlayerSlotIndex = -1;
                return;
            }

            if (!battleManager.EnemyState.Board.HasAnyCreature())
            {
                battleManager.ResolveDirectAttack(attacker, battleManager.EnemyState);
                selectedPlayerSlotIndex = -1;
            }
        }

        private void HandleTrapClicked(bool isPlayerSlot, int slotIndex)
        {
            CardRuntime card = null;
            if (battleManager != null)
            {
                card = isPlayerSlot
                    ? battleManager.PlayerState?.Board?.GetTrapAt(slotIndex)
                    : battleManager.EnemyState?.Board?.GetTrapAt(slotIndex);
            }

            if (card == null) return;
            if (!isPlayerSlot && hideEnemyTrapNames) return;
            ShowFieldPreview(card);
        }

        private void ShowFieldPreview(CardRuntime card)
        {
            if (card == null) return;
            if (cardPreviewUI == null) cardPreviewUI = FindFirstObjectByType<CardPreviewUI>();
            if (cardPreviewUI != null) cardPreviewUI.Show(card, null);
        }

        private void Refresh()
        {
            if (battleManager == null || battleManager.PlayerState == null || battleManager.EnemyState == null) return;

            RefreshCreatureSlots(playerCreatureSlots, battleManager.PlayerState.Board.CreatureSlots, true);
            RefreshCreatureSlots(enemyCreatureSlots, battleManager.EnemyState.Board.CreatureSlots, false);
            RefreshTrapSlots(playerTrapSlots, battleManager.PlayerState.Board.TrapSlots);
            RefreshTrapSlots(enemyTrapSlots, battleManager.EnemyState.Board.TrapSlots);
        }

        private void RefreshCreatureSlots(List<CardSlotUI> slots, IReadOnlyList<CardRuntime> cards, bool isPlayerRow)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                CardRuntime card = i < cards.Count ? cards[i] : null;
                bool isSelected = isPlayerRow && selectedPlayerSlotIndex == i;
                slots[i].SetCard(card, isSelected, selectedTint);
            }
        }

        private void RefreshTrapSlots(List<TrapSlotUI> slots, IReadOnlyList<CardRuntime> cards)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                CardRuntime card = i < cards.Count ? cards[i] : null;
                slots[i].SetTrap(card);
            }
        }

        private sealed class CardSlotUI
        {
            public RectTransform RectTransform { get; }

            private readonly Image background;
            private readonly Image artworkImage;
            private readonly Image frameImage;
            private readonly Text nameText;
            private readonly Text statsText;
            private readonly Text statusText;
            private readonly bool isPlayerSlot;
            private readonly bool showEmptyLabel;
            private readonly Color textColor;
            private readonly Color mutedTextColor;
            private readonly Color emptyTint;
            private readonly Color occupiedTint;
            private readonly Sprite emptySprite;
            private readonly Sprite frameSprite;
            private readonly float artworkWidthPercent;
            private readonly float artworkHeightPercent;
            private readonly Vector2 artworkPositionPercent;

            public CardSlotUI(RectTransform rectTransform, Image background, Image artworkImage, Image frameImage, Text nameText, Text statsText, Text statusText,
                bool isPlayerSlot, bool showEmptyLabel, Color textColor, Color mutedTextColor, Color emptyTint, Color occupiedTint,
                Sprite emptySprite, Sprite frameSprite, float artworkWidthPercent, float artworkHeightPercent, Vector2 artworkPositionPercent)
            {
                RectTransform = rectTransform;
                this.background = background;
                this.artworkImage = artworkImage;
                this.frameImage = frameImage;
                this.nameText = nameText;
                this.statsText = statsText;
                this.statusText = statusText;
                this.isPlayerSlot = isPlayerSlot;
                this.showEmptyLabel = showEmptyLabel;
                this.textColor = textColor;
                this.mutedTextColor = mutedTextColor;
                this.emptyTint = emptyTint;
                this.occupiedTint = occupiedTint;
                this.emptySprite = emptySprite;
                this.frameSprite = frameSprite;
                this.artworkWidthPercent = artworkWidthPercent;
                this.artworkHeightPercent = artworkHeightPercent;
                this.artworkPositionPercent = artworkPositionPercent;
            }

            public void ApplySize(float width, float height)
            {
                artworkImage.rectTransform.sizeDelta = new Vector2(width * artworkWidthPercent, height * artworkHeightPercent);
                artworkImage.rectTransform.anchoredPosition = new Vector2(width * artworkPositionPercent.x, height * artworkPositionPercent.y);

                frameImage.rectTransform.anchorMin = Vector2.zero;
                frameImage.rectTransform.anchorMax = Vector2.one;
                frameImage.rectTransform.offsetMin = Vector2.zero;
                frameImage.rectTransform.offsetMax = Vector2.zero;

                nameText.rectTransform.sizeDelta = new Vector2(width * 0.78f, height * 0.12f);
                nameText.rectTransform.anchoredPosition = new Vector2(0f, -height * 0.18f);

                statsText.rectTransform.sizeDelta = new Vector2(width * 0.82f, height * 0.16f);
                statsText.rectTransform.anchoredPosition = new Vector2(0f, -height * 0.33f);

                statusText.rectTransform.sizeDelta = new Vector2(width * 0.82f, height * 0.08f);
                statusText.rectTransform.anchoredPosition = new Vector2(0f, -height * 0.45f);
            }

            public void SetCard(CardRuntime card, bool isSelected, Color selectedColor)
            {
                if (card == null)
                {
                    Sprite placeholder = emptySprite != null ? emptySprite : frameSprite;
                    background.sprite = placeholder;
                    background.color = placeholder != null ? (isSelected ? selectedColor : emptyTint) : new Color(0.04f, 0.06f, 0.10f, 0.25f);
                    artworkImage.gameObject.SetActive(false);
                    frameImage.gameObject.SetActive(false);
                    nameText.text = showEmptyLabel ? "CRIATURA" : string.Empty;
                    statsText.text = string.Empty;
                    statusText.text = string.Empty;
                    nameText.color = mutedTextColor;
                    statsText.color = mutedTextColor;
                    statusText.color = mutedTextColor;
                    return;
                }

                Sprite artwork = card.Data != null ? card.Data.Artwork : null;
                artworkImage.gameObject.SetActive(artwork != null);
                artworkImage.sprite = artwork;

                background.sprite = emptySprite;
                background.color = isSelected ? selectedColor : (emptySprite != null ? occupiedTint : new Color(0.04f, 0.07f, 0.11f, 0.18f));

                frameImage.sprite = frameSprite;
                frameImage.color = Color.white;
                frameImage.gameObject.SetActive(frameSprite != null);

                nameText.color = textColor;
                statsText.color = textColor;
                statusText.color = new Color(textColor.r, textColor.g, textColor.b, 0.84f);
                nameText.text = ShortName(card.CardName);
                statsText.text = $"ATK {card.CurrentAttack}  HP {card.CurrentHealth}\nSPD {card.CurrentSpeed}  DEF {card.CurrentDefense}";
                statusText.text = GetStatusText(card);
            }

            private static string ShortName(string value)
            {
                if (string.IsNullOrWhiteSpace(value)) return "Carta";
                return value.Length <= 16 ? value : value.Substring(0, 16) + "...";
            }

            private static string GetStatusText(CardRuntime card)
            {
                if (card == null || card.Data == null) return string.Empty;

                List<string> tags = new();
                if (card.Data.HasTaunt) tags.Add("Provocar");
                if (card.Data.HasShield) tags.Add("Escudo");
                if (card.Data.HasPiercing) tags.Add("Piercing");
                if (card.Data.HasLifeSteal) tags.Add("Roubo");
                if (card.Data.HasRetaliation) tags.Add("Retaliação");

                return tags.Count > 0 ? string.Join(" • ", tags) : string.Empty;
            }
        }

        private sealed class TrapSlotUI
        {
            public RectTransform RectTransform { get; }

            private readonly Image background;
            private readonly Image artworkImage;
            private readonly Image frameImage;
            private readonly Text labelText;
            private readonly bool isPlayerSlot;
            private readonly bool showEmptyLabel;
            private readonly bool hideEnemyTrapNames;
            private readonly Color textColor;
            private readonly Color mutedTextColor;
            private readonly Color emptyTint;
            private readonly Color occupiedTint;
            private readonly Sprite emptySprite;
            private readonly Sprite frameSprite;
            private readonly Sprite cardBackSprite;
            private readonly float artworkWidthPercent;
            private readonly float artworkHeightPercent;
            private readonly Vector2 artworkPositionPercent;

            public TrapSlotUI(RectTransform rectTransform, Image background, Image artworkImage, Image frameImage, Text labelText,
                bool isPlayerSlot, bool showEmptyLabel, bool hideEnemyTrapNames, Color textColor, Color mutedTextColor,
                Color emptyTint, Color occupiedTint, Sprite emptySprite, Sprite frameSprite, Sprite cardBackSprite,
                float artworkWidthPercent, float artworkHeightPercent, Vector2 artworkPositionPercent)
            {
                RectTransform = rectTransform;
                this.background = background;
                this.artworkImage = artworkImage;
                this.frameImage = frameImage;
                this.labelText = labelText;
                this.isPlayerSlot = isPlayerSlot;
                this.showEmptyLabel = showEmptyLabel;
                this.hideEnemyTrapNames = hideEnemyTrapNames;
                this.textColor = textColor;
                this.mutedTextColor = mutedTextColor;
                this.emptyTint = emptyTint;
                this.occupiedTint = occupiedTint;
                this.emptySprite = emptySprite;
                this.frameSprite = frameSprite;
                this.cardBackSprite = cardBackSprite;
                this.artworkWidthPercent = artworkWidthPercent;
                this.artworkHeightPercent = artworkHeightPercent;
                this.artworkPositionPercent = artworkPositionPercent;
            }

            public void ApplySize(float width, float height)
            {
                artworkImage.rectTransform.sizeDelta = new Vector2(width * artworkWidthPercent, height * artworkHeightPercent);
                artworkImage.rectTransform.anchoredPosition = new Vector2(width * artworkPositionPercent.x, height * artworkPositionPercent.y);

                frameImage.rectTransform.anchorMin = Vector2.zero;
                frameImage.rectTransform.anchorMax = Vector2.one;
                frameImage.rectTransform.offsetMin = Vector2.zero;
                frameImage.rectTransform.offsetMax = Vector2.zero;

                labelText.rectTransform.sizeDelta = new Vector2(width * 0.92f, height * 0.30f);
                labelText.rectTransform.anchoredPosition = new Vector2(0f, -height * 0.36f);
            }

            public void SetTrap(CardRuntime card)
            {
                if (card == null)
                {
                    Sprite placeholder = emptySprite != null ? emptySprite : frameSprite;
                    background.sprite = placeholder;
                    background.color = placeholder != null ? emptyTint : new Color(0.04f, 0.06f, 0.10f, 0.25f);
                    artworkImage.gameObject.SetActive(false);
                    frameImage.gameObject.SetActive(false);
                    labelText.text = showEmptyLabel ? "TRAP" : string.Empty;
                    labelText.color = mutedTextColor;
                    return;
                }

                bool hiddenEnemyTrap = !isPlayerSlot && hideEnemyTrapNames;
                Sprite shownArtwork = hiddenEnemyTrap ? cardBackSprite : (card.Data != null ? card.Data.Artwork : null);
                artworkImage.gameObject.SetActive(shownArtwork != null);
                artworkImage.sprite = shownArtwork;

                background.sprite = emptySprite;
                background.color = emptySprite != null ? occupiedTint : new Color(0.04f, 0.07f, 0.11f, 0.18f);
                frameImage.sprite = frameSprite;
                frameImage.color = Color.white;
                frameImage.gameObject.SetActive(frameSprite != null);

                labelText.color = hiddenEnemyTrap ? mutedTextColor : textColor;
                labelText.text = hiddenEnemyTrap ? string.Empty : ShortName(card.CardName);
            }

            private static string ShortName(string value)
            {
                if (string.IsNullOrWhiteSpace(value)) return "Armadilha";
                return value.Length <= 12 ? value : value.Substring(0, 12) + "...";
            }
        }
    }
}
