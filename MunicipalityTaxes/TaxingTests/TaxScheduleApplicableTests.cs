using System;
using MunicipalityTaxes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TaxingTests
{
    [TestClass]
    public class TaxScheduleApplicableTests
    {
        [TestMethod]
        public void TestApplicable ()
        {
            var schedule = new MunicipalityTaxSchedule(municipality: "Kaunas", frequency: ScheduleFrequency.Daily, begin: new DateTime(2017, 06, 02));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddDays(-1)));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddDays(1)));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddMonths(-1)));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddMonths(1)));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddYears(-1)));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddYears(1)));

            schedule = new MunicipalityTaxSchedule(municipality: "Kaunas", frequency: ScheduleFrequency.Weekly, begin: new DateTime(2017, 06, 05));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddDays(-1)));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate.AddDays(1)));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate.AddDays(2)));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate.AddDays(3)));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate.AddDays(4)));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate.AddDays(5)));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate.AddDays(6)));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddDays(7)));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddMonths(-1)));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddMonths(1)));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddYears(-1)));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddYears(1)));

            schedule = new MunicipalityTaxSchedule(municipality: "Kaunas", frequency: ScheduleFrequency.Monthly, begin: new DateTime(2017, 06, 01));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddDays(-1)));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate.AddDays(1)));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddMonths(1)));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate.AddMonths(1).AddDays(-1)));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddYears(-1)));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddYears(1)));

            schedule = new MunicipalityTaxSchedule(municipality: "Kaunas", frequency: ScheduleFrequency.Yearly, begin: new DateTime(2017, 01, 01));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddDays(-1)));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate.AddDays(1)));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate.AddMonths(1)));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate.AddMonths(1).AddDays(-1)));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddYears(-1)));
            Assert.IsFalse(schedule.IsApplicable(schedule.ScheduleBeginDate.AddYears(1)));
            Assert.IsTrue(schedule.IsApplicable(schedule.ScheduleBeginDate.AddYears(1).AddDays(-1)));
        }
    }
}
