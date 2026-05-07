// Caminho: Assets/_Project/Scripts/Editor/CardGameTestContentCreator.cs
// Descrição: Ferramenta de Editor para criar automaticamente as primeiras cartas de teste como ScriptableObjects em Assets/_Project/ScriptableObjects/Cards.

#if UNITY_EDITOR

using System.Collections.Generic;
using CardGame.Cards;
using UnityEditor;
using UnityEngine;

namespace CardGame.EditorTools
{
    public static class CardGameTestContentCreator
    {
        private const string CardsFolder = "Assets/_Project/ScriptableObjects/Cards";

        [MenuItem("Card Game/Content/Criar Deck Inicial de Teste 01")]
        public static void CreateInitialTestDeckCards()
        {
            EnsureFolder("Assets/_Project", "ScriptableObjects");
            EnsureFolder("Assets/_Project/ScriptableObjects", "Cards");

            CreateOrUpdateCard(
                assetName: "Creature_Lury",
                cardName: "Lury",
                description: "Espírito de fogo veloz que espalha queimadura.",
                cardType: CardType.Creature,
                rarity: CardRarity.Rare,
                cost: 3,
                attack: 4,
                health: 7,
                speed: 7,
                defense: 2,
                focus: 3,
                resistance: 3,
                canAttackDirectly: false,
                hasTaunt: false,
                hasPiercing: false,
                hasLifeSteal: false,
                hasRetaliation: false,
                hasShield: false
            );

            CreateOrUpdateCard(
                assetName: "Creature_Luvy",
                cardName: "Luvy",
                description: "Espírito aquático de suporte e proteção.",
                cardType: CardType.Creature,
                rarity: CardRarity.Rare,
                cost: 3,
                attack: 2,
                health: 8,
                speed: 6,
                defense: 3,
                focus: 3,
                resistance: 4,
                canAttackDirectly: false,
                hasTaunt: false,
                hasPiercing: false,
                hasLifeSteal: false,
                hasRetaliation: false,
                hasShield: false
            );

            CreateOrUpdateCard(
                assetName: "Creature_Turga",
                cardName: "Turga",
                description: "Guardião natural focado em defesa e proteção.",
                cardType: CardType.Creature,
                rarity: CardRarity.Rare,
                cost: 3,
                attack: 2,
                health: 10,
                speed: 3,
                defense: 6,
                focus: 1,
                resistance: 4,
                canAttackDirectly: false,
                hasTaunt: true,
                hasPiercing: false,
                hasLifeSteal: false,
                hasRetaliation: false,
                hasShield: false
            );

            CreateOrUpdateCard(
                assetName: "Creature_Glovy",
                cardName: "Glovy",
                description: "Espírito sombrio que atrapalha o inimigo.",
                cardType: CardType.Creature,
                rarity: CardRarity.Epic,
                cost: 4,
                attack: 4,
                health: 7,
                speed: 6,
                defense: 2,
                focus: 4,
                resistance: 4,
                canAttackDirectly: false,
                hasTaunt: false,
                hasPiercing: false,
                hasLifeSteal: false,
                hasRetaliation: false,
                hasShield: false
            );

            CreateOrUpdateCard(
                assetName: "Creature_Cavaleiro_Rubro_Dos_Estilhacos",
                cardName: "Cavaleiro Rubro dos Estilhaços",
                description: "Guerreiro agressivo que atravessa defesas.",
                cardType: CardType.Creature,
                rarity: CardRarity.Epic,
                cost: 5,
                attack: 6,
                health: 8,
                speed: 4,
                defense: 4,
                focus: 3,
                resistance: 2,
                canAttackDirectly: false,
                hasTaunt: false,
                hasPiercing: true,
                hasLifeSteal: false,
                hasRetaliation: false,
                hasShield: false
            );

            CreateOrUpdateCard(
                assetName: "Creature_Colosso_Runico_Dourado",
                cardName: "Colosso Rúnico Dourado",
                description: "Gigante de pedra que segura a linha de frente.",
                cardType: CardType.Creature,
                rarity: CardRarity.Epic,
                cost: 5,
                attack: 5,
                health: 10,
                speed: 2,
                defense: 6,
                focus: 1,
                resistance: 3,
                canAttackDirectly: false,
                hasTaunt: true,
                hasPiercing: false,
                hasLifeSteal: false,
                hasRetaliation: false,
                hasShield: true
            );

            CreateOrUpdateCard(
                assetName: "Creature_Anciao_Do_Brejo_Luminoso",
                cardName: "Ancião do Brejo Luminoso",
                description: "Guardião vivo do pântano, resistente e regenerativo.",
                cardType: CardType.Creature,
                rarity: CardRarity.Epic,
                cost: 5,
                attack: 4,
                health: 10,
                speed: 3,
                defense: 5,
                focus: 1,
                resistance: 4,
                canAttackDirectly: false,
                hasTaunt: false,
                hasPiercing: false,
                hasLifeSteal: false,
                hasRetaliation: false,
                hasShield: false
            );

            CreateOrUpdateCard(
                assetName: "Spell_Orbe_Igneo",
                cardName: "Orbe Ígneo",
                description: "Magia rápida: causa 2 de dano a 1 criatura em campo.",
                cardType: CardType.Spell,
                rarity: CardRarity.Common,
                cost: 1,
                attack: 0,
                health: 0,
                speed: 0,
                defense: 0,
                focus: 0,
                resistance: 0,
                canAttackDirectly: false,
                hasTaunt: false,
                hasPiercing: false,
                hasLifeSteal: false,
                hasRetaliation: false,
                hasShield: false
            );

            CreateOrUpdateCard(
                assetName: "Spell_Aprisionamento_Da_Alma",
                cardName: "Aprisionamento da Alma",
                description: "Magia de ação: criatura inimiga alvo não pode atacar por 2 turnos.",
                cardType: CardType.Spell,
                rarity: CardRarity.Common,
                cost: 2,
                attack: 0,
                health: 0,
                speed: 0,
                defense: 0,
                focus: 0,
                resistance: 0,
                canAttackDirectly: false,
                hasTaunt: false,
                hasPiercing: false,
                hasLifeSteal: false,
                hasRetaliation: false,
                hasShield: false
            );

            CreateOrUpdateCard(
                assetName: "Spell_Florescer_Da_Loucura",
                cardName: "Florescer da Loucura",
                description: "Magia de ação: criatura inimiga perde 3 Speed e 2 Focus temporariamente.",
                cardType: CardType.Spell,
                rarity: CardRarity.Rare,
                cost: 3,
                attack: 0,
                health: 0,
                speed: 0,
                defense: 0,
                focus: 0,
                resistance: 0,
                canAttackDirectly: false,
                hasTaunt: false,
                hasPiercing: false,
                hasLifeSteal: false,
                hasRetaliation: false,
                hasShield: false
            );

            CreateOrUpdateCard(
                assetName: "Spell_Raio_De_Ruptura",
                cardName: "Raio de Ruptura",
                description: "Magia de ação: causa 4 de dano a uma criatura inimiga ou ao jogador sem defesa.",
                cardType: CardType.Spell,
                rarity: CardRarity.Epic,
                cost: 4,
                attack: 0,
                health: 0,
                speed: 0,
                defense: 0,
                focus: 0,
                resistance: 0,
                canAttackDirectly: false,
                hasTaunt: false,
                hasPiercing: false,
                hasLifeSteal: false,
                hasRetaliation: false,
                hasShield: false
            );

            CreateOrUpdateCard(
                assetName: "Trap_Poco_De_Estacas",
                cardName: "Poço de Estacas",
                description: "Armadilha: quando uma criatura inimiga declarar ataque, ela sofre dano e perde Speed.",
                cardType: CardType.Trap,
                rarity: CardRarity.Rare,
                cost: 2,
                attack: 0,
                health: 0,
                speed: 0,
                defense: 0,
                focus: 0,
                resistance: 0,
                canAttackDirectly: false,
                hasTaunt: false,
                hasPiercing: false,
                hasLifeSteal: false,
                hasRetaliation: false,
                hasShield: false
            );

            CreateOrUpdateCard(
                assetName: "Arena_Paz_Celestial",
                cardName: "Paz Celestial",
                description: "Arena: enquanto ativa, atributos não ativam buffs de tier.",
                cardType: CardType.Spell,
                rarity: CardRarity.Epic,
                cost: 1,
                attack: 0,
                health: 0,
                speed: 0,
                defense: 0,
                focus: 0,
                resistance: 0,
                canAttackDirectly: false,
                hasTaunt: false,
                hasPiercing: false,
                hasLifeSteal: false,
                hasRetaliation: false,
                hasShield: false
            );

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Deck Inicial de Teste 01 criado/atualizado em Assets/_Project/ScriptableObjects/Cards.");
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;

            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static void CreateOrUpdateCard(
            string assetName,
            string cardName,
            string description,
            CardType cardType,
            CardRarity rarity,
            int cost,
            int attack,
            int health,
            int speed,
            int defense,
            int focus,
            int resistance,
            bool canAttackDirectly,
            bool hasTaunt,
            bool hasPiercing,
            bool hasLifeSteal,
            bool hasRetaliation,
            bool hasShield)
        {
            string assetPath = $"{CardsFolder}/{assetName}.asset";

            CardData card = AssetDatabase.LoadAssetAtPath<CardData>(assetPath);

            if (card == null)
            {
                card = ScriptableObject.CreateInstance<CardData>();
                AssetDatabase.CreateAsset(card, assetPath);
            }

            SerializedObject serializedCard = new SerializedObject(card);

            SetString(serializedCard, "cardName", cardName);
            SetString(serializedCard, "description", description);
            SetEnum(serializedCard, "cardType", cardType);
            SetEnum(serializedCard, "rarity", rarity);
            SetInt(serializedCard, "cost", cost);
            SetInt(serializedCard, "attack", attack);
            SetInt(serializedCard, "health", health);
            SetInt(serializedCard, "speed", speed);
            SetInt(serializedCard, "defense", defense);
            SetInt(serializedCard, "focus", focus);
            SetInt(serializedCard, "resistance", resistance);
            SetBool(serializedCard, "canAttackDirectly", canAttackDirectly);
            SetBool(serializedCard, "hasTaunt", hasTaunt);
            SetBool(serializedCard, "hasPiercing", hasPiercing);
            SetBool(serializedCard, "hasLifeSteal", hasLifeSteal);
            SetBool(serializedCard, "hasRetaliation", hasRetaliation);
            SetBool(serializedCard, "hasShield", hasShield);

            serializedCard.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(card);
        }

        private static void SetString(SerializedObject serializedObject, string propertyName, string value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);

            if (property != null)
            {
                property.stringValue = value;
            }
        }

        private static void SetInt(SerializedObject serializedObject, string propertyName, int value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);

            if (property != null)
            {
                property.intValue = value;
            }
        }

        private static void SetBool(SerializedObject serializedObject, string propertyName, bool value)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);

            if (property != null)
            {
                property.boolValue = value;
            }
        }

        private static void SetEnum<TEnum>(SerializedObject serializedObject, string propertyName, TEnum value)
            where TEnum : System.Enum
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);

            if (property != null)
            {
                property.enumValueIndex = System.Convert.ToInt32(value);
            }
        }
    }
}

#endif
