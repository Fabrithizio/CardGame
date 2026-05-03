// Caminho: Assets/_Project/Scripts/Cards/CardStatusType.cs
// Descrição: Define os estados visuais e mecânicos que uma carta pode exibir durante a partida.

namespace CardGame.Cards
{
    public enum CardStatusType
    {
        None = 0,

        Shielded = 1,
        Taunting = 2,
        Stealthed = 3,
        Piercing = 4,
        LifeStealing = 5,
        Retaliating = 6,
        DirectAttacker = 7,

        BuffedAttack = 20,
        BuffedDefense = 21,
        BuffedSpeed = 22,
        BuffedFocus = 23,
        BuffedResistance = 24,

        DebuffedAttack = 40,
        DebuffedDefense = 41,
        DebuffedSpeed = 42,
        DebuffedFocus = 43,
        DebuffedResistance = 44,

        SpeedTier1 = 60,
        SpeedTier2 = 61,

        AttackTier1 = 70,
        AttackTier2 = 71,

        DefenseTier1 = 80,
        DefenseTier2 = 81,

        FocusTier1 = 90,
        FocusTier2 = 91,

        ResistanceTier1 = 100,
        ResistanceTier2 = 101
    }
}