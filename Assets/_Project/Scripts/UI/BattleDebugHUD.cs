// Caminho: Assets/_Project/Scripts/UI/BattleDebugHUD.cs
// Descrição: Mostra um HUD temporário de debug recolhível para acompanhar vida, turno, fase, deck, mão, campo, status das criaturas e Míticos durante os testes.

using System.Collections.Generic;
using CardGame.Battle;
using CardGame.Cards;
using CardGame.Mythics;
using UnityEngine;

namespace CardGame.UI
{
    public class BattleDebugHUD : MonoBehaviour
    {
        [Header("Referência")]
        [SerializeField] private BattleManager battleManager;

        [Header("Estado")]
        [SerializeField] private bool startExpanded = false;

        [Header("Visual")]
        [SerializeField] private int panelWidth = 460;
        [SerializeField] private int panelHeight = 560;
        [SerializeField] private int margin = 12;

        private GUIStyle titleStyle;
        private GUIStyle normalStyle;
        private GUIStyle smallStyle;
        private GUIStyle buttonStyle;

        private Vector2 scrollPosition;
        private bool stylesInitialized;
        private bool isExpanded;

        private void Awake()
        {
            if (battleManager == null)
            {
                battleManager = FindFirstObjectByType<BattleManager>();
            }

            isExpanded = startExpanded;
        }

        private void OnGUI()
        {
            InitializeStylesIfNeeded();

            DrawToggleButton();

            if (!isExpanded)
            {
                return;
            }

            if (battleManager == null || battleManager.PlayerState == null || battleManager.EnemyState == null)
            {
                return;
            }

            Rect panelRect = new Rect(margin, margin + 44, panelWidth, panelHeight);
            GUI.Box(panelRect, string.Empty);

            GUILayout.BeginArea(new Rect(margin + 12, margin + 54, panelWidth - 24, panelHeight - 20));

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label("CARD GAME - DEBUG HUD", titleStyle);
            GUILayout.Space(8);

            DrawTurnInfo();
            GUILayout.Space(10);

            DrawPlayerInfo("Jogador", battleManager.PlayerState, true);
            GUILayout.Space(8);

            DrawBoardInfo("Campo do Jogador", battleManager.PlayerState);
            GUILayout.Space(14);

            DrawPlayerInfo("Inimigo", battleManager.EnemyState, false);
            GUILayout.Space(8);

            DrawBoardInfo("Campo do Inimigo", battleManager.EnemyState);
            GUILayout.Space(14);

            DrawButtons();

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void InitializeStylesIfNeeded()
        {
            if (stylesInitialized)
            {
                return;
            }

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                normal =
                {
                    textColor = Color.white
                }
            };

            normalStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                normal =
                {
                    textColor = Color.white
                }
            };

            smallStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal =
                {
                    textColor = Color.white
                }
            };

            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 14
            };

            stylesInitialized = true;
        }

        private void DrawToggleButton()
        {
            Rect buttonRect = new Rect(margin, margin, 130, 34);

            string buttonText = isExpanded ? "Fechar Debug" : "Abrir Debug";

            if (GUI.Button(buttonRect, buttonText, buttonStyle))
            {
                isExpanded = !isExpanded;
            }
        }

        private void DrawTurnInfo()
        {
            string phase = battleManager.TurnManager != null
                ? battleManager.TurnManager.CurrentPhase.ToString()
                : "Sem fase";

            string activePlayer = battleManager.TurnManager != null && battleManager.TurnManager.IsEnemyTurn
                ? "Inimigo"
                : "Jogador";

            GUILayout.Label($"Turno ativo: {activePlayer}", normalStyle);
            GUILayout.Label($"Fase: {phase}", normalStyle);
        }

        private void DrawPlayerInfo(string label, PlayerBattleState state, bool showMythicNames)
        {
            GUILayout.Label(
                $"{label}: Vida {state.CurrentHealth}/{state.MaxHealth} | Deck {state.Deck.CardsRemaining} | Mão {state.Hand.Count} | Criaturas {state.Board.GetAliveCreatures().Count}",
                normalStyle
            );

            GUILayout.Label($"Míticos: {GetMythicText(state.MythicLoadout, showMythicNames)}", normalStyle);
        }

        private void DrawBoardInfo(string title, PlayerBattleState state)
        {
            GUILayout.Label(title, normalStyle);

            IReadOnlyList<CardRuntime> slots = state.Board.CreatureSlots;

            for (int i = 0; i < slots.Count; i++)
            {
                CardRuntime card = slots[i];

                if (card == null)
                {
                    GUILayout.Label($"Slot {i + 1}: vazio", smallStyle);
                    continue;
                }

                GUILayout.Label(
                    $"Slot {i + 1}: {card.CardName} | HP {card.CurrentHealth} | ATK {card.CurrentAttack} | SPD {card.CurrentSpeed} | DEF {card.CurrentDefense}",
                    smallStyle
                );

                string statusText = GetStatusText(card);

                if (!string.IsNullOrWhiteSpace(statusText))
                {
                    GUILayout.Label($"  Status: {statusText}", smallStyle);
                }
            }
        }

        private string GetStatusText(CardRuntime card)
        {
            if (card == null)
            {
                return string.Empty;
            }

            List<CardStatusType> statuses = card.GetCurrentStatuses();

            if (statuses.Count <= 0)
            {
                return "nenhum";
            }

            return string.Join(", ", statuses);
        }

        private string GetMythicText(MythicLoadoutRuntime loadout, bool showNames)
        {
            if (loadout == null)
            {
                return "nenhum";
            }

            string text = string.Empty;

            for (int i = 0; i < loadout.TotalSlots; i++)
            {
                MythicRuntime mythic = loadout.GetMythicAt(i);

                if (i > 0)
                {
                    text += " | ";
                }

                if (mythic == null)
                {
                    text += "[Vazio]";
                    continue;
                }

                if (mythic.IsUsed)
                {
                    text += "[Usado]";
                    continue;
                }

                if (!showNames && !mythic.IsRevealed)
                {
                    text += "[???]";
                    continue;
                }

                text += $"[{mythic.MythicName}]";
            }

            return text;
        }

        private void DrawButtons()
        {
            GUILayout.Label("Comandos rápidos:", normalStyle);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Próxima fase (N)", buttonStyle))
            {
                battleManager.GoToNextPhase();
            }

            if (GUILayout.Button("Comprar (D)", buttonStyle))
            {
                battleManager.ActivePlayerDrawCard();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Colocar criatura (C)", buttonStyle))
            {
                battleManager.ActivePlayerPlayFirstCreature();
            }

            if (GUILayout.Button("Atacar (A)", buttonStyle))
            {
                battleManager.ActivePlayerAttackFirstEnemyCreatureOrDirect();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Buff Speed (S)", buttonStyle))
            {
                battleManager.ActivePlayerBuffFirstCreatureSpeed();
            }

            if (GUILayout.Button("Escudo (H)", buttonStyle))
            {
                battleManager.ActivePlayerGiveShieldToFirstCreature();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Usar Mítico (M)", buttonStyle))
            {
                battleManager.ActivePlayerUseFirstAvailableMythic();
            }

            GUILayout.EndHorizontal();
        }
    }
}