// Caminho: Assets/_Project/Scripts/Battle/BattlePhase.cs
// Descrição: Define as fases principais de um turno durante a batalha.

namespace CardGame.Battle
{
    public enum BattlePhase
    {
        None = 0,

        StartTurn = 1,

        Draw = 2,

        Main = 3,

        Battle = 4,

        EndTurn = 5,

        Finished = 6
    }
}