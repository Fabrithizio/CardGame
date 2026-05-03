// Caminho: Assets/_Project/Scripts/UI/BattleBoardUI.cs
// Descrição: Cria uma visualização temporária do campo usando Canvas UI, com slots separados de criaturas e armadilhas.

using System.Collections.Generic;
using CardGame.Battle;
using CardGame.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class BattleBoardUI : MonoBehaviour
    {
        private const int CreatureSlotCount = 5;
        private const int TrapSlotCount = 3;

        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;

        [Header("Criaturas")]
        [SerializeField] private Vector2 cardSize = new Vector2(125f, 175f);
        [SerializeField] private float cardSpacing = 10f;
        [SerializeField] private float enemyCreatureRowY = -70f;
        [SerializeField] private float playerCreatureRowY = 135f;

        [Header("Armadilhas")]
        [SerializeField] private Vector2 trapSize = new Vector2(92f, 42f);
        [SerializeField] private float trapSpacing = 8f;
        [SerializeField] private float enemyTrapRowY = -245f;
        [SerializeField] private float playerTrapRowY = -55f;

        [Header("Texto")]
        [SerializeField] private int nameFontSize = 13;
        [SerializeField] private int statsFontSize = 12;
        [SerializeField] private int statusFontSize = 10;
        [SerializeField] private int trapFontSize = 10;

        [Header("Seleção")]
        [SerializeField] private Color selectedColor = new Color(0.15f, 0.85f, 1f, 0.95f);

        private Canvas canvas;
        private RectTransform root;
        private Font defaultFont;

        private readonly List<CardSlotUI> enemyCreatureSlots = new();
        private readonly List<CardSlotUI> playerCreatureSlots = new();
        private readonly List<TrapSlotUI> enemyTrapSlots = new();
        private readonly List<TrapSlotUI> playerTrapSlots = new();

        private int selectedPlayerSlotIndex = -1;

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
            CreateBoard();
        }

        private void Update()
        {
            Refresh();
        }

        private void CreateCanvas()
        {
            GameObject canvasObject = new GameObject("Battle Board UI Canvas");
            canvasObject.transform.SetParent(transform, false);

            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

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

        private void CreateBoard()
        {
            RectTransform enemyTrapRow = CreateRow("Enemy Trap Row", new Vector2(0.5f, 0.72f), enemyTrapRowY, 450f, trapSize.y, trapSpacing);
            RectTransform enemyCreatureRow = CreateRow("Enemy Creature Row", new Vector2(0.5f, 0.72f), enemyCreatureRowY, 900f, cardSize.y, cardSpacing);

            RectTransform playerCreatureRow = CreateRow("Player Creature Row", new Vector2(0.5f, 0.22f), playerCreatureRowY, 900f, cardSize.y, cardSpacing);
            RectTransform playerTrapRow = CreateRow("Player Trap Row", new Vector2(0.5f, 0.22f), playerTrapRowY, 450f, trapSize.y, trapSpacing);

            for (int i = 0; i < CreatureSlotCount; i++)
            {
                int slotIndex = i;

                CardSlotUI enemySlot = CreateCreatureSlot(enemyCreatureRow, false, slotIndex);
                CardSlotUI playerSlot = CreateCreatureSlot(playerCreatureRow, true, slotIndex);

                enemyCreatureSlots.Add(enemySlot);
                playerCreatureSlots.Add(playerSlot);
            }

            for (int i = 0; i < TrapSlotCount; i++)
            {
                TrapSlotUI enemyTrap = CreateTrapSlot(enemyTrapRow, false);
                TrapSlotUI playerTrap = CreateTrapSlot(playerTrapRow, true);

                enemyTrapSlots.Add(enemyTrap);
                playerTrapSlots.Add(playerTrap);
            }
        }

        private RectTransform CreateRow(
            string rowName,
            Vector2 anchor,
            float anchoredY,
            float width,
            float height,
            float spacing)
        {
            GameObject rowObject = new GameObject(rowName);
            rowObject.transform.SetParent(root, false);

            RectTransform rect = rowObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, anchoredY);
            rect.sizeDelta = new Vector2(width, height);

            HorizontalLayoutGroup layout = rowObject.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = spacing;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            return rect;
        }

        private CardSlotUI CreateCreatureSlot(RectTransform parent, bool isPlayerSlot, int slotIndex)
        {
            GameObject slotObject = new GameObject(isPlayerSlot ? "Player Creature Slot" : "Enemy Creature Slot");
            slotObject.transform.SetParent(parent, false);

            RectTransform rect = slotObject.AddComponent<RectTransform>();
            rect.sizeDelta = cardSize;

            LayoutElement layoutElement = slotObject.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = cardSize.x;
            layoutElement.preferredHeight = cardSize.y;

            Image background = slotObject.AddComponent<Image>();
            background.color = GetCreatureColor(isPlayerSlot, false);

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
            vertical.spacing = 6;
            vertical.childAlignment = TextAnchor.MiddleCenter;
            vertical.childControlWidth = true;
            vertical.childControlHeight = false;
            vertical.childForceExpandWidth = true;
            vertical.childForceExpandHeight = false;

            Text nameText = CreateText(slotObject.transform, "Name", nameFontSize, FontStyle.Bold, cardSize.x - 16f, 48f);
            Text statsText = CreateText(slotObject.transform, "Stats", statsFontSize, FontStyle.Normal, cardSize.x - 16f, 48f);
            Text statusText = CreateText(slotObject.transform, "Status", statusFontSize, FontStyle.Italic, cardSize.x - 16f, 48f);

            return new CardSlotUI(background, nameText, statsText, statusText, isPlayerSlot);
        }

        private TrapSlotUI CreateTrapSlot(RectTransform parent, bool isPlayerSlot)
        {
            GameObject slotObject = new GameObject(isPlayerSlot ? "Player Trap Slot" : "Enemy Trap Slot");
            slotObject.transform.SetParent(parent, false);

            RectTransform rect = slotObject.AddComponent<RectTransform>();
            rect.sizeDelta = trapSize;

            LayoutElement layoutElement = slotObject.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = trapSize.x;
            layoutElement.preferredHeight = trapSize.y;

            Image background = slotObject.AddComponent<Image>();
            background.color = isPlayerSlot
                ? new Color(0.08f, 0.18f, 0.40f, 0.42f)
                : new Color(0.38f, 0.10f, 0.12f, 0.42f);

            Text text = CreateText(slotObject.transform, "Trap Text", trapFontSize, FontStyle.Bold, trapSize.x - 8f, trapSize.y - 4f);

            return new TrapSlotUI(background, text, isPlayerSlot);
        }

        private Text CreateText(
            Transform parent,
            string objectName,
            int fontSize,
            FontStyle fontStyle,
            float width,
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
            rect.sizeDelta = new Vector2(width, height);

            return text;
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

            CardRuntime card = battleManager.PlayerState.Board.GetCreatureAt(slotIndex);

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

            Debug.Log($"{card.CardName} selecionado como atacante.");
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
            if (battleManager == null || battleManager.PlayerState == null || battleManager.EnemyState == null)
            {
                return;
            }

            RefreshCreatureSlots(playerCreatureSlots, battleManager.PlayerState.Board.CreatureSlots, true);
            RefreshCreatureSlots(enemyCreatureSlots, battleManager.EnemyState.Board.CreatureSlots, false);

            RefreshTrapSlots(playerTrapSlots, battleManager.PlayerState.Board.TrapSlots, true);
            RefreshTrapSlots(enemyTrapSlots, battleManager.EnemyState.Board.TrapSlots, false);
        }

        private void RefreshCreatureSlots(List<CardSlotUI> slots, IReadOnlyList<CardRuntime> cards, bool isPlayerRow)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                CardRuntime card = i < cards.Count ? cards[i] : null;
                bool isSelected = isPlayerRow && selectedPlayerSlotIndex == i;
                slots[i].SetCard(card, isSelected, selectedColor);
            }
        }

        private void RefreshTrapSlots(List<TrapSlotUI> slots, IReadOnlyList<CardRuntime> cards, bool isPlayerRow)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                CardRuntime card = i < cards.Count ? cards[i] : null;
                slots[i].SetTrap(card);
            }
        }

        private Color GetCreatureColor(bool isPlayerSlot, bool hasCard)
        {
            if (isPlayerSlot)
            {
                return hasCard
                    ? new Color(0.10f, 0.28f, 0.60f, 0.92f)
                    : new Color(0.10f, 0.28f, 0.60f, 0.34f);
            }

            return hasCard
                ? new Color(0.55f, 0.16f, 0.18f, 0.92f)
                : new Color(0.55f, 0.16f, 0.18f, 0.34f);
        }

        private sealed class CardSlotUI
        {
            private readonly Image background;
            private readonly Text nameText;
            private readonly Text statsText;
            private readonly Text statusText;
            private readonly bool isPlayerSlot;

            public CardSlotUI(
                Image background,
                Text nameText,
                Text statsText,
                Text statusText,
                bool isPlayerSlot)
            {
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
                        ? new Color(0.10f, 0.28f, 0.60f, 0.34f)
                        : new Color(0.55f, 0.16f, 0.18f, 0.34f);

                    nameText.text = "Vazio";
                    statsText.text = string.Empty;
                    statusText.text = string.Empty;
                    return;
                }

                background.color = isSelected
                    ? selectedColor
                    : isPlayerSlot
                        ? new Color(0.10f, 0.28f, 0.60f, 0.92f)
                        : new Color(0.55f, 0.16f, 0.18f, 0.92f);

                nameText.text = ShortName(card.CardName);

                statsText.text =
                    $"ATK {card.CurrentAttack}  HP {card.CurrentHealth}\n" +
                    $"SPD {card.CurrentSpeed}  DEF {card.CurrentDefense}";

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
            private readonly Image background;
            private readonly Text text;
            private readonly bool isPlayerSlot;

            public TrapSlotUI(Image background, Text text, bool isPlayerSlot)
            {
                this.background = background;
                this.text = text;
                this.isPlayerSlot = isPlayerSlot;
            }

            public void SetTrap(CardRuntime card)
            {
                if (card == null)
                {
                    background.color = isPlayerSlot
                        ? new Color(0.08f, 0.18f, 0.40f, 0.42f)
                        : new Color(0.38f, 0.10f, 0.12f, 0.42f);

                    text.text = "Trap";
                    return;
                }

                background.color = isPlayerSlot
                    ? new Color(0.62f, 0.32f, 0.10f, 0.92f)
                    : new Color(0.72f, 0.22f, 0.10f, 0.92f);

                text.text = ShortName(card.CardName);
            }

            private string ShortName(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return "Armadilha";
                }

                if (value.Length <= 12)
                {
                    return value;
                }

                return value.Substring(0, 12) + "...";
            }
        }
    }
}