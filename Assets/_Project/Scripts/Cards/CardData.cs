// Caminho: Assets/_Project/Scripts/Cards/CardData.cs
// Descrição: Define os dados fixos de uma carta do deck, como nome, imagem, tipo, raridade, custo, atributos base e configuração inicial de armadilhas.

using System;
using CardGame.Effects;
using UnityEngine;

namespace CardGame.Cards
{
    [CreateAssetMenu(
        fileName = "New Card Data",
        menuName = "Card Game/Cards/Card Data"
    )]
    public class CardData : ScriptableObject
    {
        [Header("Identidade da Carta")]
        [SerializeField] private string cardId;
        [SerializeField] private string cardName;

        [TextArea(3, 6)]
        [SerializeField] private string description;

        [SerializeField] private Sprite artwork;

        [Header("Classificação")]
        [SerializeField] private CardType cardType;
        [SerializeField] private CardRarity rarity;

        [Header("Custo")]
        [Min(0)]
        [SerializeField] private int cost;

        [Header("Atributos Base")]
        [Min(0)]
        [SerializeField] private int attack;

        [Min(0)]
        [SerializeField] private int health;

        [Min(0)]
        [SerializeField] private int speed;

        [Min(0)]
        [SerializeField] private int defense;

        [Min(0)]
        [SerializeField] private int focus;

        [Min(0)]
        [SerializeField] private int resistance;

        [Header("Regras Especiais de Criatura")]
        [SerializeField] private bool canAttackDirectly;
        [SerializeField] private bool hasTaunt;
        [SerializeField] private bool hasPiercing;
        [SerializeField] private bool hasLifeSteal;
        [SerializeField] private bool hasRetaliation;
        [SerializeField] private bool hasShield;

        [Header("Configuração de Armadilha")]
        [SerializeField] private TrapTriggerType trapTriggerType = TrapTriggerType.None;
        [SerializeField] private TrapEffectType trapEffectType = TrapEffectType.None;

        [Min(0)]
        [SerializeField] private int trapEffectValue;

        public string CardId => cardId;
        public string CardName => cardName;
        public string Description => description;
        public Sprite Artwork => artwork;

        public CardType CardType => cardType;
        public CardRarity Rarity => rarity;

        public int Cost => cost;

        public int Attack => attack;
        public int Health => health;
        public int Speed => speed;
        public int Defense => defense;
        public int Focus => focus;
        public int Resistance => resistance;

        public bool CanAttackDirectly => canAttackDirectly;
        public bool HasTaunt => hasTaunt;
        public bool HasPiercing => hasPiercing;
        public bool HasLifeSteal => hasLifeSteal;
        public bool HasRetaliation => hasRetaliation;
        public bool HasShield => hasShield;

        public TrapTriggerType TrapTriggerType => trapTriggerType;
        public TrapEffectType TrapEffectType => trapEffectType;
        public int TrapEffectValue => trapEffectValue;

        public int GetAttribute(CardAttributeType attributeType)
        {
            return attributeType switch
            {
                CardAttributeType.Attack => attack,
                CardAttributeType.Health => health,
                CardAttributeType.Speed => speed,
                CardAttributeType.Defense => defense,
                CardAttributeType.Focus => focus,
                CardAttributeType.Resistance => resistance,
                _ => throw new ArgumentOutOfRangeException(nameof(attributeType), attributeType, null)
            };
        }

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(cardId))
            {
                cardId = Guid.NewGuid().ToString();
            }

            cost = Mathf.Max(0, cost);
            attack = Mathf.Max(0, attack);
            health = Mathf.Max(0, health);
            speed = Mathf.Max(0, speed);
            defense = Mathf.Max(0, defense);
            focus = Mathf.Max(0, focus);
            resistance = Mathf.Max(0, resistance);
            trapEffectValue = Mathf.Max(0, trapEffectValue);

            if (cardType != CardType.Trap)
            {
                trapTriggerType = TrapTriggerType.None;
                trapEffectType = TrapEffectType.None;
                trapEffectValue = 0;
            }
        }
    }
}
