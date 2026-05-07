// Caminho: Assets/_Project/Scripts/Effects/TrapEffectType.cs
// Descrição: Define os efeitos possíveis de uma armadilha preparada no campo.

namespace CardGame.Effects
{
    public enum TrapEffectType
    {
        None = 0,

        BlockNextAttack = 1,
        DealDamageToAttacker = 2,
        GiveShieldToDefender = 3,
        BuffDefenderDefense = 4,
        DebuffAttackerAttack = 5
    }
}
