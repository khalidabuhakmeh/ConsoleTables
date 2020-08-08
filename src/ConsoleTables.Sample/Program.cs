using System;
using System.Linq;
using System.Collections.Generic;

namespace ConsoleTables.Sample
{
    class Program
    {
        static void Main(string[] args)
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

            //Example Code for creating a table
            table = new ConsoleTable("I've", "got", "nothing");
            table.Write();
            Console.WriteLine();

            //Example Code for repeating 
            var rows = Enumerable.Repeat(new Something(), 10);

            ConsoleTable.From<Something>(rows).Write();

            rows = Enumerable.Repeat(new Something(), 0);
            ConsoleTable.From<Something>(rows).Write();

            //Example Code for Alignment
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

            //New Code to create new table with own parameters
            Console.WriteLine("Press any key to create your own table");
            Console.ReadKey();
            Console.Clear();

            Console.WriteLine("Create your own table");

            Console.WriteLine("How many columns do you need? ");
            string input = Console.ReadLine();
            int number;
            Int32.TryParse(input, out number);
            if (!Int32.TryParse(input, out number))
            {
                Console.WriteLine("Only Intergers allowed.Please Restart Program and try again!");
            }

            int n = Int32.Parse(input);
            List<string> list = new List<string>();
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine("Data for Column " + (i + 1));
                list.Add(Console.ReadLine());
            }

            String[] title = list.ToArray();

            var table2 = new ConsoleTable(title);

            Console.WriteLine("How many rows do you need?");

            string input2 = Console.ReadLine();
            int number2;
            Int32.TryParse(input2, out number2);
            if (!Int32.TryParse(input2, out number2))
            {
                Console.WriteLine("Only Intergers allowed.Please Restart Program and try again!");
            }

            int row = Int32.Parse(input2);
            for (int i = 0; i < row; i++)
            {
                List<String> list2 = new List<String>();

                for (int j = 0; j < n; j++)
                {
                    Console.WriteLine("Data for Row " + (i + 1) + "Column" + (j + 1) + "(" + title[j] + ")");
                    list2.Add(Console.ReadLine());
                }

                String[] row2 = list2.ToArray();
                table2.AddRow(row2);
            }

            int choice = 0;
            Console.WriteLine("Please Enter Choices");
            Console.WriteLine("1=Default Format,2=Markdown Format,3=Alternative Format,4=Minimal Format");
            choice = Convert.ToInt32(Console.ReadLine());

            switch (choice)
            {
                case 1:

                    Console.WriteLine("\nFORMAT: Default:\n");
                    table2.Write();
                    break;
                case 2:
                    Console.WriteLine("\nFORMAT: MarkDown:\n");
                    table2.Write(Format.MarkDown);
                    break;
                case 3:
                    Console.WriteLine("\nFORMAT: Alternative:\n");
                    table2.Write(Format.Alternative);
                    break;
                case 4:
                    Console.WriteLine("\nFORMAT: Minimal:\n");
                    table2.Write(Format.Minimal);
                    Console.WriteLine();
                    break;
            }
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
        public int NumberOfChildren { get; set; }
    }
}
