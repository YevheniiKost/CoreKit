using System.Collections.Generic;
using YevheniiKostenko.CoreKit.Utils;

namespace YevheniiKostenko.CoreKit.Time
{
    public class UnityTimeProvider : SingletonMonoBehaviour<UnityTimeProvider>, ITimeProvider
    {
        private readonly List<ITimeListener> _timeListeners = new();

        protected override bool PersistBetweenScenes => true;

        public void RegisterTimeListener(ITimeListener listener)
        {
            if (listener == null || _timeListeners.Contains(listener))
                return;

            _timeListeners.Add(listener);
        }

        public void ClearTimeListener(ITimeListener listener)
        {
            if (listener == null)
                return;

            _timeListeners.Remove(listener);
        }

        public void SetTimeScale(float timeScale)
        {
            UnityEngine.Time.timeScale = timeScale;
        }

        private void Update()
        {
            float deltaTime = UnityEngine.Time.deltaTime;
            for (int i = 0; i < _timeListeners.Count; i++)
            {
                if(_timeListeners[i] == null)
                    continue;
                
                _timeListeners[i].Update(deltaTime);
            }
        }
    }
}