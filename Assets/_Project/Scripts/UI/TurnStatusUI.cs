// Caminho: Assets/_Project/Scripts/UI/TurnStatusUI.cs
// Descrição: Banner central do turno dentro da zona TurnStatus.

using CardGame.Battle;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class TurnStatusUI : MonoBehaviour
    {
        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;

        [Header("Texto")]
        [SerializeField] private int fontSize = 16;

        [Header("Cores")]
        [SerializeField] private Color playerTurnColor = new Color(0.10f, 0.32f, 0.72f, 0.92f);
        [SerializeField] private Color enemyTurnColor = new Color(0.62f, 0.16f, 0.18f, 0.92f);
        [SerializeField] private Color finishedColor = new Color(0.12f, 0.12f, 0.12f, 0.92f);

        private Image panelBackground;
        private Text statusText;
        private Font defaultFont;

        private void Awake()
        {
            if (battleManager == null)
            {
                battleManager = FindFirstObjectByType<BattleManager>();
            }

            defaultFont = ResponsiveUIUtility.GetDefaultFont();
            CreatePanel();
        }

        private void Update()
        {
            Refresh();
        }

        private void CreatePanel()
        {
            BattleScreenLayoutUI layout = BattleScreenLayoutUI.GetOrCreate();

            GameObject panelObject = new GameObject("Turn Status Panel");
            panelObject.transform.SetParent(layout.GetZone(BattleScreenZone.TurnStatus), false);

            RectTransform rect = panelObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            panelBackground = panelObject.AddComponent<Image>();
            panelBackground.color = playerTurnColor;

            Outline outline = panelObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.60f);
            outline.effectDistance = new Vector2(2f, -2f);

            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(panelObject.transform, false);

            statusText = textObject.AddComponent<Text>();
            statusText.font = defaultFont;
            statusText.fontSize = fontSize;
            statusText.fontStyle = FontStyle.Bold;
            statusText.alignment = TextAnchor.MiddleCenter;
            statusText.color = Color.white;
            statusText.horizontalOverflow = HorizontalWrapMode.Wrap;
            statusText.verticalOverflow = VerticalWrapMode.Truncate;
            statusText.resizeTextForBestFit = true;
            statusText.resizeTextMinSize = 10;
            statusText.resizeTextMaxSize = fontSize;
            statusText.text = "Turno";

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8f, 6f);
            textRect.offsetMax = new Vector2(-8f, -6f);
        }

        private void Refresh()
        {
            if (battleManager == null || battleManager.TurnManager == null)
            {
                return;
            }

            BattlePhase phase = battleManager.TurnManager.CurrentPhase;

            if (phase == BattlePhase.Finished)
            {
                panelBackground.color = finishedColor;
                statusText.text = "BATALHA FINALIZADA";
                return;
            }

            bool isPlayerTurn = battleManager.TurnManager.IsPlayerTurn;
            panelBackground.color = isPlayerTurn ? playerTurnColor : enemyTurnColor;

            string activeName = isPlayerTurn ? "SEU TURNO" : "TURNO DO INIMIGO";
            statusText.text = $"{activeName}\n{GetPhaseText(phase)}";
        }

        private string GetPhaseText(BattlePhase phase)
        {
            return phase switch
            {
                BattlePhase.StartTurn => "Início do turno",
                BattlePhase.Draw => "Compra",
                BattlePhase.Main => "Principal",
                BattlePhase.Battle => "Batalha",
                BattlePhase.EndTurn => "Fim do turno",
                BattlePhase.Finished => "Finalizada",
                _ => "Preparando"
            };
        }
    }
}
