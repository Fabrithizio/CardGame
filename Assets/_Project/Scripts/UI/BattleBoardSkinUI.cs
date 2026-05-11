// Caminho: Assets/_Project/Scripts/UI/BattleBoardSkinUI.cs
// Descrição: Skin visual provisória da arena. Desenha fundo, painéis, molduras, energia central, orbes míticos e base do HUD.

using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public sealed class BattleBoardSkinUI : MonoBehaviour
    {
        [Header("Atualização em Tempo Real")]
        [SerializeField] private bool updateLayoutEveryFrame = true;

        [Header("Fundo")]
        [SerializeField] private Color topBackgroundColor = new Color(0.025f, 0.035f, 0.065f, 1f);
        [SerializeField] private Color middleBackgroundColor = new Color(0.035f, 0.050f, 0.090f, 1f);
        [SerializeField] private Color bottomBackgroundColor = new Color(0.020f, 0.028f, 0.052f, 1f);

        [Header("Campos")]
        [SerializeField] private Color enemyPanelColor = new Color(0.16f, 0.12f, 0.20f, 0.90f);
        [SerializeField] private Color playerPanelColor = new Color(0.10f, 0.15f, 0.26f, 0.90f);
        [SerializeField] private Color panelBorderColor = new Color(0.83f, 0.64f, 0.32f, 0.50f);
        [SerializeField] private Color slotFrameColor = new Color(0.06f, 0.07f, 0.10f, 0.72f);
        [SerializeField] private Color trapFrameColor = new Color(0.05f, 0.05f, 0.07f, 0.70f);

        [Header("Área de Combate")]
        [SerializeField] private Color combatColor = new Color(0.03f, 0.035f, 0.075f, 0.96f);
        [SerializeField] private Color combatGlowColor = new Color(0.25f, 0.12f, 0.75f, 0.32f);
        [SerializeField] private Color combatLineColor = new Color(0.20f, 0.55f, 1f, 0.45f);

        [Header("HUD")]
        [SerializeField] private Color hudPanelColor = new Color(0.015f, 0.025f, 0.045f, 0.94f);
        [SerializeField] private Color hudBorderColor = new Color(0.82f, 0.67f, 0.38f, 0.44f);

        [Header("Míticos")]
        [SerializeField] private Color mythicOuterColor = new Color(0.94f, 0.72f, 0.22f, 1f);
        [SerializeField] private Color mythicInnerPlayerColor = new Color(0.08f, 0.35f, 1f, 0.92f);
        [SerializeField] private Color mythicInnerEnemyColor = new Color(0.68f, 0.08f, 0.80f, 0.92f);
        [SerializeField] private Color mythicEmptyColor = new Color(0.14f, 0.12f, 0.07f, 0.88f);

        [Header("Slots Decorativos")]
        [SerializeField] private Vector2 creatureFramePadding = new Vector2(8f, 8f);
        [SerializeField] private Vector2 trapFramePadding = new Vector2(5f, 5f);
        [SerializeField] private int creatureSlots = 5;
        [SerializeField] private int trapSlots = 6;

        private BattleScreenLayoutUI layout;
        private RectTransform skinRoot;

        private RectTransform enemyPanel;
        private RectTransform playerPanel;
        private RectTransform combatPanel;
        private RectTransform bottomHudPanel;
        private RectTransform topHudPanel;

        private readonly RectTransform[] enemyCreatureFrames = new RectTransform[5];
        private readonly RectTransform[] playerCreatureFrames = new RectTransform[5];
        private readonly RectTransform[] enemyTrapFrames = new RectTransform[6];
        private readonly RectTransform[] playerTrapFrames = new RectTransform[6];
        private readonly RectTransform[] enemyMythicFrames = new RectTransform[2];
        private readonly RectTransform[] playerMythicFrames = new RectTransform[2];

        private Font defaultFont;
        private bool built;

        private void Awake()
        {
            defaultFont = ResponsiveUIUtility.GetDefaultFont();
            Build();
        }

        private void Update()
        {
            if (!built)
            {
                Build();
            }

            if (updateLayoutEveryFrame)
            {
                ApplyLayout();
            }
        }

        private void OnValidate()
        {
            // Não cria objetos aqui. O Update aplica ajustes em Play.
        }

        private void Build()
        {
            if (built)
            {
                return;
            }

            layout = BattleScreenLayoutUI.GetOrCreate();
            skinRoot = layout.GetZone(BattleScreenZone.SkinLayer);

            CreateBackground();
            topHudPanel = CreatePanel("Top Hud Skin", layout.GetZone(BattleScreenZone.EnemyHud), hudPanelColor, hudBorderColor, 0);
            enemyPanel = CreatePanel("Enemy Board Skin", layout.GetZone(BattleScreenZone.EnemyBoard), enemyPanelColor, panelBorderColor, 0);
            combatPanel = CreatePanel("Combat Skin", layout.GetZone(BattleScreenZone.CombatInfo), combatColor, combatLineColor, 0);
            playerPanel = CreatePanel("Player Board Skin", layout.GetZone(BattleScreenZone.PlayerBoard), playerPanelColor, panelBorderColor, 0);
            bottomHudPanel = CreatePanel("Bottom Hud Skin", layout.GetZone(BattleScreenZone.PlayerHudHand), hudPanelColor, hudBorderColor, 0);

            CreateCombatEnergy(combatPanel);

            CreateCreatureFrames(enemyCreatureFrames, layout.GetZone(BattleScreenZone.EnemyCreatureRow), false);
            CreateCreatureFrames(playerCreatureFrames, layout.GetZone(BattleScreenZone.PlayerCreatureRow), true);

            CreateTrapFrames(enemyTrapFrames, layout.GetZone(BattleScreenZone.EnemyTrapRow), false);
            CreateTrapFrames(playerTrapFrames, layout.GetZone(BattleScreenZone.PlayerTrapRow), true);

            CreateMythicFrames(enemyMythicFrames, layout.GetZone(BattleScreenZone.EnemyMythic), false);
            CreateMythicFrames(playerMythicFrames, layout.GetZone(BattleScreenZone.PlayerMythic), true);

            built = true;
            ApplyLayout();
        }

        private void CreateBackground()
        {
            RectTransform rootA = CreateFillObject("Background Top", skinRoot, topBackgroundColor, 0);
            rootA.anchorMin = new Vector2(0f, 0.66f);
            rootA.anchorMax = new Vector2(1f, 1f);

            RectTransform rootB = CreateFillObject("Background Middle", skinRoot, middleBackgroundColor, 0);
            rootB.anchorMin = new Vector2(0f, 0.26f);
            rootB.anchorMax = new Vector2(1f, 0.66f);

            RectTransform rootC = CreateFillObject("Background Bottom", skinRoot, bottomBackgroundColor, 0);
            rootC.anchorMin = new Vector2(0f, 0f);
            rootC.anchorMax = new Vector2(1f, 0.26f);
        }

        private RectTransform CreatePanel(string objectName, RectTransform parent, Color color, Color outlineColor, int siblingIndex)
        {
            RectTransform rect = CreateFillObject(objectName, parent, color, siblingIndex);

            Outline outline = rect.gameObject.AddComponent<Outline>();
            outline.effectColor = outlineColor;
            outline.effectDistance = new Vector2(2.5f, -2.5f);

            Shadow shadow = rect.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.45f);
            shadow.effectDistance = new Vector2(0f, -5f);

            return rect;
        }

        private RectTransform CreateFillObject(string objectName, RectTransform parent, Color color, int siblingIndex)
        {
            GameObject obj = new GameObject(objectName);
            obj.transform.SetParent(parent, false);
            obj.transform.SetSiblingIndex(siblingIndex);

            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = obj.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;

            return rect;
        }

        private RectTransform CreateAnchoredBox(string objectName, RectTransform parent, Color color)
        {
            GameObject obj = new GameObject(objectName);
            obj.transform.SetParent(parent, false);

            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            Image image = obj.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;

            Outline outline = obj.AddComponent<Outline>();
            outline.effectColor = panelBorderColor;
            outline.effectDistance = new Vector2(2f, -2f);

            Shadow shadow = obj.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.50f);
            shadow.effectDistance = new Vector2(0f, -4f);

            return rect;
        }

        private void CreateCombatEnergy(RectTransform parent)
        {
            RectTransform glow = CreateFillObject("Combat Glow", parent, combatGlowColor, 1);
            glow.anchorMin = new Vector2(0.04f, 0.16f);
            glow.anchorMax = new Vector2(0.96f, 0.84f);

            RectTransform lineTop = CreateFillObject("Combat Line Top", parent, combatLineColor, 2);
            lineTop.anchorMin = new Vector2(0f, 0.86f);
            lineTop.anchorMax = new Vector2(1f, 0.89f);

            RectTransform lineBottom = CreateFillObject("Combat Line Bottom", parent, combatLineColor, 2);
            lineBottom.anchorMin = new Vector2(0f, 0.11f);
            lineBottom.anchorMax = new Vector2(1f, 0.14f);

            GameObject textObject = new GameObject("Combat Label");
            textObject.transform.SetParent(parent, false);

            RectTransform textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text text = textObject.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = 22;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(1f, 1f, 1f, 0.60f);
            text.text = "ÁREA DE COMBATE";
            text.raycastTarget = false;
        }

        private void CreateCreatureFrames(RectTransform[] frames, RectTransform parent, bool isPlayer)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                RectTransform frame = CreateAnchoredBox(isPlayer ? $"Player Creature Frame {i + 1}" : $"Enemy Creature Frame {i + 1}", parent, slotFrameColor);

                Image image = frame.GetComponent<Image>();
                image.color = isPlayer
                    ? new Color(0.035f, 0.065f, 0.13f, 0.72f)
                    : new Color(0.11f, 0.055f, 0.08f, 0.72f);

                frames[i] = frame;
            }
        }

        private void CreateTrapFrames(RectTransform[] frames, RectTransform parent, bool isPlayer)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                RectTransform frame = CreateAnchoredBox(isPlayer ? $"Player Trap Frame {i + 1}" : $"Enemy Trap Frame {i + 1}", parent, trapFrameColor);

                Image image = frame.GetComponent<Image>();
                image.color = isPlayer
                    ? new Color(0.025f, 0.060f, 0.12f, 0.70f)
                    : new Color(0.08f, 0.035f, 0.08f, 0.70f);

                frames[i] = frame;
            }
        }

        private void CreateMythicFrames(RectTransform[] frames, RectTransform parent, bool isPlayer)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                GameObject orb = new GameObject(isPlayer ? $"Player Mythic Orb Skin {i + 1}" : $"Enemy Mythic Orb Skin {i + 1}");
                orb.transform.SetParent(parent, false);

                RectTransform rect = orb.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);

                Image outer = orb.AddComponent<Image>();
                outer.color = mythicOuterColor;
                outer.raycastTarget = false;

                Outline outline = orb.AddComponent<Outline>();
                outline.effectColor = new Color(0f, 0f, 0f, 0.80f);
                outline.effectDistance = new Vector2(3f, -3f);

                GameObject inner = new GameObject("Inner Glow");
                inner.transform.SetParent(orb.transform, false);

                RectTransform innerRect = inner.AddComponent<RectTransform>();
                innerRect.anchorMin = new Vector2(0.16f, 0.16f);
                innerRect.anchorMax = new Vector2(0.84f, 0.84f);
                innerRect.offsetMin = Vector2.zero;
                innerRect.offsetMax = Vector2.zero;

                Image innerImage = inner.AddComponent<Image>();
                innerImage.color = i == 0
                    ? (isPlayer ? mythicInnerPlayerColor : mythicInnerEnemyColor)
                    : mythicEmptyColor;
                innerImage.raycastTarget = false;

                frames[i] = rect;
            }
        }

        private void ApplyLayout()
        {
            if (!built || layout == null)
            {
                return;
            }

            ApplyFrameRow(enemyCreatureFrames, layout.GetZone(BattleScreenZone.EnemyCreatureRow), creatureSlots, creatureFramePadding, true);
            ApplyFrameRow(playerCreatureFrames, layout.GetZone(BattleScreenZone.PlayerCreatureRow), creatureSlots, creatureFramePadding, true);

            ApplyTrapFrameRow(enemyTrapFrames, layout.GetZone(BattleScreenZone.EnemyTrapRow), trapSlots, trapFramePadding);
            ApplyTrapFrameRow(playerTrapFrames, layout.GetZone(BattleScreenZone.PlayerTrapRow), trapSlots, trapFramePadding);

            ApplyMythicColumn(enemyMythicFrames, layout.GetZone(BattleScreenZone.EnemyMythic));
            ApplyMythicColumn(playerMythicFrames, layout.GetZone(BattleScreenZone.PlayerMythic));
        }

        private void ApplyFrameRow(RectTransform[] frames, RectTransform zone, int count, Vector2 padding, bool tall)
        {
            if (frames == null || zone == null || count <= 0)
            {
                return;
            }

            float width = Mathf.Max(1f, zone.rect.width);
            float height = Mathf.Max(1f, zone.rect.height);

            float spacing = 8f;
            float frameWidth = (width - spacing * (count - 1)) / count;
            float frameHeight = tall ? height * 0.96f : height * 0.86f;
            float totalWidth = frameWidth * count + spacing * (count - 1);
            float startX = -totalWidth * 0.5f + frameWidth * 0.5f;

            for (int i = 0; i < frames.Length && i < count; i++)
            {
                if (frames[i] == null)
                {
                    continue;
                }

                frames[i].sizeDelta = new Vector2(Mathf.Max(1f, frameWidth - padding.x), Mathf.Max(1f, frameHeight - padding.y));
                frames[i].anchoredPosition = new Vector2(startX + i * (frameWidth + spacing), 0f);
            }
        }

        private void ApplyTrapFrameRow(RectTransform[] frames, RectTransform zone, int count, Vector2 padding)
        {
            if (frames == null || zone == null || count <= 0)
            {
                return;
            }

            float width = Mathf.Max(1f, zone.rect.width);
            float height = Mathf.Max(1f, zone.rect.height);

            float counterReserve = width * 0.145f + 16f;
            float availableWidth = width - counterReserve;
            float spacing = 7f;

            float frameWidth = (availableWidth - spacing * (count - 1)) / count;
            float frameHeight = Mathf.Min(height * 0.72f, frameWidth * 0.92f);
            float startX = -width * 0.5f + counterReserve + frameWidth * 0.5f + 4f;

            for (int i = 0; i < frames.Length && i < count; i++)
            {
                if (frames[i] == null)
                {
                    continue;
                }

                frames[i].sizeDelta = new Vector2(Mathf.Max(1f, frameWidth - padding.x), Mathf.Max(1f, frameHeight - padding.y));
                frames[i].anchoredPosition = new Vector2(startX + i * (frameWidth + spacing), 0f);
            }
        }

        private void ApplyMythicColumn(RectTransform[] frames, RectTransform zone)
        {
            if (frames == null || zone == null)
            {
                return;
            }

            float width = Mathf.Max(1f, zone.rect.width);
            float height = Mathf.Max(1f, zone.rect.height);

            float orbSize = Mathf.Clamp(Mathf.Min(width * 0.88f, height * 0.26f), 52f, 105f);
            float spacing = orbSize * 0.36f;
            float totalHeight = orbSize * 2f + spacing;
            float startY = totalHeight * 0.5f - orbSize * 0.5f;

            for (int i = 0; i < frames.Length; i++)
            {
                if (frames[i] == null)
                {
                    continue;
                }

                frames[i].sizeDelta = new Vector2(orbSize, orbSize);
                frames[i].anchoredPosition = new Vector2(0f, startY - i * (orbSize + spacing));
            }
        }
    }
}
