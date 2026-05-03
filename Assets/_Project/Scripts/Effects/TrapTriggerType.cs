// Caminho: Assets/_Project/Scripts/Effects/TrapTriggerType.cs
// Descrição: Define quando uma armadilha pode ser ativada durante a batalha.

namespace CardGame.Effects
{
    public enum TrapTriggerType
    {
        None = 0,

        BeforeEnemyAttack = 1,
        AfterEnemyAttack = 2,
        WhenCreatureTakesDamage = 3,
        WhenPlayerTakesDirectDamage = 4,
        OnTurnStart = 5,
        OnTurnEnd = 6
    }
}
