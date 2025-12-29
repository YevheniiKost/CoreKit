namespace YeKostenko.CoreKit.DI
{
    public interface IDependencyInjector
    {
        void Inject(object target);
        void InjectIntoHierarchy(object target);
    }
}