using System;

using UnityEngine;

namespace YeKostenko.CoreKit.Structs
{
    [Serializable]
    public class MinMaxValue : IEquatable<MinMaxValue>, IFormattable
    {
        [SerializeField]
        private float _min;
        [SerializeField]
        private float _max;

        public MinMaxValue(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public MinMaxValue()
        {
            Min = 0;
            Max = 0;
        }

        public float Min
        {
            get => _min;
            set => _min = Mathf.Min(value, Max);
        }

        public float Max
        {
            get => _max;
            set => _max = Mathf.Max(value, Min);
        }

        public float GetRandomValue()
        {
            return UnityEngine.Random.Range(Min, Max);
        }

        public bool Equals(MinMaxValue other) => Min.Equals(other.Min) && Max.Equals(other.Max);

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"Min: {Min.ToString(format, formatProvider)}, Max: {Max.ToString(format, formatProvider)}";
        }
    }
}