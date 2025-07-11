using Cysharp.Threading.Tasks;
using UnityEngine;

namespace YeKostenko.CoreKit.UI
{
    public abstract class UIWindow : MonoBehaviour, IUIWindow
    {
        public virtual UniTask OnCreateAsync(IUIContext context) => UniTask.CompletedTask;
        public virtual UniTask OnOpenAsync() => UniTask.CompletedTask;
        public virtual UniTask OnCloseAsync() => UniTask.CompletedTask;
    }
}