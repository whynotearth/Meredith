using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith
{
    public static class EnumExtensions
    {
        public static List<T> ToList<T>(this T enumValue) where T : Enum
        {
            var result = new List<T>();

            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (enumValue.HasFlag(value!))
                {
                    result.Add(value!);
                }
            }

            return result;
        }

        public static T ToFlag<T>(this List<T>? enumValues) where T : Enum
        {
            var result = (byte)(object)default(T)!;

            if (enumValues is null)
            {
                return (T)(object)result;
            }

            foreach (var value in enumValues)
            {
                var intValue = (byte)(object)value;
                result |= intValue;
            }

            return (T)(object)result;
        }
    }
}