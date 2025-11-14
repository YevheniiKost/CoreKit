using UnityEngine;

namespace YevheniiKostenko.CoreKit.Animation
{
    internal class UA_CoroutineHost : MonoBehaviour
    {
        private static UA_CoroutineHost _instance;

        public static UA_CoroutineHost Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("[UniversalAnimationRunner]");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<UA_CoroutineHost>();
                }
                return _instance;
            }
        }
    }
}