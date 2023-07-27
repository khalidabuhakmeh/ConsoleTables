using System;
using System.Collections.Generic;
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
                new User { Name = "Alexandre" , Age = 36 }
            };
            var table = ConsoleTable.From(users).ToString();

            Assert.Equal(
$@" ------------------- 
 | Name      | Age |
 ------------------- 
 | Alexandre | 36  |
 ------------------- 

 Count: 1", table);
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
$@" -------------------------------------------------- 
 | one                        | two       | three |
 -------------------------------------------------- 
 | 1                          | 2         | 3     |
 -------------------------------------------------- 
 | this line should be longer | yes it is | oh    |
 -------------------------------------------------- 

 Count: 2", table);
        }

        [Fact]
        public void NumberShouldBeRightAligned()
        {
            var users = new List<User>
            {
                new User { Name = "Alexandre" , Age = 36 }
            };
            var table = ConsoleTable
                .From(users)
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .ToString();

            Assert.Equal(
$@" ------------------- 
 | Name      | Age |
 ------------------- 
 | Alexandre |  36 |
 ------------------- 

 Count: 1", table);
        }

        [Fact]
        public void NumberShouldBeRightAlignedOnMarkDown()
        {
            var users = new List<User>
            {
                new User { Name = "Alexandre" , Age = 36 }
            };
            var table = ConsoleTable
                .From(users)
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .ToMarkDownString();

            Assert.Equal(
$@"| Name      | Age |
|-----------|-----|
| Alexandre |  36 |
", table);
        }

        [Fact]
        public void OutputShouldDefaultToConsoleOut()
        {
            var users = new List<User>
            {
                new User { Name = "Alexandre" , Age = 36 }
            };

            var table = ConsoleTable.From(users);

            Assert.Equal(table.Options.OutputTo, Console.Out);
        }

        [Fact]
        public void OutputShouldGoToConfiguredOutputWriter()
        {
            var users = new List<User>
            {
                new User { Name = "Alexandre" , Age = 36 }
            };

            var testWriter = new StringWriter();

            ConsoleTable
               .From(users)
               .Configure(o => o.OutputTo = testWriter)
               .Write();

            Assert.NotEmpty(testWriter.ToString());
        }

        class User
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}
