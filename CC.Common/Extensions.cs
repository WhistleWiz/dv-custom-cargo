using System;
using System.Collections.Generic;

namespace CC.Common
{
    public static class Extensions
    {
        public static bool TryFind<T>(this List<T> list, Predicate<T> match, out T value)
        {
            value = list.Find(match);

            if (value == null)
            {
                return false;
            }

            return true;
        }
    }
}
