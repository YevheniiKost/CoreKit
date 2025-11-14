using System.Collections.Generic;

namespace YevheniiKostenko.CoreKit.Animation
{
    internal static class UA_StepCloneUtil
    {
        public static List<AnimStep> CloneSteps(IList<AnimStep> source)
        {
            if (source == null)
            {
                return null;
            }
            List<AnimStep> list = new List<AnimStep>(source.Count);
            for (int i = 0; i < source.Count; i++)
            {
                AnimStep s = source[i];
                if (s == null)
                {
                    continue;
                }
                list.Add(s.CloneStep());
            }
            return list;
        }
    }
}