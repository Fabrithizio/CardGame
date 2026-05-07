// Caminho: Assets/_Project/Scripts/Battle/BoardRuntime.cs
// Descrição: Controla as cartas em campo durante a partida, separando espaços de criaturas e armadilhas.

using System;
using System.Collections.Generic;
using CardGame.Cards;
using UnityEngine;

namespace CardGame.Battle
{
    [Serializable]
    public class BoardRuntime
    {
        public const int MaxCreatureSlots = 5;
        public const int MaxTrapSlots = 6;

        [SerializeField] private List<CardRuntime> creatureSlots = new();
        [SerializeField] private List<CardRuntime> trapSlots = new();

        public IReadOnlyList<CardRuntime> CreatureSlots => creatureSlots;
        public IReadOnlyList<CardRuntime> TrapSlots => trapSlots;

        public BoardRuntime()
        {
            creatureSlots = new List<CardRuntime>(MaxCreatureSlots);
            trapSlots = new List<CardRuntime>(MaxTrapSlots);

            for (int i = 0; i < MaxCreatureSlots; i++)
            {
                creatureSlots.Add(null);
            }

            for (int i = 0; i < MaxTrapSlots; i++)
            {
                trapSlots.Add(null);
            }
        }

        public bool HasAnyCreature()
        {
            for (int i = 0; i < creatureSlots.Count; i++)
            {
                CardRuntime card = creatureSlots[i];

                if (card != null && card.IsAlive)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasFreeCreatureSlot()
        {
            return GetFirstFreeCreatureSlotIndex() >= 0;
        }

        public bool HasFreeTrapSlot()
        {
            return GetFirstFreeTrapSlotIndex() >= 0;
        }

        public int GetFirstFreeCreatureSlotIndex()
        {
            EnsureSlotCounts();

            for (int i = 0; i < creatureSlots.Count; i++)
            {
                if (creatureSlots[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }

        public int GetFirstFreeTrapSlotIndex()
        {
            EnsureSlotCounts();

            for (int i = 0; i < trapSlots.Count; i++)
            {
                if (trapSlots[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }

        public bool TryPlaceCreature(CardRuntime card, int slotIndex)
        {
            EnsureSlotCounts();

            if (card == null)
            {
                Debug.LogWarning("Tentativa de colocar uma criatura nula no campo.");
                return false;
            }

            if (card.CardType != CardType.Creature)
            {
                Debug.LogWarning($"A carta {card.CardName} não é uma criatura.");
                return false;
            }

            if (!IsValidCreatureSlot(slotIndex))
            {
                Debug.LogWarning($"Slot de criatura inválido: {slotIndex}.");
                return false;
            }

            if (creatureSlots[slotIndex] != null)
            {
                Debug.LogWarning($"O slot de criatura {slotIndex} já está ocupado.");
                return false;
            }

            creatureSlots[slotIndex] = card;
            return true;
        }

        public bool TryPlaceCreatureInFirstFreeSlot(CardRuntime card, out int placedSlotIndex)
        {
            placedSlotIndex = GetFirstFreeCreatureSlotIndex();

            if (placedSlotIndex < 0)
            {
                return false;
            }

            return TryPlaceCreature(card, placedSlotIndex);
        }

        public bool TryPlaceTrap(CardRuntime card, int slotIndex)
        {
            EnsureSlotCounts();

            if (card == null)
            {
                Debug.LogWarning("Tentativa de colocar uma armadilha nula no campo.");
                return false;
            }

            if (card.CardType != CardType.Trap)
            {
                Debug.LogWarning($"A carta {card.CardName} não é uma armadilha.");
                return false;
            }

            if (!IsValidTrapSlot(slotIndex))
            {
                Debug.LogWarning($"Slot de armadilha inválido: {slotIndex}.");
                return false;
            }

            if (trapSlots[slotIndex] != null)
            {
                Debug.LogWarning($"O slot de armadilha {slotIndex} já está ocupado.");
                return false;
            }

            trapSlots[slotIndex] = card;
            return true;
        }

        public bool TryPlaceTrapInFirstFreeSlot(CardRuntime card, out int placedSlotIndex)
        {
            placedSlotIndex = GetFirstFreeTrapSlotIndex();

            if (placedSlotIndex < 0)
            {
                return false;
            }

            return TryPlaceTrap(card, placedSlotIndex);
        }

        public CardRuntime GetCreatureAt(int slotIndex)
        {
            EnsureSlotCounts();

            if (!IsValidCreatureSlot(slotIndex))
            {
                return null;
            }

            return creatureSlots[slotIndex];
        }

        public CardRuntime GetTrapAt(int slotIndex)
        {
            EnsureSlotCounts();

            if (!IsValidTrapSlot(slotIndex))
            {
                return null;
            }

            return trapSlots[slotIndex];
        }

        public bool RemoveCreature(CardRuntime card)
        {
            EnsureSlotCounts();

            if (card == null)
            {
                return false;
            }

            for (int i = 0; i < creatureSlots.Count; i++)
            {
                if (creatureSlots[i] == card)
                {
                    creatureSlots[i] = null;
                    return true;
                }
            }

            return false;
        }

        public bool RemoveTrap(CardRuntime card)
        {
            EnsureSlotCounts();

            if (card == null)
            {
                return false;
            }

            for (int i = 0; i < trapSlots.Count; i++)
            {
                if (trapSlots[i] == card)
                {
                    trapSlots[i] = null;
                    return true;
                }
            }

            return false;
        }

        public List<CardRuntime> GetAliveCreatures()
        {
            EnsureSlotCounts();

            List<CardRuntime> aliveCreatures = new();

            for (int i = 0; i < creatureSlots.Count; i++)
            {
                CardRuntime card = creatureSlots[i];

                if (card != null && card.IsAlive)
                {
                    aliveCreatures.Add(card);
                }
            }

            return aliveCreatures;
        }

        public void ClearDestroyedCreatures()
        {
            EnsureSlotCounts();

            for (int i = 0; i < creatureSlots.Count; i++)
            {
                CardRuntime card = creatureSlots[i];

                if (card != null && card.IsDestroyed)
                {
                    creatureSlots[i] = null;
                }
            }
        }

        public void ResetTurnState()
        {
            EnsureSlotCounts();

            for (int i = 0; i < creatureSlots.Count; i++)
            {
                CardRuntime card = creatureSlots[i];

                if (card != null)
                {
                    card.ResetTurnState();
                }
            }
        }

        private bool IsValidCreatureSlot(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < creatureSlots.Count;
        }

        private bool IsValidTrapSlot(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < trapSlots.Count;
        }

        private void EnsureSlotCounts()
        {
            while (creatureSlots.Count < MaxCreatureSlots)
            {
                creatureSlots.Add(null);
            }

            while (trapSlots.Count < MaxTrapSlots)
            {
                trapSlots.Add(null);
            }

            while (creatureSlots.Count > MaxCreatureSlots)
            {
                creatureSlots.RemoveAt(creatureSlots.Count - 1);
            }

            while (trapSlots.Count > MaxTrapSlots)
            {
                trapSlots.RemoveAt(trapSlots.Count - 1);
            }
        }
    }
}
