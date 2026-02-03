using UnityEngine;
using Cysharp.Threading.Tasks;
using YeKostenko.CoreKit.DI;

namespace YeKostenko.CoreKit.UI
{
    public class UIFactory
    {
        private readonly IDependencyInjector _injector;
        private readonly string _resourcesPath;

        public UIFactory(IDependencyInjector injector, string resourcesPath)
        {
            _injector = injector;
            _resourcesPath = resourcesPath;
        }

        public async UniTask<T> CreateWindowAsync<T>(Transform parent) where T : UIWindow
        {
            string path = UIPathResolver.GetPathFor<T>(_resourcesPath);
            Object prefab = await Resources.LoadAsync<GameObject>(path);
            GameObject go = Object.Instantiate(prefab as GameObject, parent);
            T window = go.GetComponent<T>();
            _injector.Inject(window);
            return window;
        }
    }
}