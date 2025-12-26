using UnityEngine;
using YeKostenko.CoreKit.DI;

namespace YeKostenko.CoreKit.UI
{
    [DefaultExecutionOrder(-100)]
    public class UIRoot : MonoBehaviour
    {
        public static UIRoot Instance { get; private set; }
        public UIManager UIManager { get; private set; }

        [SerializeField] private Transform _uiContainer;
        private UIFactory _uiFactory;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Initialize(IDependencyInjector injector)
        {
            _uiFactory = new UIFactory(injector);
            UIManager = new UIManager(_uiFactory, _uiContainer);
        }
    }
}