using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace YeKostenko.CoreKit.UI
{
    public class UIManager
    {
        private readonly Stack<UIStackEntry> _windowStack = new();
        private readonly UIFactory _factory;
        private readonly Transform _container;

        public UIManager(UIFactory factory, Transform container)
        {
            _factory = factory;
            _container = container;
        }

        public async UniTask<T> OpenWindowAsync<T>(IUIContext context = null, bool modal = false) where T : UIWindow
        {
            var window = await _factory.CreateWindowAsync<T>(_container);
            await window.OnCreateAsync(context);
            if (modal) UIBlocker.ShowAbove(window.transform);
            await window.OnOpenAsync();
            _windowStack.Push(new UIStackEntry(window, modal));
            return window;
        }

        public async UniTask CloseTopWindowAsync()
        {
            if (_windowStack.TryPop(out var entry))
            {
                await entry.Window.OnCloseAsync();
                Object.Destroy(entry.Window.gameObject);
                if (entry.Modal) UIBlocker.Hide();
            }
        }

        public async UniTask CloseAllAsync()
        {
            while (_windowStack.Count > 0)
                await CloseTopWindowAsync();
        }
    }
}