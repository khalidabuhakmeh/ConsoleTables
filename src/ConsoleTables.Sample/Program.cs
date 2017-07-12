using System;
using System.Linq;
using ConsoleTables;

namespace ConsoleTables.Sample
{
    class Program
    {
        static void Main(String[] args)
        {
            var table = new ConsoleTable("one", "two", "three");
            table.AddRow(1, 2, 3)
                 .AddRow("this line should be longer", "yes it is", "oh");

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

            

            ConsoleTable.From<Something>(rows).Write();

            rows = Enumerable.Repeat(new Something(), 0);
            ConsoleTable.From<Something>(rows).Write();

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
        public Something()
        {
            Id = Guid.NewGuid().ToString("N");
            Name = "Khalid Abuhkameh";
            Date = DateTime.Now;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
}
