// Caminho: Assets/_Project/Scripts/UI/TurnControlUI.cs
// Descrição: Botão principal do turno com formato circular provisório e aparência mais próxima da referência.

using CardGame.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class TurnControlUI : MonoBehaviour
    {
        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;

        [Header("Layout")]
        [SerializeField] private bool updateLayoutEveryFrame = true;
        [SerializeField] private Vector2 maxButtonSize = new Vector2(164f, 164f);

        [Header("Texto")]
        [SerializeField] private int fontSize = 20;

        [Header("Cores")]
        [SerializeField] private Color outerColor = new Color(0.88f, 0.68f, 0.30f, 1f);
        [SerializeField] private Color buttonColor = new Color(0.04f, 0.36f, 0.92f, 0.96f);
        [SerializeField] private Color disabledColor = new Color(0.18f, 0.18f, 0.20f, 0.68f);

        private RectTransform zoneRect;
        private RectTransform buttonRect;
        private RectTransform innerRect;
        private GameObject buttonObject;
        private Button nextPhaseButton;
        private Image innerImage;
        private Text nextPhaseButtonText;
        private Font defaultFont;
        private Sprite circleSprite;

        private void Awake()
        {
            if (battleManager == null)
            {
                battleManager = FindFirstObjectByType<BattleManager>();
            }

            defaultFont = ResponsiveUIUtility.GetDefaultFont();
            circleSprite = CreateCircleSprite();
            CreateButton();
        }

        private void Update()
        {
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

        private void CreateButton()
        {
            BattleScreenLayoutUI layout = BattleScreenLayoutUI.GetOrCreate();
            zoneRect = layout.GetZone(BattleScreenZone.TurnButton);

            buttonObject = new GameObject("Next Phase Button");
            buttonObject.transform.SetParent(zoneRect, false);

            buttonRect = buttonObject.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRect.pivot = new Vector2(0.5f, 0.5f);

            Image outer = buttonObject.AddComponent<Image>();
            outer.sprite = circleSprite;
            outer.preserveAspect = true;
            outer.color = outerColor;

            Outline outline = buttonObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.80f);
            outline.effectDistance = new Vector2(4f, -4f);

            nextPhaseButton = buttonObject.AddComponent<Button>();
            nextPhaseButton.targetGraphic = outer;
            nextPhaseButton.onClick.AddListener(HandleNextPhaseClicked);

            GameObject innerObject = new GameObject("Inner");
            innerObject.transform.SetParent(buttonObject.transform, false);

            innerRect = innerObject.AddComponent<RectTransform>();
            innerRect.anchorMin = new Vector2(0.13f, 0.13f);
            innerRect.anchorMax = new Vector2(0.87f, 0.87f);
            innerRect.offsetMin = Vector2.zero;
            innerRect.offsetMax = Vector2.zero;

            innerImage = innerObject.AddComponent<Image>();
            innerImage.sprite = circleSprite;
            innerImage.preserveAspect = true;
            innerImage.color = buttonColor;
            innerImage.raycastTarget = false;

            Shadow innerShadow = innerObject.AddComponent<Shadow>();
            innerShadow.effectColor = new Color(0.28f, 0.75f, 1f, 0.35f);
            innerShadow.effectDistance = new Vector2(0f, 0f);

            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(innerObject.transform, false);

            nextPhaseButtonText = textObject.AddComponent<Text>();
            nextPhaseButtonText.font = defaultFont;
            nextPhaseButtonText.fontSize = fontSize;
            nextPhaseButtonText.fontStyle = FontStyle.Bold;
            nextPhaseButtonText.alignment = TextAnchor.MiddleCenter;
            nextPhaseButtonText.color = Color.white;
            nextPhaseButtonText.resizeTextForBestFit = true;
            nextPhaseButtonText.resizeTextMinSize = 10;
            nextPhaseButtonText.resizeTextMaxSize = fontSize;
            nextPhaseButtonText.text = "Principal";

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8f, 8f);
            textRect.offsetMax = new Vector2(-8f, -8f);

            ApplyRuntimeLayout();
        }

        private void ApplyRuntimeLayout()
        {
            if (buttonRect == null || zoneRect == null)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();

            float size = Mathf.Min(zoneRect.rect.width, zoneRect.rect.height, maxButtonSize.x, maxButtonSize.y);
            size = Mathf.Max(82f, size);

            buttonRect.sizeDelta = new Vector2(size, size);
            buttonRect.anchoredPosition = Vector2.zero;
        }

        private void HandleNextPhaseClicked()
        {
            if (battleManager == null || battleManager.TurnManager == null)
            {
                return;
            }

            battleManager.GoToNextPhase();
        }

        private void Refresh()
        {
            if (battleManager == null || battleManager.TurnManager == null || nextPhaseButton == null || buttonObject == null)
            {
                return;
            }

            if (battleManager.TurnManager.CurrentPhase == BattlePhase.Finished)
            {
                buttonObject.SetActive(false);
                return;
            }

            buttonObject.SetActive(true);

            bool isPlayerTurn = battleManager.TurnManager.IsPlayerTurn;
            nextPhaseButton.interactable = isPlayerTurn;

            if (innerImage != null)
            {
                innerImage.color = isPlayerTurn ? buttonColor : disabledColor;
            }

            nextPhaseButtonText.text = GetButtonText();
        }

        private string GetButtonText()
        {
            if (battleManager == null || battleManager.TurnManager == null)
            {
                return "Principal";
            }

            BattlePhase phase = battleManager.TurnManager.CurrentPhase;

            return phase switch
            {
                BattlePhase.StartTurn => "PRINCIPAL",
                BattlePhase.Draw => "PRINCIPAL",
                BattlePhase.Main => "BATALHA",
                BattlePhase.Battle => "ENCERRAR",
                BattlePhase.EndTurn => "PASSAR",
                BattlePhase.Finished => "FIM",
                _ => "PRINCIPAL"
            };
        }

        private Sprite CreateCircleSprite()
        {
            const int size = 96;

            Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Bilinear;

            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size * 0.46f;
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
    }
}
