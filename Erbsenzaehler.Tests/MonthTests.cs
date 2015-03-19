using System;
using Erbsenzaehler.Models;
using Xunit;

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
            Assert.Throws(typeof (ArgumentException), () => new Month(input, false));
        }


        [Theory]
        [InlineData("2006-13")]
        public void Invalid_String_Constructor_Throws_RangeException(string input)
        {
            Assert.Throws(typeof (ArgumentOutOfRangeException), () => new Month(input, false));
        }

        #endregion

        #region ToString

        [Theory]
        [InlineData("2006-6", "2006-06")]
        [InlineData("2006-06", "2006-06")]
        [InlineData("2012-12", "2012-12")]
        public void ToString_Returns_Correct_Month(string input, string expectedResult)
        {
            var m = new Month(input);
            Assert.Equal(expectedResult, m.ToString());
        }

        #endregion

        #region NumberOfDays

        [Theory]
        [InlineData("2006-06", 30)]
        [InlineData("2006-05", 31)]
        [InlineData("2006-02", 28)]
        [InlineData("2004-02", 29)]
        public void NumberOfDays_Returns_Correct_Days(string input, int expectedResult)
        {
            var m = new Month(input);
            Assert.Equal(expectedResult, m.NumberOfDays);
        }

        #endregion

        #region NumberOfDaysLeft

        [Theory]
        [InlineData("2006-06")]
        [InlineData("2006-05")]
        [InlineData("2006-02")]
        [InlineData("2004-02")]
        public void NumberOfDaysLeft_Returns_Zero_In_Past_Months(string input)
        {
            var m = new Month(input);
            Assert.Equal(0, m.NumberOfDaysLeft);
        }


        [Theory]
        [InlineData("2066-06", 30)]
        [InlineData("2066-05", 31)]
        [InlineData("2066-02", 28)]
        [InlineData("2080-02", 29)]
        public void NumberOfDaysLeft_Returns_All_Days_For_Future_Months(string input, int expectedResult)
        {
            var m = new Month(input);
            Assert.Equal(expectedResult, m.NumberOfDaysLeft);
        }


        [Fact]
        public void NumberOfDaysLeft_Returns_Correct_Days_For_Current_Month()
        {
            var m = new Month();

            var firstDayOfNextMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
            Assert.Equal((int) (firstDayOfNextMonth - DateTime.Now).TotalDays, m.NumberOfDaysLeft);
        }

        #endregion

        #region Previous and Next Month

        [Theory]
        [InlineData("2006-06", "2006-05")]
        [InlineData("2004-01", "2003-12")]
        public void PreviousMonth_Returns_Previous_Month(string input, string expectedResult)
        {
            var m = new Month(input);
            Assert.Equal(expectedResult, m.PreviousMonth.ToString());
        }


        [Theory]
        [InlineData("2006-06", "2006-07")]
        [InlineData("2004-12", "2005-01")]
        public void NextMonth_Returns_Next_Month(string input, string expectedResult)
        {
            var m = new Month(input);
            Assert.Equal(expectedResult, m.NextMonth.ToString());
        }

        #endregion

        #region IsCurrentMonth 

        [Fact]
        public void IsCurrentMonth_Returns_False_On_Past_Month()
        {
            var m = new Month("2010-01");
            Assert.False(m.IsCurrentMonth);
        }


        [Fact]
        public void IsCurrentMonth_Returns_False_On_Future_Month()
        {
            var m = new Month("2060-12");
            Assert.False(m.IsCurrentMonth);
        }


        [Fact]
        public void IsCurrentMonth_Returns_True_On_Current_Month()
        {
            var m = new Month(DateTime.Now);
            Assert.False(m.IsCurrentMonth);
        }

        #endregion
    }
}
