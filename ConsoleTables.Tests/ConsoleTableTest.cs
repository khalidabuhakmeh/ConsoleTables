using System;
using System.Collections.Generic;
using Xunit;

namespace ConsoleTables.Tests
{
    public class ConsoleTableTest
    {
        [Fact]
        public void ShouldBeToStringFromList()
        {
            var users = new List<User>
            {
                new User { Name = "Alexandre" , Age = 36 }
            };
            var table = ConsoleTable.From(users).ToString();

            Assert.Equal($@" ------------------- 
 | Name      | Age |
 ------------------- 
 | Alexandre | 36  |
 ------------------- 

 Count: 1", table);
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
                .Configure(o => o.NumberRigthAligned = true)
                .ToString();

            Assert.Equal($@" ------------------- 
 | Name      | Age |
 ------------------- 
 | Alexandre |  36 |
 ------------------- 

 Count: 1", table);
        }

        class User
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}
