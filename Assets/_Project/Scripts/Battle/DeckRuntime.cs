// Caminho: Assets/_Project/Scripts/Battle/DeckRuntime.cs
// Descrição: Controla o deck durante a partida, incluindo embaralhamento, compra de cartas e controle de deck vazio.

using System;
using System.Collections.Generic;
using CardGame.Cards;
using UnityEngine;

namespace CardGame.Battle
{
    [Serializable]
    public class DeckRuntime
    {
        [SerializeField] private List<CardData> originalCards = new();
        [SerializeField] private List<CardData> drawPile = new();
        [SerializeField] private List<CardData> discardPile = new();

        private int fatigueDamage;

        public int CardsRemaining => drawPile.Count;
        public int DiscardCount => discardPile.Count;
        public int FatigueDamage => fatigueDamage;
        public bool IsEmpty => drawPile.Count <= 0;

        public IReadOnlyList<CardData> OriginalCards => originalCards;
        public IReadOnlyList<CardData> DrawPile => drawPile;
        public IReadOnlyList<CardData> DiscardPile => discardPile;

        public DeckRuntime(IEnumerable<CardData> cards)
        {
            if (cards == null)
            {
                throw new ArgumentNullException(nameof(cards), "A lista de cartas do deck não pode ser nula.");
            }

            originalCards = new List<CardData>();
            drawPile = new List<CardData>();
            discardPile = new List<CardData>();
            fatigueDamage = 0;

            foreach (CardData card in cards)
            {
                if (card == null)
                {
                    Debug.LogWarning("Uma carta nula foi ignorada ao criar o deck.");
                    continue;
                }

                originalCards.Add(card);
                drawPile.Add(card);
            }

            Shuffle();
        }

        public void Shuffle()
        {
            for (int i = 0; i < drawPile.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, drawPile.Count);

                (drawPile[i], drawPile[randomIndex]) = (drawPile[randomIndex], drawPile[i]);
            }
        }

        public bool TryDrawCard(out CardRuntime drawnCard)
        {
            drawnCard = null;

            if (drawPile.Count <= 0)
            {
                return false;
            }

            CardData cardData = drawPile[0];
            drawPile.RemoveAt(0);

            drawnCard = new CardRuntime(cardData);
            return true;
        }

        public int RegisterFailedDrawAndGetFatigueDamage()
        {
            fatigueDamage++;
            return fatigueDamage;
        }

        public void AddToDiscard(CardData cardData)
        {
            if (cardData == null)
            {
                return;
            }

            discardPile.Add(cardData);
        }

        public void ResetDeck()
        {
            drawPile.Clear();
            discardPile.Clear();
            fatigueDamage = 0;

            foreach (CardData card in originalCards)
            {
                if (card != null)
                {
                    drawPile.Add(card);
                }
            }

            Shuffle();
        }
    }
}