// Caminho: Assets/_Project/Scripts/UI/TurnControlUI.cs
// Descrição: Cria botões temporários de controle de turno/fase para mobile. O botão some quando a batalha termina.

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
        [SerializeField] private Vector2 buttonSize = new Vector2(170f, 52f);
        [SerializeField] private Vector2 anchorPosition = new Vector2(-26f, 36f);

        [Header("Texto")]
        [SerializeField] private int fontSize = 18;

        [Header("Cores")]
        [SerializeField] private Color buttonColor = new Color(0.12f, 0.42f, 0.78f, 0.95f);
        [SerializeField] private Color disabledColor = new Color(0.12f, 0.12f, 0.12f, 0.55f);

        private Canvas canvas;
        private RectTransform root;
        private GameObject buttonObject;
        private Button nextPhaseButton;
        private Text nextPhaseButtonText;
        private Font defaultFont;

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
            CreateButtons();
        }

        private void Update()
        {
            Refresh();
        }

        private void CreateCanvas()
        {
            GameObject canvasObject = new GameObject("Turn Control UI Canvas");
            canvasObject.transform.SetParent(transform, false);

            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;

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

        private void CreateButtons()
        {
            buttonObject = new GameObject("Next Phase Button");
            buttonObject.transform.SetParent(root, false);

            RectTransform rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.anchoredPosition = anchorPosition;
            rect.sizeDelta = buttonSize;

            Image background = buttonObject.AddComponent<Image>();
            background.color = buttonColor;

            nextPhaseButton = buttonObject.AddComponent<Button>();
            nextPhaseButton.targetGraphic = background;
            nextPhaseButton.onClick.AddListener(HandleNextPhaseClicked);

            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(buttonObject.transform, false);

            nextPhaseButtonText = textObject.AddComponent<Text>();
            nextPhaseButtonText.font = defaultFont;
            nextPhaseButtonText.fontSize = fontSize;
            nextPhaseButtonText.fontStyle = FontStyle.Bold;
            nextPhaseButtonText.alignment = TextAnchor.MiddleCenter;
            nextPhaseButtonText.color = Color.white;
            nextPhaseButtonText.text = "Avançar";

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
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
            if (battleManager == null || battleManager.TurnManager == null || nextPhaseButton == null)
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

            Image image = nextPhaseButton.targetGraphic as Image;

            if (image != null)
            {
                image.color = isPlayerTurn ? buttonColor : disabledColor;
            }

            nextPhaseButtonText.text = GetButtonText();
        }

        private string GetButtonText()
        {
            if (battleManager == null || battleManager.TurnManager == null)
            {
                return "Avançar";
            }

            BattlePhase phase = battleManager.TurnManager.CurrentPhase;

            return phase switch
            {
                BattlePhase.StartTurn => "Comprar",
                BattlePhase.Draw => "Principal",
                BattlePhase.Main => "Batalha",
                BattlePhase.Battle => "Encerrar",
                BattlePhase.EndTurn => "Passar",
                BattlePhase.Finished => "Fim",
                _ => "Avançar"
            };
        }
    }
}