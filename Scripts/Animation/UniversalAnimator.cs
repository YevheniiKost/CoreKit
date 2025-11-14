using System;
using UnityEngine;
using UnityEngine.Events;

namespace YevheniiKostenko.CoreKit.Animation
{
    [DefaultExecutionOrder(10)]
    public class UniversalAnimator : MonoBehaviour
    {
        [SerializeField]
        private AnimSet _set;
        [SerializeField]
        private bool _playShowOnEnable = true;
        [SerializeField]
        private bool _playHideOnDisable = false;
        [SerializeField]
        private bool _deactivateAfterHide = true;

        [Header("Idle")]
        [SerializeField]
        private bool _playIdleOnEnable = true;
        [SerializeField]
        private bool _suspendIdleDuringForeground = true;
        [SerializeField]
        private bool _autoRestartIdleAfterForeground = true;
        [SerializeField]
        private bool _stopIdleOnDisable = true;

        public enum Restart
        {
            KillAndPlay,
            CompleteAndPlay,
            IgnoreIfPlaying
        }

        [SerializeField]
        private Restart _onRestart = Restart.KillAndPlay;

        [SerializeField]
        private DriverChoice _driverChoice = DriverChoice.Auto;

        [SerializeField]
        private UnityEvent _onStarted;

        [SerializeField]
        private UnityEvent _onCompleted;

        private IAnimationDriver _driver;
        private IAnimHandle _current;
        private IAnimHandle _idle;
        private bool _pendingRestartIdle;

        public AnimSet Set => _set;
        public bool PlayShowOnEnable => _playShowOnEnable;
        public bool PlayHideOnDisable => _playHideOnDisable;
        public bool DeactivateAfterHide => _deactivateAfterHide;
        public bool PlayIdleOnEnable => _playIdleOnEnable;
        public bool SuspendIdleDuringForeground => _suspendIdleDuringForeground;
        public bool AutoRestartIdleAfterForeground => _autoRestartIdleAfterForeground;
        public bool StopIdleOnDisable => _stopIdleOnDisable;
        public Restart OnRestartPolicy => _onRestart;
        public DriverChoice DriverChoiceSetting => _driverChoice;
        public UnityEvent OnStarted => _onStarted;
        public UnityEvent OnCompleted => _onCompleted;
        public bool IsIdleActive => _idle != null && _idle.IsActive;

        private void Awake()
        {
            _driver = AnimationDriverFactory.Create(_driverChoice);
        }

        private void OnEnable()
        {
            if (_playShowOnEnable)
            {
                PlayShow();
            }

            if (_playIdleOnEnable)
            {
                PlayIdle();
            }
        }

        private void OnDisable()
        {
            if (_playHideOnDisable)
            {
                PlayHide();
            }

            if (_stopIdleOnDisable)
            {
                StopIdle();
            }
        }

        public void PlayShow(Action onComplete = null)
        {
            Play("Show", onComplete);
        }

        public void PlayHide(Action onComplete = null)
        {
            Play("Hide", onComplete);
        }

        public void PlayIdle()
        {
            if (_set == null)
            {
                return;
            }

            AnimSequence idle = _set.Idle;
            if (idle == null)
            {
                return;
            }

            if (_idle != null && _idle.IsActive)
            {
                return;
            }

            if (_current != null && _current.IsActive)
            {
                _pendingRestartIdle = _autoRestartIdleAfterForeground;
                return;
            }

            _idle = _driver.PlaySequence(gameObject, idle, null);
        }

        public void StopIdle(bool complete = false)
        {
            if (_idle != null)
            {
                _driver.Kill(_idle, complete);
                _idle = null;
            }

            _pendingRestartIdle = false;
        }

        public void Play(string key, Action onComplete = null)
        {
            if (_set == null)
            {
                return;
            }

            AnimSequence seq = _set.Get(key);
            if (seq == null)
            {
                return;
            }

            if (_current != null && _current.IsActive)
            {
                switch (_onRestart)
                {
                    case Restart.IgnoreIfPlaying:
                        return;
                    case Restart.CompleteAndPlay:
                        _driver.Kill(_current, true);
                        break;
                    default:
                        _driver.Kill(_current, false);
                        break;
                }
            }

            if (_suspendIdleDuringForeground && _idle != null && _idle.IsActive)
            {
                _driver.Kill(_idle, false);
                _idle = null;
                _pendingRestartIdle = _autoRestartIdleAfterForeground;
            }

            _onStarted?.Invoke();
            bool isHide = string.Equals(key, "Hide", StringComparison.OrdinalIgnoreCase);
            _current = _driver.PlaySequence(gameObject, seq, () =>
            {
                if (isHide && _deactivateAfterHide)
                {
                    gameObject.SetActive(false);
                }

                _onCompleted?.Invoke();
                onComplete?.Invoke();
                _current = null;
                if (_pendingRestartIdle && !isHide)
                {
                    _pendingRestartIdle = false;
                    PlayIdle();
                }
            });
        }
    }
}