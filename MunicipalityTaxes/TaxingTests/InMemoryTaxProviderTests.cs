using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MunicipalityTaxes;

namespace TaxingTests
{
    [TestClass]
    public class InMemoryProviderTests
    {
        [TestMethod]
        public void CanAddAndFindTax ()
        {
            var tax1 = new MunicipalityTaxDetails() { MunicipalitySchedule = new MunicipalityTaxSchedule("Vilnius", ScheduleFrequency.Daily, new DateTime(2017, 06, 05)), TaxAmount = 0.5 };
            var tax2 = new MunicipalityTaxDetails() { MunicipalitySchedule = new MunicipalityTaxSchedule("Vilnius", ScheduleFrequency.Weekly, new DateTime(2017, 06, 05)), TaxAmount = 1.0 };
            var tax3 = new MunicipalityTaxDetails() { MunicipalitySchedule = new MunicipalityTaxSchedule("Kaunas", ScheduleFrequency.Daily, new DateTime(2017, 06, 05)), TaxAmount = 1.5 };
            var tax4 = new MunicipalityTaxDetails() { MunicipalitySchedule = new MunicipalityTaxSchedule("Vilnius", ScheduleFrequency.Daily, new DateTime(2017, 06, 18)), TaxAmount = 2.0 };

            var db = new InMemoryTaxStorageProvider();

            Assert.IsNull(db.FindTaxSchedule(tax1.MunicipalitySchedule));
            Assert.IsNull(db.FindTaxSchedule(tax2.MunicipalitySchedule));
            Assert.IsNull(db.FindTaxSchedule(tax3.MunicipalitySchedule));
            Assert.IsNull(db.FindTaxSchedule(tax4.MunicipalitySchedule));

            db.InsertTaxSchedule(tax1);

            Assert.AreEqual(db.FindTaxSchedule(tax1.MunicipalitySchedule).MunicipalitySchedule, tax1.MunicipalitySchedule);
            Assert.AreEqual(db.FindTaxSchedule(tax1.MunicipalitySchedule), tax1);
            Assert.IsNull(db.FindTaxSchedule(tax2.MunicipalitySchedule));
            Assert.IsNull(db.FindTaxSchedule(tax3.MunicipalitySchedule));
            Assert.IsNull(db.FindTaxSchedule(tax4.MunicipalitySchedule));
        }

        [TestMethod]
        public void TestFromAssignmentSpecification ()
        {
            var tax1 = new MunicipalityTaxDetails() { MunicipalitySchedule = new MunicipalityTaxSchedule("Vilnius", ScheduleFrequency.Yearly, new DateTime(2016, 01, 01)), TaxAmount = 0.2 };
            var tax2 = new MunicipalityTaxDetails() { MunicipalitySchedule = new MunicipalityTaxSchedule("Vilnius", ScheduleFrequency.Monthly, new DateTime(2016, 05, 01)), TaxAmount = 0.4 };
            var tax3 = new MunicipalityTaxDetails() { MunicipalitySchedule = new MunicipalityTaxSchedule("Vilnius", ScheduleFrequency.Daily, new DateTime(2016, 01, 01)), TaxAmount = 0.1 };
            var tax4 = new MunicipalityTaxDetails() { MunicipalitySchedule = new MunicipalityTaxSchedule("Vilnius", ScheduleFrequency.Daily, new DateTime(2016, 12, 25)), TaxAmount = 0.1 };

            var db = new InMemoryTaxStorageProvider();

            db.InsertTaxSchedule(tax1);
            db.InsertTaxSchedule(tax2);
            db.InsertTaxSchedule(tax3);
            db.InsertTaxSchedule(tax4);

            Assert.AreEqual(db.GetTax("Vilnius", new DateTime(2016, 01, 01)).TaxAmount, 0.1);
            Assert.AreEqual(db.GetTax("Vilnius", new DateTime(2016, 05, 02)).TaxAmount, 0.4);
            Assert.AreEqual(db.GetTax("Vilnius", new DateTime(2016, 07, 10)).TaxAmount, 0.2);
            Assert.AreEqual(db.GetTax("Vilnius", new DateTime(2016, 03, 16)).TaxAmount, 0.2);
        }

        [TestMethod]
        public void CanDeleteTaxTest()
        {
            var tax1 = new MunicipalityTaxDetails() { MunicipalitySchedule = new MunicipalityTaxSchedule("Vilnius", ScheduleFrequency.Yearly, new DateTime(2016, 01, 01)), TaxAmount = 0.2 };
            var tax2 = new MunicipalityTaxDetails() { MunicipalitySchedule = new MunicipalityTaxSchedule("Vilnius", ScheduleFrequency.Monthly, new DateTime(2016, 05, 01)), TaxAmount = 0.4 };
            
            var db = new InMemoryTaxStorageProvider();

            db.InsertTaxSchedule(tax1);
            db.InsertTaxSchedule(tax2);

            Assert.IsTrue(db.TaxScheduleExists(tax1.MunicipalitySchedule));
            Assert.IsTrue(db.TaxScheduleExists(tax2.MunicipalitySchedule));

            db.DeleteTaxSchedule(tax1.MunicipalitySchedule);

            Assert.IsFalse(db.TaxScheduleExists(tax1.MunicipalitySchedule));
            Assert.IsTrue(db.TaxScheduleExists(tax2.MunicipalitySchedule));
        }
    }
}
