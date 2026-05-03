// Caminho: Assets/_Project/Scripts/Mythics/MythicEffectType.cs
// Descrição: Define os tipos de efeitos que um Mítico pode executar durante a partida.

namespace CardGame.Mythics
{
    public enum MythicEffectType
    {
        None = 0,

        GiveShieldToCreature = 1,
        BuffCreatureAttack = 2,
        BuffCreatureSpeed = 3,
        HealCreature = 4,
        ReviveCreature = 5,
        DamageEnemyPlayer = 6
    }
}