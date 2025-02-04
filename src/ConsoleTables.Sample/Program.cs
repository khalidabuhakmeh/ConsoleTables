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
            Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>>()
            {
                {"A", new Dictionary<string, object>() { { "A", true }, { "B", false }, { "C", true } }},
                {"B", new Dictionary<string, object>() { { "A", false }, { "B", true }, { "C", false } }},
                {"C", new Dictionary<string, object>() { { "A", false }, { "B", false }, { "C", true } }}
            };

            var table = ConsoleTable.FromDictionary(data);
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
}
