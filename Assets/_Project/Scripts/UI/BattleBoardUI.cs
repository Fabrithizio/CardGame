// Caminho: Assets/_Project/Scripts/UI/BattleBoardUI.cs
// Descrição: Campo clicável com visual mais transparente para permitir a skin da arena aparecer.

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

        [Header("Layout em Tempo Real")]
        [SerializeField] private bool updateLayoutEveryFrame = true;

        [Header("Criaturas")]
        [SerializeField] private Vector2 creatureSize = new Vector2(210f, 272f);
        [SerializeField] private float creatureSpacing = 8f;
        [SerializeField] [Range(0.6f, 1f)] private float creatureHeightUse = 0.98f;

        [Header("Armadilhas/Magias")]
        [SerializeField] private Vector2 trapSize = new Vector2(126f, 104f);
        [SerializeField] private float trapSpacing = 7f;
        [SerializeField] private float trapCounterWidth = 84f;
        [SerializeField] [Range(0.55f, 1f)] private float trapHeightUse = 0.94f;

        [Header("Visual")]
        [SerializeField] private bool showTrapBandBackground = false;
        [SerializeField] private Color trapBandColor = new Color(0.28f, 0.23f, 0.12f, 0.18f);
        [SerializeField] private Color boardLineColor = new Color(0.90f, 0.74f, 0.34f, 0.10f);
        [SerializeField] private Color emptyCreaturePlayerColor = new Color(0.10f, 0.16f, 0.34f, 0.32f);
        [SerializeField] private Color emptyCreatureEnemyColor = new Color(0.36f, 0.12f, 0.13f, 0.32f);
        [SerializeField] private Color emptyTrapPlayerColor = new Color(0.08f, 0.14f, 0.32f, 0.30f);
        [SerializeField] private Color emptyTrapEnemyColor = new Color(0.30f, 0.10f, 0.16f, 0.30f);

        [Header("Texto")]
        [SerializeField] private int nameFontSize = 15;
        [SerializeField] private int statsFontSize = 12;
        [SerializeField] private int statusFontSize = 10;
        [SerializeField] private int trapFontSize = 11;
        [SerializeField] private int trapCounterFontSize = 13;

        [Header("Seleção")]
        [SerializeField] private Color selectedColor = new Color(0.15f, 0.85f, 1f, 0.95f);

        private Font defaultFont;

        private readonly List<CardSlotUI> enemyCreatureSlots = new();
        private readonly List<CardSlotUI> playerCreatureSlots = new();
        private readonly List<TrapSlotUI> enemyTrapSlots = new();
        private readonly List<TrapSlotUI> playerTrapSlots = new();

        private RectTransform enemyCreatureZone;
        private RectTransform playerCreatureZone;
        private TrapBandUI enemyTrapBand;
        private TrapBandUI playerTrapBand;

        private int selectedPlayerSlotIndex = -1;
        private bool built;

        private void Awake()
        {
            if (battleManager == null)
            {
                battleManager = FindFirstObjectByType<BattleManager>();
            }

            defaultFont = ResponsiveUIUtility.GetDefaultFont();
            CreateBoard();
        }

        private void Update()
        {
            if (!built)
            {
                CreateBoard();
            }

            if (updateLayoutEveryFrame)
            {
                ApplyRuntimeLayout();
            }

            Refresh();
        }

        private void OnValidate()
        {
            // Não cria nem reposiciona objetos aqui.
        }

        private void CreateBoard()
        {
            if (built)
            {
                return;
            }

            BattleScreenLayoutUI layout = BattleScreenLayoutUI.GetOrCreate();

            enemyCreatureZone = CreateContainer(layout.GetZone(BattleScreenZone.EnemyCreatureRow), "Enemy Creature Row");
            playerCreatureZone = CreateContainer(layout.GetZone(BattleScreenZone.PlayerCreatureRow), "Player Creature Row");

            enemyTrapBand = CreateTrapBand(layout.GetZone(BattleScreenZone.EnemyTrapRow), false);
            playerTrapBand = CreateTrapBand(layout.GetZone(BattleScreenZone.PlayerTrapRow), true);

            for (int i = 0; i < CreatureSlotCount; i++)
            {
                int slotIndex = i;
                enemyCreatureSlots.Add(CreateCreatureSlot(enemyCreatureZone, false, slotIndex));
                playerCreatureSlots.Add(CreateCreatureSlot(playerCreatureZone, true, slotIndex));
            }

            for (int i = 0; i < TrapSlotCount; i++)
            {
                enemyTrapSlots.Add(CreateTrapSlot(enemyTrapBand.slotsParent, false));
                playerTrapSlots.Add(CreateTrapSlot(playerTrapBand.slotsParent, true));
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

        private TrapBandUI CreateTrapBand(RectTransform zone, bool isPlayerBand)
        {
            GameObject bandObject = new GameObject(isPlayerBand ? "Player Trap Band" : "Enemy Trap Band");
            bandObject.transform.SetParent(zone, false);

            RectTransform bandRect = bandObject.AddComponent<RectTransform>();
            bandRect.anchorMin = Vector2.zero;
            bandRect.anchorMax = Vector2.one;
            bandRect.offsetMin = Vector2.zero;
            bandRect.offsetMax = Vector2.zero;

            Image bandImage = bandObject.AddComponent<Image>();
            bandImage.color = trapBandColor;
            bandImage.enabled = showTrapBandBackground;
            bandImage.raycastTarget = false;

            Outline bandOutline = bandObject.AddComponent<Outline>();
            bandOutline.effectColor = boardLineColor;
            bandOutline.effectDistance = new Vector2(1.5f, -1.5f);

            GameObject counterObject = new GameObject("Trap Counter");
            counterObject.transform.SetParent(bandObject.transform, false);

            RectTransform counterRect = counterObject.AddComponent<RectTransform>();
            counterRect.anchorMin = new Vector2(0f, 0.5f);
            counterRect.anchorMax = new Vector2(0f, 0.5f);
            counterRect.pivot = new Vector2(0f, 0.5f);

            Image counterBg = counterObject.AddComponent<Image>();
            counterBg.color = isPlayerBand
                ? new Color(0.02f, 0.10f, 0.20f, 0.88f)
                : new Color(0.08f, 0.05f, 0.16f, 0.88f);

            Outline counterOutline = counterObject.AddComponent<Outline>();
            counterOutline.effectColor = new Color(0f, 0f, 0f, 0.65f);
            counterOutline.effectDistance = new Vector2(2f, -2f);

            Text counterText = CreateFullText(counterObject.transform, "Counter Text", trapCounterFontSize, FontStyle.Bold);
            counterText.alignment = TextAnchor.MiddleCenter;
            counterText.text = $"TRAPS\n0/{TrapSlotCount}";

            GameObject slotsObject = new GameObject("Trap Slots");
            slotsObject.transform.SetParent(bandObject.transform, false);

            RectTransform slotsRect = slotsObject.AddComponent<RectTransform>();
            slotsRect.anchorMin = new Vector2(0f, 0.5f);
            slotsRect.anchorMax = new Vector2(0f, 0.5f);
            slotsRect.pivot = new Vector2(0f, 0.5f);

            return new TrapBandUI(bandRect, bandImage, bandOutline, counterRect, counterText, slotsRect);
        }

        private CardSlotUI CreateCreatureSlot(RectTransform parent, bool isPlayerSlot, int slotIndex)
        {
            GameObject slotObject = new GameObject(isPlayerSlot ? "Player Creature Slot" : "Enemy Creature Slot");
            slotObject.transform.SetParent(parent, false);

            RectTransform rect = slotObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            Image background = slotObject.AddComponent<Image>();
            background.color = GetCreatureEmptyColor(isPlayerSlot);

            Outline outline = slotObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.90f, 0.74f, 0.34f, 0.28f);
            outline.effectDistance = new Vector2(2f, -2f);

            Shadow shadow = slotObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.55f);
            shadow.effectDistance = new Vector2(0f, -4f);

            Button button = slotObject.AddComponent<Button>();
            button.targetGraphic = background;

            if (isPlayerSlot)
            {
                button.onClick.AddListener(() => HandlePlayerSlotClicked(slotIndex));
            }
            else
            {
                button.onClick.AddListener(() => HandleEnemySlotClicked(slotIndex));
            }

            VerticalLayoutGroup vertical = slotObject.AddComponent<VerticalLayoutGroup>();
            vertical.padding = new RectOffset(8, 8, 8, 8);
            vertical.spacing = 3;
            vertical.childAlignment = TextAnchor.MiddleCenter;
            vertical.childControlWidth = true;
            vertical.childControlHeight = false;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;

            Image artworkImage = CreateSizedImage(slotObject.transform, "Artwork", creatureSize.x - 16f, 86f);
            Text nameText = CreateSizedText(slotObject.transform, "Name", nameFontSize, FontStyle.Bold, creatureSize.x - 16f, 36f);
            Text statsText = CreateSizedText(slotObject.transform, "Stats", statsFontSize, FontStyle.Normal, creatureSize.x - 16f, 42f);
            Text statusText = CreateSizedText(slotObject.transform, "Status", statusFontSize, FontStyle.Italic, creatureSize.x - 16f, 24f);

            return new CardSlotUI(rect, background, artworkImage, nameText, statsText, statusText, isPlayerSlot, emptyCreaturePlayerColor, emptyCreatureEnemyColor);
        }

        private TrapSlotUI CreateTrapSlot(RectTransform parent, bool isPlayerSlot)
        {
            GameObject slotObject = new GameObject(isPlayerSlot ? "Player Trap Slot" : "Enemy Trap Slot");
            slotObject.transform.SetParent(parent, false);

            RectTransform rect = slotObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);

            if (!isPlayerSlot)
            {
                rect.localRotation = Quaternion.Euler(0f, 0f, 180f);
            }

            Image background = slotObject.AddComponent<Image>();
            background.color = isPlayerSlot ? emptyTrapPlayerColor : emptyTrapEnemyColor;

            Outline outline = slotObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.90f, 0.74f, 0.34f, 0.10f);
            outline.effectDistance = new Vector2(2f, -2f);

            Text text = CreateSizedText(slotObject.transform, "Trap Text", trapFontSize, FontStyle.Bold, trapSize.x - 10f, trapSize.y - 4f);
            return new TrapSlotUI(rect, background, text, isPlayerSlot, emptyTrapPlayerColor, emptyTrapEnemyColor);
        }

        private Text CreateFullText(Transform parent, string objectName, int fontSize, FontStyle fontStyle)
        {
            GameObject textObject = new GameObject(objectName);
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(6f, 5f);
            rect.offsetMax = new Vector2(-6f, -5f);

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

            return text;
        }

        private Text CreateSizedText(Transform parent, string objectName, int fontSize, FontStyle fontStyle, float width, float height)
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
            rect.sizeDelta = new Vector2(width, height);

            return text;
        }

        private Image CreateSizedImage(Transform parent, string objectName, float width, float height)
        {
            GameObject imageObject = new GameObject(objectName);
            imageObject.transform.SetParent(parent, false);

            RectTransform rect = imageObject.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(width, height);

            Image image = imageObject.AddComponent<Image>();
            image.color = Color.white;
            image.preserveAspect = true;
            image.raycastTarget = false;
            imageObject.SetActive(false);

            return image;
        }

        private void ApplyRuntimeLayout()
        {
            if (!built)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();

            ApplyCreatureRowLayout(enemyCreatureSlots, enemyCreatureZone);
            ApplyCreatureRowLayout(playerCreatureSlots, playerCreatureZone);
            ApplyTrapBandLayout(enemyTrapBand, enemyTrapSlots);
            ApplyTrapBandLayout(playerTrapBand, playerTrapSlots);
        }

        private void ApplyCreatureRowLayout(List<CardSlotUI> slots, RectTransform zone)
        {
            if (slots == null || zone == null || slots.Count == 0)
            {
                return;
            }

            float rowWidth = Mathf.Max(1f, zone.rect.width);
            float rowHeight = Mathf.Max(1f, zone.rect.height);

            float spacing = Mathf.Max(0f, creatureSpacing);
            float slotWidth = Mathf.Min(creatureSize.x, (rowWidth - spacing * (slots.Count - 1)) / slots.Count);
            slotWidth = Mathf.Max(42f, slotWidth);

            float slotHeight = Mathf.Min(creatureSize.y, rowHeight * creatureHeightUse);
            slotHeight = Mathf.Max(58f, slotHeight);

            float totalWidth = (slotWidth * slots.Count) + spacing * (slots.Count - 1);
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

        private void ApplyTrapBandLayout(TrapBandUI band, List<TrapSlotUI> slots)
        {
            if (band == null || band.root == null || band.counterRect == null || band.slotsParent == null || slots == null || slots.Count == 0)
            {
                return;
            }

            if (band.background != null)
            {
                band.background.enabled = showTrapBandBackground;
                band.background.color = trapBandColor;
            }

            if (band.outline != null)
            {
                band.outline.effectColor = boardLineColor;
            }

            float width = Mathf.Max(1f, band.root.rect.width);
            float height = Mathf.Max(1f, band.root.rect.height);

            float counterWidth = Mathf.Clamp(trapCounterWidth, 42f, width * 0.145f);
            float outerPadding = 8f;
            float gap = Mathf.Max(6f, trapSpacing);
            float slotAreaWidth = Mathf.Max(1f, width - counterWidth - gap - outerPadding * 2f);
            float spacing = Mathf.Max(2f, trapSpacing);

            float slotWidth = Mathf.Min(trapSize.x, (slotAreaWidth - spacing * (slots.Count - 1)) / slots.Count);
            slotWidth = Mathf.Max(32f, slotWidth);

            float slotHeight = Mathf.Min(trapSize.y, height * trapHeightUse, slotWidth * 0.92f);
            slotHeight = Mathf.Max(30f, slotHeight);

            float contentHeight = Mathf.Max(slotHeight, Mathf.Min(height * 0.92f, slotHeight + 12f));
            band.counterRect.sizeDelta = new Vector2(counterWidth, contentHeight);
            band.counterRect.anchoredPosition = new Vector2(outerPadding, 0f);

            band.slotsParent.sizeDelta = new Vector2(slotAreaWidth, height);
            band.slotsParent.anchoredPosition = new Vector2(outerPadding + counterWidth + gap, 0f);

            for (int i = 0; i < slots.Count; i++)
            {
                RectTransform rect = slots[i].RectTransform;
                rect.sizeDelta = new Vector2(slotWidth, slotHeight);
                rect.anchoredPosition = new Vector2(i * (slotWidth + spacing), 0f);
                rect.localScale = Vector3.one;
            }
        }

        private void HandlePlayerSlotClicked(int slotIndex)
        {
            if (battleManager == null || battleManager.PlayerState == null)
            {
                return;
            }

            if (battleManager.TurnManager == null || !battleManager.TurnManager.IsPlayerTurn)
            {
                Debug.Log("Você só pode selecionar atacante no seu turno.");
                return;
            }

            CardRuntime card = battleManager.PlayerState.Board != null ? battleManager.PlayerState.Board.GetCreatureAt(slotIndex) : null;

            if (card == null || !card.IsAlive)
            {
                Debug.Log($"Slot {slotIndex + 1} do jogador está vazio.");
                selectedPlayerSlotIndex = -1;
                return;
            }

            if (card.HasAttackedThisTurn)
            {
                Debug.Log($"{card.CardName} já atacou neste turno.");
                return;
            }

            selectedPlayerSlotIndex = slotIndex;
            Debug.Log($"{card.CardName} foi selecionada para atacar.");
        }

        private void HandleEnemySlotClicked(int slotIndex)
        {
            if (battleManager == null || battleManager.PlayerState == null || battleManager.EnemyState == null)
            {
                return;
            }

            if (battleManager.TurnManager == null || !battleManager.TurnManager.IsPlayerTurn)
            {
                Debug.Log("Você só pode atacar no seu turno.");
                return;
            }

            if (selectedPlayerSlotIndex < 0)
            {
                Debug.Log("Selecione primeiro uma criatura sua para atacar.");
                return;
            }

            if (battleManager.PlayerState.Board == null || battleManager.EnemyState.Board == null)
            {
                selectedPlayerSlotIndex = -1;
                return;
            }

            CardRuntime attacker = battleManager.PlayerState.Board.GetCreatureAt(selectedPlayerSlotIndex);

            if (attacker == null || !attacker.IsAlive)
            {
                Debug.Log("Atacante inválido.");
                selectedPlayerSlotIndex = -1;
                return;
            }

            CardRuntime defender = battleManager.EnemyState.Board.GetCreatureAt(slotIndex);

            if (defender != null && defender.IsAlive)
            {
                battleManager.ResolveCreatureAttack(attacker, defender);
                selectedPlayerSlotIndex = -1;
                return;
            }

            if (battleManager.EnemyState.Board.HasAnyCreature())
            {
                Debug.Log("Você não pode atacar diretamente enquanto o inimigo tem criaturas em campo.");
                return;
            }

            battleManager.ResolveDirectAttack(attacker, battleManager.EnemyState);
            selectedPlayerSlotIndex = -1;
        }

        private void Refresh()
        {
            if (battleManager == null)
            {
                return;
            }

            if (battleManager.PlayerState == null || battleManager.EnemyState == null)
            {
                return;
            }

            if (battleManager.PlayerState.Board == null || battleManager.EnemyState.Board == null)
            {
                return;
            }

            RefreshCreatureSlots(playerCreatureSlots, battleManager.PlayerState.Board.CreatureSlots, true);
            RefreshCreatureSlots(enemyCreatureSlots, battleManager.EnemyState.Board.CreatureSlots, false);

            RefreshTrapSlots(playerTrapSlots, battleManager.PlayerState.Board.TrapSlots, playerTrapBand);
            RefreshTrapSlots(enemyTrapSlots, battleManager.EnemyState.Board.TrapSlots, enemyTrapBand);
        }

        private void RefreshCreatureSlots(List<CardSlotUI> slots, IReadOnlyList<CardRuntime> cards, bool isPlayerRow)
        {
            if (slots == null || cards == null)
            {
                return;
            }

            for (int i = 0; i < slots.Count; i++)
            {
                CardRuntime card = i < cards.Count ? cards[i] : null;
                bool isSelected = isPlayerRow && selectedPlayerSlotIndex == i;
                slots[i].SetCard(card, isSelected, selectedColor);
            }
        }

        private void RefreshTrapSlots(List<TrapSlotUI> slots, IReadOnlyList<CardRuntime> cards, TrapBandUI band)
        {
            if (slots == null || cards == null || band == null)
            {
                return;
            }

            int occupiedCount = 0;

            for (int i = 0; i < slots.Count; i++)
            {
                CardRuntime card = i < cards.Count ? cards[i] : null;

                if (card != null)
                {
                    occupiedCount++;
                }

                slots[i].SetTrap(card);
            }

            if (band.counterText != null)
            {
                band.counterText.text = $"TRAPS\n{occupiedCount}/{TrapSlotCount}";
            }
        }

        private Color GetCreatureEmptyColor(bool isPlayerSlot)
        {
            return isPlayerSlot ? emptyCreaturePlayerColor : emptyCreatureEnemyColor;
        }

        private sealed class CardSlotUI
        {
            public RectTransform RectTransform { get; }

            private readonly Image background;
            private readonly Image artworkImage;
            private readonly Text nameText;
            private readonly Text statsText;
            private readonly Text statusText;
            private readonly bool isPlayerSlot;
            private readonly Color emptyPlayerColor;
            private readonly Color emptyEnemyColor;

            public CardSlotUI(RectTransform rectTransform, Image background, Image artworkImage, Text nameText, Text statsText, Text statusText, bool isPlayerSlot, Color emptyPlayerColor, Color emptyEnemyColor)
            {
                RectTransform = rectTransform;
                this.background = background;
                this.artworkImage = artworkImage;
                this.nameText = nameText;
                this.statsText = statsText;
                this.statusText = statusText;
                this.isPlayerSlot = isPlayerSlot;
                this.emptyPlayerColor = emptyPlayerColor;
                this.emptyEnemyColor = emptyEnemyColor;
            }

            public void ApplySize(float width, float height)
            {
                float innerWidth = Mathf.Max(16f, width - 18f);

                if (artworkImage != null)
                {
                    artworkImage.rectTransform.sizeDelta = new Vector2(innerWidth, Mathf.Clamp(height * 0.43f, 46f, 110f));
                }

                nameText.rectTransform.sizeDelta = new Vector2(innerWidth, Mathf.Clamp(height * 0.17f, 22f, 40f));
                statsText.rectTransform.sizeDelta = new Vector2(innerWidth, Mathf.Clamp(height * 0.20f, 28f, 46f));
                statusText.rectTransform.sizeDelta = new Vector2(innerWidth, Mathf.Clamp(height * 0.13f, 18f, 30f));
            }

            public void SetCard(CardRuntime card, bool isSelected, Color selectedColor)
            {
                if (card == null)
                {
                    if (artworkImage != null)
                    {
                        artworkImage.gameObject.SetActive(false);
                        artworkImage.sprite = null;
                    }

                    background.color = isPlayerSlot ? emptyPlayerColor : emptyEnemyColor;
                    nameText.color = new Color(1f, 1f, 1f, 0.42f);
                    statsText.color = new Color(1f, 1f, 1f, 0.26f);
                    statusText.color = new Color(1f, 1f, 1f, 0.26f);
                    nameText.text = "CRIATURA";
                    statsText.text = string.Empty;
                    statusText.text = string.Empty;
                    return;
                }

                if (artworkImage != null)
                {
                    Sprite artwork = card.Data != null ? card.Data.Artwork : null;
                    artworkImage.gameObject.SetActive(artwork != null);
                    artworkImage.sprite = artwork;
                    artworkImage.color = Color.white;
                }

                background.color = isSelected
                    ? selectedColor
                    : isPlayerSlot
                        ? new Color(0.11f, 0.23f, 0.56f, 0.86f)
                        : new Color(0.46f, 0.15f, 0.18f, 0.86f);

                nameText.color = Color.white;
                statsText.color = Color.white;
                statusText.color = new Color(0.92f, 0.96f, 1f, 0.86f);
                nameText.text = ShortName(card.CardName);
                statsText.text = $"ATK {card.CurrentAttack}  HP {card.CurrentHealth}\nSPD {card.CurrentSpeed}  DEF {card.CurrentDefense}";
                statusText.text = GetStatusText(card);
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

            private string GetStatusText(CardRuntime card)
            {
                List<CardStatusType> statuses = card.GetCurrentStatuses();

                if (statuses.Count <= 0)
                {
                    return "Sem status";
                }

                if (statuses.Count == 1)
                {
                    return statuses[0].ToString();
                }

                return $"{statuses[0]}, {statuses[1]}";
            }
        }

        private sealed class TrapSlotUI
        {
            public RectTransform RectTransform { get; }

            private readonly Image background;
            private readonly Text text;
            private readonly bool isPlayerSlot;
            private readonly Color emptyPlayerColor;
            private readonly Color emptyEnemyColor;

            public TrapSlotUI(RectTransform rectTransform, Image background, Text text, bool isPlayerSlot, Color emptyPlayerColor, Color emptyEnemyColor)
            {
                RectTransform = rectTransform;
                this.background = background;
                this.text = text;
                this.isPlayerSlot = isPlayerSlot;
                this.emptyPlayerColor = emptyPlayerColor;
                this.emptyEnemyColor = emptyEnemyColor;
            }

            public void SetTrap(CardRuntime card)
            {
                if (card == null)
                {
                    background.color = isPlayerSlot ? emptyPlayerColor : emptyEnemyColor;
                    text.text = string.Empty;
                    return;
                }

                background.color = isPlayerSlot
                    ? new Color(0.66f, 0.36f, 0.12f, 0.86f)
                    : new Color(0.76f, 0.24f, 0.12f, 0.86f);

                text.text = isPlayerSlot ? ShortName(card.CardName) : "???";
            }

            private string ShortName(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return "Armadilha";
                }

                if (value.Length <= 14)
                {
                    return value;
                }

                return value.Substring(0, 14) + "...";
            }
        }

        private sealed class TrapBandUI
        {
            public readonly RectTransform root;
            public readonly Image background;
            public readonly Outline outline;
            public readonly RectTransform counterRect;
            public readonly Text counterText;
            public readonly RectTransform slotsParent;

            public TrapBandUI(RectTransform root, Image background, Outline outline, RectTransform counterRect, Text counterText, RectTransform slotsParent)
            {
                this.root = root;
                this.background = background;
                this.outline = outline;
                this.counterRect = counterRect;
                this.counterText = counterText;
                this.slotsParent = slotsParent;
            }
        }
    }
}
