// Caminho: Assets/_Project/Scripts/Mythics/MythicData.cs
// Descrição: Define os dados fixos de um Mítico, que é um poder especial fora do deck, limitado e usado uma vez por partida.

using UnityEngine;

namespace CardGame.Mythics
{
    [CreateAssetMenu(
        fileName = "New Mythic Data",
        menuName = "Card Game/Mythics/Mythic Data"
    )]
    public class MythicData : ScriptableObject
    {
        [Header("Identidade do Mítico")]
        [SerializeField] private string mythicId;
        [SerializeField] private string mythicName;

        [TextArea(3, 6)]
        [SerializeField] private string description;

        [SerializeField] private Sprite icon;

        [Header("Apresentação")]
        [SerializeField] private string activationText;
        [SerializeField] private AudioClip activationSound;

        [Header("Efeito")]
        [SerializeField] private MythicEffectType effectType = MythicEffectType.None;
        [SerializeField] private int effectValue = 0;

        [Header("Regras")]
        [SerializeField] private bool canBeUsedAnytime = true;
        [SerializeField] private bool isRevealedOnlyOnUse = true;

        public string MythicId => mythicId;
        public string MythicName => mythicName;
        public string Description => description;
        public Sprite Icon => icon;

        public string ActivationText => activationText;
        public AudioClip ActivationSound => activationSound;

        public MythicEffectType EffectType => effectType;
        public int EffectValue => effectValue;

        public bool CanBeUsedAnytime => canBeUsedAnytime;
        public bool IsRevealedOnlyOnUse => isRevealedOnlyOnUse;

        private void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(mythicId))
            {
                mythicId = System.Guid.NewGuid().ToString();
            }

            effectValue = Mathf.Max(0, effectValue);
        }
    }
}