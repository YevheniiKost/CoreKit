using UnityEngine;

namespace YeKostenko.CoreKit.UI
{
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

            _uiFactory = new UIFactory(new UIDependencyInjector());
            UIManager = new UIManager(_uiFactory, _uiContainer);
        }
    }
}