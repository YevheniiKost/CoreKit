namespace YeKostenko.CoreKit.Scripts.Saving
{
    public readonly struct CloudSyncResult
    {
        public CloudSyncResultStatus Status { get; }
        public string Message { get; }

        public CloudSyncResult(CloudSyncResultStatus status, string message = "")
        {
            Status = status;
            Message = message ?? string.Empty;
        }

        public bool IsSuccess => Status == CloudSyncResultStatus.Success;

        public static CloudSyncResult Success()
        {
            return new CloudSyncResult(CloudSyncResultStatus.Success);
        }

        public static CloudSyncResult NotAvailable()
        {
            return new CloudSyncResult(CloudSyncResultStatus.NotAvailable);
        }

        public static CloudSyncResult IoError(string message)
        {
            return new CloudSyncResult(CloudSyncResultStatus.IoError, message);
        }
    }
}
