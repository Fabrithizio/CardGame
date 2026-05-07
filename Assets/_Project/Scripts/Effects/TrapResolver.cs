// Caminho: Assets/_Project/Scripts/Effects/TrapResolver.cs
// Descrição: Resolve armadilhas preparadas no campo, começando pelo gatilho de bloquear o próximo ataque inimigo.

using CardGame.Battle;
using CardGame.Cards;
using UnityEngine;

namespace CardGame.Effects
{
    public static class TrapResolver
    {
        public static bool TryResolveBeforeAttack(
            PlayerBattleState attackingPlayer,
            PlayerBattleState defendingPlayer,
            CardRuntime attacker,
            CardRuntime defender,
            out string resultMessage)
        {
            resultMessage = string.Empty;

            if (attackingPlayer == null || defendingPlayer == null || attacker == null)
            {
                return false;
            }

            for (int i = 0; i < defendingPlayer.Board.TrapSlots.Count; i++)
            {
                CardRuntime trap = defendingPlayer.Board.GetTrapAt(i);

                if (trap == null || trap.Data == null)
                {
                    continue;
                }

                if (trap.Data.CardType != CardType.Trap)
                {
                    continue;
                }

                if (trap.Data.TrapTriggerType != TrapTriggerType.BeforeEnemyAttack)
                {
                    continue;
                }

                bool wasResolved = ResolveTrapEffect(
                    trap,
                    defendingPlayer,
                    attackingPlayer,
                    attacker,
                    defender,
                    out resultMessage
                );

                if (wasResolved)
                {
                    defendingPlayer.Board.RemoveTrap(trap);
                    return true;
                }
            }

            return false;
        }

        private static bool ResolveTrapEffect(
            CardRuntime trap,
            PlayerBattleState trapOwner,
            PlayerBattleState attackingPlayer,
            CardRuntime attacker,
            CardRuntime defender,
            out string resultMessage)
        {
            resultMessage = string.Empty;

            switch (trap.Data.TrapEffectType)
            {
                case TrapEffectType.BlockNextAttack:
                    resultMessage = $"{trapOwner.PlayerName} ativou {trap.CardName}: o ataque de {attacker.CardName} foi bloqueado.";
                    return true;

                case TrapEffectType.DealDamageToAttacker:
                    attacker.TakeDamage(trap.Data.TrapEffectValue);
                    resultMessage = $"{trapOwner.PlayerName} ativou {trap.CardName}: {attacker.CardName} recebeu {trap.Data.TrapEffectValue} de dano.";
                    return true;

                case TrapEffectType.GiveShieldToDefender:
                    if (defender == null)
                    {
                        return false;
                    }

                    defender.AddTemporaryShield();
                    resultMessage = $"{trapOwner.PlayerName} ativou {trap.CardName}: {defender.CardName} recebeu escudo.";
                    return true;

                case TrapEffectType.BuffDefenderDefense:
                    if (defender == null)
                    {
                        return false;
                    }

                    defender.AddAttribute(CardAttributeType.Defense, trap.Data.TrapEffectValue);
                    resultMessage = $"{trapOwner.PlayerName} ativou {trap.CardName}: {defender.CardName} recebeu +{trap.Data.TrapEffectValue} DEF.";
                    return true;

                case TrapEffectType.DebuffAttackerAttack:
                    attacker.AddAttribute(CardAttributeType.Attack, -trap.Data.TrapEffectValue);
                    resultMessage = $"{trapOwner.PlayerName} ativou {trap.CardName}: {attacker.CardName} perdeu {trap.Data.TrapEffectValue} ATK.";
                    return true;

                case TrapEffectType.None:
                default:
                    Debug.LogWarning($"{trap.CardName} não possui efeito de armadilha configurado.");
                    return false;
            }
        }
    }
}
