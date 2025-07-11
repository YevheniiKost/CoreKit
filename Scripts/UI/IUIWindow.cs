using Cysharp.Threading.Tasks;

namespace YeKostenko.CoreKit.UI
{
    public interface IUIWindow
    {
        UniTask OnCreateAsync(IUIContext context);
        UniTask OnOpenAsync();
        UniTask OnCloseAsync();
    }
}