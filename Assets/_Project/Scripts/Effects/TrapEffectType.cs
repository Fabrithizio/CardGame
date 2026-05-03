// Caminho: Assets/_Project/Scripts/Effects/TrapEffectType.cs
// Descrição: Define os tipos de efeitos que uma carta de armadilha pode executar durante a batalha.
// Observação: Este arquivo é a base para começarmos a criar armadilhas reais, como bloquear ataque, causar dano ou aplicar penalidades.

namespace CardGame.Effects
{
    public enum TrapEffectType
    {
        None = 0,

        BlockNextAttack = 1,
        ReduceIncomingDamage = 2,
        DamageAttacker = 3,
        WeakenAttacker = 4,
        StunAttacker = 5,
        GiveShieldToDefender = 6
    }
}
