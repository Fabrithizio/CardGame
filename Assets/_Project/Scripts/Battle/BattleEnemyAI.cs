// Caminho: Assets/_Project/Scripts/Battle/BattleEnemyAI.cs
// Descrição: IA temporária do inimigo para testes. Ela joga a primeira criatura possível, ataca criaturas inimigas ou ataca diretamente se o jogador estiver sem criaturas.

using System.Collections.Generic;
using CardGame.Cards;
using UnityEngine;

namespace CardGame.Battle
{
    public class BattleEnemyAI
    {
        public void ExecuteMainPhase(PlayerBattleState enemyState)
        {
            if (enemyState == null)
            {
                return;
            }

            for (int i = 0; i < enemyState.Hand.Count; i++)
            {
                CardRuntime card = enemyState.Hand.GetCardAt(i);

                if (card == null || card.CardType != CardType.Creature)
                {
                    continue;
                }

                if (enemyState.TryPlayCreatureInFirstFreeSlot(card, out int slotIndex))
                {
                    Debug.Log($"IA colocou {card.CardName} no slot {slotIndex + 1}.");
                    return;
                }
            }

            Debug.Log("IA não encontrou criatura para jogar.");
        }

        public void ExecuteBattlePhase(
            BattleManager battleManager,
            PlayerBattleState enemyState,
            PlayerBattleState playerState)
        {
            if (battleManager == null || enemyState == null || playerState == null)
            {
                return;
            }

            CardRuntime attacker = GetFirstCreatureThatCanAttack(enemyState);

            if (attacker == null)
            {
                Debug.Log("IA não tem criatura disponível para atacar.");
                return;
            }

            CardRuntime defender = GetFirstAliveCreature(playerState);

            if (defender != null)
            {
                Debug.Log($"IA escolheu {attacker.CardName} para atacar {defender.CardName}.");
                battleManager.ResolveCreatureAttack(attacker, defender);
                return;
            }

            Debug.Log($"IA escolheu {attacker.CardName} para atacar diretamente o jogador.");
            battleManager.ResolveDirectAttack(attacker, playerState);
        }

        private CardRuntime GetFirstCreatureThatCanAttack(PlayerBattleState state)
        {
            List<CardRuntime> creatures = state.Board.GetAliveCreatures();

            for (int i = 0; i < creatures.Count; i++)
            {
                CardRuntime card = creatures[i];

                if (card != null && card.IsAlive && !card.HasAttackedThisTurn)
                {
                    return card;
                }
            }

            return null;
        }

        private CardRuntime GetFirstAliveCreature(PlayerBattleState state)
        {
            List<CardRuntime> creatures = state.Board.GetAliveCreatures();

            if (creatures.Count <= 0)
            {
                return null;
            }

            return creatures[0];
        }
    }
}