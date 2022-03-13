using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModernConsoleTables;
using ModernConsoleTables.Enum;
using System;
using System.Collections.Generic;
using System.IO;

namespace ModernConsoleTableTests
{
    [TestClass]
    public class ModernConsoleTableUnitTests
    {
        [TestMethod]
        public void ShouldBeToStringFromListTest()
        {
            List<User> users = new()
            {
                new User { Name = "Alexandre" , Age = 36 }
            };

            var table = ConsoleTable.From(users).ToString();

            Console.Write(table);

            Assert.AreEqual(
$@" ------------------- 
 | Name      | Age |
 ------------------- 
 | Alexandre | 36  |
 ------------------- 

 Count: 1", table);
        }

        [TestMethod]
        public void ShouldBeAvoidErrorOnToStringFromAddRowsTest()
        {
            var table = new ConsoleTable("one", "two", "three")
                .AddRow(1, 2, 3)
                .AddRow("this line should be longer", "yes it is", "oh")
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .ToString();

            Console.Write(table);

            Assert.AreEqual(
$@" -------------------------------------------------- 
 | one                        | two       | three |
 -------------------------------------------------- 
 | 1                          | 2         | 3     |
 -------------------------------------------------- 
 | this line should be longer | yes it is | oh    |
 -------------------------------------------------- 

 Count: 2", table);
        }

        [TestMethod]
        public void NumberShouldBeRightAlignedTest()
        {
            List<User> users = new()
            {
                new User { Name = "Alexandre" , Age = 36 }
            };

            var table = ConsoleTable
                .From(users)
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .ToString();

            Console.Write(table);

            Assert.AreEqual(
$@" ------------------- 
 | Name      | Age |
 ------------------- 
 | Alexandre |  36 |
 ------------------- 

 Count: 1", table);
        }

        [TestMethod]
        public void NumberShouldBeRightAlignedOnMarkDownTest()
        {
            List<User> users = new()
            {
                new User { Name = "Alexandre" , Age = 36 }
            };

            var table = ConsoleTable
                .From(users)
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .ToMarkDownString();

            Console.Write(table);

            Assert.AreEqual(
 $@"| Name      | Age |
|-----------|-----|
| Alexandre |  36 |
", table);
        }

        [TestMethod]
        public void OutputShouldDefaultToConsoleOutTest()
        {
            List<User> users = new()
            {
                new User { Name = "Alexandre" , Age = 36 }
            };

            var table = ConsoleTable.From(users);

            Console.Write(table);

            Assert.AreEqual(table.Options.OutputTo, Console.Out);
        }

        [TestMethod]
        public void OutputShouldGoToConfiguredOutputWriterTest()
        {
            List<User> users = new()
            {
                new User { Name = "Alexandre" , Age = 36 }
            };

            var testWriter = new StringWriter();

            ConsoleTable
               .From(users)
               .Configure(o => o.OutputTo = testWriter)
               .Write();

            Assert.IsFalse(string.IsNullOrEmpty(testWriter.ToString()));
        }
    }
}