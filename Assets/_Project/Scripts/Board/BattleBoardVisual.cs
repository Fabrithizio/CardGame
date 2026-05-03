// Caminho: Assets/_Project/Scripts/Board/BattleBoardVisual.cs
// Descrição: Cria uma visualização temporária mais limpa do campo de batalha com 5 slots para o jogador e 5 para o inimigo.

using System.Collections.Generic;
using CardGame.Battle;
using CardGame.Cards;
using UnityEngine;

namespace CardGame.Board
{
    public class BattleBoardVisual : MonoBehaviour
    {
        private const int SlotCount = 5;

        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;

        [Header("Posicionamento")]
        [SerializeField] private float slotSpacing = 2.2f;
        [SerializeField] private float playerRowY = -2.8f;
        [SerializeField] private float enemyRowY = 2.0f;

        [Header("Tamanho do Slot")]
        [SerializeField] private Vector2 slotSize = new Vector2(1.6f, 2.1f);

        private readonly List<SlotVisual> playerSlots = new();
        private readonly List<SlotVisual> enemySlots = new();

        private Sprite slotSprite;

        private void Awake()
        {
            if (battleManager == null)
            {
                battleManager = FindFirstObjectByType<BattleManager>();
            }

            slotSprite = CreateSolidSprite();
            CreateBoard();
        }

        private void Update()
        {
            Refresh();
        }

        private void CreateBoard()
        {
            ClearExistingSlots();

            for (int i = 0; i < SlotCount; i++)
            {
                float x = GetSlotX(i);

                SlotVisual enemySlot = CreateSlot(
                    $"Enemy Slot {i + 1}",
                    new Vector3(x, enemyRowY, 0f),
                    false
                );

                SlotVisual playerSlot = CreateSlot(
                    $"Player Slot {i + 1}",
                    new Vector3(x, playerRowY, 0f),
                    true
                );

                enemySlots.Add(enemySlot);
                playerSlots.Add(playerSlot);
            }
        }

        private void ClearExistingSlots()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            playerSlots.Clear();
            enemySlots.Clear();
        }

        private float GetSlotX(int index)
        {
            float totalWidth = (SlotCount - 1) * slotSpacing;
            return -totalWidth * 0.5f + index * slotSpacing;
        }

        private SlotVisual CreateSlot(string slotName, Vector3 position, bool isPlayerSlot)
        {
            GameObject slotObject = new GameObject(slotName);
            slotObject.transform.SetParent(transform);
            slotObject.transform.position = position;
            slotObject.transform.localScale = new Vector3(slotSize.x, slotSize.y, 1f);

            SpriteRenderer background = slotObject.AddComponent<SpriteRenderer>();
            background.sprite = slotSprite;
            background.sortingOrder = 0;
            background.color = isPlayerSlot
                ? new Color(0.15f, 0.35f, 0.70f, 0.92f)
                : new Color(0.55f, 0.20f, 0.20f, 0.92f);

            TextMesh titleText = CreateText(
                slotObject.transform,
                "Title Text",
                new Vector3(0f, 0.45f, -0.1f),
                0.055f,
                FontStyle.Bold,
                TextAnchor.MiddleCenter
            );

            TextMesh statsText = CreateText(
                slotObject.transform,
                "Stats Text",
                new Vector3(0f, -0.05f, -0.1f),
                0.045f,
                FontStyle.Normal,
                TextAnchor.MiddleCenter
            );

            TextMesh statusText = CreateText(
                slotObject.transform,
                "Status Text",
                new Vector3(0f, -0.60f, -0.1f),
                0.038f,
                FontStyle.Italic,
                TextAnchor.MiddleCenter
            );

            return new SlotVisual(background, titleText, statsText, statusText, isPlayerSlot);
        }

        private TextMesh CreateText(
            Transform parent,
            string objectName,
            Vector3 localPosition,
            float characterSize,
            FontStyle fontStyle,
            TextAnchor anchor)
        {
            GameObject textObject = new GameObject(objectName);
            textObject.transform.SetParent(parent);
            textObject.transform.localPosition = localPosition;
            textObject.transform.localRotation = Quaternion.identity;
            textObject.transform.localScale = Vector3.one;

            TextMesh textMesh = textObject.AddComponent<TextMesh>();
            textMesh.anchor = anchor;
            textMesh.alignment = TextAlignment.Center;
            textMesh.characterSize = characterSize;
            textMesh.fontSize = 64;
            textMesh.fontStyle = fontStyle;
            textMesh.color = Color.white;
            textMesh.text = string.Empty;

            MeshRenderer renderer = textObject.GetComponent<MeshRenderer>();
            renderer.sortingOrder = 5;

            return textMesh;
        }

        private void Refresh()
        {
            if (battleManager == null || battleManager.PlayerState == null || battleManager.EnemyState == null)
            {
                return;
            }

            RefreshSlots(playerSlots, battleManager.PlayerState.Board.CreatureSlots);
            RefreshSlots(enemySlots, battleManager.EnemyState.Board.CreatureSlots);
        }

        private void RefreshSlots(List<SlotVisual> slots, IReadOnlyList<CardRuntime> cards)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                CardRuntime card = i < cards.Count ? cards[i] : null;
                slots[i].SetCard(card);
            }
        }

        private Sprite CreateSolidSprite()
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();

            return Sprite.Create(
                texture,
                new Rect(0, 0, 1, 1),
                new Vector2(0.5f, 0.5f),
                1f
            );
        }

        private sealed class SlotVisual
        {
            private readonly SpriteRenderer background;
            private readonly TextMesh titleText;
            private readonly TextMesh statsText;
            private readonly TextMesh statusText;
            private readonly bool isPlayerSlot;

            public SlotVisual(
                SpriteRenderer background,
                TextMesh titleText,
                TextMesh statsText,
                TextMesh statusText,
                bool isPlayerSlot)
            {
                this.background = background;
                this.titleText = titleText;
                this.statsText = statsText;
                this.statusText = statusText;
                this.isPlayerSlot = isPlayerSlot;
            }

            public void SetCard(CardRuntime card)
            {
                if (card == null)
                {
                    background.color = isPlayerSlot
                        ? new Color(0.15f, 0.35f, 0.70f, 0.35f)
                        : new Color(0.55f, 0.20f, 0.20f, 0.35f);

                    titleText.text = "Vazio";
                    statsText.text = string.Empty;
                    statusText.text = string.Empty;
                    return;
                }

                background.color = isPlayerSlot
                    ? new Color(0.15f, 0.35f, 0.70f, 0.95f)
                    : new Color(0.55f, 0.20f, 0.20f, 0.95f);

                titleText.text = ShortName(card.CardName);
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

                if (value.Length <= 14)
                {
                    return value;
                }

                return value.Substring(0, 14) + "...";
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
    }
}