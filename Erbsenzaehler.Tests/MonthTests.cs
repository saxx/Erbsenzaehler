using System;
using Xunit;
using Erbsenzaehler.Models;

namespace Erbsenzaehler.Tests
{
    public class MonthTests
    {

        #region Constructor

        [Fact]
        public void Empty_Constructor_Creates_Current_Month()
        {
            var m = new Month();
            Assert.Equal(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), m.Date);
        }


        [Fact]
        public void Date_Constructor_Creates_Current_Month()
        {
            var m = new Month(DateTime.Now);
            Assert.Equal(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), m.Date);
        }


        [Fact]
        public void Date_Constructor_Creates_Correct_Month()
        {
            var m = new Month(new DateTime(2006, 6, 6));
            Assert.Equal(new DateTime(2006, 6, 1), m.Date);
        }
        


        [Theory]
        [InlineData("2006-06")]
        [InlineData("2006-6")]
        public void String_Constructor_Creates_Correct_Month(string input)
        {
            var m = new Month(input);
            Assert.Equal(new DateTime(2006, 6, 1), m.Date);
        }


        [Theory]
        [InlineData("2006")]
        [InlineData("")]
        [InlineData("asdf")]
        [InlineData("2006-13")]
        public void Invalid_String_Constructor_Creates_Current_Month(string input)
        {
            var m = new Month(input);
            Assert.Equal(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), m.Date);
        }

        [Theory]
        [InlineData("2006")]
        [InlineData("")]
        [InlineData("asdf")]
        public void Invalid_String_Constructor_Throws_ArgumentException(string input)
        {
            Assert.Throws(typeof(ArgumentException), () => new Month(input, false));
        }

        [Theory]
        [InlineData("2006-13")]
        public void Invalid_String_Constructor_Throws_RangeException(string input)
        {
            Assert.Throws(typeof(ArgumentOutOfRangeException), () => new Month(input, false));
        }
        #endregion



    }
}
