// Caminho: Assets/_Project/Scripts/UI/BattleScreenLayoutUI.cs
// Descrição: Layout mestre mobile em retrato. Deixa o campo simétrico,
// aproxima as linhas do centro e cria zonas estáveis para HUD, mão e botão.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public enum BattleScreenZone
    {
        Root,
        SkinLayer,
        BoardLayer,
        HudLayer,
        HandLayer,

        EnemyHud,
        EnemyDeck,
        EnemyHealth,
        TurnStatus,
        BattleLog,

        EnemyBoard,
        EnemyCreatureRow,
        EnemyTrapRow,
        EnemyMythic,

        CombatInfo,

        PlayerBoard,
        PlayerCreatureRow,
        PlayerTrapRow,
        PlayerMythic,

        PlayerHudHand,
        PlayerDeck,
        PlayerHealth,
        PlayerHand,
        TurnButton
    }

    public sealed class BattleScreenLayoutUI : MonoBehaviour
    {
        public static BattleScreenLayoutUI Instance { get; private set; }

        [Header("Canvas")]
        [SerializeField] private int sortingOrder = 0;
        [SerializeField] private Vector2 referenceResolution = new Vector2(1080f, 2400f);
        [SerializeField] [Range(0f, 1f)] private float matchWidthOrHeight = 0.65f;

        [Header("Atualização em Tempo Real")]
        [SerializeField] private bool updateLayoutEveryFrame = true;

        [Header("Blocos Principais")]
        [SerializeField] [Range(0f, 1f)] private float enemyHudMinY = 0.872f;
        [SerializeField] [Range(0f, 1f)] private float enemyBoardMinY = 0.545f;
        [SerializeField] [Range(0f, 1f)] private float combatMinY = 0.455f;
        [SerializeField] [Range(0f, 1f)] private float playerBoardMinY = 0.205f;
        [SerializeField] [Range(0f, 1f)] private float bottomHudMaxY = 0.205f;

        [Header("Campo - Largura")]
        [SerializeField] [Range(0f, 0.18f)] private float boardLeft = 0.070f;
        [SerializeField] [Range(0.70f, 1f)] private float boardCardsRight = 0.885f;
        [SerializeField] [Range(0.80f, 1f)] private float mythicLeft = 0.890f;
        [SerializeField] [Range(0.88f, 1f)] private float mythicRight = 0.985f;

        [Header("Campo Inimigo")]
        [SerializeField] [Range(0f, 1f)] private float enemyTrapMinY = 0.605f;
        [SerializeField] [Range(0f, 1f)] private float enemyTrapMaxY = 0.785f;
        [SerializeField] [Range(0f, 1f)] private float enemyCreatureMinY = 0.115f;
        [SerializeField] [Range(0f, 1f)] private float enemyCreatureMaxY = 0.515f;

        [Header("Campo Jogador")]
        [SerializeField] [Range(0f, 1f)] private float playerCreatureMinY = 0.325f;
        [SerializeField] [Range(0f, 1f)] private float playerCreatureMaxY = 0.835f;
        [SerializeField] [Range(0f, 1f)] private float playerTrapMinY = 0.095f;
        [SerializeField] [Range(0f, 1f)] private float playerTrapMaxY = 0.275f;

        [Header("Rodapé")]
        [SerializeField] [Range(0f, 0.2f)] private float bottomLeft = 0.018f;
        [SerializeField] [Range(0.07f, 0.18f)] private float playerDeckRight = 0.110f;
        [SerializeField] [Range(0.08f, 0.25f)] private float playerHealthLeft = 0.118f;
        [SerializeField] [Range(0.25f, 0.45f)] private float playerHealthRight = 0.350f;
        [SerializeField] [Range(0.05f, 0.40f)] private float playerHandLeft = 0.092f;
        [SerializeField] [Range(0.60f, 0.98f)] private float playerHandRight = 0.898f;
        [SerializeField] [Range(0.70f, 0.98f)] private float turnButtonLeft = 0.810f;
        [SerializeField] [Range(0.86f, 1f)] private float turnButtonRight = 0.990f;

        [Header("Limpeza")]
        [SerializeField] private bool hideLegacyBoardVisual = true;
        [SerializeField] private bool hideLegacyDebugHud = true;

        [Header("Debug")]
        [SerializeField] private bool showDebugZoneNames = false;

        private Canvas canvas;
        private CanvasScaler canvasScaler;
        private RectTransform root;
        private Font defaultFont;
        private readonly Dictionary<BattleScreenZone, RectTransform> zones = new();
        private bool legacyObjectsHidden;

        public static BattleScreenLayoutUI GetOrCreate()
        {
            if (Instance != null)
            {
                return Instance;
            }

            BattleScreenLayoutUI existing = FindFirstObjectByType<BattleScreenLayoutUI>();
            if (existing != null)
            {
                Instance = existing;
                existing.EnsureBuilt();
                return existing;
            }

            GameObject layoutObject = new GameObject("BattleScreenLayoutUI");
            BattleScreenLayoutUI created = layoutObject.AddComponent<BattleScreenLayoutUI>();
            created.EnsureBuilt();
            return created;
        }

        public RectTransform GetZone(BattleScreenZone zone)
        {
            EnsureBuilt();
            return zones.TryGetValue(zone, out RectTransform rect) && rect != null ? rect : root;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            EnsureBuilt();
            ApplyLayout();
            HideLegacyObjectsIfNeeded();
        }

        private void Update()
        {
            if (updateLayoutEveryFrame)
            {
                EnsureBuilt();
                ApplyLayout();
            }

            HideLegacyObjectsIfNeeded();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void EnsureBuilt()
        {
            if (root != null && zones.Count > 0) return;

            defaultFont = ResponsiveUIUtility.GetDefaultFont();
            CreateCanvas();
            CreateZonesIfNeeded();
        }

        private void CreateCanvas()
        {
            canvas = GetComponentInChildren<Canvas>();

            if (canvas == null)
            {
                GameObject canvasObject = new GameObject("Battle Screen Canvas");
                canvasObject.transform.SetParent(transform, false);

                canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = sortingOrder;

                canvasScaler = canvasObject.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = referenceResolution;
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = matchWidthOrHeight;

                canvasObject.AddComponent<GraphicRaycaster>();
            }
            else
            {
                canvasScaler = canvas.GetComponent<CanvasScaler>();
                if (canvasScaler == null)
                {
                    canvasScaler = canvas.gameObject.AddComponent<CanvasScaler>();
                }

                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = referenceResolution;
                canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
                canvas.sortingOrder = sortingOrder;
            }

            root = canvas.GetComponent<RectTransform>();
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;
        }

        private void CreateZonesIfNeeded()
        {
            zones.Clear();
            zones[BattleScreenZone.Root] = root;

            RectTransform skinLayer = GetOrCreateZone("00_SkinLayer", root);
            RectTransform boardLayer = GetOrCreateZone("10_BoardLayer", root);
            RectTransform hudLayer = GetOrCreateZone("20_HudLayer", root);
            RectTransform handLayer = GetOrCreateZone("30_HandLayer", root);

            zones[BattleScreenZone.SkinLayer] = skinLayer;
            zones[BattleScreenZone.BoardLayer] = boardLayer;
            zones[BattleScreenZone.HudLayer] = hudLayer;
            zones[BattleScreenZone.HandLayer] = handLayer;

            zones[BattleScreenZone.EnemyHud] = GetOrCreateZone("EnemyHud", hudLayer);
            zones[BattleScreenZone.EnemyBoard] = GetOrCreateZone("EnemyBoard", boardLayer);
            zones[BattleScreenZone.CombatInfo] = GetOrCreateZone("CombatInfo", boardLayer);
            zones[BattleScreenZone.PlayerBoard] = GetOrCreateZone("PlayerBoard", boardLayer);
            zones[BattleScreenZone.PlayerHudHand] = GetOrCreateZone("PlayerHudHand", hudLayer);

            zones[BattleScreenZone.EnemyDeck] = GetOrCreateZone("EnemyDeck", zones[BattleScreenZone.EnemyHud]);
            zones[BattleScreenZone.EnemyHealth] = GetOrCreateZone("EnemyHealth", zones[BattleScreenZone.EnemyHud]);
            zones[BattleScreenZone.TurnStatus] = GetOrCreateZone("TurnStatus", zones[BattleScreenZone.EnemyHud]);
            zones[BattleScreenZone.BattleLog] = GetOrCreateZone("BattleLog", zones[BattleScreenZone.EnemyHud]);

            zones[BattleScreenZone.EnemyTrapRow] = GetOrCreateZone("EnemyTrapRow", zones[BattleScreenZone.EnemyBoard]);
            zones[BattleScreenZone.EnemyCreatureRow] = GetOrCreateZone("EnemyCreatureRow", zones[BattleScreenZone.EnemyBoard]);
            zones[BattleScreenZone.EnemyMythic] = GetOrCreateZone("EnemyMythic", zones[BattleScreenZone.EnemyBoard]);

            zones[BattleScreenZone.PlayerCreatureRow] = GetOrCreateZone("PlayerCreatureRow", zones[BattleScreenZone.PlayerBoard]);
            zones[BattleScreenZone.PlayerTrapRow] = GetOrCreateZone("PlayerTrapRow", zones[BattleScreenZone.PlayerBoard]);
            zones[BattleScreenZone.PlayerMythic] = GetOrCreateZone("PlayerMythic", zones[BattleScreenZone.PlayerBoard]);

            zones[BattleScreenZone.PlayerDeck] = GetOrCreateZone("PlayerDeck", zones[BattleScreenZone.PlayerHudHand]);
            zones[BattleScreenZone.PlayerHealth] = GetOrCreateZone("PlayerHealth", zones[BattleScreenZone.PlayerHudHand]);
            zones[BattleScreenZone.PlayerHand] = GetOrCreateZone("PlayerHand", zones[BattleScreenZone.HandLayer]);
            zones[BattleScreenZone.TurnButton] = GetOrCreateZone("TurnButton", zones[BattleScreenZone.PlayerHudHand]);
        }

        private RectTransform GetOrCreateZone(string name, Transform parent)
        {
            Transform existing = parent.Find(name);
            if (existing != null && existing.TryGetComponent(out RectTransform existingRect))
            {
                return existingRect;
            }

            GameObject zoneObject = new GameObject(name);
            zoneObject.transform.SetParent(parent, false);

            RectTransform rect = zoneObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);
            return rect;
        }

        private void ApplyLayout()
        {
            if (root == null || zones.Count == 0) return;

            if (canvasScaler != null)
            {
                canvasScaler.referenceResolution = referenceResolution;
                canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
            }

            SetFull(zones[BattleScreenZone.SkinLayer]);
            SetFull(zones[BattleScreenZone.BoardLayer]);
            SetFull(zones[BattleScreenZone.HudLayer]);
            SetFull(zones[BattleScreenZone.HandLayer]);

            SetAnchors(zones[BattleScreenZone.EnemyHud], 0f, enemyHudMinY, 1f, 1f);
            SetAnchors(zones[BattleScreenZone.EnemyBoard], 0f, enemyBoardMinY, 1f, enemyHudMinY);
            SetAnchors(zones[BattleScreenZone.CombatInfo], 0f, combatMinY, 1f, enemyBoardMinY);
            SetAnchors(zones[BattleScreenZone.PlayerBoard], 0f, playerBoardMinY, 1f, combatMinY);
            SetAnchors(zones[BattleScreenZone.PlayerHudHand], 0f, 0f, 1f, bottomHudMaxY);

            SetAnchors(zones[BattleScreenZone.EnemyDeck], 0.020f, 0.15f, 0.090f, 0.92f);
            SetAnchors(zones[BattleScreenZone.EnemyHealth], 0.112f, 0.36f, 0.320f, 0.90f);
            SetAnchors(zones[BattleScreenZone.TurnStatus], 0.345f, 0.45f, 0.640f, 0.92f);
            SetAnchors(zones[BattleScreenZone.BattleLog], 0.655f, 0.34f, 0.905f, 0.84f);

            SetAnchors(zones[BattleScreenZone.EnemyTrapRow], boardLeft, enemyTrapMinY, boardCardsRight, enemyTrapMaxY);
            SetAnchors(zones[BattleScreenZone.EnemyCreatureRow], boardLeft, enemyCreatureMinY, boardCardsRight, enemyCreatureMaxY);
            SetAnchors(zones[BattleScreenZone.EnemyMythic], mythicLeft, 0.08f, mythicRight, 0.94f);

            SetAnchors(zones[BattleScreenZone.PlayerCreatureRow], boardLeft, playerCreatureMinY, boardCardsRight, playerCreatureMaxY);
            SetAnchors(zones[BattleScreenZone.PlayerTrapRow], boardLeft, playerTrapMinY, boardCardsRight, playerTrapMaxY);
            SetAnchors(zones[BattleScreenZone.PlayerMythic], mythicLeft, 0.08f, mythicRight, 0.94f);

            SetAnchors(zones[BattleScreenZone.PlayerDeck], bottomLeft, 0.16f, playerDeckRight, 0.88f);
            SetAnchors(zones[BattleScreenZone.PlayerHealth], playerHealthLeft, 0.34f, playerHealthRight, 0.82f);
            SetAnchors(zones[BattleScreenZone.PlayerHand], playerHandLeft, 0.00f, playerHandRight, 1f);
            SetAnchors(zones[BattleScreenZone.TurnButton], turnButtonLeft, 0.20f, turnButtonRight, 0.86f);

            if (showDebugZoneNames)
            {
                CreateMissingDebugLabels();
            }
        }

        private void HideLegacyObjectsIfNeeded()
        {
            if (legacyObjectsHidden) return;

            if (hideLegacyBoardVisual)
            {
                DisableObjectByName("BattleBoardVisual");
            }

            if (hideLegacyDebugHud)
            {
                DisableObjectByName("BattleDebugHUD");
            }

            legacyObjectsHidden = true;
        }

        private void DisableObjectByName(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName)) return;
            GameObject target = GameObject.Find(objectName);
            if (target != null && target != gameObject)
            {
                target.SetActive(false);
            }
        }

        private void SetFull(RectTransform rect)
        {
            SetAnchors(rect, 0f, 0f, 1f, 1f);
        }

        private void SetAnchors(RectTransform rect, float minX, float minY, float maxX, float maxY)
        {
            if (rect == null) return;

            minX = Mathf.Clamp01(minX);
            minY = Mathf.Clamp01(minY);
            maxX = Mathf.Clamp01(maxX);
            maxY = Mathf.Clamp01(maxY);

            if (maxX < minX) (minX, maxX) = (maxX, minX);
            if (maxY < minY) (minY, maxY) = (maxY, minY);

            rect.anchorMin = new Vector2(minX, minY);
            rect.anchorMax = new Vector2(maxX, maxY);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);
        }

        private void CreateMissingDebugLabels()
        {
            foreach (KeyValuePair<BattleScreenZone, RectTransform> entry in zones)
            {
                if (entry.Value == null || entry.Value.Find("DebugLabel") != null) continue;

                if (entry.Key == BattleScreenZone.Root ||
                    entry.Key == BattleScreenZone.SkinLayer ||
                    entry.Key == BattleScreenZone.BoardLayer ||
                    entry.Key == BattleScreenZone.HudLayer ||
                    entry.Key == BattleScreenZone.HandLayer)
                {
                    continue;
                }

                CreateDebugLabel(entry.Value, entry.Key.ToString());
            }
        }

        private void CreateDebugLabel(RectTransform parent, string text)
        {
            GameObject labelObject = new GameObject("DebugLabel");
            labelObject.transform.SetParent(parent, false);

            RectTransform rect = labelObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Text label = labelObject.AddComponent<Text>();
            label.font = defaultFont;
            label.fontSize = 16;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color(1f, 1f, 1f, 0.20f);
            label.text = text;
            label.raycastTarget = false;
        }
    }
}
