using System;
using System.Linq;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Data;
using System.Diagnostics;
using System.Xml;

namespace ConsoleTables.Sample
{
    class Program
    {

        static void Main(string[] args)
        {
            string path = @"C:\Tables\";
            string newFileName = "new.txt";
            string[] filePaths;
            int rowCount = 0;
            int columnCount = 0;
            int switch_option;
            int continue_option;
            List<String> list_header = new List<string>();
            List<String> list_cells = new List<string>();
            var table = new ConsoleTable();

            do
            {
                //rootFolders
                filePaths = Directory.GetFiles(@"c:\Tables\", "*.txt", SearchOption.AllDirectories);
                do
                {
                    Console.WriteLine("Press 1 - Import values into documents");
                    Console.WriteLine("Press 2 - Import values from documents");
                    switch_option = Convert.ToInt32(Console.ReadLine());
                    if (switch_option != 1 && switch_option != 2)
                    {
                        Console.Clear();
                        Console.WriteLine("Invalid input please input again!!");
                    }
                } while (switch_option != 1 && switch_option != 2);

                if (switch_option == 1)
                {
                    //create a new file for storing new round of data.
                    foreach (string value in filePaths)
                    {
                        if (value.Contains("new.txt"))
                        {
                            File.Move(value, path + "T" + filePaths.Length + ".txt");
                            File.Delete(path + "new.txt");
                        }
                    }
                    using FileStream fs = File.Create(path + newFileName);
                    using var sr = new StreamWriter(fs);
                    String newString = "";
                    do
                    {
                        Console.Write("\nDefine The number of rows:");
                        rowCount = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Define The number of columns:");
                        columnCount = Convert.ToInt32(Console.ReadLine());

                        if (columnCount <= 0)
                            throw new ArgumentException("Column count can't be less than 1", nameof(columnCount));

                        if (rowCount <= 0)
                            throw new ArgumentException("Column count can't be less than 1", nameof(rowCount));
                    } while (rowCount <= 0 || columnCount <= 0);

                    Console.Write("\n");

                    //define headers value 
                    for (int i = 0; i < columnCount; i++)
                    {
                        Console.Write("Enter the " + (i + 1) + "th header value:");
                        list_header.Add(Console.ReadLine());
                    }
                    String[] header = list_header.ToArray();
                    foreach (string value in header)
                    {
                        newString = newString + value + "|";
                    }
                    newString = newString.Remove(newString.LastIndexOf("|"));
                    sr.WriteLine(newString);
                    newString = "";
                    //define cells values
                    for (int i = 0; i < rowCount - 1; i++)
                    {
                        for (int j = 0; j < columnCount; j++)
                        {
                            Console.Write("Enter the " + (j + 1) + "th cells value in " + (i + 1) + " th row:");
                            list_cells.Add(Console.ReadLine());
                        }
                        String[] cells = list_cells.ToArray();
                        foreach (string value in cells)
                        {
                            newString = newString + value + "|";
                        }
                        newString = newString.Remove(newString.LastIndexOf("|"));
                        sr.WriteLine(newString);
                        newString = "";
                        sr.Flush();
                        sr.Close();
                    }

                }
                else
                {
                    int iter = 0;
                    int file_id = 1;
                    int selection;
                    string selected_file;
                    Console.WriteLine("Select the file that you wants to read the tables:");
                    foreach (string found_files in filePaths)
                    {
                        Console.WriteLine(file_id + " - " + found_files);
                        file_id++;
                    }
                    selection = Convert.ToInt32(Console.ReadLine());
                    selected_file = filePaths[selection - 1];
                    string[] lines = System.IO.File.ReadAllLines(@selected_file);
                    foreach (string line in lines)
                    {
                        string[] array = line.Split("|");
                        if (iter == 0)
                            table = new ConsoleTable(array);
                        else
                            table.AddRow(array);
                        iter++;
                    }

                    // [1] default format 
                    Console.WriteLine("\nFORMAT: Default:\n");
                    table.Write();

                    // [2] markdown format
                    Console.WriteLine("\nFORMAT: MarkDown:\n");
                    table.Write(Format.MarkDown);

                    // [3] alternative format
                    Console.WriteLine("\nFORMAT: Alternative:\n");
                    table.Write(Format.Alternative);
                    Console.WriteLine();

                    // [4] minimal format
                    Console.WriteLine("\nFORMAT: Minimal:\n");
                    table.Write(Format.Minimal);
                    Console.WriteLine();
                }

                Console.Write("Do you wish to continue? 1-Yes 0-N0:");
                continue_option = Convert.ToInt32(Console.ReadLine());
                while (continue_option != 1 && continue_option != 0)
                {
                    Console.Clear();
                    Console.Write("Invalid Input..\n");
                    Console.Write("Do you wish to continue? 1-Yes 0-N0:");
                    continue_option = Convert.ToInt32(Console.ReadLine());
                }

                if (continue_option == 0)
                    System.Environment.Exit(0);
                Console.Clear();
            } while (continue_option == 1);


            //table = new ConsoleTable("I've", "got", "nothing");
            // table.Write();
            // Console.WriteLine();

            var rows = Enumerable.Repeat(new Something(), 10);

            ConsoleTable.From<Something>(rows).Write();

            rows = Enumerable.Repeat(new Something(), 0);
            ConsoleTable.From<Something>(rows).Write();

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
