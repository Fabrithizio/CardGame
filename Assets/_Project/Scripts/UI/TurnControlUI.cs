// Caminho: Assets/_Project/Scripts/UI/TurnControlUI.cs
// Descrição: Botão principal do turno, mais compacto e centralizado dentro da zona TurnButton.

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
        [SerializeField] private Vector2 maxButtonSize = new Vector2(150f, 150f);

        [Header("Texto")]
        [SerializeField] private int fontSize = 18;

        [Header("Cores")]
        [SerializeField] private Color buttonColor = new Color(0.10f, 0.42f, 0.86f, 0.96f);
        [SerializeField] private Color disabledColor = new Color(0.18f, 0.18f, 0.20f, 0.68f);

        private RectTransform zoneRect;
        private RectTransform buttonRect;
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

            defaultFont = ResponsiveUIUtility.GetDefaultFont();
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
            if (!Application.isPlaying)
            {
                return;
            }

            ApplyRuntimeLayout();
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

            Image background = buttonObject.AddComponent<Image>();
            background.color = buttonColor;

            Outline outline = buttonObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.75f);
            outline.effectDistance = new Vector2(3f, -3f);

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
            size = Mathf.Max(76f, size);

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

            if (nextPhaseButton.targetGraphic is Image image)
            {
                image.color = isPlayerTurn ? buttonColor : disabledColor;
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
                BattlePhase.StartTurn => "Principal",
                BattlePhase.Draw => "Principal",
                BattlePhase.Main => "Batalha",
                BattlePhase.Battle => "Encerrar",
                BattlePhase.EndTurn => "Passar",
                BattlePhase.Finished => "Fim",
                _ => "Principal"
            };
        }
    }
}
