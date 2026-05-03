// Caminho: Assets/_Project/Scripts/Battle/HandRuntime.cs
// Descrição: Controla a mão do jogador durante a partida, incluindo adicionar, remover e verificar cartas disponíveis.

using System;
using System.Collections.Generic;
using CardGame.Cards;
using UnityEngine;

namespace CardGame.Battle
{
    [Serializable]
    public class HandRuntime
    {
        [SerializeField] private List<CardRuntime> cards = new();

        public int Count => cards.Count;
        public IReadOnlyList<CardRuntime> Cards => cards;

        public bool IsEmpty => cards.Count <= 0;

        public void AddCard(CardRuntime card)
        {
            if (card == null)
            {
                Debug.LogWarning("Tentativa de adicionar uma carta nula à mão.");
                return;
            }

            cards.Add(card);
        }

        public bool RemoveCard(CardRuntime card)
        {
            if (card == null)
            {
                return false;
            }

            return cards.Remove(card);
        }

        public bool RemoveCardAt(int index, out CardRuntime removedCard)
        {
            removedCard = null;

            if (index < 0 || index >= cards.Count)
            {
                return false;
            }

            removedCard = cards[index];
            cards.RemoveAt(index);
            return true;
        }

        public bool Contains(CardRuntime card)
        {
            return card != null && cards.Contains(card);
        }

        public CardRuntime GetCardAt(int index)
        {
            if (index < 0 || index >= cards.Count)
            {
                return null;
            }

            return cards[index];
        }

        public List<CardRuntime> GetPlayableCardsByCost(int availableEnergy)
        {
            List<CardRuntime> playableCards = new();

            foreach (CardRuntime card in cards)
            {
                if (card == null || card.Data == null)
                {
                    continue;
                }

                if (card.Data.Cost <= availableEnergy)
                {
                    playableCards.Add(card);
                }
            }

            return playableCards;
        }

        public bool HasAnyCreature()
        {
            foreach (CardRuntime card in cards)
            {
                if (card != null && card.CardType == CardType.Creature)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasAnyDamageSpell()
        {
            // Ainda vamos criar o sistema real de efeitos.
            // Por enquanto fica preparado para a regra futura de game over.
            return false;
        }

        public void Clear()
        {
            cards.Clear();
        }
    }
}