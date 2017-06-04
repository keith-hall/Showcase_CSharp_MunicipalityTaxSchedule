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

            var tax = new MunicipalityTaxSchedule(municipality: "", begin: new DateTime(2017, 06, 03), frequency: ScheduleFrequency.Daily);
            var status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.MunicipalityInvalid, status);

            tax = new MunicipalityTaxSchedule(municipality: "   ", begin: new DateTime(2017, 06, 03), frequency: ScheduleFrequency.Daily);
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.MunicipalityInvalid, status);

            tax = new MunicipalityTaxSchedule(municipality: null, begin: new DateTime(2017, 06, 03), frequency: ScheduleFrequency.Daily);
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.MunicipalityInvalid, status);
        }
        
        [TestMethod]
        public void ScheduleDateCheck ()
        {
            var validator = new TaxScheduleValidator();

            #region Daily
            var tax = new MunicipalityTaxSchedule(municipality: "Test", begin: new DateTime(2017, 06, 03), frequency: ScheduleFrequency.Daily);
            var status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.Valid, status);
            tax = new MunicipalityTaxSchedule(municipality: "Test", begin: new DateTime(2017, 06, 01), frequency: ScheduleFrequency.Daily);
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.Valid, status);
            #endregion

            #region Weekly
            tax = new MunicipalityTaxSchedule(municipality: "Test", begin: new DateTime(2017, 06, 05), frequency: ScheduleFrequency.Weekly);
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.Valid, status);
            tax = new MunicipalityTaxSchedule(municipality: "Test", begin: new DateTime(2017, 06, 06), frequency: ScheduleFrequency.Weekly);
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            tax = new MunicipalityTaxSchedule(municipality: "Test", begin: new DateTime(2017, 06, 04), frequency: ScheduleFrequency.Weekly);
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            #endregion

            #region Monthly
            tax = new MunicipalityTaxSchedule(municipality: "Test", begin: new DateTime(2017, 06, 01), frequency: ScheduleFrequency.Monthly);
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.Valid, status);
            tax = new MunicipalityTaxSchedule(municipality: "Test", begin: new DateTime(2017, 06, 06), frequency: ScheduleFrequency.Monthly);
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            tax = new MunicipalityTaxSchedule(municipality: "Test", begin: new DateTime(2017, 06, 05), frequency: ScheduleFrequency.Monthly);
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            #endregion

            #region Yearly
            tax = new MunicipalityTaxSchedule(municipality: "Test", begin: new DateTime(2017, 01, 01), frequency: ScheduleFrequency.Yearly);
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.Valid, status);
            tax = new MunicipalityTaxSchedule(municipality: "Test", begin: new DateTime(2017, 02, 01), frequency: ScheduleFrequency.Yearly);
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            tax = new MunicipalityTaxSchedule(municipality: "Test", begin: new DateTime(2017, 01, 02), frequency: ScheduleFrequency.Yearly);
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            tax = new MunicipalityTaxSchedule(municipality: "Test", begin: new DateTime(2017, 12, 31), frequency: ScheduleFrequency.Yearly);
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            #endregion

            #region Time
            tax = new MunicipalityTaxSchedule(municipality: "Test", begin: new DateTime(2017, 03, 04, 5, 6, 7), frequency: ScheduleFrequency.Daily);
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            tax = new MunicipalityTaxSchedule(municipality: "Test", begin: new DateTime(2017, 03, 04, 12, 0, 0), frequency: ScheduleFrequency.Daily);
            status = validator.ValidateTaxSchedule(tax);
            Assert.AreEqual(TaxScheduleValidationResult.DateUnsuitableForSchedule, status);
            #endregion
        }

        [TestMethod]
        public void TaxAmountChecks ()
        {
            var validator = new TaxScheduleValidator();

            var schedule = new MunicipalityTaxSchedule(municipality: "Test", begin: new DateTime(2017, 06, 03), frequency: ScheduleFrequency.Daily);
            var tax = new MunicipalityTaxDetails() { MunicipalitySchedule = schedule, TaxAmount = 0.1 };
            var status = validator.ValidateTaxDetails(tax);
            Assert.AreEqual(TaxScheduleValidationResult.Valid, status);

            tax.TaxAmount = 0;
            status = validator.ValidateTaxDetails(tax);
            Assert.AreEqual(TaxScheduleValidationResult.Valid, status);

            tax.TaxAmount = -0.1;
            status = validator.ValidateTaxDetails(tax);
            Assert.AreEqual(TaxScheduleValidationResult.TaxAmountInvalid, status);

            schedule = new MunicipalityTaxSchedule(municipality: null, begin: new DateTime(2017, 06, 03), frequency: ScheduleFrequency.Daily);
            tax = new MunicipalityTaxDetails() { MunicipalitySchedule = schedule, TaxAmount = 0.1 };
            status = validator.ValidateTaxDetails(tax);
            Assert.AreEqual(TaxScheduleValidationResult.MunicipalityInvalid, status);
        }
    }
}
