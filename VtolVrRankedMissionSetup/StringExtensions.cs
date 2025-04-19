using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup
{
    internal static class StringExtensions
    {
        public static string ValueOrDefault(this string? str, string defaultValue) => string.IsNullOrWhiteSpace(str) ? defaultValue : str;
    }
}
