using System;

namespace CreateMask.Containers
{
    [Serializable]
    public class MinMax<T>
    {
        public MinMax()
        {
        }

        public MinMax(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public T Min { get; set; }
        public T Max { get; set; }
    }
}
