// Caminho: Assets/_Project/Scripts/Mythics/MythicLoadoutRuntime.cs
// Descrição: Controla os 3 Míticos escolhidos por um jogador para a partida, incluindo uso, revelação e slots vazios/usados.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Mythics
{
    [Serializable]
    public class MythicLoadoutRuntime
    {
        public const int MaxMythicSlots = 3;

        [SerializeField] private List<MythicRuntime> mythicSlots = new();

        public IReadOnlyList<MythicRuntime> MythicSlots => mythicSlots;

        public int TotalSlots => MaxMythicSlots;

        public MythicLoadoutRuntime(IEnumerable<MythicData> mythics, bool startsRevealed)
        {
            mythicSlots = new List<MythicRuntime>(MaxMythicSlots);

            if (mythics != null)
            {
                foreach (MythicData mythicData in mythics)
                {
                    if (mythicSlots.Count >= MaxMythicSlots)
                    {
                        Debug.LogWarning("Foram enviados mais de 3 Míticos. Os extras foram ignorados.");
                        break;
                    }

                    if (mythicData == null)
                    {
                        Debug.LogWarning("Um Mítico nulo foi ignorado ao criar o loadout.");
                        continue;
                    }

                    mythicSlots.Add(new MythicRuntime(mythicData, startsRevealed));
                }
            }

            while (mythicSlots.Count < MaxMythicSlots)
            {
                mythicSlots.Add(null);
            }
        }

        public MythicRuntime GetMythicAt(int slotIndex)
        {
            if (!IsValidSlot(slotIndex))
            {
                return null;
            }

            return mythicSlots[slotIndex];
        }

        public bool HasAvailableMythic()
        {
            for (int i = 0; i < mythicSlots.Count; i++)
            {
                MythicRuntime mythic = mythicSlots[i];

                if (mythic != null && mythic.CanUse())
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryUseMythic(int slotIndex, out MythicRuntime usedMythic)
        {
            usedMythic = null;

            if (!IsValidSlot(slotIndex))
            {
                Debug.LogWarning($"Slot de Mítico inválido: {slotIndex}.");
                return false;
            }

            MythicRuntime mythic = mythicSlots[slotIndex];

            if (mythic == null)
            {
                Debug.LogWarning($"Não existe Mítico no slot {slotIndex}.");
                return false;
            }

            if (!mythic.TryUse())
            {
                Debug.LogWarning($"O Mítico {mythic.MythicName} já foi usado.");
                return false;
            }

            usedMythic = mythic;
            return true;
        }

        public int GetAvailableCount()
        {
            int count = 0;

            for (int i = 0; i < mythicSlots.Count; i++)
            {
                MythicRuntime mythic = mythicSlots[i];

                if (mythic != null && mythic.CanUse())
                {
                    count++;
                }
            }

            return count;
        }

        public int GetUsedCount()
        {
            int count = 0;

            for (int i = 0; i < mythicSlots.Count; i++)
            {
                MythicRuntime mythic = mythicSlots[i];

                if (mythic != null && mythic.IsUsed)
                {
                    count++;
                }
            }

            return count;
        }

        private bool IsValidSlot(int slotIndex)
        {
            return slotIndex >= 0 && slotIndex < mythicSlots.Count;
        }
    }
}