using System;
using System.Collections.Generic;

using UnityRandom = UnityEngine.Random;

namespace YeKostenko.CoreKit.Extensions
{
    public static class CollectionExtensions
    {
        private static Random s_random = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = s_random.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
        
        public static List<TOut> ConvertAll<TIn, TOut>(this List<TIn> list, Func<TIn, TOut> converter)
        {
            if(list == null || list.Count == 0)
                return new List<TOut>();
            
            List<TOut> result = new List<TOut>(list.Count);
            foreach (var item in list)
            {
                result.Add(converter(item));
            }

            return result;
        }
        
        public static List<TOut> ConvertAll<TIn, TOut>(this TIn[] inputArray, Func<TIn, TOut> converter)
        {
            if(inputArray == null || inputArray.Length == 0)
                return new List<TOut>();
            
            List<TOut> result = new List<TOut>(inputArray.Length);
            foreach (var item in inputArray)
            {
                result.Add(converter(item));
            }

            return result;
        }

        public static HashSet<TOut> ConvertAll<TIn, TOut>(this HashSet<TIn> inputSet, Func<TIn, TOut> converter)
        {
            if (inputSet == null || inputSet.Count == 0)
            {
                return new HashSet<TOut>();
            }

            HashSet<TOut> result = new HashSet<TOut>();

            foreach (var inputValue in inputSet)
            {
                result.Add(converter(inputValue));
            }

            return result;
        }
        
        public static List<TOut> ConvertAll<TIn, TOut>(this IReadOnlyList<TIn> list, Func<TIn, TOut> converter)
        {
            if(list == null || list.Count == 0)
                return new List<TOut>();
            
            List<TOut> result = new List<TOut>(list.Count);
            foreach (var item in list)
            {
                result.Add(converter(item));
            }

            return result;
        }
        
        public static T Find<T>(this IReadOnlyList<T> list, Predicate<T> match)
        {
            if (list == null || list.Count == 0)
            {
                return default;
            }
            
            for (int i = 0; i < list.Count; i++)
            {
                if (match(list[i]))
                    return list[i];
            }

            return default;
        }

        public static T GetRandomElement<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return default;
            }
            
            int randomIndex = UnityRandom.Range(0, list.Count);
            return list[randomIndex];
        }
        
        public static bool IsEmpty<T>(this IReadOnlyCollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }
    }
}