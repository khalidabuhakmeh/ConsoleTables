using System;
using System.Linq;
using ConsoleTables.Core;

namespace ConsoleTables
{
    class Program
    {
        static void Main(String[] args)
        {
            var table = new ConsoleTable("one", "two", "three");
            table.AddRow(1, 2, 3)
                 .AddRow("this line should be longer", "yes it is", "oh");

            table.Write();
            Console.WriteLine();

            var rows = Enumerable.Repeat(new Something(), 10);

            ConsoleTable.From<Something>(rows).Write();

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
