using System;

namespace YellowTape.CoreKit.Time
{
    public interface IClock
    {
        event EventHandler PausedChanged;

        TimeLayer Layer { get; }

        float DeltaTime { get; }

        float Time { get; }

        bool IsPaused { get; }

        void RegisterTimeListener(ITimeListener listener);

        void ClearTimeListener(ITimeListener listener);

        void Pause(object token);

        void Resume(object token);
    }
}
