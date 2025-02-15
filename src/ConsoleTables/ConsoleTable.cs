using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Wcwidth;

namespace ConsoleTables
{
    public class ConsoleTable
    {
        public IList<object> Columns { get; }
        public IList<object[]> Rows { get; }

        public ConsoleTableOptions Options { get; }
        public Type[] ColumnTypes { get; private set; }

        public IList<string> Formats { get; private set; }

        public static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(int),  typeof(double),  typeof(decimal),
            typeof(long), typeof(short),   typeof(sbyte),
            typeof(byte), typeof(ulong),   typeof(ushort),
            typeof(uint), typeof(float)
        };

        public ConsoleTable(params string[] columns)
            : this(new ConsoleTableOptions { Columns = new List<string>(columns) })
        {
        }

        public ConsoleTable(ConsoleTableOptions options)
        {
            Options = options ?? throw new ArgumentNullException("options");
            Rows = new List<object[]>();
            Columns = new List<object>(options.Columns);
        }

        public ConsoleTable AddColumn(IEnumerable<string> names)
        {
            foreach (var name in names)
                Columns.Add(name);
            return this;
        }

        public ConsoleTable AddRow(params object[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (!Columns.Any())
                throw new Exception("Please set the columns first");

            if (Columns.Count != values.Length)
                throw new Exception(
                    $"The number columns in the row ({Columns.Count}) does not match the values ({values.Length})");

            Rows.Add(values);
            return this;
        }

        public ConsoleTable Configure(Action<ConsoleTableOptions> action)
        {
            action(Options);
            return this;
        }


        public static ConsoleTable FromDictionary(Dictionary<string, Dictionary<string, object>> values)
        {
            var table = new ConsoleTable();

            var columNames = values.SelectMany(x => x.Value.Keys).Distinct().ToList();
            columNames.Insert(0, "");
            table.AddColumn(columNames);
            foreach (var row in values)
            {
                var r = new List<object> { row.Key };
                foreach (var columName in columNames.Skip(1))
                {
                    r.Add(row.Value.TryGetValue(columName, out var value) ? value : "");
                }

                table.AddRow(r.Cast<object>().ToArray());
            }

            return table;
        }

        public static ConsoleTable From<T>(IEnumerable<T> values)
        {
            var table = new ConsoleTable
            {
                ColumnTypes = GetColumnsType<T>().ToArray()
            };

            var columns = GetColumns<T>().ToList();

            table.AddColumn(columns);

            foreach (
                var propertyValues
                in values.Select(value => columns.Select(column => GetColumnValue<T>(value, column)))
            ) table.AddRow(propertyValues.ToArray());

            return table;
        }

        public static ConsoleTable From(DataTable dataTable)
        {
            var table = new ConsoleTable();

            var columns = dataTable.Columns
                .Cast<DataColumn>()
                .Select(x => x.ColumnName)
                .ToList();

            table.AddColumn(columns);

            foreach (DataRow row in dataTable.Rows)
            {
                var items = row.ItemArray.Select(x => x is byte[] data ? Convert.ToBase64String(data) : x.ToString())
                    .ToArray();
                table.AddRow(items);
            }

            return table;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            // find the longest column by searching each row
            var columnLengths = ColumnLengths();

            // set right alinment if is a number
            var columnAlignment = Enumerable.Range(0, Columns.Count)
                .Select(GetNumberAlignment)
                .ToList();

            // create the string format with padding ; just use for maxRowLength
            var format = Enumerable.Range(0, Columns.Count)
                .Select(i => " | {" + i + "," + columnAlignment[i] + columnLengths[i] + "}")
                .Aggregate((s, a) => s + a) + " |";

            SetFormats(ColumnLengths(), columnAlignment);

            // find the longest formatted line
            var maxRowLength = Math.Max(0, Rows.Any() ? Rows.Max(row => string.Format(format, row).Length) : 0);
            var columnHeaders = string.Format(Formats[0], Columns.ToArray());

            // longest line is greater of formatted columnHeader and longest row
            var longestLine = Math.Max(maxRowLength, columnHeaders.Length);

            // add each row
            var results = Rows.Select((row, i) => string.Format(Formats[i + 1], row)).ToList();

            // create the divider
            var divider = " " + string.Join("", Enumerable.Repeat("-", longestLine - 1)) + " ";

			if(Options.IncludeHeaderRow)
			{
				builder.AppendLine(divider);
				builder.AppendLine(columnHeaders);
			}

            foreach (var row in results)
            {
                builder.AppendLine(divider);
                builder.AppendLine(row);
            }

            builder.AppendLine(divider);

            if (Options.EnableCount)
            {
                builder.AppendLine("");
                builder.AppendFormat(" Count: {0}", Rows.Count);
            }

            return builder.ToString();
        }


        private void SetFormats(List<int> columnLengths, List<string> columnAlignment)
        {
            var allLines = new List<object[]>();
            allLines.Add(Columns.ToArray());
            allLines.AddRange(Rows);

            Formats = allLines.Select(d =>
            {
                return Enumerable.Range(0, Columns.Count)
                    .Select(i =>
                    {
                        var value = d[i]?.ToString() ?? "";
                        var length = columnLengths[i] - (GetTextWidth(value) - value.Length);
                        return " | {" + i + "," + columnAlignment[i] + length + "}";
                    })
                    .Aggregate((s, a) => s + a) + " |";
            }).ToList();
        }

        public static int GetTextWidth(string value)
        {
            if (value == null)
                return 0;

            var length = value.ToCharArray().Sum(c => UnicodeCalculator.GetWidth(c));
            return length;
        }

        public string ToMarkDownString()
        {
            return ToMarkDownString('|');
        }

        private string ToMarkDownString(char delimiter)
        {
            var builder = new StringBuilder();

            // find the longest column by searching each row
            var columnLengths = ColumnLengths();

            // create the string format with padding
            _ = Format(columnLengths, delimiter);

            // find the longest formatted line
            var columnHeaders = string.Format(Formats[0].TrimStart(), Columns.ToArray());

            // add each row
            var results = Rows.Select((row, i) => string.Format(Formats[i + 1].TrimStart(), row)).ToList();

            // create the divider
            var divider = Regex.Replace(columnHeaders, "[^|]", "-");


			if (Options.IncludeHeaderRow)
			{
				builder.AppendLine(columnHeaders);
				builder.AppendLine(divider);
			}

            results.ForEach(row => builder.AppendLine(row));

            return builder.ToString();
        }

        public string ToMinimalString()
        {
            return ToMarkDownString(char.MinValue);
        }

        public string ToStringAlternative()
        {
            var builder = new StringBuilder();

            // find the longest formatted line
            var columnHeaders = string.Format(Formats[0].TrimStart(), Columns.ToArray());

            // add each row
            var results = Rows.Select((row, i) => string.Format(Formats[i + 1].TrimStart(), row)).ToList();

            // create the divider
            var divider = Regex.Replace(columnHeaders, "[^| ]", "-");
            var dividerPlus = divider.Replace("|", "+");

			if (Options.IncludeHeaderRow)
			{
				builder.AppendLine(dividerPlus);
				builder.AppendLine(columnHeaders);
			}

            foreach (var row in results)
            {
                builder.AppendLine(dividerPlus);
                builder.AppendLine(row);
            }
            builder.AppendLine(dividerPlus);

            return builder.ToString();
        }


        private string Format(List<int> columnLengths, char delimiter = '|')
        {
            // set right alignment if is a number
            var columnAlignment = Enumerable.Range(0, Columns.Count)
                .Select(GetNumberAlignment)
                .ToList();

            SetFormats(columnLengths, columnAlignment);

            var delimiterStr = delimiter == char.MinValue ? string.Empty : delimiter.ToString();
            var format = (Enumerable.Range(0, Columns.Count)
                .Select(i => " " + delimiterStr + " {" + i + "," + columnAlignment[i] + columnLengths[i] + "}")
                .Aggregate((s, a) => s + a) + " " + delimiterStr).Trim();
            return format;
        }

        private string GetNumberAlignment(int i)
        {
            return Options.NumberAlignment == Alignment.Right
                    && ColumnTypes != null
                    && NumericTypes.Contains(ColumnTypes[i])
                ? ""
                : "-";
        }

        private List<int> ColumnLengths()
        {
            var columnLengths = Columns
                .Select((t, i) => Rows.Select(x => x[i])
                    .Union(new[] { Columns[i] })
                    .Where(x => x != null)
                    .Select(x => x.ToString().ToCharArray().Sum(c => UnicodeCalculator.GetWidth(c))).Max())
                .ToList();
            return columnLengths;
        }

        public void Write(Format format = ConsoleTables.Format.Default)
        {
            SetFormats(ColumnLengths(), Enumerable.Range(0, Columns.Count).Select(GetNumberAlignment).ToList());

            switch (format)
            {
                case ConsoleTables.Format.Default:
                    Options.OutputTo.WriteLine(ToString());
                    break;
                case ConsoleTables.Format.MarkDown:
                    Options.OutputTo.WriteLine(ToMarkDownString());
                    break;
                case ConsoleTables.Format.Alternative:
                    Options.OutputTo.WriteLine(ToStringAlternative());
                    break;
                case ConsoleTables.Format.Minimal:
                    Options.OutputTo.WriteLine(ToMinimalString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        private static IEnumerable<string> GetColumns<T>()
        {
            return typeof(T).GetProperties().Select(x => x.Name).ToArray();
        }

        private static object GetColumnValue<T>(object target, string column)
        {
            return typeof(T).GetProperty(column)?.GetValue(target, null);
        }

        private static IEnumerable<Type> GetColumnsType<T>()
        {
            return typeof(T).GetProperties().Select(x => x.PropertyType).ToArray();
        }
    }

    public class ConsoleTableOptions
    {
        public IEnumerable<string> Columns { get; set; } = new List<string>();
        public bool EnableCount { get; set; } = true;

        /// <summary>
        /// Enable only from a list of objects
        /// </summary>
        public Alignment NumberAlignment { get; set; } = Alignment.Left;

        /// <summary>
        /// The <see cref="TextWriter"/> to write to. Defaults to <see cref="Console.Out"/>.
        /// </summary>
        public TextWriter OutputTo { get; set; } = Console.Out;

		public bool IncludeHeaderRow { get; set; } = true;

	}

	public enum Format
    {
        Default = 0,
        MarkDown = 1,
        Alternative = 2,
        Minimal = 3
    }

    public enum Alignment
    {
        Left,
        Right
    }
}
