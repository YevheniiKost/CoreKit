using System;
using System.Collections.Generic;

namespace YellowTape.CoreKit.Time
{
    public sealed class LayerClock : IClock
    {
        private readonly HashSet<object> _pauseTokens = new HashSet<object>();
        private readonly List<ITimeListener> _listeners = new List<ITimeListener>();

        public LayerClock(TimeLayer layer)
        {
            Layer = layer;
        }

        public event EventHandler PausedChanged;

        public TimeLayer Layer { get; }

        public float DeltaTime { get; private set; }

        public float Time { get; private set; }

        public bool IsPaused => _pauseTokens.Count > 0;

        public void RegisterTimeListener(ITimeListener listener)
        {
            if (listener == null || _listeners.Contains(listener))
            {
                return;
            }

            _listeners.Add(listener);
        }

        public void ClearTimeListener(ITimeListener listener)
        {
            if (listener == null)
            {
                return;
            }

            _listeners.Remove(listener);
        }

        public void Pause(object token)
        {
            if (token == null)
            {
                return;
            }

            bool wasPaused = IsPaused;
            _pauseTokens.Add(token);
            if (!wasPaused)
            {
                DeltaTime = 0f;
                PausedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Resume(object token)
        {
            if (token == null || !_pauseTokens.Remove(token))
            {
                return;
            }

            if (!IsPaused)
            {
                PausedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Tick(float rawDeltaTime)
        {
            if (IsPaused)
            {
                DeltaTime = 0f;
                return;
            }

            DeltaTime = rawDeltaTime;
            Time += rawDeltaTime;

            for (int i = 0; i < _listeners.Count; i++)
            {
                ITimeListener listener = _listeners[i];
                if (listener == null)
                {
                    continue;
                }

                listener.Update(rawDeltaTime);
            }
        }
    }
}
