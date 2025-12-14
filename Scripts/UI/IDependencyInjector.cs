namespace YeKostenko.CoreKit.UI
{
    public interface IDependencyInjector
    {
        void Inject(object target);
        void InjectIntoHierarchy(object target);
    }
}