using System.Collections.Generic;
using UnityEngine;

namespace YevheniiKostenko.CoreKit.Animation
{
    [CreateAssetMenu(menuName = "Tools/CoreKit/Animation/AnimStep")]
    public class AnimSequence : ScriptableObject
    {
        [SerializeReference]
        private List<AnimStep> _steps = new List<AnimStep>();

        [SerializeField]
        private bool _playParallel = false;

        [SerializeField]
        [Tooltip("DOTween-compatible: 1 = play once, 2 = play twice, -1 = infinite")]
        private int _loopCount = 1;

        [SerializeField]
        private LoopMode _loopMode = LoopMode.None;

        public IList<AnimStep> Steps => _steps;
        public bool PlayParallel => _playParallel;
        public int LoopCount => _loopCount;
        public LoopMode LoopModeSetting => _loopMode;
    }
}