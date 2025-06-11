using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Xunit;

namespace ConsoleTables.Tests
{
    public class ConsoleTableTest
    {
        [Fact]
        public void ShouldAccountForUnicodeCharacters()
        {
            var table = new ConsoleTable("one", "two", "three")
                .AddRow(1, 2, 3)
                .AddRow("this line should be longer 哈哈哈哈", "yes it is", "oh")
                .ToString();
            
            Assert.Equal(
                """
                 ----------------------------------------------------------- 
                 | one                                 | two       | three |
                 ----------------------------------------------------------- 
                 | 1                                   | 2         | 3     |
                 ----------------------------------------------------------- 
                 | this line should be longer 哈哈哈哈 | yes it is | oh    |
                 ----------------------------------------------------------- 

                 Count: 2
                """,
                table
            );
        }
        
        [Fact]
        public void ShouldBeToStringFromList()
        {
            var users = new List<User>
            {
                new() { Name = "Alexandre" , Age = 36 }
            };
            var table = ConsoleTable.From(users).ToString();

            Assert.Equal(
                $"""
                  ------------------- 
                  | Name      | Age |
                  ------------------- 
                  | Alexandre | 36  |
                  ------------------- 

                  Count: 1
                 """, table);
        }

        [Fact]
        public void ShouldBeAvoidErrorOnToStringFromAddRows()
        {
            var table = new ConsoleTable("one", "two", "three")
                .AddRow(1, 2, 3)
                .AddRow("this line should be longer", "yes it is", "oh")
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .ToString();

            Assert.Equal(
                $"""
                  -------------------------------------------------- 
                  | one                        | two       | three |
                  -------------------------------------------------- 
                  | 1                          | 2         | 3     |
                  -------------------------------------------------- 
                  | this line should be longer | yes it is | oh    |
                  -------------------------------------------------- 

                  Count: 2
                 """, table);
        }

        [Fact]
        public void SpecialCharactersShouldNotBreakTable()
        {
            var users = new List<User>
            {
                new() { Name = "René", Age = 59 },
                new() { Name = "Otto", Age = 52 }
            };
            var table = ConsoleTable
                .From(users)
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .ToString();

            Assert.Equal(
                $"""
                  -------------- 
                  | Name | Age |
                  -------------- 
                  | René |  59 |
                  -------------- 
                  | Otto |  52 |
                  -------------- 

                  Count: 2
                 """, table);
        }

        [Fact]
        public void TestGetTextWidth()
        {
            Assert.Equal(3, ConsoleTable.GetTextWidth("abc"));
            Assert.Equal(3, ConsoleTable.GetTextWidth("äöü"));
            Assert.Equal(4, ConsoleTable.GetTextWidth("René"));
        }

        [Fact]
        public void NumberShouldBeRightAligned()
        {
            var users = new List<User>
            {
                new() { Name = "Alexandre" , Age = 36 }
            };
            var table = ConsoleTable
                .From(users)
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .ToString();

            Assert.Equal(
                $"""
                  ------------------- 
                  | Name      | Age |
                  ------------------- 
                  | Alexandre |  36 |
                  ------------------- 

                  Count: 1
                 """, table);
        }

        [Fact]
        public void NumberShouldBeRightAlignedOnMarkDown()
        {
            var users = new List<User>
            {
                new() { Name = "Alexandre" , Age = 36 }
            };
            var table = ConsoleTable
                .From(users)
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .ToMarkDownString();

            Assert.Equal(
                $"""
                 | Name      | Age |
                 |-----------|-----|
                 | Alexandre |  36 |

                 """, table);
        }

        [Fact]
        public void OutputShouldDefaultToConsoleOut()
        {
            var users = new List<User>
            {
                new() { Name = "Alexandre" , Age = 36 }
            };

            var table = ConsoleTable.From(users);

            Assert.Equal(table.Options.OutputTo, Console.Out);
        }

        [Fact]
        public void OutputShouldGoToConfiguredOutputWriter()
        {
            var users = new List<User>
            {
                new() { Name = "Alexandre" , Age = 36 }
            };

            var testWriter = new StringWriter();

            ConsoleTable
               .From(users)
               .Configure(o => o.OutputTo = testWriter)
               .Write();

            Assert.NotEmpty(testWriter.ToString());
        }

        [Fact]
        public void TestDictionaryTable()
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

            Assert.Equal("""
                         |   | A     | B     | C     |
                         |---|-------|-------|-------|
                         | A | True  | False | True  |
                         | B | False | True  | False |
                         | C | False | False | True  |

                         """,table.ToMarkDownString());

        }
        [Fact]
        public void TestDataTable()
        {
            DataTable data = new DataTable();
            data.Columns.Add("A", typeof(bool));
            data.Columns.Add("B", typeof(bool));
            data.Columns.Add("C", typeof(bool));
            data.Rows.Add(true, false, true);
            data.Rows.Add(false, true, false);
            data.Rows.Add(false, false, true);
            var table = ConsoleTable.From(data);
            Assert.Equal("""
                         | A     | B     | C     |
                         |-------|-------|-------|
                         | True  | False | True  |
                         | False | True  | False |
                         | False | False | True  |

                         """,table.ToMarkDownString());

        }
        [Fact]
        public void TestObjectArray()
        {
            Object[][] userData =
            [
                ["Column1", "Column2"],
                ["Test", 1],
                ["Test", 2]
            ];
            var table = ConsoleTable.From(userData);
            string minimalString = table.ToMinimalString();
            Assert.Equal("""
                         Column1  Column2 
                         -----------------
                         Test     1       
                         Test     2       
                         
                         """, minimalString);
        }

        [Fact]
        public void TestMinimalFormating()
        {
            var table = new ConsoleTable("one", "two", "three")
                .AddRow(1, 2, 3)
                .AddRow("this line should be longer 哈哈哈哈", "yes it is", "oh")
                .ToMinimalString();
            Assert.Equal("""
                         one                                  two        three 
                         ------------------------------------------------------
                         1                                    2          3     
                         this line should be longer 哈哈哈哈  yes it is  oh    

                         """, table);
        }

        class User
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}
