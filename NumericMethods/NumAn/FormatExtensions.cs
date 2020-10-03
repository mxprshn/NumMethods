using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NumAn
{
    public static class FormatExtensions
    {
        public static string Format(this double value, int precision = 20)
        {
            return value.ToString($"F{precision}");
        }

        public static string Format(this (double x, double y)[] table, string header1, string header2, int precision = 20)
        {
            var result = new StringBuilder();

            result.Append(string.Format($"{{0,{-precision}}} | {{1,{-precision}}}\n", header1, header2));

            foreach (var value in table)
            {
                result.Append(string.Format($"{{0,{-precision}}} | {{1,{-precision}}}\n", value.x, value.y));
            }

            return result.ToString();
        }

        public static string Format(this string[,] table, int maxLength)
        {
            var result = new StringBuilder();

            for (var i = 0; i < table.GetLength(0); ++i)
            {
                for (var j = 0; j < table.GetLength(1); ++j)
                {
                    result.Append(string.Format($"{{0,{maxLength}}}", table[i, j]));
                    result.Append(j == table.GetLength(1) - 1 ? "\n" : " | ");
                }
            }

            return result.ToString();
        }
    }
}