using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleTables.Sample;

static class Program
{
    static void TestDictionaryTable()
    {
        Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>>()
        {
            {"A", new Dictionary<string, object>()
            {
                { "A", true },
                { "B", false },
                { "C", true },
            }},
            {"B", new Dictionary<string, object>()
            {
                { "A", false },
                { "B", true },
                { "C", false },
            }},
            {"C", new Dictionary<string, object>()
            {
                { "A", false },
                { "B", false },
                { "C", true },
            }}
        };
        var table = ConsoleTable.FromDictionary(data);

        Console.WriteLine(table.ToString());
    }
        
    static void Main(string[] args)
    {
        TestDictionaryTable();
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
        table.Write(Format.Minimal);
        Console.WriteLine();

        table = new ConsoleTable("I've", "got", "nothing");
        table.Write();
        Console.WriteLine();

        var rows = Enumerable.Repeat(new Something(), 10);

        ConsoleTable.From(rows).Write();

        rows = Enumerable.Repeat(new Something(), 0);
        ConsoleTable.From(rows).Write();

        Console.WriteLine("\nNumberAlignment = Alignment.Right\n");
        rows = Enumerable.Repeat(new Something(), 2);
        ConsoleTable
            .From(rows)
            .Configure(o => o.NumberAlignment = Alignment.Right)
            .Write();

        var noCount =
            new ConsoleTable(new ConsoleTableOptions
            {
                Columns = new[] { "one", "two", "three" },
                EnableCount = false
            });

        noCount.AddRow(1, 2, 3).Write();

        Console.ReadKey();
    }
}

public class Something
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "Khalid Abuhkameh";
    public DateTime Date { get; set; } = DateTime.Now;
    public int NumberOfChildren { get; set; }
}