using System;
using System.Collections.Generic;

namespace CC.Common
{
    public static class Extensions
    {
        // Just the default find but works like a try method, so it can be put inside an if.
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
