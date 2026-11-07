using System;

using UnityEngine;

using YellowTape.CoreKit.Utils;

namespace YellowTape.CoreKit.Time
{
    [DefaultExecutionOrder(-1000)]
    public class UnityTimeProvider : SingletonMonoBehaviour<UnityTimeProvider>, ITimeProvider
    {
        private readonly LayerClock _realClock = new LayerClock(TimeLayer.Real);
        private readonly LayerClock _uiClock = new LayerClock(TimeLayer.UI);
        private readonly LayerClock _gameClock = new LayerClock(TimeLayer.Game);

        protected override bool PersistBetweenScenes => true;

        public IClock GetClock(TimeLayer layer)
        {
            switch (layer)
            {
                case TimeLayer.Real:
                    return _realClock;
                case TimeLayer.UI:
                    return _uiClock;
                case TimeLayer.Game:
                    return _gameClock;
                default:
                    throw new ArgumentOutOfRangeException(nameof(layer), layer, null);
            }
        }

        public void RegisterTimeListener(ITimeListener listener)
        {
            _gameClock.RegisterTimeListener(listener);
        }

        public void ClearTimeListener(ITimeListener listener)
        {
            _gameClock.ClearTimeListener(listener);
        }

        [Obsolete("Time.timeScale is not used for pause. Pause the Game clock via GetClock(TimeLayer.Game).Pause(token).")]
        public void SetTimeScale(float timeScale)
        {
            UnityEngine.Time.timeScale = timeScale;
        }

        private void Update()
        {
            float rawDeltaTime = Mathf.Min(UnityEngine.Time.unscaledDeltaTime, UnityEngine.Time.maximumDeltaTime);
            _realClock.Tick(rawDeltaTime);
            _uiClock.Tick(rawDeltaTime);
            _gameClock.Tick(rawDeltaTime);
        }
    }
}
