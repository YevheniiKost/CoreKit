namespace YeKostenko.CoreKit.Scripts.Saving
{
    public readonly struct LoadResult
    {
        public LoadResultStatus Status { get; }
        public string Message { get; }

        public LoadResult(LoadResultStatus status, string message = "")
        {
            Status = status;
            Message = message ?? string.Empty;
        }

        public bool IsSuccess => Status == LoadResultStatus.Success;

        public static LoadResult Success()
        {
            return new LoadResult(LoadResultStatus.Success);
        }

        public static LoadResult NotFound()
        {
            return new LoadResult(LoadResultStatus.NotFound);
        }

        public static LoadResult Corrupted(string message)
        {
            return new LoadResult(LoadResultStatus.Corrupted, message);
        }

        public static LoadResult IoError(string message)
        {
            return new LoadResult(LoadResultStatus.IoError, message);
        }

        public static LoadResult SerializationError(string message)
        {
            return new LoadResult(LoadResultStatus.SerializationError, message);
        }
    }
}
