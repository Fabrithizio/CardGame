// Caminho: Assets/_Project/Scripts/UI/MythicLoadoutUI.cs
// Descrição: Exibe 2 Míticos por lado dentro das zonas laterais do BattleScreenLayoutUI.

using CardGame.Battle;
using CardGame.Cards;
using CardGame.Mythics;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class MythicLoadoutUI : MonoBehaviour
    {
        private const int MythicSlotCount = 2;

        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;

        [Header("Layout")]
        [SerializeField] private Vector2 iconSize = new Vector2(84f, 84f);
        [SerializeField] private float iconSpacing = 22f;

        [Header("Cores do Jogador")]
        [SerializeField] private Color playerAvailableColor = new Color(0.10f, 0.46f, 1f, 0.98f);
        [SerializeField] private Color playerUsedColor = new Color(0.10f, 0.10f, 0.12f, 0.86f);
        [SerializeField] private Color playerEmptyColor = new Color(0.04f, 0.04f, 0.05f, 0.52f);

        [Header("Cores do Inimigo")]
        [SerializeField] private Color enemyAvailableColor = new Color(0.76f, 0.16f, 1f, 0.98f);
        [SerializeField] private Color enemyUsedColor = new Color(0.10f, 0.10f, 0.12f, 0.86f);
        [SerializeField] private Color enemyEmptyColor = new Color(0.04f, 0.04f, 0.05f, 0.52f);

        private Font defaultFont;
        private MythicSlotUI[] enemySlots;
        private MythicSlotUI[] playerSlots;
        private Sprite circleSprite;

        private void Awake()
        {
            if (battleManager == null)
            {
                battleManager = FindFirstObjectByType<BattleManager>();
            }

            defaultFont = ResponsiveUIUtility.GetDefaultFont();
            circleSprite = CreateCircleSprite();

            BattleScreenLayoutUI layout = BattleScreenLayoutUI.GetOrCreate();

            enemySlots = CreateSlotsColumn(layout.GetZone(BattleScreenZone.EnemyMythic), false);
            playerSlots = CreateSlotsColumn(layout.GetZone(BattleScreenZone.PlayerMythic), true);
        }

        private void Update()
        {
            Refresh();
        }

        private MythicSlotUI[] CreateSlotsColumn(RectTransform parentZone, bool isPlayerColumn)
        {
            GameObject columnObject = new GameObject(isPlayerColumn ? "Player Mythics" : "Enemy Mythics");
            columnObject.transform.SetParent(parentZone, false);

            RectTransform columnRect = columnObject.AddComponent<RectTransform>();
            columnRect.anchorMin = Vector2.zero;
            columnRect.anchorMax = Vector2.one;
            columnRect.offsetMin = Vector2.zero;
            columnRect.offsetMax = Vector2.zero;

            VerticalLayoutGroup layout = columnObject.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = iconSpacing;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            MythicSlotUI[] slots = new MythicSlotUI[MythicSlotCount];

            for (int i = 0; i < MythicSlotCount; i++)
            {
                int slotIndex = i;
                slots[i] = CreateSlot(columnRect, isPlayerColumn, slotIndex);
            }

            return slots;
        }

        private MythicSlotUI CreateSlot(RectTransform parent, bool isPlayerSlot, int slotIndex)
        {
            GameObject slotObject = new GameObject(isPlayerSlot ? "Player Mythic Slot" : "Enemy Mythic Slot");
            slotObject.transform.SetParent(parent, false);

            RectTransform rect = slotObject.AddComponent<RectTransform>();
            rect.sizeDelta = iconSize;

            LayoutElement layout = slotObject.AddComponent<LayoutElement>();
            layout.preferredWidth = iconSize.x;
            layout.preferredHeight = iconSize.y;

            Image outer = slotObject.AddComponent<Image>();
            outer.sprite = circleSprite;
            outer.type = Image.Type.Simple;
            outer.preserveAspect = true;
            outer.color = new Color(0.02f, 0.02f, 0.03f, 0.95f);

            Outline outline = slotObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.95f, 0.78f, 0.28f, 0.84f);
            outline.effectDistance = new Vector2(3f, -3f);

            GameObject innerObject = new GameObject("Inner Glow");
            innerObject.transform.SetParent(slotObject.transform, false);

            RectTransform innerRect = innerObject.AddComponent<RectTransform>();
            innerRect.anchorMin = new Vector2(0.16f, 0.16f);
            innerRect.anchorMax = new Vector2(0.84f, 0.84f);
            innerRect.offsetMin = Vector2.zero;
            innerRect.offsetMax = Vector2.zero;

            Image innerImage = innerObject.AddComponent<Image>();
            innerImage.sprite = circleSprite;
            innerImage.preserveAspect = true;

            GameObject labelObject = new GameObject("Label");
            labelObject.transform.SetParent(slotObject.transform, false);

            RectTransform labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            Text label = labelObject.AddComponent<Text>();
            label.font = defaultFont;
            label.fontSize = 18;
            label.fontStyle = FontStyle.Bold;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.text = "?";

            if (isPlayerSlot)
            {
                Button button = slotObject.AddComponent<Button>();
                button.targetGraphic = outer;
                button.onClick.AddListener(() => TryUsePlayerMythic(slotIndex));
            }

            return new MythicSlotUI(outer, innerImage, label, circleSprite);
        }

        private void TryUsePlayerMythic(int slotIndex)
        {
            if (battleManager == null || battleManager.PlayerState == null)
            {
                return;
            }

            MythicLoadoutRuntime loadout = battleManager.PlayerState.MythicLoadout;

            if (loadout == null)
            {
                return;
            }

            MythicRuntime mythic = loadout.GetMythicAt(slotIndex);

            if (mythic == null)
            {
                Debug.Log("Este slot de Mítico está vazio.");
                return;
            }

            if (!mythic.CanUse())
            {
                Debug.Log($"{mythic.MythicName} já foi usado.");
                return;
            }

            if (!CanApplyPlayerMythic(mythic.Data))
            {
                Debug.Log($"Não há alvo válido para o Mítico {mythic.MythicName}.");
                return;
            }

            if (!loadout.TryUseMythic(slotIndex, out MythicRuntime usedMythic))
            {
                return;
            }

            Debug.Log($"MÍTICO ATIVADO pelo Jogador: {usedMythic.MythicName}");
            Debug.Log($"Texto de ativação: {usedMythic.Data.ActivationText}");

            ApplyPlayerMythicEffect(usedMythic.Data);
        }

        private bool CanApplyPlayerMythic(MythicData mythicData)
        {
            if (mythicData == null || battleManager == null || battleManager.PlayerState == null)
            {
                return false;
            }

            switch (mythicData.EffectType)
            {
                case MythicEffectType.GiveShieldToCreature:
                case MythicEffectType.BuffCreatureAttack:
                case MythicEffectType.BuffCreatureSpeed:
                case MythicEffectType.HealCreature:
                    return GetFirstAliveCreature(battleManager.PlayerState) != null;

                case MythicEffectType.DamageEnemyPlayer:
                case MythicEffectType.None:
                    return true;

                default:
                    return true;
            }
        }

        private void ApplyPlayerMythicEffect(MythicData mythicData)
        {
            if (mythicData == null || battleManager == null || battleManager.PlayerState == null)
            {
                return;
            }

            CardRuntime target = GetFirstAliveCreature(battleManager.PlayerState);

            switch (mythicData.EffectType)
            {
                case MythicEffectType.GiveShieldToCreature:
                    if (target == null)
                    {
                        return;
                    }

                    target.AddTemporaryShield();
                    Debug.Log($"{mythicData.MythicName} concedeu escudo para {target.CardName}.");
                    break;

                case MythicEffectType.BuffCreatureAttack:
                    if (target == null)
                    {
                        return;
                    }

                    target.AddAttribute(CardAttributeType.Attack, mythicData.EffectValue);
                    Debug.Log($"{mythicData.MythicName} deu +{mythicData.EffectValue} ATK para {target.CardName}.");
                    break;

                case MythicEffectType.BuffCreatureSpeed:
                    if (target == null)
                    {
                        return;
                    }

                    target.AddAttribute(CardAttributeType.Speed, mythicData.EffectValue);
                    Debug.Log($"{mythicData.MythicName} deu +{mythicData.EffectValue} Speed para {target.CardName}.");
                    break;

                case MythicEffectType.HealCreature:
                    if (target == null)
                    {
                        return;
                    }

                    target.Heal(mythicData.EffectValue);
                    Debug.Log($"{mythicData.MythicName} curou {mythicData.EffectValue} de vida de {target.CardName}.");
                    break;

                case MythicEffectType.DamageEnemyPlayer:
                    if (battleManager.EnemyState == null)
                    {
                        return;
                    }

                    battleManager.EnemyState.TakeDamageDirect(mythicData.EffectValue);
                    Debug.Log($"{mythicData.MythicName} causou {mythicData.EffectValue} de dano direto no inimigo.");
                    break;

                case MythicEffectType.None:
                    Debug.Log($"{mythicData.MythicName} não possui efeito configurado.");
                    break;

                default:
                    Debug.Log($"{mythicData.MythicName} possui efeito ainda não tratado pela UI: {mythicData.EffectType}.");
                    break;
            }
        }

        private CardRuntime GetFirstAliveCreature(PlayerBattleState state)
        {
            if (state == null)
            {
                return null;
            }

            System.Collections.Generic.List<CardRuntime> creatures = state.Board.GetAliveCreatures();
            return creatures.Count <= 0 ? null : creatures[0];
        }

        private void Refresh()
        {
            if (battleManager == null || battleManager.PlayerState == null || battleManager.EnemyState == null)
            {
                return;
            }

            RefreshSlots(playerSlots, battleManager.PlayerState.MythicLoadout, playerAvailableColor, playerUsedColor, playerEmptyColor, true);
            RefreshSlots(enemySlots, battleManager.EnemyState.MythicLoadout, enemyAvailableColor, enemyUsedColor, enemyEmptyColor, false);
        }

        private void RefreshSlots(MythicSlotUI[] slots, MythicLoadoutRuntime loadout, Color availableColor, Color usedColor, Color emptyColor, bool revealNumbers)
        {
            if (slots == null)
            {
                return;
            }

            for (int i = 0; i < slots.Length; i++)
            {
                MythicRuntime mythic = loadout != null ? loadout.GetMythicAt(i) : null;
                slots[i].SetState(mythic, availableColor, usedColor, emptyColor, revealNumbers, i + 1);
            }
        }

        private Sprite CreateCircleSprite()
        {
            const int size = 64;

            Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Bilinear;

            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size * 0.42f;
            float radiusSquared = radius * radius;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 point = new Vector2(x, y);
                    float distanceSquared = (point - center).sqrMagnitude;
                    texture.SetPixel(x, y, distanceSquared <= radiusSquared ? Color.white : Color.clear);
                }
            }

            texture.Apply();

            return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        }

        private sealed class MythicSlotUI
        {
            private readonly Image outer;
            private readonly Image inner;
            private readonly Text label;
            private readonly Sprite emptySprite;

            public MythicSlotUI(Image outer, Image inner, Text label, Sprite emptySprite)
            {
                this.outer = outer;
                this.inner = inner;
                this.label = label;
                this.emptySprite = emptySprite;
            }

            public void SetState(MythicRuntime mythic, Color availableColor, Color usedColor, Color emptyColor, bool revealNumbers, int fallbackNumber)
            {
                if (mythic == null)
                {
                    outer.color = new Color(0.02f, 0.02f, 0.03f, 0.90f);
                    inner.sprite = emptySprite;
                    inner.color = emptyColor;
                    label.text = string.Empty;
                    return;
                }

                bool used = mythic.IsUsed;
                bool canRevealIcon = revealNumbers || mythic.IsRevealed;
                Sprite icon = canRevealIcon && mythic.Data != null ? mythic.Data.Icon : null;

                outer.color = new Color(0.02f, 0.02f, 0.03f, 0.96f);
                inner.sprite = icon != null ? icon : emptySprite;
                inner.color = icon != null ? (used ? new Color(0.34f, 0.34f, 0.38f, 0.88f) : Color.white) : (used ? usedColor : availableColor);
                label.text = used ? "×" : (revealNumbers ? fallbackNumber.ToString() : "?");
            }
        }
    }
}
