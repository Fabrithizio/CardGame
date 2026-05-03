// Caminho: Assets/_Project/Scripts/Cards/CardRuntime.cs
// Descrição: Representa uma carta durante a partida, guardando atributos atuais, buffs, escudos, estados temporários e status visuais.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Cards
{
    [Serializable]
    public class CardRuntime
    {
        [SerializeField] private CardData data;

        [SerializeField] private int currentAttack;
        [SerializeField] private int currentHealth;
        [SerializeField] private int currentSpeed;
        [SerializeField] private int currentDefense;
        [SerializeField] private int currentFocus;
        [SerializeField] private int currentResistance;

        [SerializeField] private bool hasTemporaryShield;
        [SerializeField] private bool hasAttackedThisTurn;
        [SerializeField] private bool isDestroyed;

        public CardData Data => data;

        public string CardId => data != null ? data.CardId : string.Empty;
        public string CardName => data != null ? data.CardName : "Carta sem dados";
        public CardType CardType => data != null ? data.CardType : CardType.Creature;
        public CardRarity Rarity => data != null ? data.Rarity : CardRarity.Common;

        public int BaseAttack => data != null ? data.Attack : 0;
        public int BaseHealth => data != null ? data.Health : 0;
        public int BaseSpeed => data != null ? data.Speed : 0;
        public int BaseDefense => data != null ? data.Defense : 0;
        public int BaseFocus => data != null ? data.Focus : 0;
        public int BaseResistance => data != null ? data.Resistance : 0;

        public int CurrentAttack => currentAttack;
        public int CurrentHealth => currentHealth;
        public int CurrentSpeed => currentSpeed;
        public int CurrentDefense => currentDefense;
        public int CurrentFocus => currentFocus;
        public int CurrentResistance => currentResistance;

        public bool HasTemporaryShield => hasTemporaryShield;
        public bool HasAttackedThisTurn => hasAttackedThisTurn;
        public bool IsDestroyed => isDestroyed;
        public bool IsAlive => !isDestroyed && currentHealth > 0;

        public CardRuntime(CardData cardData)
        {
            if (cardData == null)
            {
                throw new ArgumentNullException(nameof(cardData), "CardData não pode ser nulo ao criar CardRuntime.");
            }

            data = cardData;

            currentAttack = data.Attack;
            currentHealth = data.Health;
            currentSpeed = data.Speed;
            currentDefense = data.Defense;
            currentFocus = data.Focus;
            currentResistance = data.Resistance;

            hasTemporaryShield = data.HasShield;
            hasAttackedThisTurn = false;
            isDestroyed = false;
        }

        public int GetAttribute(CardAttributeType attributeType)
        {
            return attributeType switch
            {
                CardAttributeType.Attack => currentAttack,
                CardAttributeType.Health => currentHealth,
                CardAttributeType.Speed => currentSpeed,
                CardAttributeType.Defense => currentDefense,
                CardAttributeType.Focus => currentFocus,
                CardAttributeType.Resistance => currentResistance,
                _ => throw new ArgumentOutOfRangeException(nameof(attributeType), attributeType, null)
            };
        }

        public int GetBaseAttribute(CardAttributeType attributeType)
        {
            return attributeType switch
            {
                CardAttributeType.Attack => BaseAttack,
                CardAttributeType.Health => BaseHealth,
                CardAttributeType.Speed => BaseSpeed,
                CardAttributeType.Defense => BaseDefense,
                CardAttributeType.Focus => BaseFocus,
                CardAttributeType.Resistance => BaseResistance,
                _ => throw new ArgumentOutOfRangeException(nameof(attributeType), attributeType, null)
            };
        }

        public void AddAttribute(CardAttributeType attributeType, int amount)
        {
            switch (attributeType)
            {
                case CardAttributeType.Attack:
                    currentAttack = Mathf.Max(0, currentAttack + amount);
                    break;

                case CardAttributeType.Health:
                    currentHealth = Mathf.Max(0, currentHealth + amount);
                    if (currentHealth <= 0)
                    {
                        Destroy();
                    }
                    break;

                case CardAttributeType.Speed:
                    currentSpeed = Mathf.Max(0, currentSpeed + amount);
                    break;

                case CardAttributeType.Defense:
                    currentDefense = Mathf.Max(0, currentDefense + amount);
                    break;

                case CardAttributeType.Focus:
                    currentFocus = Mathf.Max(0, currentFocus + amount);
                    break;

                case CardAttributeType.Resistance:
                    currentResistance = Mathf.Max(0, currentResistance + amount);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeType), attributeType, null);
            }
        }

        public void SetAttribute(CardAttributeType attributeType, int value)
        {
            int safeValue = Mathf.Max(0, value);

            switch (attributeType)
            {
                case CardAttributeType.Attack:
                    currentAttack = safeValue;
                    break;

                case CardAttributeType.Health:
                    currentHealth = safeValue;
                    if (currentHealth <= 0)
                    {
                        Destroy();
                    }
                    break;

                case CardAttributeType.Speed:
                    currentSpeed = safeValue;
                    break;

                case CardAttributeType.Defense:
                    currentDefense = safeValue;
                    break;

                case CardAttributeType.Focus:
                    currentFocus = safeValue;
                    break;

                case CardAttributeType.Resistance:
                    currentResistance = safeValue;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(attributeType), attributeType, null);
            }
        }

        public void TakeDamage(int damageAmount)
        {
            if (isDestroyed)
            {
                return;
            }

            int safeDamage = Mathf.Max(0, damageAmount);

            if (safeDamage <= 0)
            {
                return;
            }

            if (hasTemporaryShield)
            {
                hasTemporaryShield = false;
                return;
            }

            int reducedDamage = Mathf.Max(0, safeDamage - currentDefense);

            currentHealth = Mathf.Max(0, currentHealth - reducedDamage);

            if (currentHealth <= 0)
            {
                Destroy();
            }
        }

        public void Heal(int healAmount)
        {
            if (isDestroyed)
            {
                return;
            }

            int safeHeal = Mathf.Max(0, healAmount);

            if (safeHeal <= 0)
            {
                return;
            }

            currentHealth += safeHeal;
        }

        public void AddTemporaryShield()
        {
            if (isDestroyed)
            {
                return;
            }

            hasTemporaryShield = true;
        }

        public void RemoveTemporaryShield()
        {
            hasTemporaryShield = false;
        }

        public void MarkAsAttacked()
        {
            hasAttackedThisTurn = true;
        }

        public void ResetTurnState()
        {
            hasAttackedThisTurn = false;
        }

        public int GetAttackCountBySpeed()
        {
            if (currentSpeed >= 20)
            {
                return 3;
            }

            if (currentSpeed >= 10)
            {
                return 2;
            }

            return 1;
        }

        public List<CardStatusType> GetCurrentStatuses()
        {
            List<CardStatusType> statuses = new();

            AddNativeStatuses(statuses);
            AddTemporaryStatuses(statuses);
            AddAttributeBuffStatuses(statuses);
            AddAttributeTierStatuses(statuses);

            return statuses;
        }

        private void AddNativeStatuses(List<CardStatusType> statuses)
        {
            if (data == null)
            {
                return;
            }

            if (data.HasTaunt)
            {
                statuses.Add(CardStatusType.Taunting);
            }

            if (data.HasPiercing)
            {
                statuses.Add(CardStatusType.Piercing);
            }

            if (data.HasLifeSteal)
            {
                statuses.Add(CardStatusType.LifeStealing);
            }

            if (data.HasRetaliation)
            {
                statuses.Add(CardStatusType.Retaliating);
            }

            if (data.CanAttackDirectly)
            {
                statuses.Add(CardStatusType.DirectAttacker);
            }
        }

        private void AddTemporaryStatuses(List<CardStatusType> statuses)
        {
            if (hasTemporaryShield)
            {
                statuses.Add(CardStatusType.Shielded);
            }
        }

        private void AddAttributeBuffStatuses(List<CardStatusType> statuses)
        {
            AddBuffOrDebuffStatus(
                statuses,
                currentAttack,
                BaseAttack,
                CardStatusType.BuffedAttack,
                CardStatusType.DebuffedAttack
            );

            AddBuffOrDebuffStatus(
                statuses,
                currentSpeed,
                BaseSpeed,
                CardStatusType.BuffedSpeed,
                CardStatusType.DebuffedSpeed
            );

            AddBuffOrDebuffStatus(
                statuses,
                currentDefense,
                BaseDefense,
                CardStatusType.BuffedDefense,
                CardStatusType.DebuffedDefense
            );

            AddBuffOrDebuffStatus(
                statuses,
                currentFocus,
                BaseFocus,
                CardStatusType.BuffedFocus,
                CardStatusType.DebuffedFocus
            );

            AddBuffOrDebuffStatus(
                statuses,
                currentResistance,
                BaseResistance,
                CardStatusType.BuffedResistance,
                CardStatusType.DebuffedResistance
            );
        }

        private void AddBuffOrDebuffStatus(
            List<CardStatusType> statuses,
            int currentValue,
            int baseValue,
            CardStatusType buffedStatus,
            CardStatusType debuffedStatus)
        {
            if (currentValue > baseValue)
            {
                statuses.Add(buffedStatus);
                return;
            }

            if (currentValue < baseValue)
            {
                statuses.Add(debuffedStatus);
            }
        }

        private void AddAttributeTierStatuses(List<CardStatusType> statuses)
        {
            AddTierStatus(statuses, currentSpeed, CardStatusType.SpeedTier1, CardStatusType.SpeedTier2);
            AddTierStatus(statuses, currentAttack, CardStatusType.AttackTier1, CardStatusType.AttackTier2);
            AddTierStatus(statuses, currentDefense, CardStatusType.DefenseTier1, CardStatusType.DefenseTier2);
            AddTierStatus(statuses, currentFocus, CardStatusType.FocusTier1, CardStatusType.FocusTier2);
            AddTierStatus(statuses, currentResistance, CardStatusType.ResistanceTier1, CardStatusType.ResistanceTier2);
        }

        private void AddTierStatus(
            List<CardStatusType> statuses,
            int value,
            CardStatusType tier1Status,
            CardStatusType tier2Status)
        {
            if (value >= 20)
            {
                statuses.Add(tier2Status);
                return;
            }

            if (value >= 10)
            {
                statuses.Add(tier1Status);
            }
        }

        public void Destroy()
        {
            isDestroyed = true;
            currentHealth = 0;
            hasTemporaryShield = false;
        }
    }
}