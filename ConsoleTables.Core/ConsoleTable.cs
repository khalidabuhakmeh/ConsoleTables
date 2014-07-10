using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleTables.Core
{
    public class ConsoleTable
    {
        public IList<string> Columns { get; protected set; }
        public IList<object[]> Rows { get; protected set; }

        public ConsoleTable(params string[] columns)
        {
            Columns = new List<string>(columns);
            Rows = new List<object[]>();
        }

        public ConsoleTable AddColumn(string[] names)
        {
            foreach (var name in names)
                Columns.Add(name);

            return this;
        }

        public ConsoleTable AddRow(params object[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            if (!Columns.Any())
                throw new Exception("Please set the columns first");

            if (Columns.Count != values.Length)
                throw new Exception(string.Format("The number columns in the row ({0}) does not match the values ({1}", Columns.Count, values.Length));

            Rows.Add(values);
            return this;
        }

        public static ConsoleTable From<T>(IEnumerable<T> values)
        {
            var table = new ConsoleTable();

            var columns = typeof(T).GetProperties().Select(x => x.Name).ToArray();
            table.AddColumn(columns);

            foreach (var propertyValues in values.Select(value => columns.Select(column => typeof(T).GetProperty(column).GetValue(value))))
                table.AddRow(propertyValues.ToArray());

            return table;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            // find the longest column by searching each row
            var columnLengths = Columns
                .Select((t, i) => Rows.Select(x => x[i])
                    .Union(Columns)
                    .Where(x => x != null)
                    .Select(x => x.ToString().Length).Max())
                    .ToList();

            // create the string format with padding
            var format = Enumerable.Range(0, Columns.Count)
                .Select(i => " | {" + i + ", -" + columnLengths[i] + " }")
                .Aggregate((s, a) => s + a) + " |";

            var longestLine = 0;
            var results = new List<string>();

            // find the longest formatted line
            foreach (var result in Rows.Select(row => string.Format(format, row)))
            {
                longestLine = Math.Max(longestLine, result.Length);
                results.Add(result);
            }

            // create the divider
            var line = " " + string.Join("", Enumerable.Repeat("-", longestLine - 1)) + " ";

            builder.AppendLine(line);
            builder.AppendLine(string.Format(format, Columns.ToArray()));

            foreach (var row in results)
            {
                builder.AppendLine(line);
                builder.AppendLine(row);
            }

            builder.AppendLine(line);
            builder.AppendLine("");
            builder.AppendFormat(" Count: {0}", Rows.Count);

            return builder.ToString();
        }

        public void Write()
        {
            Console.WriteLine(ToString());
        }
    }
}
