using System;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    public static class SaveSlotKey
    {
        public const int ManualSlotCount = 5;

        public static string ToStorageKey(SaveSlotId slotId)
        {
            if (slotId == SaveSlotId.AutoSave)
            {
                return "autosave";
            }

            int slotIndex = (int)slotId;
            if (slotIndex < 0 || slotIndex >= ManualSlotCount)
            {
                throw new ArgumentOutOfRangeException(nameof(slotId), slotId, "Invalid manual save slot.");
            }

            return "slot_" + slotIndex;
        }

        public static bool IsManualSlot(SaveSlotId slotId)
        {
            return slotId != SaveSlotId.AutoSave;
        }

        public static SaveSlotId FromManualSlotIndex(int index)
        {
            if (index < 0 || index >= ManualSlotCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Invalid manual save slot index.");
            }

            return (SaveSlotId)index;
        }
    }
}
