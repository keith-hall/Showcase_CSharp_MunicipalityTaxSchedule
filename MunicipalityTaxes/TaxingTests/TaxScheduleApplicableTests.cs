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

        [TestMethod]
        public void TestMostApplicable ()
        {
            var daily = new MunicipalityTaxSchedule(municipality: "Kaunas", frequency: ScheduleFrequency.Daily, begin: new DateTime(2017, 06, 05));
            var weekly = new MunicipalityTaxSchedule(municipality: "Kaunas", frequency: ScheduleFrequency.Weekly, begin: new DateTime(2017, 06, 05));
            var monthly = new MunicipalityTaxSchedule(municipality: "Kaunas", frequency: ScheduleFrequency.Monthly, begin: new DateTime(2017, 06, 01));
            var yearly = new MunicipalityTaxSchedule(municipality: "Kaunas", frequency: ScheduleFrequency.Yearly, begin: new DateTime(2017, 01, 01));

            // assuming a check date of 2017-06-05
            Assert.AreEqual(MunicipalityTaxSchedule.MostApplicable(new[] { yearly, monthly, weekly, daily }), daily);
            Assert.AreEqual(MunicipalityTaxSchedule.MostApplicable(new[] { daily, weekly, monthly, yearly }), daily);

            // assuming a check date of 2017-06-06
            Assert.AreEqual(MunicipalityTaxSchedule.MostApplicable(new[] { yearly, monthly, weekly }), weekly);
            Assert.AreEqual(MunicipalityTaxSchedule.MostApplicable(new[] { monthly, weekly, yearly }), weekly);

            // assuming a check date of 2017-05-01
            Assert.AreEqual(MunicipalityTaxSchedule.MostApplicable(new[] { yearly, monthly }), monthly);
            Assert.AreEqual(MunicipalityTaxSchedule.MostApplicable(new[] { monthly, yearly }), monthly);

            // assuming a check date of 2017-04-05
            Assert.AreEqual(MunicipalityTaxSchedule.MostApplicable(new[] { yearly }), yearly);
            
        }
    }
}
