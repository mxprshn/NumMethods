using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using ConsoleTables;

namespace NumAn
{
    public static class FormatExtensions
    {
        public static void PrintDivider()
        {
            Console.WriteLine("\n----------------------------------------------\n");
        }

        public static string Format(this double value, int precision = 17)
        {
            return value.ToString($"g{precision}");
        }

        public static string Format(this (double x, double y)[] table, string header1, string header2, int maxLength = 25)
        {
            var result = new StringBuilder();

            result.Append(string.Format($"{{0,{maxLength}}} | {{1,{-maxLength}}}\n", header1, header2));

            foreach (var value in table)
            {
                result.Append(string.Format($"{{0,{maxLength}}} | {{1,{-maxLength}}}\n", value.x.Format(), value.y.Format()));
            }

            return result.ToString();
        }

        public static void Format(this double[] values, string[] headers = null)
        {
            var actualHeaders = headers ?? Enumerable.Range(0, values.Length - 1).Select(n => n.ToString());
            var table = new ConsoleTable(headers);
            table.AddRow(values.Select(n => n.Format()));
            table.Write(ConsoleTables.Format.Alternative);
        }

        public static void Format(this double[][] values, string[] verticalHeaders, string[] headers)
        {
            var table = new ConsoleTable(headers.Prepend("").ToArray());

            for (var i = 0; i < values[0].Length; ++i)
            {
                var row = new string[values.Length + 1];

                row[0] = verticalHeaders[i];

                for (var j = 0; j < values.Length; ++j)
                {
                    row[j + 1] = values[j][i].Format();
                }
            
                table.AddRow(row.ToArray());
            }

            table.Write(ConsoleTables.Format.Alternative);
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