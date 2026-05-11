// Assets/_Project/Editor/CardGameArtThemeBuilder.cs

#if UNITY_EDITOR

using System.IO;
using CardGame.Visual;
using UnityEditor;
using UnityEngine;

namespace CardGame.EditorTools
{
    public static class CardGameArtThemeBuilder
    {
        private const string ThemeFolder = "Assets/_Project/Data/Art";
        private const string ThemeAssetPath = ThemeFolder + "/CardGameArtTheme.asset";

        [MenuItem("Card Game/Visual/Create Or Update Art Theme")]
        public static void CreateOrUpdateTheme()
        {
            EnsureFolder("Assets/_Project/Data");
            EnsureFolder(ThemeFolder);

            CardGameArtTheme theme = AssetDatabase.LoadAssetAtPath<CardGameArtTheme>(ThemeAssetPath);

            if (theme == null)
            {
                theme = ScriptableObject.CreateInstance<CardGameArtTheme>();
                AssetDatabase.CreateAsset(theme, ThemeAssetPath);
            }

            AssignSprites(theme);

            EditorUtility.SetDirty(theme);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = theme;

            Debug.Log($"[CardGameArtThemeBuilder] Tema criado/atualizado em: {ThemeAssetPath}");
        }

        private static void AssignSprites(CardGameArtTheme theme)
        {
            theme.boardBattleCosmic = LoadSprite("Assets/_Project/Art/UI/Board/board_battle_cosmic.png");

            theme.arenaBackgroundMain = LoadSprite("Assets/_Project/Art/UI/Arena/arena_bg_main.png");
            theme.combatBand = LoadSprite("Assets/_Project/Art/UI/Arena/combat_band.png");

            theme.cardBackCosmic = LoadSprite("Assets/_Project/Art/UI/CardBacks/card_back_cosmic.png");

            theme.frameCreature = LoadSprite("Assets/_Project/Art/UI/CardFrames/frame_creature.png");
            theme.frameSpell = LoadSprite("Assets/_Project/Art/UI/CardFrames/frame_spell.png");
            theme.frameTrap = LoadSprite("Assets/_Project/Art/UI/CardFrames/frame_trap.png");
            theme.frameMythic = LoadSprite("Assets/_Project/Art/UI/CardFrames/frame_mythic.png");

            theme.hudLeftPanel = LoadSprite("Assets/_Project/Art/UI/HUD/hud_left_panel.png");
            theme.hudStatusPanel = LoadSprite("Assets/_Project/Art/UI/HUD/hud_status_panel.png");

            theme.mainButtonFrame = LoadSprite("Assets/_Project/Art/UI/Buttons/main_button_frame.png");

            theme.cardBackDefault = LoadSprite("Assets/_Project/Art/UI/Slots/card_back_default.png");
            theme.slotCreatureEmpty = LoadSprite("Assets/_Project/Art/UI/Slots/slot_creature_empty.png");
            theme.slotTrapEmpty = LoadSprite("Assets/_Project/Art/UI/Slots/slot_trap_empty.png");
            theme.mythicSlot = LoadSprite("Assets/_Project/Art/UI/Mythics/mythic_slot.png");

            theme.iconSpeed = LoadSprite("Assets/_Project/Art/UI/Icons/Stats/icon_speed.png");
            theme.iconDefense = LoadSprite("Assets/_Project/Art/UI/Icons/Stats/icon_defense.png");
            theme.iconFocus = LoadSprite("Assets/_Project/Art/UI/Icons/Stats/icon_focus.png");
            theme.iconResistance = LoadSprite("Assets/_Project/Art/UI/Icons/Stats/icon_resistance.png");

            theme.buffVortex = LoadSprite("Assets/_Project/Art/UI/Icons/Buffs/buff_vortex.png");
            theme.buffSkull = LoadSprite("Assets/_Project/Art/UI/Icons/Buffs/buff_skull.png");
            theme.buffShield = LoadSprite("Assets/_Project/Art/UI/Icons/Buffs/buff_shield.png");
            theme.buffSword = LoadSprite("Assets/_Project/Art/UI/Icons/Buffs/buff_sword.png");
            theme.buffHourglass = LoadSprite("Assets/_Project/Art/UI/Icons/Buffs/buff_hourglass.png");

            theme.vfxSlashViolet = LoadSprite("Assets/_Project/Art/UI/VFX/vfx_slash_violet.png");
            theme.vfxHealGold = LoadSprite("Assets/_Project/Art/UI/VFX/vfx_heal_gold.png");
            theme.vfxMythicBurst = LoadSprite("Assets/_Project/Art/UI/VFX/vfx_mythic_burst.png");
            theme.vfxShieldBlue = LoadSprite("Assets/_Project/Art/UI/VFX/vfx_shield_blue.png");
        }

        private static Sprite LoadSprite(string path)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite == null)
            {
                Debug.LogWarning($"[CardGameArtThemeBuilder] Sprite não encontrado: {path}");
            }

            return sprite;
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string parent = Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
            string folderName = Path.GetFileName(folderPath);

            if (string.IsNullOrWhiteSpace(parent) || string.IsNullOrWhiteSpace(folderName))
            {
                Debug.LogError($"[CardGameArtThemeBuilder] Caminho inválido: {folderPath}");
                return;
            }

            if (!AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}

#endif