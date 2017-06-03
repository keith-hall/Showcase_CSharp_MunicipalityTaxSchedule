using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MunicipalityTaxes;

namespace TaxingTests
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        public void NoMunicipality ()
        {
            var validator = new TaxScheduleValidator();

            var tax = new MunicipalityTaxSchedule() { Municipality = "", ScheduleBeginDate = new DateTime(2017, 06, 03), ScheduleType = ScheduleFrequency.Daily };
            var status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.MunicipalityInvalid, status);

            tax = new MunicipalityTaxSchedule() { Municipality = "   ", ScheduleBeginDate = new DateTime(2017, 06, 03), ScheduleType = ScheduleFrequency.Daily };
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.MunicipalityInvalid, status);

            tax = new MunicipalityTaxSchedule() { Municipality = null, ScheduleBeginDate = new DateTime(2017, 06, 03), ScheduleType = ScheduleFrequency.Daily };
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.MunicipalityInvalid, status);
        }
        
        [TestMethod]
        public void ScheduleDateCheck ()
        {
            var validator = new TaxScheduleValidator();

            #region Daily
            var tax = new MunicipalityTaxSchedule() { Municipality = "Test", ScheduleBeginDate = new DateTime(2017, 06, 03), ScheduleType = ScheduleFrequency.Daily };
            var status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.Valid, status);
            tax = new MunicipalityTaxSchedule() { Municipality = "Test", ScheduleBeginDate = new DateTime(2017, 06, 01), ScheduleType = ScheduleFrequency.Daily };
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.Valid, status);
            #endregion

            #region Weekly
            tax = new MunicipalityTaxSchedule() { Municipality = "Test", ScheduleBeginDate = new DateTime(2017, 06, 05), ScheduleType = ScheduleFrequency.Weekly };
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.Valid, status);
            tax = new MunicipalityTaxSchedule() { Municipality = "Test", ScheduleBeginDate = new DateTime(2017, 06, 06), ScheduleType = ScheduleFrequency.Weekly };
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            tax = new MunicipalityTaxSchedule() { Municipality = "Test", ScheduleBeginDate = new DateTime(2017, 06, 04), ScheduleType = ScheduleFrequency.Weekly };
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            #endregion

            #region Monthly
            tax = new MunicipalityTaxSchedule() { Municipality = "Test", ScheduleBeginDate = new DateTime(2017, 06, 01), ScheduleType = ScheduleFrequency.Monthly  };
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.Valid, status);
            tax = new MunicipalityTaxSchedule() { Municipality = "Test", ScheduleBeginDate = new DateTime(2017, 06, 06), ScheduleType = ScheduleFrequency.Monthly };
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            tax = new MunicipalityTaxSchedule() { Municipality = "Test", ScheduleBeginDate = new DateTime(2017, 06, 05), ScheduleType = ScheduleFrequency.Monthly };
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            #endregion

            #region Yearly
            tax = new MunicipalityTaxSchedule() { Municipality = "Test", ScheduleBeginDate = new DateTime(2017, 01, 01), ScheduleType = ScheduleFrequency.Yearly };
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.Valid, status);
            tax = new MunicipalityTaxSchedule() { Municipality = "Test", ScheduleBeginDate = new DateTime(2017, 02, 01), ScheduleType = ScheduleFrequency.Yearly };
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            tax = new MunicipalityTaxSchedule() { Municipality = "Test", ScheduleBeginDate = new DateTime(2017, 01, 02), ScheduleType = ScheduleFrequency.Yearly };
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            tax = new MunicipalityTaxSchedule() { Municipality = "Test", ScheduleBeginDate = new DateTime(2017, 12, 31), ScheduleType = ScheduleFrequency.Yearly };
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            #endregion

            #region Time
            tax = new MunicipalityTaxSchedule() { Municipality = "Test", ScheduleBeginDate = new DateTime(2017, 03, 04, 5, 6, 7), ScheduleType = ScheduleFrequency.Daily };
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            tax = new MunicipalityTaxSchedule() { Municipality = "Test", ScheduleBeginDate = new DateTime(2017, 03, 04, 12, 0, 0), ScheduleType = ScheduleFrequency.Daily };
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            #endregion
        }
    }
}
