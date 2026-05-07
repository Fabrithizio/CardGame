// Caminho: Assets/_Project/Scripts/Battle/TurnManager.cs
// Descrição: Controla o fluxo de turnos e fases da batalha. A compra de carta acontece no começo do turno, então StartTurn avança direto para Main.

using System;
using UnityEngine;

namespace CardGame.Battle
{
    [Serializable]
    public class TurnManager
    {
        [SerializeField] private int currentTurnNumber;
        [SerializeField] private int activePlayerIndex;
        [SerializeField] private BattlePhase currentPhase;

        public int CurrentTurnNumber => currentTurnNumber;
        public int ActivePlayerIndex => activePlayerIndex;
        public BattlePhase CurrentPhase => currentPhase;

        public bool IsPlayerTurn => activePlayerIndex == 0;
        public bool IsEnemyTurn => activePlayerIndex == 1;

        public event Action<int> OnTurnStarted;
        public event Action<int> OnTurnEnded;
        public event Action<BattlePhase> OnPhaseChanged;

        public TurnManager()
        {
            currentTurnNumber = 0;
            activePlayerIndex = 0;
            currentPhase = BattlePhase.None;
        }

        public void StartBattle()
        {
            currentTurnNumber = 1;
            activePlayerIndex = 0;
            SetPhase(BattlePhase.StartTurn);
            OnTurnStarted?.Invoke(activePlayerIndex);
        }

        public void GoToNextPhase()
        {
            switch (currentPhase)
            {
                case BattlePhase.None:
                    SetPhase(BattlePhase.StartTurn);
                    OnTurnStarted?.Invoke(activePlayerIndex);
                    break;

                case BattlePhase.StartTurn:
                    SetPhase(BattlePhase.Main);
                    break;

                case BattlePhase.Draw:
                    SetPhase(BattlePhase.Main);
                    break;

                case BattlePhase.Main:
                    SetPhase(BattlePhase.Battle);
                    break;

                case BattlePhase.Battle:
                    SetPhase(BattlePhase.EndTurn);
                    break;

                case BattlePhase.EndTurn:
                    EndCurrentTurnAndStartNext();
                    break;

                case BattlePhase.Finished:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(currentPhase), currentPhase, null);
            }
        }

        public void GoToMainPhase()
        {
            SetPhase(BattlePhase.Main);
        }

        public void GoToBattlePhase()
        {
            SetPhase(BattlePhase.Battle);
        }

        public void FinishBattle()
        {
            SetPhase(BattlePhase.Finished);
        }

        private void EndCurrentTurnAndStartNext()
        {
            OnTurnEnded?.Invoke(activePlayerIndex);

            activePlayerIndex = activePlayerIndex == 0 ? 1 : 0;

            if (activePlayerIndex == 0)
            {
                currentTurnNumber++;
            }

            SetPhase(BattlePhase.StartTurn);
            OnTurnStarted?.Invoke(activePlayerIndex);
        }

        private void SetPhase(BattlePhase newPhase)
        {
            if (currentPhase == newPhase)
            {
                return;
            }

            currentPhase = newPhase;
            OnPhaseChanged?.Invoke(currentPhase);
        }
    }
}
