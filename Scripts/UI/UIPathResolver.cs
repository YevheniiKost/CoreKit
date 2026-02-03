namespace YeKostenko.CoreKit.UI
{
    public static class UIPathResolver
    {
        public static string GetPathFor<T>(string resourcesPath) => $"{resourcesPath}{typeof(T).Name}";
    }
}