// Caminho: Assets/_Project/Scripts/UI/BattleBoardSkinUI.cs
// Descrição: Fundo visual das zonas do tabuleiro, agora preso ao BattleScreenLayoutUI.

using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UI
{
    public class BattleBoardSkinUI : MonoBehaviour
    {
        [Header("Cores")]
        [SerializeField] private Color enemyBoardColor = new Color(0.04f, 0.05f, 0.12f, 0.64f);
        [SerializeField] private Color playerBoardColor = new Color(0.03f, 0.08f, 0.18f, 0.66f);
        [SerializeField] private Color trapColor = new Color(0.02f, 0.03f, 0.07f, 0.68f);
        [SerializeField] private Color combatColor = new Color(0.02f, 0.04f, 0.10f, 0.88f);
        [SerializeField] private Color bottomColor = new Color(0.02f, 0.06f, 0.14f, 0.96f);
        [SerializeField] private Color lineColor = new Color(0.95f, 0.78f, 0.36f, 0.22f);

        [Header("Texto")]
        [SerializeField] private int labelFontSize = 13;

        private Font defaultFont;

        private void Awake()
        {
            defaultFont = ResponsiveUIUtility.GetDefaultFont();
            Build();
        }

        private void Build()
        {
            BattleScreenLayoutUI layout = BattleScreenLayoutUI.GetOrCreate();

            CreatePanel(layout.GetZone(BattleScreenZone.EnemyBoard), "Enemy Board Background", enemyBoardColor, "CAMPO INIMIGO");
            CreatePanel(layout.GetZone(BattleScreenZone.EnemyTrapRow), "Enemy Trap Background", trapColor, string.Empty);
            CreatePanel(layout.GetZone(BattleScreenZone.CombatInfo), "Combat Background", combatColor, "ÁREA DE COMBATE");
            CreatePanel(layout.GetZone(BattleScreenZone.PlayerBoard), "Player Board Background", playerBoardColor, "SEU CAMPO");
            CreatePanel(layout.GetZone(BattleScreenZone.PlayerTrapRow), "Player Trap Background", trapColor, string.Empty);
            CreatePanel(layout.GetZone(BattleScreenZone.PlayerHudHand), "Bottom Background", bottomColor, string.Empty);
        }

        private void CreatePanel(RectTransform parent, string objectName, Color color, string label)
        {
            GameObject obj = new GameObject(objectName);
            obj.transform.SetParent(parent, false);

            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = obj.AddComponent<Image>();
            image.color = color;

            Outline outline = obj.AddComponent<Outline>();
            outline.effectColor = lineColor;
            outline.effectDistance = new Vector2(2f, -2f);

            if (!string.IsNullOrWhiteSpace(label))
            {
                CreateLabel(obj.transform, label);
            }

            obj.transform.SetAsFirstSibling();
        }

        private void CreateLabel(Transform parent, string label)
        {
            GameObject labelObject = new GameObject("Label");
            labelObject.transform.SetParent(parent, false);

            RectTransform rect = labelObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0.86f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.offsetMin = new Vector2(12f, 0f);
            rect.offsetMax = new Vector2(-12f, 0f);

            Text text = labelObject.AddComponent<Text>();
            text.font = defaultFont;
            text.fontSize = labelFontSize;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(1f, 1f, 1f, 0.42f);
            text.text = label;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 8;
            text.resizeTextMaxSize = labelFontSize;
        }
    }
}
