// Caminho: Assets/_Project/Scripts/Battle/PlayerBattleState.cs
// Descrição: Representa o estado completo de um jogador durante a batalha, incluindo vida, energia, deck, mão, campo e Míticos. O limite para jogar cartas é energia + espaço correto no campo.

using System;
using System.Collections.Generic;
using CardGame.Cards;
using CardGame.Mythics;
using UnityEngine;

namespace CardGame.Battle
{
    [Serializable]
    public class PlayerBattleState
    {
        private const int MaxEnergyLimit = 10;

        [SerializeField] private string playerName;
        [SerializeField] private int maxHealth;
        [SerializeField] private int currentHealth;

        [SerializeField] private int maxEnergy;
        [SerializeField] private int currentEnergy;

        [SerializeField] private DeckRuntime deck;
        [SerializeField] private HandRuntime hand;
        [SerializeField] private BoardRuntime board;
        [SerializeField] private MythicLoadoutRuntime mythicLoadout;

        public string PlayerName => playerName;
        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;

        public int MaxEnergy => maxEnergy;
        public int CurrentEnergy => currentEnergy;

        public DeckRuntime Deck => deck;
        public HandRuntime Hand => hand;
        public BoardRuntime Board => board;
        public MythicLoadoutRuntime MythicLoadout => mythicLoadout;

        public bool IsDefeated => currentHealth <= 0;

        public PlayerBattleState(
            string playerName,
            int maxHealth,
            IEnumerable<CardData> deckCards,
            IEnumerable<MythicData> mythics,
            bool mythicsStartRevealed)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                throw new ArgumentException("O nome do jogador não pode ser vazio.", nameof(playerName));
            }

            this.playerName = playerName;
            this.maxHealth = Mathf.Max(1, maxHealth);
            currentHealth = this.maxHealth;

            maxEnergy = 0;
            currentEnergy = 0;

            deck = new DeckRuntime(deckCards);
            hand = new HandRuntime();
            board = new BoardRuntime();
            mythicLoadout = new MythicLoadoutRuntime(mythics, mythicsStartRevealed);
        }

        public void StartTurnReset()
        {
            board.ResetTurnState();
            IncreaseAndRefillEnergy();
        }

        private void IncreaseAndRefillEnergy()
        {
            maxEnergy = Mathf.Min(MaxEnergyLimit, maxEnergy + 1);
            currentEnergy = maxEnergy;

            Debug.Log($"{playerName} começou o turno com {currentEnergy}/{maxEnergy} de energia.");
        }

        public bool HasEnoughEnergy(int cost)
        {
            return currentEnergy >= Mathf.Max(0, cost);
        }

        public bool TrySpendEnergy(int amount)
        {
            int safeAmount = Mathf.Max(0, amount);

            if (safeAmount <= 0)
            {
                return true;
            }

            if (currentEnergy < safeAmount)
            {
                Debug.Log($"{playerName} não tem energia suficiente. Precisa de {safeAmount}, possui {currentEnergy}.");
                return false;
            }

            currentEnergy -= safeAmount;
            return true;
        }

        public bool TryDrawCard(out CardRuntime drawnCard, out int fatigueDamageTaken)
        {
            drawnCard = null;
            fatigueDamageTaken = 0;

            if (deck.TryDrawCard(out drawnCard))
            {
                hand.AddCard(drawnCard);
                return true;
            }

            fatigueDamageTaken = deck.RegisterFailedDrawAndGetFatigueDamage();
            TakeDamageDirect(fatigueDamageTaken);

            return false;
        }

        public bool TryPlayCreatureFromHand(CardRuntime card, int slotIndex)
        {
            if (card == null)
            {
                return false;
            }

            if (card.CardType != CardType.Creature)
            {
                Debug.LogWarning($"{card.CardName} não é uma criatura.");
                return false;
            }

            if (!hand.Contains(card))
            {
                Debug.LogWarning($"{card.CardName} não está na mão de {playerName}.");
                return false;
            }

            if (!HasEnoughEnergy(card.Data.Cost))
            {
                Debug.Log($"{playerName} não tem energia para jogar {card.CardName}. Custo {card.Data.Cost}, energia {currentEnergy}.");
                return false;
            }

            if (!board.TryPlaceCreature(card, slotIndex))
            {
                return false;
            }

            if (!TrySpendEnergy(card.Data.Cost))
            {
                board.RemoveCreature(card);
                return false;
            }

            hand.RemoveCard(card);
            return true;
        }

        public bool TryPlayCreatureInFirstFreeSlot(CardRuntime card, out int slotIndex)
        {
            slotIndex = -1;

            if (card == null)
            {
                return false;
            }

            if (card.CardType != CardType.Creature)
            {
                return false;
            }

            if (!hand.Contains(card))
            {
                return false;
            }

            if (!HasEnoughEnergy(card.Data.Cost))
            {
                Debug.Log($"{playerName} não tem energia para jogar {card.CardName}. Custo {card.Data.Cost}, energia {currentEnergy}.");
                return false;
            }

            if (!board.TryPlaceCreatureInFirstFreeSlot(card, out slotIndex))
            {
                return false;
            }

            if (!TrySpendEnergy(card.Data.Cost))
            {
                board.RemoveCreature(card);
                slotIndex = -1;
                return false;
            }

            hand.RemoveCard(card);
            return true;
        }

        public bool TrySetTrapFromHand(CardRuntime card, int slotIndex)
        {
            if (card == null)
            {
                return false;
            }

            if (card.CardType != CardType.Trap)
            {
                Debug.LogWarning($"{card.CardName} não é uma armadilha.");
                return false;
            }

            if (!hand.Contains(card))
            {
                Debug.LogWarning($"{card.CardName} não está na mão de {playerName}.");
                return false;
            }

            if (!HasEnoughEnergy(card.Data.Cost))
            {
                Debug.Log($"{playerName} não tem energia para preparar {card.CardName}. Custo {card.Data.Cost}, energia {currentEnergy}.");
                return false;
            }

            if (!board.TryPlaceTrap(card, slotIndex))
            {
                return false;
            }

            if (!TrySpendEnergy(card.Data.Cost))
            {
                board.RemoveTrap(card);
                return false;
            }

            hand.RemoveCard(card);
            return true;
        }

        public bool TrySetTrapInFirstFreeSlot(CardRuntime card, out int slotIndex)
        {
            slotIndex = -1;

            if (card == null)
            {
                return false;
            }

            if (card.CardType != CardType.Trap)
            {
                return false;
            }

            if (!hand.Contains(card))
            {
                return false;
            }

            if (!HasEnoughEnergy(card.Data.Cost))
            {
                Debug.Log($"{playerName} não tem energia para preparar {card.CardName}. Custo {card.Data.Cost}, energia {currentEnergy}.");
                return false;
            }

            if (!board.TryPlaceTrapInFirstFreeSlot(card, out slotIndex))
            {
                return false;
            }

            if (!TrySpendEnergy(card.Data.Cost))
            {
                board.RemoveTrap(card);
                slotIndex = -1;
                return false;
            }

            hand.RemoveCard(card);
            return true;
        }

        public void TakeDamageDirect(int damageAmount)
        {
            int safeDamage = Mathf.Max(0, damageAmount);

            if (safeDamage <= 0)
            {
                return;
            }

            currentHealth = Mathf.Max(0, currentHealth - safeDamage);
        }

        public void HealPlayer(int healAmount)
        {
            int safeHeal = Mathf.Max(0, healAmount);

            if (safeHeal <= 0)
            {
                return;
            }

            currentHealth = Mathf.Min(maxHealth, currentHealth + safeHeal);
        }

        public bool HasAnyPossibleSurvivalPlay()
        {
            if (board.HasAnyCreature())
            {
                return true;
            }

            if (hand.HasAnyCreature())
            {
                return true;
            }

            if (hand.HasAnyDamageSpell())
            {
                return true;
            }

            if (mythicLoadout.HasAvailableMythic())
            {
                return true;
            }

            return false;
        }

        public void ClearDestroyedCreatures()
        {
            board.ClearDestroyedCreatures();
        }
    }
}