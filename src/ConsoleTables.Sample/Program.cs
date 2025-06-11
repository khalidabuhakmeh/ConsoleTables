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

            table.Write();
        }

        static void ConfigureAlignment()
        {
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

    public class Something
    {
        public string Id { get; } = Guid.NewGuid().ToString("N");
        public string Name { get; private set; } = "Khalid Abuhkameh";
        public DateTime Date { get; }

        public Something()
        {
            Date = DateTime.UtcNow;
        }
    }
}
