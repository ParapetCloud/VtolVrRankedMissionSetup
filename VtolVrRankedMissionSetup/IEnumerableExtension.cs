using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Windows.Security.Isolation;

namespace VtolVrRankedMissionSetup
{
    public static class IEnumerableExtension
    {

        public static bool AnyTrue<T>(this IEnumerable<T> wow, Expression<Func<T, bool>> expression)
        {
            return true;
        }

        public static bool AllTrue<T>(this IEnumerable<T> wow, Expression<Func<T, bool>> expression)
        {
            return true;
        }
    }
}
