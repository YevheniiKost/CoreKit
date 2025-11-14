namespace YevheniiKostenko.CoreKit.Animation
{
    public static class AnimationDriverFactory
    {
        public static IAnimationDriver Create(DriverChoice choice = DriverChoice.Auto)
        {
            switch (choice)
            {
                case DriverChoice.DoTween:
#if DOTWEEN
                    return new DoTweenDriver();
#else
                Debug.LogWarning("DOTWEEN define not found. Falling back to CoroutineDriver.");
                return new CoroutineDriver();
#endif
                case DriverChoice.Coroutine:
                    return new CoroutineDriver();
                default: // Auto
#if DOTWEEN
                    return new DoTweenDriver();
#else
                return new CoroutineDriver();
#endif
            }
        }
    }
}