using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YevheniiKostenko.CoreKit.Animation
{
    public class CoroutineDriver : IAnimationDriver
    {
        private class Handle : IAnimHandle
        {
            public bool active = true;
            public Coroutine routine;
            public GameObject target;
            public List<AnimStep> steps;
            public AnimSequence seqTemplate;
            public Action onComplete;
            public bool IsActive => active;
        }

        public IAnimHandle PlaySequence(GameObject target, AnimSequence sequence, Action onComplete = null)
        {
            Handle h = new Handle
            {
                target = target,
                seqTemplate = sequence,
                steps = UA_StepCloneUtil.CloneSteps(sequence.Steps),
                onComplete = onComplete,
                active = true
            };
            h.routine = UA_CoroutineHost.Instance.StartCoroutine(Run(h));
            return h;
        }

        public void Kill(IAnimHandle handle, bool complete = false)
        {
            if (handle is Handle h)
            {
                h.active = false;
                if (h.routine != null)
                {
                    UA_CoroutineHost.Instance.StopCoroutine(h.routine);
                    h.routine = null;
                }

                if (complete && h.steps != null && h.target != null)
                {
                    foreach (AnimStep s in h.steps)
                    {
                        s.CaptureInitial(h.target);
                        s.Complete(h.target);
                    }

                    h.onComplete?.Invoke();
                }
            }
        }

        private IEnumerator Run(Handle h)
        {
            List<AnimStep> steps = h.steps;
            GameObject target = h.target;
            AnimSequence tpl = h.seqTemplate;
            if (steps == null || target == null || tpl == null)
            {
                yield break;
            }

            IEnumerator PlayOnce(bool forward)
            {
                foreach (AnimStep step in steps)
                {
                    step.CaptureInitial(target);
                }

                if (!tpl.PlayParallel)
                {
                    foreach (AnimStep step in steps)
                    {
                        if (!h.active)
                        {
                            yield break;
                        }

                        if (step.Delay > 0f)
                        {
                            yield return new WaitForSeconds(step.Delay);
                        }

                        float d = Mathf.Max(0.0001f, step.Duration);
                        float t = 0f;
                        while (t < d)
                        {
                            if (!h.active)
                            {
                                yield break;
                            }

                            float progress = t / d;
                            float eased = step.UseCurve
                                ? (forward
                                    ? EaseUtil.Evaluate(step.EaseCurve, progress)
                                    : EaseUtil.EvaluateReverse(step.EaseCurve, progress))
                                : (forward
                                    ? EaseUtil.Evaluate(step.Ease, progress)
                                    : EaseUtil.EvaluateReverse(step.Ease, progress));
                            step.Apply(target, eased);
                            t += Time.deltaTime;
                            yield return null;
                        }

                        step.Apply(target, forward ? 1f : 0f);
                    }
                }
                else
                {
                    float longest = 0f;
                    foreach (AnimStep s in steps)
                    {
                        float w = s.Delay + s.Duration;
                        longest = Mathf.Max(longest, w);
                    }

                    float t = 0f;
                    while (t < longest)
                    {
                        if (!h.active)
                        {
                            yield break;
                        }

                        foreach (AnimStep s in steps)
                        {
                            float d = Mathf.Max(0.0001f, s.Duration);
                            float localT = Mathf.Clamp01((t - s.Delay) / d);
                            float eased = s.UseCurve
                                ? (forward
                                    ? EaseUtil.Evaluate(s.EaseCurve, localT)
                                    : EaseUtil.EvaluateReverse(s.EaseCurve, localT))
                                : (forward
                                    ? EaseUtil.Evaluate(s.Ease, localT)
                                    : EaseUtil.EvaluateReverse(s.Ease, localT));
                            if (t >= s.Delay)
                            {
                                s.Apply(target, eased);
                            }
                        }

                        t += Time.deltaTime;
                        yield return null;
                    }

                    foreach (AnimStep s in steps)
                    {
                        s.Apply(target, forward ? 1f : 0f);
                    }
                }

                yield return null;
            }

            int loops = tpl.LoopModeSetting == LoopMode.None ? 1 : (tpl.LoopCount == 0 ? 1 : tpl.LoopCount);
            if (loops < 0)
            {
                loops = int.MinValue;
            }

            int played = 0;
            while (h.active && (loops == int.MinValue || played < loops))
            {
                yield return PlayOnce(true);
                played += 1;
                if (!h.active)
                {
                    break;
                }

                if (tpl.LoopModeSetting == LoopMode.YoYo && (loops == int.MinValue || played < loops))
                {
                    yield return PlayOnce(false);
                    played += 1;
                }
            }

            if (h.active)
            {
                h.onComplete?.Invoke();
            }
        }
    }
}