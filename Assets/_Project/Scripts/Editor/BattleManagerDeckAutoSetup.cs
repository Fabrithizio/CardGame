// Caminho: Assets/_Project/Scripts/Editor/BattleManagerDeckAutoSetup.cs
// Descrição: Ferramenta de Editor para preencher automaticamente o BattleManager da cena com o Deck Inicial de Teste 01.

#if UNITY_EDITOR

using System.Collections.Generic;
using CardGame.Cards;
using CardGame.Mythics;
using UnityEditor;
using UnityEngine;

namespace CardGame.EditorTools
{
    public static class BattleManagerDeckAutoSetup
    {
        private const string CardsFolder = "Assets/_Project/ScriptableObjects/Cards";
        private const string MythicsFolder = "Assets/_Project/ScriptableObjects/Mythics";

        [MenuItem("Card Game/Content/Aplicar Deck Inicial no BattleManager")]
        public static void ApplyInitialDeckToBattleManager()
        {
            Object battleManagerObject = FindBattleManagerObject();

            if (battleManagerObject == null)
            {
                Debug.LogError("Não encontrei um BattleManager na cena atual. Abra a cena Battle e tente de novo.");
                return;
            }

            SerializedObject serializedBattleManager = new SerializedObject(battleManagerObject);

            SerializedProperty playerDeckCards = serializedBattleManager.FindProperty("playerDeckCards");
            SerializedProperty enemyDeckCards = serializedBattleManager.FindProperty("enemyDeckCards");
            SerializedProperty playerMythics = serializedBattleManager.FindProperty("playerMythics");
            SerializedProperty enemyMythics = serializedBattleManager.FindProperty("enemyMythics");

            if (playerDeckCards == null || enemyDeckCards == null)
            {
                Debug.LogError("Não encontrei os campos playerDeckCards/enemyDeckCards no BattleManager. O nome dos campos pode ter mudado.");
                return;
            }

            FillCardList(playerDeckCards, BuildPlayerDeck());
            FillCardList(enemyDeckCards, BuildEnemyDeck());

            if (playerMythics != null)
            {
                FillMythicList(playerMythics, BuildPlayerMythics());
            }

            if (enemyMythics != null)
            {
                FillMythicList(enemyMythics, BuildEnemyMythics());
            }

            serializedBattleManager.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(battleManagerObject);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

            Debug.Log("Deck Inicial de Teste 01 aplicado no BattleManager da cena atual.");
        }

        private static Object FindBattleManagerObject()
        {
            MonoBehaviour[] behaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

            for (int i = 0; i < behaviours.Length; i++)
            {
                MonoBehaviour behaviour = behaviours[i];

                if (behaviour == null)
                {
                    continue;
                }

                if (behaviour.GetType().FullName == "CardGame.Battle.BattleManager")
                {
                    return behaviour;
                }
            }

            return null;
        }

        private static List<CardData> BuildPlayerDeck()
        {
            List<CardData> deck = new List<CardData>();

            AddCards(deck, "Creature_Lury", 3);
            AddCards(deck, "Creature_Luvy", 3);
            AddCards(deck, "Creature_Turga", 2);
            AddCards(deck, "Creature_Glovy", 2);
            AddCards(deck, "Creature_Cavaleiro_Rubro_Dos_Estilhacos", 1);
            AddCards(deck, "Creature_Colosso_Runico_Dourado", 1);

            AddCards(deck, "Spell_Orbe_Igneo", 2);
            AddCards(deck, "Spell_Aprisionamento_Da_Alma", 1);
            AddCards(deck, "Spell_Florescer_Da_Loucura", 1);
            AddCards(deck, "Spell_Raio_De_Ruptura", 1);

            AddCards(deck, "Trap_Poco_De_Estacas", 2);

            AddCards(deck, "Arena_Paz_Celestial", 1);

            return deck;
        }

        private static List<CardData> BuildEnemyDeck()
        {
            List<CardData> deck = new List<CardData>();

            AddCards(deck, "Creature_Lury", 4);
            AddCards(deck, "Creature_Turga", 3);
            AddCards(deck, "Creature_Glovy", 2);
            AddCards(deck, "Creature_Cavaleiro_Rubro_Dos_Estilhacos", 2);
            AddCards(deck, "Creature_Colosso_Runico_Dourado", 1);
            AddCards(deck, "Creature_Anciao_Do_Brejo_Luminoso", 1);

            AddCards(deck, "Spell_Orbe_Igneo", 2);
            AddCards(deck, "Spell_Florescer_Da_Loucura", 1);
            AddCards(deck, "Spell_Raio_De_Ruptura", 1);

            AddCards(deck, "Trap_Poco_De_Estacas", 2);

            AddCards(deck, "Arena_Paz_Celestial", 1);

            return deck;
        }

        private static List<MythicData> BuildPlayerMythics()
        {
            List<MythicData> mythics = new List<MythicData>();

            AddMythic(mythics, "New Mythic Data");

            return mythics;
        }

        private static List<MythicData> BuildEnemyMythics()
        {
            List<MythicData> mythics = new List<MythicData>();

            AddMythic(mythics, "New Mythic Data");

            return mythics;
        }

        private static void AddCards(List<CardData> deck, string assetName, int amount)
        {
            CardData card = LoadCard(assetName);

            if (card == null)
            {
                Debug.LogWarning($"Carta não encontrada: {assetName}. Rode primeiro Card Game > Content > Criar Deck Inicial de Teste 01.");
                return;
            }

            for (int i = 0; i < amount; i++)
            {
                deck.Add(card);
            }
        }

        private static CardData LoadCard(string assetName)
        {
            return AssetDatabase.LoadAssetAtPath<CardData>($"{CardsFolder}/{assetName}.asset");
        }

        private static void AddMythic(List<MythicData> mythics, string assetName)
        {
            MythicData mythic = AssetDatabase.LoadAssetAtPath<MythicData>($"{MythicsFolder}/{assetName}.asset");

            if (mythic == null)
            {
                Debug.LogWarning($"Mítico não encontrado: {assetName}. Mantive o loadout sem esse Mítico.");
                return;
            }

            mythics.Add(mythic);
        }

        private static void FillCardList(SerializedProperty listProperty, List<CardData> cards)
        {
            listProperty.ClearArray();

            for (int i = 0; i < cards.Count; i++)
            {
                listProperty.InsertArrayElementAtIndex(i);
                SerializedProperty element = listProperty.GetArrayElementAtIndex(i);
                element.objectReferenceValue = cards[i];
            }
        }

        private static void FillMythicList(SerializedProperty listProperty, List<MythicData> mythics)
        {
            listProperty.ClearArray();

            for (int i = 0; i < mythics.Count; i++)
            {
                listProperty.InsertArrayElementAtIndex(i);
                SerializedProperty element = listProperty.GetArrayElementAtIndex(i);
                element.objectReferenceValue = mythics[i];
            }
        }
    }
}

#endif
