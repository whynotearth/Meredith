using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith
{
    public static class EnumExtensions 
    {
        public static List<T> ToList<T>(this T enumValue) where T : Enum
        {
            var result = new List<T>();

            foreach(T value in Enum.GetValues(typeof(T)))
            {
                if (enumValue.HasFlag(value!))
                {
                    result.Add(value!);
                }
            }

            return result;
        }
    }
}
