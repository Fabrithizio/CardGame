// Assets/_Project/Scripts/Visual/CardGameArtTheme.cs

using System;
using UnityEngine;

namespace CardGame.Visual
{
    public enum CardVisualKind
    {
        Creature,
        Spell,
        Trap,
        Mythic
    }

    public enum StatIconKind
    {
        Speed,
        Defense,
        Focus,
        Resistance
    }

    public enum BuffIconKind
    {
        Vortex,
        Skull,
        Shield,
        Sword,
        Hourglass
    }

    public enum VfxKind
    {
        SlashViolet,
        HealGold,
        MythicBurst,
        ShieldBlue
    }

    [CreateAssetMenu(
        fileName = "CardGameArtTheme",
        menuName = "Card Game/Visual/Card Game Art Theme"
    )]
    public sealed class CardGameArtTheme : ScriptableObject
    {
        [Header("Board")]
        public Sprite boardBattleCosmic;

        [Header("Arena / Extra Backgrounds")]
        public Sprite arenaBackgroundMain;
        public Sprite combatBand;

        [Header("Card Backs")]
        public Sprite cardBackCosmic;

        [Header("Card Frames")]
        public Sprite frameCreature;
        public Sprite frameSpell;
        public Sprite frameTrap;
        public Sprite frameMythic;

        [Header("HUD")]
        public Sprite hudLeftPanel;
        public Sprite hudStatusPanel;

        [Header("Buttons")]
        public Sprite mainButtonFrame;

        [Header("Slots")]
        public Sprite cardBackDefault;
        public Sprite slotCreatureEmpty;
        public Sprite slotTrapEmpty;
        public Sprite mythicSlot;

        [Header("Stat Icons")]
        public Sprite iconSpeed;
        public Sprite iconDefense;
        public Sprite iconFocus;
        public Sprite iconResistance;

        [Header("Buff Icons")]
        public Sprite buffVortex;
        public Sprite buffSkull;
        public Sprite buffShield;
        public Sprite buffSword;
        public Sprite buffHourglass;

        [Header("VFX")]
        public Sprite vfxSlashViolet;
        public Sprite vfxHealGold;
        public Sprite vfxMythicBurst;
        public Sprite vfxShieldBlue;

        public Sprite GetFrame(CardVisualKind kind)
        {
            return kind switch
            {
                CardVisualKind.Creature => frameCreature,
                CardVisualKind.Spell => frameSpell,
                CardVisualKind.Trap => frameTrap,
                CardVisualKind.Mythic => frameMythic,
                _ => frameCreature
            };
        }

        public Sprite GetStatIcon(StatIconKind kind)
        {
            return kind switch
            {
                StatIconKind.Speed => iconSpeed,
                StatIconKind.Defense => iconDefense,
                StatIconKind.Focus => iconFocus,
                StatIconKind.Resistance => iconResistance,
                _ => null
            };
        }

        public Sprite GetBuffIcon(BuffIconKind kind)
        {
            return kind switch
            {
                BuffIconKind.Vortex => buffVortex,
                BuffIconKind.Skull => buffSkull,
                BuffIconKind.Shield => buffShield,
                BuffIconKind.Sword => buffSword,
                BuffIconKind.Hourglass => buffHourglass,
                _ => null
            };
        }

        public Sprite GetVfx(VfxKind kind)
        {
            return kind switch
            {
                VfxKind.SlashViolet => vfxSlashViolet,
                VfxKind.HealGold => vfxHealGold,
                VfxKind.MythicBurst => vfxMythicBurst,
                VfxKind.ShieldBlue => vfxShieldBlue,
                _ => null
            };
        }

        public bool HasEssentialSprites()
        {
            return boardBattleCosmic != null
                   && cardBackCosmic != null
                   && frameCreature != null
                   && frameSpell != null
                   && frameTrap != null
                   && frameMythic != null;
        }

        public void ValidateTheme()
        {
            if (!HasEssentialSprites())
            {
                Debug.LogWarning(
                    "[CardGameArtTheme] O tema visual está incompleto. " +
                    "Verifique board, card back e as 4 molduras principais."
                );
            }
        }
    }
}