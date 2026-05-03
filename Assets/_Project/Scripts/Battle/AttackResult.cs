// Caminho: Assets/_Project/Scripts/Battle/AttackResult.cs
// Descrição: Guarda o resultado completo de um ataque, incluindo múltiplos golpes separados por Speed, dano real e escudos consumidos.

using System.Collections.Generic;
using CardGame.Cards;

namespace CardGame.Battle
{
    public readonly struct AttackHitResult
    {
        public int HitNumber { get; }
        public int AttemptedDamageToDefender { get; }
        public int AttemptedDamageToAttacker { get; }
        public int ActualDamageToDefender { get; }
        public int ActualDamageToAttacker { get; }
        public bool DefenderShieldConsumed { get; }
        public bool AttackerShieldConsumed { get; }
        public bool DefenderDestroyedAfterHit { get; }
        public bool AttackerDestroyedAfterHit { get; }

        public AttackHitResult(
            int hitNumber,
            int attemptedDamageToDefender,
            int attemptedDamageToAttacker,
            int actualDamageToDefender,
            int actualDamageToAttacker,
            bool defenderShieldConsumed,
            bool attackerShieldConsumed,
            bool defenderDestroyedAfterHit,
            bool attackerDestroyedAfterHit)
        {
            HitNumber = hitNumber;
            AttemptedDamageToDefender = attemptedDamageToDefender;
            AttemptedDamageToAttacker = attemptedDamageToAttacker;
            ActualDamageToDefender = actualDamageToDefender;
            ActualDamageToAttacker = actualDamageToAttacker;
            DefenderShieldConsumed = defenderShieldConsumed;
            AttackerShieldConsumed = attackerShieldConsumed;
            DefenderDestroyedAfterHit = defenderDestroyedAfterHit;
            AttackerDestroyedAfterHit = attackerDestroyedAfterHit;
        }
    }

    public readonly struct AttackResult
    {
        public CardRuntime Attacker { get; }
        public CardRuntime Defender { get; }

        public int TotalHitsAttempted { get; }
        public int TotalHitsResolved { get; }

        public int TotalActualDamageToDefender { get; }
        public int TotalActualDamageToAttacker { get; }

        public bool DefenderDestroyed { get; }
        public bool AttackerDestroyed { get; }

        public IReadOnlyList<AttackHitResult> Hits { get; }

        public AttackResult(
            CardRuntime attacker,
            CardRuntime defender,
            int totalHitsAttempted,
            int totalHitsResolved,
            int totalActualDamageToDefender,
            int totalActualDamageToAttacker,
            bool defenderDestroyed,
            bool attackerDestroyed,
            IReadOnlyList<AttackHitResult> hits)
        {
            Attacker = attacker;
            Defender = defender;
            TotalHitsAttempted = totalHitsAttempted;
            TotalHitsResolved = totalHitsResolved;
            TotalActualDamageToDefender = totalActualDamageToDefender;
            TotalActualDamageToAttacker = totalActualDamageToAttacker;
            DefenderDestroyed = defenderDestroyed;
            AttackerDestroyed = attackerDestroyed;
            Hits = hits;
        }
    }
}