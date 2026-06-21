namespace YeKostenko.CoreKit.Scripts.Saving
{
    public class SaveSlotMetadata
    {
        public SaveSlotId SlotId;
        public bool IsEmpty;
        public long SavedAtUtc;
        public string Version = string.Empty;
        public string LocationName = string.Empty;
        public float PlayTimeSeconds;
    }
}
