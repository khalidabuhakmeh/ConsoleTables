using ModernConsoleTables.Enum;
using System.Text;
using System.Text.RegularExpressions;

namespace ModernConsoleTables;

public class ConsoleTable
{
    #region Globals

    public IList<object> Columns { get; set; }
    public IList<object[]> Rows { get; protected set; }

    public ConsoleTableOptions Options { get; protected set; }
    public Type[] ColumnTypes { get; private set; }

    private const string SPACE               = " ";
    const char CHR_PIPE_DELIMITER            = '|';
    const string STR_DIVIDER_PLUS            = "|";
    private const string PLUS                = "+";
    private const string HYPHEN              = "-";
    public static HashSet<Type> NumericTypes = new()
    {
        typeof(int),
        typeof(double),
        typeof(decimal),
        typeof(long),
        typeof(short),
        typeof(sbyte),
        typeof(byte),
        typeof(ulong),
        typeof(ushort),
        typeof(uint),
        typeof(float)
    };

    #endregion

    #region Constructors

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


    #endregion

    #region Methods

    public ConsoleTable AddColumn(IEnumerable<string> names)
    {
        foreach (var name in names)
            Columns.Add(name);
        return this;
    }

    public ConsoleTable AddRow(params object[] values)
    {
        if (values is null)
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

    public static ConsoleTable From<T>(IEnumerable<T> values)
    {
        ConsoleTable table = new() 
        {
            ColumnTypes = GetColumnsType<T>().ToArray()
        };

        var columns = GetColumns<T>();

        table.AddColumn(columns);

        foreach (
            var propertyValues
            in values.Select(value => columns.Select(column => GetColumnValue<T>(value, column)))
        ) table.AddRow(propertyValues.ToArray());

        return table;
    }

    public override string ToString()
    {
        StringBuilder builder = new();

        // find the longest column by searching each row
        var columnLengths = ColumnLengths();

        // set right alignment if is a number
        var columnAlignment = Enumerable.Range(0, Columns.Count)
            .Select(GetNumberAlignment)
            .ToList();

        // create the string format with padding
        var format = Enumerable.Range(0, Columns.Count)
            .Select(i => string.Concat(" | {", i, ",", columnAlignment[i], columnLengths[i], "}"))
            .Aggregate((s, a) => s + a) + " |";

        // find the longest formatted line
        var maxRowLength = Math.Max(0, Rows.Any() ? Rows.Max(row => string.Format(format, row).Length) : 0);
        var columnHeaders = string.Format(format, Columns.ToArray());

        // longest line is greater of formatted columnHeader and longest row
        var longestLine = Math.Max(maxRowLength, columnHeaders.Length);

        // add each row
        var results = Rows.Select(row => string.Format(format, row)).ToList();

        // create the divider
        var divider = string.Concat(SPACE, 
            string.Join("", Enumerable.Repeat(HYPHEN, longestLine - 1)), 
            SPACE);

        builder.AppendLine(divider);
        builder.AppendLine(columnHeaders);

        foreach (var row in results)
        {
            builder.AppendLine(divider);
            builder.AppendLine(row);
        }

        builder.AppendLine(divider);

        if (Options.EnableCount)
        {
            builder.AppendLine(string.Empty);
            builder.AppendFormat(" Count: {0}", Rows.Count);
        }

        return builder.ToString();
    }

    public string ToMarkDownString()
    {
        return ToMarkDownString(CHR_PIPE_DELIMITER);
    }

    private string ToMarkDownString(char delimiter)
    {
        StringBuilder builder = new();

        // find the longest column by searching each row
        var columnLengths = ColumnLengths();

        // create the string format with padding
        var format = Format(columnLengths, delimiter);

        // find the longest formatted line
        var columnHeaders = string.Format(format, Columns.ToArray());

        // add each row
        var results = Rows.Select(row => string.Format(format, row)).ToList();

        // create the divider
        var divider = Regex.Replace(columnHeaders, @"[^|]", HYPHEN);

        builder.AppendLine(columnHeaders);
        builder.AppendLine(divider);
        results.ForEach(row => builder.AppendLine(row));

        return builder.ToString();
    }

    public string ToMinimalString()
    {
        return ToMarkDownString(char.MinValue);
    }

    public string ToStringAlternative()
    {
        StringBuilder builder = new();

        // find the longest column by searching each row
        var columnLengths = ColumnLengths();

        // create the string format with padding
        var format = Format(columnLengths);

        // find the longest formatted line
        var columnHeaders = string.Format(format, Columns.ToArray());

        // add each row
        var results = Rows.Select(row => string.Format(format, row)).ToList();

        // create the divider
        var divider = Regex.Replace(columnHeaders, @"[^|]", HYPHEN);
        var dividerPlus = divider.Replace(STR_DIVIDER_PLUS, PLUS);

        builder.AppendLine(dividerPlus);
        builder.AppendLine(columnHeaders);

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

        var delimiterStr = delimiter == char.MinValue ? string.Empty : delimiter.ToString();
        var format = (Enumerable.Range(0, Columns.Count)
            .Select(i => string.Concat(SPACE, delimiterStr, " {", i, ",", columnAlignment[i], columnLengths[i], "}"))
            .Aggregate((s, a) => s + a) + SPACE + delimiterStr).Trim();
        return format;
    }

    private string GetNumberAlignment(int i)
    {
        return Options.NumberAlignment == Alignment.Right
                && ColumnTypes != null
                && NumericTypes.Contains(ColumnTypes[i])
            ? string.Empty
            : HYPHEN;
    }

    private List<int> ColumnLengths()
    {
        var columnLengths = Columns
            .Select((t, i) => Rows.Select(x => x[i])
                .Union(new[] { Columns[i] })
                .Where(x => x != null)
                .Select(x => x.ToString().Length).Max())
            .ToList();
        return columnLengths;
    }

    public void Write(Format format = Enum.Format.Default)
    {
        switch (format)
        {
            case Enum.Format.Default:
                Options.OutputTo.WriteLine(ToString());
                break;
            case Enum.Format.MarkDown:
                Options.OutputTo.WriteLine(ToMarkDownString());
                break;
            case Enum.Format.Alternative:
                Options.OutputTo.WriteLine(ToStringAlternative());
                break;
            case Enum.Format.Minimal:
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
        return typeof(T).GetProperty(column).GetValue(target, null);
    }

    private static IEnumerable<Type> GetColumnsType<T>()
    {
        return typeof(T).GetProperties().Select(x => x.PropertyType).ToArray();
    } 

    #endregion
}
