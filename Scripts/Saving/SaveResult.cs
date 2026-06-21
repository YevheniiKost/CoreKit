namespace YeKostenko.CoreKit.Scripts.Saving
{
    public readonly struct SaveResult
    {
        public SaveResultStatus Status { get; }
        public string Message { get; }

        public SaveResult(SaveResultStatus status, string message = "")
        {
            Status = status;
            Message = message ?? string.Empty;
        }

        public bool IsSuccess => Status == SaveResultStatus.Success;

        public static SaveResult Success()
        {
            return new SaveResult(SaveResultStatus.Success);
        }

        public static SaveResult NothingToSave()
        {
            return new SaveResult(SaveResultStatus.NothingToSave);
        }

        public static SaveResult IoError(string message)
        {
            return new SaveResult(SaveResultStatus.IoError, message);
        }

        public static SaveResult SerializationError(string message)
        {
            return new SaveResult(SaveResultStatus.SerializationError, message);
        }
    }
}
