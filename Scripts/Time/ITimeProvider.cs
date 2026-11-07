using System;

namespace YellowTape.CoreKit.Time
{
    public interface ITimeProvider
    {
        IClock GetClock(TimeLayer layer);

        void RegisterTimeListener(ITimeListener listener);

        void ClearTimeListener(ITimeListener listener);

        [Obsolete("Time.timeScale is not used for pause. Pause the Game clock via GetClock(TimeLayer.Game).Pause(token).")]
        void SetTimeScale(float timeScale);
    }
}
