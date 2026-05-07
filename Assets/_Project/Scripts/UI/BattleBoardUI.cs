// Caminho: Assets/_Project/Scripts/UI/BattleBoardUI.cs
// Descrição: Campo com 5 criaturas grandes, 6 armadilhas menores, contador lateral e layout recalculado em tempo real.

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
        [SerializeField] private Vector2 creatureSize = new Vector2(154f, 208f);
        [SerializeField] private float creatureSpacing = 8f;
        [SerializeField] [Range(0.6f, 1f)] private float creatureHeightUse = 0.94f;

        [Header("Armadilhas/Magias")]
        [SerializeField] private Vector2 trapSize = new Vector2(104f, 72f);
        [SerializeField] private float trapSpacing = 7f;
        [SerializeField] private float trapCounterWidth = 108f;
        [SerializeField] [Range(0.55f, 1f)] private float trapHeightUse = 0.82f;

        [Header("Texto")]
        [SerializeField] private int nameFontSize = 15;
        [SerializeField] private int statsFontSize = 12;
        [SerializeField] private int statusFontSize = 10;
        [SerializeField] private int trapFontSize = 11;
        [SerializeField] private int trapCounterFontSize = 16;

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
            if (!Application.isPlaying)
            {
                return;
            }

            ApplyRuntimeLayout();
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

            GameObject counterObject = new GameObject("Trap Counter");
            counterObject.transform.SetParent(bandObject.transform, false);

            RectTransform counterRect = counterObject.AddComponent<RectTransform>();
            counterRect.anchorMin = new Vector2(0f, 0.5f);
            counterRect.anchorMax = new Vector2(0f, 0.5f);
            counterRect.pivot = new Vector2(0f, 0.5f);

            Image counterBg = counterObject.AddComponent<Image>();
            counterBg.color = isPlayerBand
                ? new Color(0.02f, 0.10f, 0.20f, 0.86f)
                : new Color(0.08f, 0.05f, 0.16f, 0.86f);

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

            return new TrapBandUI(bandRect, counterRect, counterText, slotsRect);
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
            outline.effectColor = new Color(0.90f, 0.74f, 0.34f, 0.25f);
            outline.effectDistance = new Vector2(2f, -2f);

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
            vertical.padding = new RectOffset(8, 8, 10, 8);
            vertical.spacing = 4;
            vertical.childAlignment = TextAnchor.MiddleCenter;
            vertical.childControlWidth = true;
            vertical.childControlHeight = false;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;

            Text nameText = CreateSizedText(slotObject.transform, "Name", nameFontSize, FontStyle.Bold, creatureSize.x - 16f, 54f);
            Text statsText = CreateSizedText(slotObject.transform, "Stats", statsFontSize, FontStyle.Normal, creatureSize.x - 16f, 56f);
            Text statusText = CreateSizedText(slotObject.transform, "Status", statusFontSize, FontStyle.Italic, creatureSize.x - 16f, 42f);

            return new CardSlotUI(rect, background, nameText, statsText, statusText, isPlayerSlot);
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
            background.color = isPlayerSlot
                ? new Color(0.10f, 0.19f, 0.52f, 0.86f)
                : new Color(0.44f, 0.14f, 0.24f, 0.86f);

            Outline outline = slotObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.90f, 0.74f, 0.34f, 0.20f);
            outline.effectDistance = new Vector2(2f, -2f);

            Text text = CreateSizedText(slotObject.transform, "Trap Text", trapFontSize, FontStyle.Bold, trapSize.x - 10f, trapSize.y - 4f);
            return new TrapSlotUI(rect, background, text, isPlayerSlot);
        }

        private Text CreateFullText(Transform parent, string objectName, int fontSize, FontStyle fontStyle)
        {
            GameObject textObject = new GameObject(objectName);
            textObject.transform.SetParent(parent, false);

            RectTransform rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(8f, 6f);
            rect.offsetMax = new Vector2(-8f, -6f);

            Text text = textObject.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 8;
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

            float availableWidth = rowWidth;
            float spacing = Mathf.Max(0f, creatureSpacing);
            float slotWidth = Mathf.Min(creatureSize.x, (availableWidth - spacing * (slots.Count - 1)) / slots.Count);
            slotWidth = Mathf.Max(32f, slotWidth);

            float slotHeight = Mathf.Min(creatureSize.y, rowHeight * creatureHeightUse);
            slotHeight = Mathf.Max(48f, slotHeight);

            float totalWidth = (slotWidth * slots.Count) + spacing * (slots.Count - 1);
            float startX = -totalWidth * 0.5f + slotWidth * 0.5f;

            for (int i = 0; i < slots.Count; i++)
            {
                RectTransform rect = slots[i].RectTransform;
                rect.sizeDelta = new Vector2(slotWidth, slotHeight);
                rect.anchoredPosition = new Vector2(startX + i * (slotWidth + spacing), 0f);
                rect.localScale = Vector3.one;
            }
        }

        private void ApplyTrapBandLayout(TrapBandUI band, List<TrapSlotUI> slots)
        {
            if (band == null || band.root == null || band.counterRect == null || band.slotsParent == null || slots == null || slots.Count == 0)
            {
                return;
            }

            float width = Mathf.Max(1f, band.root.rect.width);
            float height = Mathf.Max(1f, band.root.rect.height);

            float counterWidth = Mathf.Clamp(trapCounterWidth, 46f, width * 0.22f);
            float gap = Mathf.Max(4f, trapSpacing);
            float slotAreaWidth = Mathf.Max(1f, width - counterWidth - gap);
            float spacing = Mathf.Max(2f, trapSpacing);

            float slotWidth = Mathf.Min(trapSize.x, (slotAreaWidth - spacing * (slots.Count - 1)) / slots.Count);
            slotWidth = Mathf.Max(28f, slotWidth);

            float slotHeight = Mathf.Min(trapSize.y, height * trapHeightUse, slotWidth * 0.78f);
            slotHeight = Mathf.Max(24f, slotHeight);

            band.counterRect.sizeDelta = new Vector2(counterWidth, Mathf.Min(height * 0.92f, slotHeight + 22f));
            band.counterRect.anchoredPosition = new Vector2(0f, 0f);

            band.slotsParent.sizeDelta = new Vector2(slotAreaWidth, height);
            band.slotsParent.anchoredPosition = new Vector2(counterWidth + gap, 0f);

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
            return isPlayerSlot
                ? new Color(0.11f, 0.23f, 0.56f, 0.34f)
                : new Color(0.46f, 0.15f, 0.18f, 0.34f);
        }

        private sealed class CardSlotUI
        {
            public RectTransform RectTransform { get; }

            private readonly Image background;
            private readonly Text nameText;
            private readonly Text statsText;
            private readonly Text statusText;
            private readonly bool isPlayerSlot;

            public CardSlotUI(RectTransform rectTransform, Image background, Text nameText, Text statsText, Text statusText, bool isPlayerSlot)
            {
                RectTransform = rectTransform;
                this.background = background;
                this.nameText = nameText;
                this.statsText = statsText;
                this.statusText = statusText;
                this.isPlayerSlot = isPlayerSlot;
            }

            public void SetCard(CardRuntime card, bool isSelected, Color selectedColor)
            {
                if (card == null)
                {
                    background.color = isPlayerSlot
                        ? new Color(0.11f, 0.23f, 0.56f, 0.34f)
                        : new Color(0.46f, 0.15f, 0.18f, 0.34f);

                    nameText.text = "Vazio";
                    statsText.text = string.Empty;
                    statusText.text = string.Empty;
                    return;
                }

                background.color = isSelected
                    ? selectedColor
                    : isPlayerSlot
                        ? new Color(0.11f, 0.23f, 0.56f, 0.92f)
                        : new Color(0.46f, 0.15f, 0.18f, 0.92f);

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

            public TrapSlotUI(RectTransform rectTransform, Image background, Text text, bool isPlayerSlot)
            {
                RectTransform = rectTransform;
                this.background = background;
                this.text = text;
                this.isPlayerSlot = isPlayerSlot;
            }

            public void SetTrap(CardRuntime card)
            {
                if (card == null)
                {
                    background.color = isPlayerSlot
                        ? new Color(0.12f, 0.20f, 0.50f, 0.42f)
                        : new Color(0.42f, 0.14f, 0.24f, 0.42f);

                    text.text = "Trap";
                    return;
                }

                background.color = isPlayerSlot
                    ? new Color(0.66f, 0.36f, 0.12f, 0.92f)
                    : new Color(0.76f, 0.24f, 0.12f, 0.92f);

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
            public readonly RectTransform counterRect;
            public readonly Text counterText;
            public readonly RectTransform slotsParent;

            public TrapBandUI(RectTransform root, RectTransform counterRect, Text counterText, RectTransform slotsParent)
            {
                this.root = root;
                this.counterRect = counterRect;
                this.counterText = counterText;
                this.slotsParent = slotsParent;
            }
        }
    }
}
