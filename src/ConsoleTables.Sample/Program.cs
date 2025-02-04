using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleTables.Sample
{
    static class Program
    {
        static void Main(string[] args)
        {
            TestDictionaryTable();
            SetupAndWriteTables();
            ConfigureAlignment();
            WriteTableWithoutCount();

            Console.ReadKey();
        }

        static void TestDictionaryTable()
        {
            var data = new List<TableData>
            {
                new TableData("A", new Dictionary<string, object> { { "A", true }, { "B", false }, { "C", true } }),
                new TableData("B", new Dictionary<string, object> { { "A", false }, { "B", true }, { "C", false } }),
                new TableData("C", new Dictionary<string, object> { { "A", false }, { "B", false }, { "C", true } })
            };

            var dictionary = data.ToDictionary(d => d.Key, d => d.Values);
            var table = ConsoleTable.FromDictionary(dictionary);
            Console.WriteLine(table.ToString());
        }

        static void SetupAndWriteTables()
        {
            var table = new ConsoleTable("one", "two", "three");
            table.AddRow(1, 2, 3)
                 .AddRow("this line should be longer 哈哈哈哈", "yes it is", "oh");

            Console.WriteLine("\nFORMAT: Default:\n");
            table.Write();

            Console.WriteLine("\nFORMAT: MarkDown:\n");
            table.Write(Format.MarkDown);

            Console.WriteLine("\nFORMAT: Alternative:\n");
            table.Write(Format.Alternative);
            Console.WriteLine();

            Console.WriteLine("\nFORMAT: Minimal:\n");
            table.Write();
            Console.WriteLine();

            table = new ConsoleTable("I've", "got", "nothing");
            table.Write();
            Console.WriteLine();
        }

        static void ConfigureAlignment()
        {
            Console.WriteLine("\nNumberAlignment = Alignment.Right\n");

            var rows = Enumerable.Repeat(new Something(), 2);
            ConsoleTable.From(rows)
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .Write();
        }

        static void WriteTableWithoutCount()
        {
            var noCount = new ConsoleTable(new ConsoleTableOptions
            {
                Columns = new[] { "one", "two", "three" },
                EnableCount = false
            });

            noCount.AddRow(1, 2, 3).Write();
        }
    }

    public class TableData
    {
        public string Key { get; }
        public Dictionary<string, object> Values { get; }

        public TableData(string key, Dictionary<string, object> values)
        {
            Key = key;
            Values = values;
        }
    }
}
