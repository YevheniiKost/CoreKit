namespace YeKostenko.CoreKit.Scripts.Saving
{
    public readonly struct CloudConflictResolution
    {
        public readonly SaveSlotId SlotId;

        public CloudConflictWinner Winner { get; }
        public string Message { get; }

        public CloudConflictResolution(CloudConflictWinner winner, SaveSlotId slotId, string message = "")
        {
            Winner = winner;
            SlotId = slotId;
            Message = message ?? string.Empty;
        }

        public static CloudConflictResolution None()
        {
            return new CloudConflictResolution(CloudConflictWinner.None, SaveSlotId.Slot0);
        }
    }
}
