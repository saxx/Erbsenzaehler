using System;
using Erbsenzaehler.Models;
using Erbsenzaehler.SummaryMail;
using Xunit;

namespace Erbsenzaehler.Tests.SummaryMail
{
    public class SummaryMailIntervalServiceTests
    {

        [Fact]
        public void Always_Return_False_When_Disable_Setting()
        {
            Assert.False(SummaryMailIntervalService.ShouldReceiveSummaryMail(SummaryMailIntervalOptions.Disable, null));
            Assert.False(SummaryMailIntervalService.ShouldReceiveSummaryMail(SummaryMailIntervalOptions.Disable, DateTime.MinValue));
        }

        [Theory]
        [InlineData(SummaryMailIntervalOptions.Daily)]
        [InlineData(SummaryMailIntervalOptions.Weekly)]
        [InlineData(SummaryMailIntervalOptions.Monthly)]
        public void Always_Return_True_When_No_Previous_Mail(SummaryMailIntervalOptions setting)
        {
            Assert.True(SummaryMailIntervalService.ShouldReceiveSummaryMail(setting, null));
        }


        [Fact]
        public void Return_True_On_Daily_Setting_On_Different_Date()
        {
            Assert.True(SummaryMailIntervalService.ShouldReceiveSummaryMail(SummaryMailIntervalOptions.Daily, DateTime.UtcNow.AddDays(-1)));
        }

        [Fact]
        public void Return_False_On_Daily_Setting_On_Some_Date()
        {
            Assert.False(SummaryMailIntervalService.ShouldReceiveSummaryMail(SummaryMailIntervalOptions.Daily, DateTime.UtcNow));
        }

        [Fact]
        public void Return_False_On_Weekly_Setting_On_Same_Date()
        {
            Assert.False(SummaryMailIntervalService.ShouldReceiveSummaryMail(SummaryMailIntervalOptions.Weekly, DateTime.UtcNow));
        }

        [Fact]
        public void Return_Correct_Value_On_Weekly_Setting_Based_On_Current_DayOfWeek()
        {
            // this test is quite stupid, because it depends on the day it runs
            // but the tested logic is trivial, and I put this test here so one day in a code review somebody thinks of a better test

            if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Sunday)
            {
                Assert.True(SummaryMailIntervalService.ShouldReceiveSummaryMail(SummaryMailIntervalOptions.Weekly, DateTime.UtcNow.AddYears(-1)));
            }
            else
            {
                Assert.False(SummaryMailIntervalService.ShouldReceiveSummaryMail(SummaryMailIntervalOptions.Weekly, DateTime.UtcNow.AddYears(-1)));
            }
        }

        [Fact]
        public void Return_True_On_Monthly_Setting_On_Different_Month()
        {
            Assert.True(SummaryMailIntervalService.ShouldReceiveSummaryMail(SummaryMailIntervalOptions.Monthly, DateTime.UtcNow.AddMonths(-1)));
        }

        [Fact]
        public void Return_True_On_Monthly_Setting_On_Different_Year()
        {
            Assert.True(SummaryMailIntervalService.ShouldReceiveSummaryMail(SummaryMailIntervalOptions.Monthly, new DateTime(DateTime.UtcNow.Year - 1, DateTime.UtcNow.Month, DateTime.UtcNow.Day)));
        }

        [Fact]
        public void Return_False_On_Monthly_Setting_On_Same_Date()
        {
            Assert.False(SummaryMailIntervalService.ShouldReceiveSummaryMail(SummaryMailIntervalOptions.Monthly, DateTime.UtcNow));
        }

        [Fact]
        public void Return_False_On_Monthly_Setting_On_Same_Month()
        {
            Assert.False(SummaryMailIntervalService.ShouldReceiveSummaryMail(SummaryMailIntervalOptions.Monthly, new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1)));
        }





    }
}
