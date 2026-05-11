// Assets/_Project/Scripts/Visual/BattleSceneThemeApplier.cs

using System;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.Visual
{
    [DisallowMultipleComponent]
    public sealed class BattleSceneThemeApplier : MonoBehaviour
    {
        [Serializable]
        private sealed class ThemedImageBinding
        {
            public string name;
            public Image image;
            public ThemedImageSlot slot;
            public bool preserveAspect = true;
            public bool setNativeSize;
        }

        private enum ThemedImageSlot
        {
            BoardBattleCosmic,

            CardBackCosmic,

            FrameCreature,
            FrameSpell,
            FrameTrap,
            FrameMythic,

            HudLeftPanel,
            HudStatusPanel,

            MainButtonFrame,

            CardBackDefault,
            SlotCreatureEmpty,
            SlotTrapEmpty,
            MythicSlot,

            IconSpeed,
            IconDefense,
            IconFocus,
            IconResistance,

            BuffVortex,
            BuffSkull,
            BuffShield,
            BuffSword,
            BuffHourglass,

            VfxSlashViolet,
            VfxHealGold,
            VfxMythicBurst,
            VfxShieldBlue
        }

        [Header("Auto Apply")]
        [SerializeField] private bool applyOnStart = true;

        [Header("Main Scene Images")]
        [SerializeField] private Image boardBackgroundImage;
        [SerializeField] private Image playerHudPanelImage;
        [SerializeField] private Image enemyHudPanelImage;
        [SerializeField] private Image turnStatusPanelImage;
        [SerializeField] private Image phaseButtonImage;
        [SerializeField] private Image mythicSlotImage;

        [Header("Optional Extra Bindings")]
        [SerializeField] private ThemedImageBinding[] extraBindings = Array.Empty<ThemedImageBinding>();

        private void Start()
        {
            if (applyOnStart)
            {
                ApplyTheme();
            }
        }

        [ContextMenu("Apply Theme")]
        public void ApplyTheme()
        {
            CardGameArtTheme theme = CardGameArtThemeProvider.Current;

            if (theme == null)
            {
                Debug.LogWarning("[BattleSceneThemeApplier] Nenhum CardGameArtThemeProvider ativo foi encontrado na cena.");
                return;
            }

            ApplyImage(boardBackgroundImage, theme.boardBattleCosmic, true, false);

            ApplyImage(playerHudPanelImage, theme.hudStatusPanel, true, false);
            ApplyImage(enemyHudPanelImage, theme.hudStatusPanel, true, false);
            ApplyImage(turnStatusPanelImage, theme.hudStatusPanel, true, false);
            ApplyImage(phaseButtonImage, theme.mainButtonFrame, true, false);
            ApplyImage(mythicSlotImage, theme.mythicSlot, true, false);

            for (int i = 0; i < extraBindings.Length; i++)
            {
                ThemedImageBinding binding = extraBindings[i];

                if (binding == null || binding.image == null)
                {
                    continue;
                }

                Sprite sprite = ResolveSprite(theme, binding.slot);
                ApplyImage(binding.image, sprite, binding.preserveAspect, binding.setNativeSize);
            }

            Debug.Log("[BattleSceneThemeApplier] Tema visual aplicado na cena.");
        }

        private static void ApplyImage(Image image, Sprite sprite, bool preserveAspect, bool setNativeSize)
        {
            if (image == null || sprite == null)
            {
                return;
            }

            image.sprite = sprite;
            image.preserveAspect = preserveAspect;

            Color color = image.color;
            color.a = 1f;
            image.color = color;

            if (setNativeSize)
            {
                image.SetNativeSize();
            }
        }

        private static Sprite ResolveSprite(CardGameArtTheme theme, ThemedImageSlot slot)
        {
            return slot switch
            {
                ThemedImageSlot.BoardBattleCosmic => theme.boardBattleCosmic,

                ThemedImageSlot.CardBackCosmic => theme.cardBackCosmic,

                ThemedImageSlot.FrameCreature => theme.frameCreature,
                ThemedImageSlot.FrameSpell => theme.frameSpell,
                ThemedImageSlot.FrameTrap => theme.frameTrap,
                ThemedImageSlot.FrameMythic => theme.frameMythic,

                ThemedImageSlot.HudLeftPanel => theme.hudLeftPanel,
                ThemedImageSlot.HudStatusPanel => theme.hudStatusPanel,

                ThemedImageSlot.MainButtonFrame => theme.mainButtonFrame,

                ThemedImageSlot.CardBackDefault => theme.cardBackDefault,
                ThemedImageSlot.SlotCreatureEmpty => theme.slotCreatureEmpty,
                ThemedImageSlot.SlotTrapEmpty => theme.slotTrapEmpty,
                ThemedImageSlot.MythicSlot => theme.mythicSlot,

                ThemedImageSlot.IconSpeed => theme.iconSpeed,
                ThemedImageSlot.IconDefense => theme.iconDefense,
                ThemedImageSlot.IconFocus => theme.iconFocus,
                ThemedImageSlot.IconResistance => theme.iconResistance,

                ThemedImageSlot.BuffVortex => theme.buffVortex,
                ThemedImageSlot.BuffSkull => theme.buffSkull,
                ThemedImageSlot.BuffShield => theme.buffShield,
                ThemedImageSlot.BuffSword => theme.buffSword,
                ThemedImageSlot.BuffHourglass => theme.buffHourglass,

                ThemedImageSlot.VfxSlashViolet => theme.vfxSlashViolet,
                ThemedImageSlot.VfxHealGold => theme.vfxHealGold,
                ThemedImageSlot.VfxMythicBurst => theme.vfxMythicBurst,
                ThemedImageSlot.VfxShieldBlue => theme.vfxShieldBlue,

                _ => null
            };
        }
    }
}