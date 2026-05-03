// Caminho: Assets/_Project/Scripts/Battle/BattleManager.cs
// Descrição: Controla a batalha, turnos, compra automática na fase Draw, IA simples, ataque entre criaturas, ataque direto, buffs, escudo e Míticos.

using System.Collections;
using System.Collections.Generic;
using CardGame.Cards;
using CardGame.Mythics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CardGame.Battle
{
    public class BattleManager : MonoBehaviour
    {
        [Header("Configuração da Batalha")]
        [SerializeField] private int playerStartingHealth = 30;
        [SerializeField] private int enemyStartingHealth = 30;
        [SerializeField] private int startingHandSize = 5;

        [Header("Deck do Jogador")]
        [SerializeField] private List<CardData> playerDeckCards = new();

        [Header("Deck do Inimigo")]
        [SerializeField] private List<CardData> enemyDeckCards = new();

        [Header("Míticos do Jogador")]
        [SerializeField] private List<MythicData> playerMythics = new();

        [Header("Míticos do Inimigo")]
        [SerializeField] private List<MythicData> enemyMythics = new();

        [Header("IA do Inimigo")]
        [SerializeField] private bool enableEnemyAI = true;
        [SerializeField] private float enemyActionDelay = 0.8f;

        [Header("Debug Temporário")]
        [SerializeField] private bool enableKeyboardDebug = true;
        [SerializeField] private int debugSpeedBuffAmount = 3;

        private PlayerBattleState playerState;
        private PlayerBattleState enemyState;
        private TurnManager turnManager;
        private BattleEnemyAI enemyAI;
        private Coroutine enemyTurnCoroutine;

        public PlayerBattleState PlayerState => playerState;
        public PlayerBattleState EnemyState => enemyState;
        public TurnManager TurnManager => turnManager;

        private void Start()
        {
            StartBattle();
        }

        private void Update()
        {
            if (!enableKeyboardDebug || turnManager == null)
            {
                return;
            }

            Keyboard keyboard = Keyboard.current;

            if (keyboard == null)
            {
                return;
            }

            if (keyboard.nKey.wasPressedThisFrame)
            {
                GoToNextPhase();
            }

            if (keyboard.dKey.wasPressedThisFrame)
            {
                ActivePlayerDrawCard();
            }

            if (keyboard.cKey.wasPressedThisFrame)
            {
                ActivePlayerPlayFirstCreature();
            }

            if (keyboard.mKey.wasPressedThisFrame)
            {
                ActivePlayerUseFirstAvailableMythic();
            }

            if (keyboard.aKey.wasPressedThisFrame)
            {
                ActivePlayerAttackFirstEnemyCreatureOrDirect();
            }

            if (keyboard.sKey.wasPressedThisFrame)
            {
                ActivePlayerBuffFirstCreatureSpeed();
            }

            if (keyboard.hKey.wasPressedThisFrame)
            {
                ActivePlayerGiveShieldToFirstCreature();
            }
        }

        public void StartBattle()
        {
            if (playerDeckCards.Count <= 0)
            {
                Debug.LogWarning("O deck do jogador está vazio. Adicione cartas no BattleManager.");
            }

            if (enemyDeckCards.Count <= 0)
            {
                Debug.LogWarning("O deck do inimigo está vazio. Adicione cartas no BattleManager.");
            }

            playerState = new PlayerBattleState(
                "Jogador",
                playerStartingHealth,
                playerDeckCards,
                playerMythics,
                true
            );

            enemyState = new PlayerBattleState(
                "Inimigo",
                enemyStartingHealth,
                enemyDeckCards,
                enemyMythics,
                false
            );

            enemyAI = new BattleEnemyAI();
            turnManager = new TurnManager();

            turnManager.OnTurnStarted += HandleTurnStarted;
            turnManager.OnTurnEnded += HandleTurnEnded;
            turnManager.OnPhaseChanged += HandlePhaseChanged;

            DrawStartingHands();

            turnManager.StartBattle();

            Debug.Log("Batalha iniciada.");
            Debug.Log("Debug ativo: N = próxima fase | D = comprar manual | C = colocar criatura | M = usar Mítico | A = atacar/direto | S = buffar Speed | H = dar escudo.");
            PrintBattleDebug();
        }

        private void DrawStartingHands()
        {
            for (int i = 0; i < startingHandSize; i++)
            {
                playerState.TryDrawCard(out _, out _);
                enemyState.TryDrawCard(out _, out _);
            }
        }

        private void HandleTurnStarted(int activePlayerIndex)
        {
            PlayerBattleState activePlayer = GetPlayerByIndex(activePlayerIndex);
            activePlayer.StartTurnReset();

            Debug.Log($"Turno iniciado: {activePlayer.PlayerName}");

            if (enableEnemyAI && activePlayerIndex == 1)
            {
                StartEnemyTurnAI();
            }
        }

        private void HandleTurnEnded(int activePlayerIndex)
        {
            PlayerBattleState endingPlayer = GetPlayerByIndex(activePlayerIndex);
            endingPlayer.ClearDestroyedCreatures();

            Debug.Log($"Turno encerrado: {endingPlayer.PlayerName}");
        }

        private void HandlePhaseChanged(BattlePhase phase)
        {
            Debug.Log($"Fase atual: {phase}");

            if (phase == BattlePhase.Draw)
            {
                ActivePlayerDrawCard();
            }
        }

        private void StartEnemyTurnAI()
        {
            if (enemyTurnCoroutine != null)
            {
                StopCoroutine(enemyTurnCoroutine);
            }

            enemyTurnCoroutine = StartCoroutine(EnemyTurnRoutine());
        }

        private IEnumerator EnemyTurnRoutine()
        {
            yield return new WaitForSeconds(enemyActionDelay);

            if (turnManager == null || !turnManager.IsEnemyTurn)
            {
                yield break;
            }

            GoToNextPhase();

            yield return new WaitForSeconds(enemyActionDelay);

            if (turnManager == null || !turnManager.IsEnemyTurn)
            {
                yield break;
            }

            GoToNextPhase();

            yield return new WaitForSeconds(enemyActionDelay);

            if (turnManager == null || !turnManager.IsEnemyTurn)
            {
                yield break;
            }

            enemyAI.ExecuteMainPhase(enemyState);
            PrintBattleDebug();

            yield return new WaitForSeconds(enemyActionDelay);

            if (turnManager == null || !turnManager.IsEnemyTurn)
            {
                yield break;
            }

            GoToNextPhase();

            yield return new WaitForSeconds(enemyActionDelay);

            if (turnManager == null || !turnManager.IsEnemyTurn)
            {
                yield break;
            }

            enemyAI.ExecuteBattlePhase(this, enemyState, playerState);

            yield return new WaitForSeconds(enemyActionDelay);

            if (turnManager == null || !turnManager.IsEnemyTurn)
            {
                yield break;
            }

            GoToNextPhase();

            yield return new WaitForSeconds(enemyActionDelay);

            if (turnManager == null || !turnManager.IsEnemyTurn)
            {
                yield break;
            }

            GoToNextPhase();
        }

        public void GoToNextPhase()
        {
            if (turnManager == null)
            {
                Debug.LogWarning("TurnManager ainda não foi iniciado.");
                return;
            }

            turnManager.GoToNextPhase();
            PrintBattleDebug();
        }

        public void ActivePlayerDrawCard()
        {
            PlayerBattleState activePlayer = GetActivePlayer();

            bool drewCard = activePlayer.TryDrawCard(out CardRuntime drawnCard, out int fatigueDamage);

            if (drewCard)
            {
                Debug.Log($"{activePlayer.PlayerName} comprou: {drawnCard.CardName}");
            }
            else
            {
                Debug.Log($"{activePlayer.PlayerName} tentou comprar sem deck e recebeu {fatigueDamage} de dano.");
            }

            PrintBattleDebug();
            CheckForBattleEnd();
        }

        public void ActivePlayerPlayFirstCreature()
        {
            PlayerBattleState activePlayer = GetActivePlayer();

            for (int i = 0; i < activePlayer.Hand.Count; i++)
            {
                CardRuntime card = activePlayer.Hand.GetCardAt(i);

                if (card == null || card.CardType != CardType.Creature)
                {
                    continue;
                }

                if (activePlayer.TryPlayCreatureInFirstFreeSlot(card, out int slotIndex))
                {
                    Debug.Log($"{activePlayer.PlayerName} colocou {card.CardName} no slot de criatura {slotIndex + 1}.");
                    PrintCardStats(card);
                    PrintCardStatuses(card);
                    PrintBattleDebug();
                    return;
                }
            }

            Debug.Log($"{activePlayer.PlayerName} não tem criatura na mão ou não tem slot livre.");
        }

        public void ActivePlayerUseFirstAvailableMythic()
        {
            PlayerBattleState activePlayer = GetActivePlayer();

            for (int i = 0; i < activePlayer.MythicLoadout.TotalSlots; i++)
            {
                MythicRuntime mythic = activePlayer.MythicLoadout.GetMythicAt(i);

                if (mythic == null || !mythic.CanUse())
                {
                    continue;
                }

                if (!CanApplyMythicEffect(activePlayer, mythic.Data))
                {
                    Debug.Log($"Não há alvo válido para o Mítico {mythic.MythicName}.");
                    return;
                }

                if (!activePlayer.MythicLoadout.TryUseMythic(i, out MythicRuntime usedMythic))
                {
                    return;
                }

                Debug.Log($"MÍTICO ATIVADO por {activePlayer.PlayerName}: {usedMythic.MythicName}");
                Debug.Log($"Texto de ativação: {usedMythic.Data.ActivationText}");

                ApplyMythicEffect(activePlayer, usedMythic.Data);

                PrintBattleDebug();
                CheckForBattleEnd();
                return;
            }

            Debug.Log($"{activePlayer.PlayerName} não possui Míticos disponíveis.");
        }

        private bool CanApplyMythicEffect(PlayerBattleState activePlayer, MythicData mythicData)
        {
            if (mythicData == null)
            {
                return false;
            }

            switch (mythicData.EffectType)
            {
                case MythicEffectType.GiveShieldToCreature:
                case MythicEffectType.BuffCreatureAttack:
                case MythicEffectType.BuffCreatureSpeed:
                case MythicEffectType.HealCreature:
                    return GetFirstAliveCreature(activePlayer) != null;

                case MythicEffectType.None:
                    return true;

                default:
                    return true;
            }
        }

        private void ApplyMythicEffect(PlayerBattleState activePlayer, MythicData mythicData)
        {
            if (mythicData == null)
            {
                return;
            }

            CardRuntime target = GetFirstAliveCreature(activePlayer);

            switch (mythicData.EffectType)
            {
                case MythicEffectType.GiveShieldToCreature:
                    if (target == null)
                    {
                        Debug.Log("Mítico falhou: não existe criatura viva para receber escudo.");
                        return;
                    }

                    target.AddTemporaryShield();
                    Debug.Log($"{mythicData.MythicName} concedeu escudo para {target.CardName}.");
                    PrintCardStatuses(target);
                    break;

                case MythicEffectType.BuffCreatureAttack:
                    if (target == null)
                    {
                        Debug.Log("Mítico falhou: não existe criatura viva para receber ataque.");
                        return;
                    }

                    target.AddAttribute(CardAttributeType.Attack, mythicData.EffectValue);
                    Debug.Log($"{mythicData.MythicName} deu +{mythicData.EffectValue} ATK para {target.CardName}.");
                    PrintCardStats(target);
                    PrintCardStatuses(target);
                    break;

                case MythicEffectType.BuffCreatureSpeed:
                    if (target == null)
                    {
                        Debug.Log("Mítico falhou: não existe criatura viva para receber Speed.");
                        return;
                    }

                    target.AddAttribute(CardAttributeType.Speed, mythicData.EffectValue);
                    Debug.Log($"{mythicData.MythicName} deu +{mythicData.EffectValue} Speed para {target.CardName}.");
                    PrintCardStats(target);
                    PrintCardStatuses(target);
                    break;

                case MythicEffectType.HealCreature:
                    if (target == null)
                    {
                        Debug.Log("Mítico falhou: não existe criatura viva para curar.");
                        return;
                    }

                    target.Heal(mythicData.EffectValue);
                    Debug.Log($"{mythicData.MythicName} curou {mythicData.EffectValue} de vida de {target.CardName}.");
                    PrintCardStats(target);
                    PrintCardStatuses(target);
                    break;

                case MythicEffectType.DamageEnemyPlayer:
                    PlayerBattleState enemy = GetEnemyOfActivePlayer();
                    enemy.TakeDamageDirect(mythicData.EffectValue);
                    Debug.Log($"{mythicData.MythicName} causou {mythicData.EffectValue} de dano direto em {enemy.PlayerName}.");
                    break;

                case MythicEffectType.None:
                    Debug.Log($"{mythicData.MythicName} não possui efeito configurado.");
                    break;

                default:
                    Debug.Log($"{mythicData.MythicName} possui um efeito ainda não implementado: {mythicData.EffectType}.");
                    break;
            }
        }

        public void ActivePlayerGiveShieldToFirstCreature()
        {
            PlayerBattleState activePlayer = GetActivePlayer();
            CardRuntime target = GetFirstAliveCreature(activePlayer);

            if (target == null)
            {
                Debug.Log($"{activePlayer.PlayerName} não tem criatura viva para receber escudo.");
                return;
            }

            target.AddTemporaryShield();

            Debug.Log($"{activePlayer.PlayerName} deu escudo temporário para {target.CardName}.");

            PrintCardStatuses(target);
        }

        public void ActivePlayerBuffFirstCreatureSpeed()
        {
            PlayerBattleState activePlayer = GetActivePlayer();
            CardRuntime target = GetFirstAliveCreature(activePlayer);

            if (target == null)
            {
                Debug.Log($"{activePlayer.PlayerName} não tem criatura viva para receber buff de Speed.");
                return;
            }

            int oldSpeed = target.CurrentSpeed;

            target.AddAttribute(CardAttributeType.Speed, debugSpeedBuffAmount);

            Debug.Log(
                $"{activePlayer.PlayerName} buffou Speed de {target.CardName}: " +
                $"{oldSpeed} → {target.CurrentSpeed}."
            );

            Debug.Log($"{target.CardName} agora pode realizar {target.GetAttackCountBySpeed()} golpe(s) por ataque.");

            PrintCardStats(target);
            PrintCardStatuses(target);
        }

        public void ActivePlayerAttackFirstEnemyCreatureOrDirect()
        {
            PlayerBattleState activePlayer = GetActivePlayer();
            PlayerBattleState enemyPlayer = GetEnemyOfActivePlayer();

            CardRuntime attacker = GetFirstAliveCreature(activePlayer);

            if (attacker == null)
            {
                Debug.Log($"{activePlayer.PlayerName} não tem criatura viva para atacar.");
                return;
            }

            CardRuntime defender = GetFirstAliveCreature(enemyPlayer);

            if (defender != null)
            {
                ResolveCreatureAttack(attacker, defender);
                return;
            }

            ResolveDirectAttack(attacker, enemyPlayer);
        }

        public bool ResolveCreatureAttack(CardRuntime attacker, CardRuntime defender)
        {
            bool success = AttackResolver.TryResolveCreatureAttack(attacker, defender, out AttackResult result);

            if (!success)
            {
                Debug.Log("Ataque não foi realizado.");
                return false;
            }

            Debug.Log($"{attacker.CardName} atacou {defender.CardName}.");

            foreach (AttackHitResult hit in result.Hits)
            {
                string defenderShieldText = hit.DefenderShieldConsumed ? " | Escudo do defensor consumido" : string.Empty;
                string attackerShieldText = hit.AttackerShieldConsumed ? " | Escudo do atacante consumido" : string.Empty;

                Debug.Log(
                    $"Golpe {hit.HitNumber}/{result.TotalHitsAttempted}: " +
                    $"{attacker.CardName} tentou causar {hit.AttemptedDamageToDefender}, dano real {hit.ActualDamageToDefender} | " +
                    $"{defender.CardName} tentou revidar {hit.AttemptedDamageToAttacker}, dano real {hit.ActualDamageToAttacker}" +
                    $"{defenderShieldText}{attackerShieldText}."
                );
            }

            if (result.DefenderDestroyed)
            {
                Debug.Log($"{defender.CardName} foi destruído.");
            }

            if (result.AttackerDestroyed)
            {
                Debug.Log($"{attacker.CardName} foi destruído.");
            }

            playerState.ClearDestroyedCreatures();
            enemyState.ClearDestroyedCreatures();

            PrintBattleDebug();
            CheckForBattleEnd();

            return true;
        }

        public bool ResolveDirectAttack(CardRuntime attacker, PlayerBattleState defendingPlayer)
        {
            if (attacker == null || !attacker.IsAlive)
            {
                Debug.Log("Ataque direto cancelado: atacante inválido.");
                return false;
            }

            if (defendingPlayer == null || defendingPlayer.IsDefeated)
            {
                Debug.Log("Ataque direto cancelado: defensor inválido.");
                return false;
            }

            if (attacker.HasAttackedThisTurn)
            {
                Debug.Log($"{attacker.CardName} já atacou neste turno.");
                return false;
            }

            int totalHits = attacker.GetAttackCountBySpeed();

            Debug.Log($"{attacker.CardName} atacou diretamente {defendingPlayer.PlayerName} com {totalHits} golpe(s).");

            for (int i = 0; i < totalHits; i++)
            {
                if (defendingPlayer.IsDefeated)
                {
                    break;
                }

                int damage = Mathf.Max(0, attacker.CurrentAttack);
                defendingPlayer.TakeDamageDirect(damage);

                Debug.Log(
                    $"Golpe direto {i + 1}/{totalHits}: {attacker.CardName} causou {damage} em {defendingPlayer.PlayerName}."
                );
            }

            attacker.MarkAsAttacked();

            PrintBattleDebug();
            CheckForBattleEnd();

            return true;
        }

        private void CheckForBattleEnd()
        {
            if (turnManager == null || turnManager.CurrentPhase == BattlePhase.Finished)
            {
                return;
            }

            if (playerState.IsDefeated)
            {
                Debug.Log("DERROTA: o jogador ficou sem vida.");
                turnManager.FinishBattle();
                return;
            }

            if (enemyState.IsDefeated)
            {
                Debug.Log("VITÓRIA: o inimigo ficou sem vida.");
                turnManager.FinishBattle();
            }
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

        private PlayerBattleState GetActivePlayer()
        {
            return GetPlayerByIndex(turnManager.ActivePlayerIndex);
        }

        private PlayerBattleState GetEnemyOfActivePlayer()
        {
            return turnManager.ActivePlayerIndex == 0 ? enemyState : playerState;
        }

        private PlayerBattleState GetPlayerByIndex(int index)
        {
            return index == 0 ? playerState : enemyState;
        }

        private void PrintCardStats(CardRuntime card)
        {
            if (card == null)
            {
                return;
            }

            Debug.Log(
                $"{card.CardName} | " +
                $"ATK {card.CurrentAttack} | " +
                $"HP {card.CurrentHealth} | " +
                $"SPD {card.CurrentSpeed} | " +
                $"DEF {card.CurrentDefense} | " +
                $"FOC {card.CurrentFocus} | " +
                $"RES {card.CurrentResistance}"
            );
        }

        private void PrintCardStatuses(CardRuntime card)
        {
            if (card == null)
            {
                return;
            }

            List<CardStatusType> statuses = card.GetCurrentStatuses();

            if (statuses.Count <= 0)
            {
                Debug.Log($"Status de {card.CardName}: nenhum.");
                return;
            }

            Debug.Log($"Status de {card.CardName}: {string.Join(", ", statuses)}.");
        }

        private void PrintBattleDebug()
        {
            Debug.Log(
                $"[Estado] " +
                $"Jogador Vida: {playerState.CurrentHealth}/{playerState.MaxHealth} | " +
                $"Deck: {playerState.Deck.CardsRemaining} | " +
                $"Mão: {playerState.Hand.Count} | " +
                $"Criaturas: {playerState.Board.GetAliveCreatures().Count} | " +
                $"Míticos disponíveis: {playerState.MythicLoadout.GetAvailableCount()}"
            );

            Debug.Log(
                $"[Estado] " +
                $"Inimigo Vida: {enemyState.CurrentHealth}/{enemyState.MaxHealth} | " +
                $"Deck: {enemyState.Deck.CardsRemaining} | " +
                $"Mão: {enemyState.Hand.Count} | " +
                $"Criaturas: {enemyState.Board.GetAliveCreatures().Count} | " +
                $"Míticos disponíveis: {enemyState.MythicLoadout.GetAvailableCount()}"
            );
        }
    }
}