// Caminho: Assets/_Project/Scripts/UI/BattleScreenArtUI.cs
// Descrição: Camada visual real da arena. Usa sprites importados em Assets/_Project/Art/UI para substituir o visual provisório sem mexer na lógica da batalha.

using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CardGame.UI
{
    [DisallowMultipleComponent]
    public class BattleScreenArtUI : MonoBehaviour
    {
        private const float ReferenceWidth = 1080f;
        private const float ReferenceHeight = 2400f;

        [Header("Sprites - Arena")]
        [SerializeField] private Sprite arenaBackgroundSprite;
        [SerializeField] private Sprite combatBandSprite;

        [Header("Sprites - Slots e HUD")]
        [SerializeField] private Sprite creatureSlotSprite;
        [SerializeField] private Sprite trapSlotSprite;
        [SerializeField] private Sprite cardBackSprite;
        [SerializeField] private Sprite mythicSlotSprite;
        [SerializeField] private Sprite hudLeftPanelSprite;
        [SerializeField] private Sprite hudStatusPanelSprite;
        [SerializeField] private Sprite mainButtonFrameSprite;

        [Header("Camada")]
        [SerializeField] private int sortingOrder = -20;
        [SerializeField] private bool showDecorativeCreatureSlots = false;
        [SerializeField] private bool showDecorativeTrapSlots = false;
        [SerializeField] private bool showDecorativeMythicSlots = false;
        [SerializeField] private bool showHudPanels = false;
        [SerializeField] private bool showMainButtonFrame = true;

        [Header("Ajuste fino - Fundo")]
        [SerializeField] private Color arenaTint = new Color(0.70f, 0.76f, 0.90f, 1f);
        [SerializeField] private bool preserveArenaAspect = false;
        [SerializeField] private Vector2 arenaSize = new Vector2(1080f, 2400f);
        [SerializeField] private Vector2 arenaPosition = Vector2.zero;
        [SerializeField] private bool showReadabilityOverlay = true;
        [SerializeField] private Color readabilityOverlayColor = new Color(0f, 0f, 0f, 0.34f);
        [SerializeField] private bool showPlayMats = true;
        [SerializeField] private Color playMatColor = new Color(0.02f, 0.025f, 0.045f, 0.42f);
        [SerializeField] private Color playMatOutlineColor = new Color(0.80f, 0.64f, 0.30f, 0.28f);
        [SerializeField] private Vector2 enemyPlayMatSize = new Vector2(1010f, 520f);
        [SerializeField] private Vector2 enemyPlayMatPosition = new Vector2(0f, 590f);
        [SerializeField] private Vector2 playerPlayMatSize = new Vector2(1010f, 520f);
        [SerializeField] private Vector2 playerPlayMatPosition = new Vector2(0f, -480f);

        [Header("Ajuste fino - Área de Combate")]
        [SerializeField] private Vector2 combatBandSize = new Vector2(1080f, 285f);
        [SerializeField] private Vector2 combatBandPosition = new Vector2(0f, 0f);
        [SerializeField] private Color combatBandTint = new Color(0.86f, 0.90f, 1f, 0.86f);

        [Header("Ajuste fino - Slots de Criatura")]
        [SerializeField] private Vector2 creatureSlotSize = new Vector2(165f, 230f);
        [SerializeField] private float creatureSlotSpacing = 8f;
        [SerializeField] private Vector2 enemyCreatureRowPosition = new Vector2(0f, 650f);
        [SerializeField] private Vector2 playerCreatureRowPosition = new Vector2(0f, -430f);

        [Header("Ajuste fino - Slots de Armadilha")]
        [SerializeField] private Vector2 trapSlotSize = new Vector2(135f, 88f);
        [SerializeField] private float trapSlotSpacing = 7f;
        [SerializeField] private Vector2 enemyTrapRowPosition = new Vector2(44f, 418f);
        [SerializeField] private Vector2 playerTrapRowPosition = new Vector2(44f, -665f);

        [Header("Ajuste fino - Indicador de Armadilhas")]
        [SerializeField] private Vector2 trapCounterPanelSize = new Vector2(118f, 108f);
        [SerializeField] private Vector2 enemyTrapCounterPosition = new Vector2(-420f, 418f);
        [SerializeField] private Vector2 playerTrapCounterPosition = new Vector2(-420f, -665f);
        [SerializeField] private Color trapCounterColor = new Color(0.015f, 0.03f, 0.06f, 0.92f);
        [SerializeField] private Color trapCounterTextColor = Color.white;
        [SerializeField] private int trapCounterFontSize = 22;

        [Header("Ajuste fino - Míticos")]
        [SerializeField] private Vector2 mythicSlotSize = new Vector2(84f, 84f);
        [SerializeField] private Vector2 enemyMythicTopPosition = new Vector2(486f, 600f);
        [SerializeField] private Vector2 enemyMythicBottomPosition = new Vector2(486f, 485f);
        [SerializeField] private Vector2 playerMythicTopPosition = new Vector2(486f, -445f);
        [SerializeField] private Vector2 playerMythicBottomPosition = new Vector2(486f, -560f);

        [Header("Ajuste fino - HUD")]
        [SerializeField] private Vector2 enemyHudPosition = new Vector2(-392f, 1010f);
        [SerializeField] private Vector2 playerHudPosition = new Vector2(-392f, -945f);
        [SerializeField] private Vector2 hudStatusSize = new Vector2(250f, 160f);
        [SerializeField] private Vector2 leftHudPosition = new Vector2(-490f, -880f);
        [SerializeField] private Vector2 leftHudSize = new Vector2(122f, 420f);

        [Header("Ajuste fino - Botão Principal")]
        [SerializeField] private Vector2 mainButtonFramePosition = new Vector2(430f, -880f);
        [SerializeField] private Vector2 mainButtonFrameSize = new Vector2(210f, 210f);

        private Canvas canvas;
        private RectTransform canvasRoot;
        private Font defaultFont;

        private void Awake()
        {
            defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            Build();
        }

        private void OnEnable()
        {
            if (Application.isPlaying && canvas == null)
            {
                Build();
            }
        }

        [ContextMenu("Auto Bind Sprites From Project Paths")]
        public void AutoBindSpritesFromProjectPaths()
        {
#if UNITY_EDITOR
            arenaBackgroundSprite = LoadSprite("Assets/_Project/Art/UI/Arena/arena_bg_main.png", arenaBackgroundSprite);
            combatBandSprite = LoadSprite("Assets/_Project/Art/UI/Arena/combat_band.png", combatBandSprite);
            creatureSlotSprite = LoadSprite("Assets/_Project/Art/UI/Slots/slot_creature_empty.png", creatureSlotSprite);
            trapSlotSprite = LoadSprite("Assets/_Project/Art/UI/Slots/slot_trap_empty.png", trapSlotSprite);
            cardBackSprite = LoadSprite("Assets/_Project/Art/UI/Slots/card_back_default.png", cardBackSprite);
            mythicSlotSprite = LoadSprite("Assets/_Project/Art/UI/Mythics/mythic_slot.png", mythicSlotSprite);
            hudLeftPanelSprite = LoadSprite("Assets/_Project/Art/UI/HUD/hud_left_panel.png", hudLeftPanelSprite);
            hudStatusPanelSprite = LoadSprite("Assets/_Project/Art/UI/HUD/hud_status_panel.png", hudStatusPanelSprite);
            mainButtonFrameSprite = LoadSprite("Assets/_Project/Art/UI/Buttons/main_button_frame.png", mainButtonFrameSprite);
            EditorUtility.SetDirty(this);
#endif
        }

        [ContextMenu("Rebuild Visual Skin")]
        public void RebuildVisualSkin()
        {
            ClearGeneratedChildren();
            Build();
        }

        private void Build()
        {
            ClearGeneratedChildren();
            CreateCanvas();
            CreateArenaBackground();
            CreateReadabilityOverlay();
            CreatePlayMats();
            CreateCombatBand();

            if (showDecorativeCreatureSlots)
            {
                CreateCreatureSlots(enemyCreatureRowPosition, false);
                CreateCreatureSlots(playerCreatureRowPosition, true);
            }

            if (showDecorativeTrapSlots)
            {
                CreateTrapArea(enemyTrapRowPosition, enemyTrapCounterPosition, false);
                CreateTrapArea(playerTrapRowPosition, playerTrapCounterPosition, true);
            }

            if (showDecorativeMythicSlots)
            {
                CreateMythicSlot(enemyMythicTopPosition, false, true);
                CreateMythicSlot(enemyMythicBottomPosition, false, false);
                CreateMythicSlot(playerMythicTopPosition, true, true);
                CreateMythicSlot(playerMythicBottomPosition, true, false);
            }

            if (showHudPanels)
            {
                CreateHudPanel(enemyHudPosition, hudStatusSize, "Enemy Status Panel", hudStatusPanelSprite);
                CreateHudPanel(playerHudPosition, hudStatusSize, "Player Status Panel", hudStatusPanelSprite);
                CreateHudPanel(leftHudPosition, leftHudSize, "Left Resource Panel", hudLeftPanelSprite);
            }

            if (showMainButtonFrame)
            {
                CreateMainButtonFrame();
            }
        }

        private void CreateCanvas()
        {
            GameObject canvasObject = new GameObject("Battle Screen Art Canvas");
            canvasObject.transform.SetParent(transform, false);

            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            GraphicRaycaster raycaster = canvasObject.AddComponent<GraphicRaycaster>();
            raycaster.enabled = false;

            canvasRoot = canvasObject.GetComponent<RectTransform>();
            canvasRoot.anchorMin = Vector2.zero;
            canvasRoot.anchorMax = Vector2.one;
            canvasRoot.offsetMin = Vector2.zero;
            canvasRoot.offsetMax = Vector2.zero;
        }

        private void CreateArenaBackground()
        {
            Image image = CreateImage("Arena Background", canvasRoot, arenaBackgroundSprite, arenaTint, arenaPosition, arenaSize, 0.5f, 0.5f);
            image.preserveAspect = preserveArenaAspect;
        }

        private void CreateReadabilityOverlay()
        {
            if (!showReadabilityOverlay)
            {
                return;
            }

            GameObject overlayObject = new GameObject("Arena Readability Overlay");
            overlayObject.transform.SetParent(canvasRoot, false);

            RectTransform rect = overlayObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image overlay = overlayObject.AddComponent<Image>();
            overlay.color = readabilityOverlayColor;
            overlay.raycastTarget = false;
        }

        private void CreatePlayMats()
        {
            if (!showPlayMats)
            {
                return;
            }

            CreateMat("Enemy Play Mat", enemyPlayMatPosition, enemyPlayMatSize);
            CreateMat("Player Play Mat", playerPlayMatPosition, playerPlayMatSize);
        }

        private void CreateMat(string objectName, Vector2 position, Vector2 size)
        {
            GameObject matObject = new GameObject(objectName);
            matObject.transform.SetParent(canvasRoot, false);

            RectTransform rect = matObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image mat = matObject.AddComponent<Image>();
            mat.color = playMatColor;
            mat.raycastTarget = false;

            Outline outline = matObject.AddComponent<Outline>();
            outline.effectColor = playMatOutlineColor;
            outline.effectDistance = new Vector2(2f, -2f);
        }

        private void CreateCombatBand()
        {
            if (combatBandSprite == null) return;
            Image image = CreateImage("Combat Band", canvasRoot, combatBandSprite, combatBandTint, combatBandPosition, combatBandSize, 0.5f, 0.5f);
            image.type = Image.Type.Sliced;
            image.preserveAspect = false;
        }

        private void CreateCreatureSlots(Vector2 rowPosition, bool isPlayer)
        {
            float totalWidth = creatureSlotSize.x * 5f + creatureSlotSpacing * 4f;
            float startX = rowPosition.x - totalWidth * 0.5f + creatureSlotSize.x * 0.5f;
            for (int i = 0; i < 5; i++)
            {
                Vector2 position = new Vector2(startX + i * (creatureSlotSize.x + creatureSlotSpacing), rowPosition.y);
                Image slot = CreateImage((isPlayer ? "Player" : "Enemy") + " Decorative Creature Slot " + (i + 1), canvasRoot, creatureSlotSprite, Color.white, position, creatureSlotSize, 0.5f, 0.5f);
                slot.preserveAspect = false;
            }
        }

        private void CreateTrapArea(Vector2 rowPosition, Vector2 counterPosition, bool isPlayer)
        {
            CreateTrapCounter(counterPosition, isPlayer);

            float totalWidth = trapSlotSize.x * 6f + trapSlotSpacing * 5f;
            float startX = rowPosition.x - totalWidth * 0.5f + trapSlotSize.x * 0.5f;
            for (int i = 0; i < 6; i++)
            {
                Vector2 position = new Vector2(startX + i * (trapSlotSize.x + trapSlotSpacing), rowPosition.y);
                Image slot = CreateImage((isPlayer ? "Player" : "Enemy") + " Decorative Trap Slot " + (i + 1), canvasRoot, trapSlotSprite, Color.white, position, trapSlotSize, 0.5f, 0.5f);
                slot.preserveAspect = false;
            }
        }

        private void CreateTrapCounter(Vector2 position, bool isPlayer)
        {
            GameObject panelObject = new GameObject((isPlayer ? "Player" : "Enemy") + " Trap Counter Decorative Panel");
            panelObject.transform.SetParent(canvasRoot, false);
            RectTransform rect = panelObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = trapCounterPanelSize;

            Image background = panelObject.AddComponent<Image>();
            background.color = trapCounterColor;
            background.raycastTarget = false;

            Outline outline = panelObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.95f, 0.68f, 0.16f, 0.85f);
            outline.effectDistance = new Vector2(2f, -2f);

            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(panelObject.transform, false);
            Text text = textObject.AddComponent<Text>();
            text.font = defaultFont;
            text.text = "TRAPS\n0/6";
            text.fontSize = trapCounterFontSize;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = trapCounterTextColor;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 8;
            text.resizeTextMaxSize = trapCounterFontSize;
            text.raycastTarget = false;

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(6f, 6f);
            textRect.offsetMax = new Vector2(-6f, -6f);
        }

        private void CreateMythicSlot(Vector2 position, bool isPlayer, bool first)
        {
            Image slot = CreateImage((isPlayer ? "Player" : "Enemy") + " Decorative Mythic Slot " + (first ? "A" : "B"), canvasRoot, mythicSlotSprite, Color.white, position, mythicSlotSize, 0.5f, 0.5f);
            slot.preserveAspect = true;
        }

        private void CreateHudPanel(Vector2 position, Vector2 size, string objectName, Sprite sprite)
        {
            if (sprite == null) return;
            Image panel = CreateImage(objectName, canvasRoot, sprite, Color.white, position, size, 0.5f, 0.5f);
            panel.preserveAspect = false;
            panel.type = Image.Type.Simple;
        }

        private void CreateMainButtonFrame()
        {
            if (mainButtonFrameSprite == null) return;
            Image buttonFrame = CreateImage("Main Button Decorative Frame", canvasRoot, mainButtonFrameSprite, Color.white, mainButtonFramePosition, mainButtonFrameSize, 0.5f, 0.5f);
            buttonFrame.preserveAspect = true;
        }

        private Image CreateImage(string objectName, Transform parent, Sprite sprite, Color color, Vector2 anchoredPosition, Vector2 size, float pivotX, float pivotY)
        {
            GameObject imageObject = new GameObject(objectName);
            imageObject.transform.SetParent(parent, false);

            RectTransform rect = imageObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(pivotX, pivotY);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;

            Image image = imageObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = false;
            image.type = Image.Type.Simple;
            image.preserveAspect = false;
            return image;
        }

        private void ClearGeneratedChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }

            canvas = null;
            canvasRoot = null;
        }

#if UNITY_EDITOR
        private static Sprite LoadSprite(string path, Sprite currentValue)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            return sprite != null ? sprite : currentValue;
        }
#endif
    }
}
