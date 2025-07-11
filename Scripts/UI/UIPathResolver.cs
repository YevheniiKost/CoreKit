namespace YeKostenko.CoreKit.UI
{
    public static class UIPathResolver
    {
        public static string GetPathFor<T>()
        {
            return $"UI/Windows/{typeof(T).Name}";
        }
    }
}