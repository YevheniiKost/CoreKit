#if DOTWEEN
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace YevheniiKostenko.CoreKit.Animation
{
    public class DoTweenDriver : IAnimationDriver
    {
        private class Handle : IAnimHandle
        {
            public Tween tween;
            public bool IsActive => tween != null && tween.IsActive();
        }

        public IAnimHandle PlaySequence(GameObject target, AnimSequence sequence, Action onComplete = null)
        {
            List<AnimStep> steps = UA_StepCloneUtil.CloneSteps(sequence.Steps);
            Sequence seq = DOTween.Sequence();
            foreach (AnimStep s in steps)
            {
                s.CaptureInitial(target);
                Tweener t = DOTween
                    .To(() => 0f, v => s.Apply(target, v), 1f, Mathf.Max(0.0001f, s.Duration))
                    .SetDelay(s.Delay);
                if (s.UseCurve)
                {
                    t.SetEase(s.EaseCurve);
                }
                else
                {
                    t.SetEase(ToEase(s.Ease));
                }

                if (sequence.PlayParallel)
                {
                    seq.Join(t);
                }
                else
                {
                    seq.Append(t);
                }
            }

            if (sequence.LoopModeSetting == LoopMode.None)
            {
                seq.SetLoops(Mathf.Max(1, sequence.LoopCount));
            }
            else
            {
                LoopType lt = sequence.LoopModeSetting == LoopMode.YoYo ? LoopType.Yoyo : LoopType.Restart;
                int count = sequence.LoopCount == 0 ? 1 : sequence.LoopCount;
                seq.SetLoops(count, lt);
            }

            if (onComplete != null)
            {
                seq.OnComplete(() => onComplete());
            }

            seq.Play();
            return new Handle { tween = seq };
        }

        public void Kill(IAnimHandle handle, bool complete = false)
        {
            if (handle is Handle h && h.tween != null)
            {
                h.tween.Kill(complete);
            }
        }

        private static Ease ToEase(AnimEase e)
        {
            switch (e)
            {
                case AnimEase.Linear:
                    return Ease.Linear;
                case AnimEase.InSine:
                    return Ease.InSine;
                case AnimEase.OutSine:
                    return Ease.OutSine;
                case AnimEase.InOutSine:
                    return Ease.InOutSine;
                case AnimEase.InQuad:
                    return Ease.InQuad;
                case AnimEase.OutQuad:
                    return Ease.OutQuad;
                case AnimEase.InOutQuad:
                    return Ease.InOutQuad;
                case AnimEase.InCubic:
                    return Ease.InCubic;
                case AnimEase.OutCubic:
                    return Ease.OutCubic;
                case AnimEase.InOutCubic:
                    return Ease.InOutCubic;
                case AnimEase.InQuart:
                    return Ease.InQuart;
                case AnimEase.OutQuart:
                    return Ease.OutQuart;
                case AnimEase.InOutQuart:
                    return Ease.InOutQuart;
                case AnimEase.InQuint:
                    return Ease.InQuint;
                case AnimEase.OutQuint:
                    return Ease.OutQuint;
                case AnimEase.InOutQuint:
                    return Ease.InOutQuint;
                case AnimEase.InExpo:
                    return Ease.InExpo;
                case AnimEase.OutExpo:
                    return Ease.OutExpo;
                case AnimEase.InOutExpo:
                    return Ease.InOutExpo;
                case AnimEase.InCirc:
                    return Ease.InCirc;
                case AnimEase.OutCirc:
                    return Ease.OutCirc;
                case AnimEase.InOutCirc:
                    return Ease.InOutCirc;
                case AnimEase.InBack:
                    return Ease.InBack;
                case AnimEase.OutBack:
                    return Ease.OutBack;
                case AnimEase.InOutBack:
                    return Ease.InOutBack;
                case AnimEase.InBounce:
                    return Ease.InBounce;
                case AnimEase.OutBounce:
                    return Ease.OutBounce;
                case AnimEase.InOutBounce:
                    return Ease.InOutBounce;
                case AnimEase.InElastic:
                    return Ease.InElastic;
                case AnimEase.OutElastic:
                    return Ease.OutElastic;
                case AnimEase.InOutElastic:
                    return Ease.InOutElastic;
                default:
                    return Ease.Linear;
            }
        }
    }
}
#endif