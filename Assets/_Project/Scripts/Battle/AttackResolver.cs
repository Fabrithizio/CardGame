// Caminho: Assets/_Project/Scripts/Battle/AttackResolver.cs
// Descrição: Resolve ataques entre criaturas, aplicando golpes múltiplos por Speed como ataques separados e detectando escudos consumidos.

using System.Collections.Generic;
using CardGame.Cards;
using UnityEngine;

namespace CardGame.Battle
{
    public static class AttackResolver
    {
        public static bool TryResolveCreatureAttack(
            CardRuntime attacker,
            CardRuntime defender,
            out AttackResult result)
        {
            result = default;

            if (attacker == null)
            {
                Debug.LogWarning("Ataque cancelado: atacante nulo.");
                return false;
            }

            if (defender == null)
            {
                Debug.LogWarning("Ataque cancelado: defensor nulo.");
                return false;
            }

            if (!attacker.IsAlive)
            {
                Debug.LogWarning($"Ataque cancelado: {attacker.CardName} não está vivo.");
                return false;
            }

            if (!defender.IsAlive)
            {
                Debug.LogWarning($"Ataque cancelado: {defender.CardName} não está vivo.");
                return false;
            }

            if (attacker.HasAttackedThisTurn)
            {
                Debug.LogWarning($"Ataque cancelado: {attacker.CardName} já atacou neste turno.");
                return false;
            }

            int totalHitsAttempted = attacker.GetAttackCountBySpeed();
            int totalHitsResolved = 0;
            int totalActualDamageToDefender = 0;
            int totalActualDamageToAttacker = 0;

            List<AttackHitResult> hits = new();

            for (int hitIndex = 0; hitIndex < totalHitsAttempted; hitIndex++)
            {
                if (!attacker.IsAlive || !defender.IsAlive)
                {
                    break;
                }

                int attemptedDamageToDefender = Mathf.Max(0, attacker.CurrentAttack);
                int attemptedDamageToAttacker = Mathf.Max(0, defender.CurrentAttack);

                int defenderHealthBefore = defender.CurrentHealth;
                int attackerHealthBefore = attacker.CurrentHealth;

                bool defenderHadShield = defender.HasTemporaryShield;
                bool attackerHadShield = attacker.HasTemporaryShield;

                defender.TakeDamage(attemptedDamageToDefender);
                attacker.TakeDamage(attemptedDamageToAttacker);

                bool defenderShieldConsumed = defenderHadShield && !defender.HasTemporaryShield;
                bool attackerShieldConsumed = attackerHadShield && !attacker.HasTemporaryShield;

                int actualDamageToDefender = Mathf.Max(0, defenderHealthBefore - defender.CurrentHealth);
                int actualDamageToAttacker = Mathf.Max(0, attackerHealthBefore - attacker.CurrentHealth);

                totalHitsResolved++;
                totalActualDamageToDefender += actualDamageToDefender;
                totalActualDamageToAttacker += actualDamageToAttacker;

                hits.Add(new AttackHitResult(
                    hitIndex + 1,
                    attemptedDamageToDefender,
                    attemptedDamageToAttacker,
                    actualDamageToDefender,
                    actualDamageToAttacker,
                    defenderShieldConsumed,
                    attackerShieldConsumed,
                    defender.IsDestroyed,
                    attacker.IsDestroyed
                ));
            }

            attacker.MarkAsAttacked();

            result = new AttackResult(
                attacker,
                defender,
                totalHitsAttempted,
                totalHitsResolved,
                totalActualDamageToDefender,
                totalActualDamageToAttacker,
                defender.IsDestroyed,
                attacker.IsDestroyed,
                hits
            );

            return totalHitsResolved > 0;
        }
    }
}