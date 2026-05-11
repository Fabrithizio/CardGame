// Assets/_Project/Scripts/Visual/ThemedImage.cs

using UnityEngine;
using UnityEngine.UI;

namespace CardGame.Visual
{
    [RequireComponent(typeof(Image))]
    public sealed class ThemedImage : MonoBehaviour
    {
        public enum ThemeSpriteSlot
        {
            BoardBattleCosmic,

            ArenaBackgroundMain,
            CombatBand,

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

        [Header("Theme Sprite")]
        [SerializeField] private ThemeSpriteSlot spriteSlot;

        [Header("Image Settings")]
        [SerializeField] private bool preserveAspect = true;
        [SerializeField] private bool setNativeSizeOnApply;
        [SerializeField] private bool applyOnStart = true;

        private Image cachedImage;

        private void Awake()
        {
            cachedImage = GetComponent<Image>();
        }

        private void Start()
        {
            if (applyOnStart)
            {
                Apply();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                cachedImage = GetComponent<Image>();
            }
        }
#endif

        [ContextMenu("Apply Theme Sprite")]
        public void Apply()
        {
            if (cachedImage == null)
            {
                cachedImage = GetComponent<Image>();
            }

            CardGameArtTheme theme = CardGameArtThemeProvider.Current;

            if (theme == null)
            {
                Debug.LogWarning(
                    $"[ThemedImage] Nenhum tema ativo encontrado para '{gameObject.name}'. " +
                    "Adicione um CardGameArtThemeProvider na cena."
                );
                return;
            }

            Sprite sprite = ResolveSprite(theme);

            if (sprite == null)
            {
                Debug.LogWarning(
                    $"[ThemedImage] Sprite '{spriteSlot}' está vazio no tema. Objeto: {gameObject.name}"
                );
                return;
            }

            cachedImage.sprite = sprite;
            cachedImage.preserveAspect = preserveAspect;

            if (setNativeSizeOnApply)
            {
                cachedImage.SetNativeSize();
            }
        }

        private Sprite ResolveSprite(CardGameArtTheme theme)
        {
            return spriteSlot switch
            {
                ThemeSpriteSlot.BoardBattleCosmic => theme.boardBattleCosmic,

                ThemeSpriteSlot.ArenaBackgroundMain => theme.arenaBackgroundMain,
                ThemeSpriteSlot.CombatBand => theme.combatBand,

                ThemeSpriteSlot.CardBackCosmic => theme.cardBackCosmic,

                ThemeSpriteSlot.FrameCreature => theme.frameCreature,
                ThemeSpriteSlot.FrameSpell => theme.frameSpell,
                ThemeSpriteSlot.FrameTrap => theme.frameTrap,
                ThemeSpriteSlot.FrameMythic => theme.frameMythic,

                ThemeSpriteSlot.HudLeftPanel => theme.hudLeftPanel,
                ThemeSpriteSlot.HudStatusPanel => theme.hudStatusPanel,

                ThemeSpriteSlot.MainButtonFrame => theme.mainButtonFrame,

                ThemeSpriteSlot.CardBackDefault => theme.cardBackDefault,
                ThemeSpriteSlot.SlotCreatureEmpty => theme.slotCreatureEmpty,
                ThemeSpriteSlot.SlotTrapEmpty => theme.slotTrapEmpty,
                ThemeSpriteSlot.MythicSlot => theme.mythicSlot,

                ThemeSpriteSlot.IconSpeed => theme.iconSpeed,
                ThemeSpriteSlot.IconDefense => theme.iconDefense,
                ThemeSpriteSlot.IconFocus => theme.iconFocus,
                ThemeSpriteSlot.IconResistance => theme.iconResistance,

                ThemeSpriteSlot.BuffVortex => theme.buffVortex,
                ThemeSpriteSlot.BuffSkull => theme.buffSkull,
                ThemeSpriteSlot.BuffShield => theme.buffShield,
                ThemeSpriteSlot.BuffSword => theme.buffSword,
                ThemeSpriteSlot.BuffHourglass => theme.buffHourglass,

                ThemeSpriteSlot.VfxSlashViolet => theme.vfxSlashViolet,
                ThemeSpriteSlot.VfxHealGold => theme.vfxHealGold,
                ThemeSpriteSlot.VfxMythicBurst => theme.vfxMythicBurst,
                ThemeSpriteSlot.VfxShieldBlue => theme.vfxShieldBlue,

                _ => null
            };
        }
    }
}