using System;
using UnityEngine;

namespace YellowTape.CoreKit.Animation
{
    public interface IAnimationDriver
    {
        IAnimHandle PlaySequence(GameObject target, AnimSequence sequence, Action onComplete = null);
        void Kill(IAnimHandle handle, bool complete = false);
    }

    public enum DriverChoice
    {
        Auto,
        DoTween,
        Coroutine
    }
}