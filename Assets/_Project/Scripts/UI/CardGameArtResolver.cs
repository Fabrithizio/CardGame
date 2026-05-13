// Caminho: Assets/_Project/Scripts/UI/CardGameArtResolver.cs
// Descrição: Resolve sprites do tema e da cena com fallback seguro, evitando quadrados brancos
// quando alguma referência visual ainda não está ligada no inspector.

using System;
using System.Reflection;
using CardGame.Cards;
using CardGame.Visual;
using UnityEngine;

namespace CardGame.UI
{
    public static class CardGameArtResolver
    {
        private static MonoBehaviour cachedSceneArtComponent;
        private static float nextRefreshTime;

        public static CardGameArtTheme Theme => CardGameArtThemeProvider.Current;

        public static Sprite CreatureSlot => FirstNonNull(
            Theme != null ? Theme.slotCreatureEmpty : null,
            GetSceneSprite("creatureSlotSprite", "CreatureSlotSprite"),
            Theme != null ? Theme.frameCreature : null);

        public static Sprite TrapSlot => FirstNonNull(
            Theme != null ? Theme.slotTrapEmpty : null,
            GetSceneSprite("trapSlotSprite", "TrapSlotSprite"),
            Theme != null ? Theme.frameTrap : null);

        public static Sprite CardBack => FirstNonNull(
            Theme != null ? Theme.cardBackCosmic : null,
            GetSceneSprite("cardBackSprite", "CardBackSprite"));

        public static Sprite FrameCreature => FirstNonNull(
            Theme != null ? Theme.frameCreature : null,
            GetSceneSprite("frameCreature", "creatureFrameSprite", "CreatureFrameSprite"));

        public static Sprite FrameSpell => FirstNonNull(
            Theme != null ? Theme.frameSpell : null,
            GetSceneSprite("frameSpell", "spellFrameSprite", "SpellFrameSprite"),
            FrameCreature);

        public static Sprite FrameTrap => FirstNonNull(
            Theme != null ? Theme.frameTrap : null,
            GetSceneSprite("frameTrap", "trapFrameSprite", "TrapFrameSprite"),
            FrameCreature);

        public static Sprite FrameMythic => FirstNonNull(
            GetSceneSprite("frameMythic", "mythicFrameSprite", "MythicFrameSprite"),
            FrameCreature);

        public static Sprite HudLeftPanel => FirstNonNull(
            GetSceneSprite("hudLeftPanelSprite", "HudLeftPanelSprite"));

        public static Sprite HudStatusPanel => FirstNonNull(
            GetSceneSprite("hudStatusPanelSprite", "HudStatusPanelSprite"));

        public static Sprite MainButtonFrame => FirstNonNull(
            GetSceneSprite("mainButtonFrameSprite", "MainButtonFrameSprite"));

        public static Sprite GetFrame(CardType type)
        {
            return type switch
            {
                CardType.Creature => FrameCreature,
                CardType.Spell => FrameSpell,
                CardType.Trap => FrameTrap,
                CardType.Equipment => FrameMythic,
                _ => FrameCreature
            };
        }

        private static Sprite GetSceneSprite(params string[] memberNames)
        {
            MonoBehaviour component = FindSceneArtComponent();
            if (component == null || memberNames == null || memberNames.Length == 0)
            {
                return null;
            }

            Type type = component.GetType();

            for (int i = 0; i < memberNames.Length; i++)
            {
                string name = memberNames[i];
                if (string.IsNullOrWhiteSpace(name)) continue;

                FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                if (field != null && typeof(Sprite).IsAssignableFrom(field.FieldType))
                {
                    Sprite sprite = field.GetValue(component) as Sprite;
                    if (sprite != null) return sprite;
                }

                PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
                if (property != null && typeof(Sprite).IsAssignableFrom(property.PropertyType))
                {
                    Sprite sprite = property.GetValue(component, null) as Sprite;
                    if (sprite != null) return sprite;
                }
            }

            return null;
        }

        private static MonoBehaviour FindSceneArtComponent()
        {
            if (cachedSceneArtComponent != null)
            {
                return cachedSceneArtComponent;
            }

            if (Time.unscaledTime < nextRefreshTime)
            {
                return null;
            }

            nextRefreshTime = Time.unscaledTime + 1f;

            MonoBehaviour[] behaviours = UnityEngine.Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            for (int i = 0; i < behaviours.Length; i++)
            {
                MonoBehaviour behaviour = behaviours[i];
                if (behaviour == null) continue;

                string typeName = behaviour.GetType().Name;
                if (typeName == "BattleScreenArtUI" || typeName == "CardGameArtThemeProvider")
                {
                    cachedSceneArtComponent = behaviour;
                    return cachedSceneArtComponent;
                }
            }

            return null;
        }

        private static Sprite FirstNonNull(params Sprite[] sprites)
        {
            if (sprites == null) return null;
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i] != null) return sprites[i];
            }
            return null;
        }
    }
}
