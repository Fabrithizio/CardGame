// Caminho: Assets/_Project/Scripts/Mythics/MythicRuntime.cs
// Descrição: Representa um Mítico durante a partida, controlando se ele já foi usado e revelado.

using System;
using UnityEngine;

namespace CardGame.Mythics
{
    [Serializable]
    public class MythicRuntime
    {
        [SerializeField] private MythicData data;
        [SerializeField] private bool isUsed;
        [SerializeField] private bool isRevealed;

        public MythicData Data => data;
        public bool IsUsed => isUsed;
        public bool IsRevealed => isRevealed;

        public string MythicName => data != null ? data.MythicName : "Mítico sem dados";

        public MythicRuntime(MythicData mythicData, bool startsRevealed)
        {
            if (mythicData == null)
            {
                throw new ArgumentNullException(nameof(mythicData), "MythicData não pode ser nulo ao criar MythicRuntime.");
            }

            data = mythicData;
            isUsed = false;
            isRevealed = startsRevealed;
        }

        public bool CanUse()
        {
            return data != null && !isUsed;
        }

        public bool TryUse()
        {
            if (!CanUse())
            {
                return false;
            }

            isUsed = true;
            isRevealed = true;
            return true;
        }

        public void Reveal()
        {
            isRevealed = true;
        }
    }
}