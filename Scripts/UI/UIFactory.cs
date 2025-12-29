using Cysharp.Threading.Tasks;
using UnityEngine;
using YeKostenko.CoreKit.DI;

namespace YeKostenko.CoreKit.UI
{
    public class UIFactory
    {
        private readonly IDependencyInjector _injector;

        public UIFactory(IDependencyInjector injector)
        {
            _injector = injector;
        }

        public async UniTask<T> CreateWindowAsync<T>(Transform parent) where T : UIWindow
        {
            var path = UIPathResolver.GetPathFor<T>();
            var prefab = await Resources.LoadAsync<GameObject>(path);
            var go = Object.Instantiate(prefab as GameObject, parent);
            var window = go.GetComponent<T>();
            _injector.Inject(window);
            return window;
        }
    }
}