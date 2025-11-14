using UnityEngine;

namespace YevheniiKostenko.CoreKit.Animation
{
    public enum AnimEase
    {
        Linear,
        InSine,
        OutSine,
        InOutSine,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InBack,
        OutBack,
        InOutBack,
        InBounce,
        OutBounce,
        InOutBounce,
        InElastic,
        OutElastic,
        InOutElastic
    }

    public static class EaseUtil
    {
        private const float _cBack = 1.70158f;

        public static float Evaluate(AnimEase ease, float t)
        {
            t = Mathf.Clamp01(t);
            switch (ease)
            {
                case AnimEase.InSine:
                    return 1f - Mathf.Cos((t * Mathf.PI) * 0.5f);
                case AnimEase.OutSine:
                    return Mathf.Sin((t * Mathf.PI) * 0.5f);
                case AnimEase.InOutSine:
                    return -(Mathf.Cos(Mathf.PI * t) - 1f) * 0.5f;
                case AnimEase.InQuad:
                    return t * t;
                case AnimEase.OutQuad:
                    return 1f - (1f - t) * (1f - t);
                case AnimEase.InOutQuad:
                    return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) * 0.5f;
                case AnimEase.InCubic:
                    return t * t * t;
                case AnimEase.OutCubic:
                    return 1f - Mathf.Pow(1f - t, 3f);
                case AnimEase.InOutCubic:
                    return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;
                case AnimEase.InQuart:
                    return t * t * t * t;
                case AnimEase.OutQuart:
                    return 1f - Mathf.Pow(1f - t, 4f);
                case AnimEase.InOutQuart:
                    return t < 0.5f ? 8f * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 4f) * 0.5f;
                case AnimEase.InQuint:
                    return t * t * t * t * t;
                case AnimEase.OutQuint:
                    return 1f - Mathf.Pow(1f - t, 5f);
                case AnimEase.InOutQuint:
                    return t < 0.5f ? 16f * t * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 5f) * 0.5f;
                case AnimEase.InExpo:
                    return t <= 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f);
                case AnimEase.OutExpo:
                    return t >= 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);
                case AnimEase.InOutExpo:
                    if (t <= 0f)
                    {
                        return 0f;
                    }

                    if (t >= 1f)
                    {
                        return 1f;
                    }

                    return t < 0.5f ? Mathf.Pow(2f, 20f * t - 10f) * 0.5f : (2f - Mathf.Pow(2f, -20f * t + 10f)) * 0.5f;
                case AnimEase.InCirc:
                    return 1f - Mathf.Sqrt(1f - t * t);
                case AnimEase.OutCirc:
                    return Mathf.Sqrt(1f - Mathf.Pow(t - 1f, 2f));
                case AnimEase.InOutCirc:
                    return t < 0.5f
                        ? (1f - Mathf.Sqrt(1f - 4f * t * t)) * 0.5f
                        : (Mathf.Sqrt(1f - Mathf.Pow(-2f * t + 2f, 2f)) + 1f) * 0.5f;
                case AnimEase.InBack:
                    return (_cBack + 1f) * t * t * t - _cBack * t * t;
                case AnimEase.OutBack:
                    float tOb = t - 1f;
                    return 1f + (_cBack + 1f) * tOb * tOb * tOb + _cBack * tOb * tOb;
                case AnimEase.InOutBack:
                    float s = _cBack * 1.525f;
                    return t < 0.5f
                        ? (Mathf.Pow(2f * t, 2f) * ((s + 1f) * 2f * t - s)) * 0.5f
                        : (Mathf.Pow(2f * t - 2f, 2f) * (((s + 1f) * (t * 2f - 2f)) + s) + 2f) * 0.5f;
                case AnimEase.InBounce:
                    return 1f - OutBounce(1f - t);
                case AnimEase.OutBounce:
                    return OutBounce(t);
                case AnimEase.InOutBounce:
                    return t < 0.5f ? (1f - OutBounce(1f - 2f * t)) * 0.5f : (1f + OutBounce(2f * t - 1f)) * 0.5f;
                case AnimEase.InElastic:
                    return InElastic(t);
                case AnimEase.OutElastic:
                    return OutElastic(t);
                case AnimEase.InOutElastic:
                    return t < 0.5f ? InElastic(t * 2f) * 0.5f : OutElastic(t * 2f - 1f) * 0.5f + 0.5f;
                default:
                    return t;
            }
        }

        public static float EvaluateReverse(AnimEase ease, float t)
        {
            return Evaluate(ease, 1f - Mathf.Clamp01(t));
        }

        public static float Evaluate(AnimationCurve curve, float t)
        {
            if (curve == null)
            {
                return t;
            }

            return curve.Evaluate(Mathf.Clamp01(t));
        }

        public static float EvaluateReverse(AnimationCurve curve, float t)
        {
            if (curve == null)
            {
                return 1f - Mathf.Clamp01(t);
            }

            return curve.Evaluate(1f - Mathf.Clamp01(t));
        }

        private static float OutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;
            if (t < 1f / d1)
            {
                return n1 * t * t;
            }

            if (t < 2f / d1)
            {
                t -= 1.5f / d1;
                return n1 * t * t + 0.75f;
            }

            if (t < 2.5f / d1)
            {
                t -= 2.25f / d1;
                return n1 * t * t + 0.9375f;
            }

            t -= 2.625f / d1;
            return n1 * t * t + 0.984375f;
        }

        private static float InElastic(float t)
        {
            if (t <= 0f)
            {
                return 0f;
            }

            if (t >= 1f)
            {
                return 1f;
            }

            const float c4 = (2f * Mathf.PI) / 3f;
            return -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * c4);
        }

        private static float OutElastic(float t)
        {
            if (t <= 0f)
            {
                return 0f;
            }

            if (t >= 1f)
            {
                return 1f;
            }

            const float c4 = (2f * Mathf.PI) / 3f;
            return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
        }
    }
}