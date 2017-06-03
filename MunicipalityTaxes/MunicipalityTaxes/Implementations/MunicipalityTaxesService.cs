using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using NLog;

namespace MunicipalityTaxes
{
    public class MunicipalityTaxesService : IMunicipalityTaxesService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        internal ITaxStorage TaxStorage;
        internal ITaxScheduleValidator TaxValidator;

        public MunicipalityTaxesService ()
        {
            TaxValidator = new TaxScheduleValidator();
        }

        public TaxScheduleActionResult<TaxScheduleInsertionResult> InsertTaxScheduleDetails (MunicipalityTaxDetails tax)
        {
            logger.Trace("{0} request received with parameters: {1}: {2}", nameof(InsertTaxScheduleDetails), nameof(tax), tax.DebuggerDisplay);
            if (tax == null)
                throw new ArgumentNullException(nameof(tax));
            if (tax.MunicipalitySchedule == null)
                throw new ArgumentNullException(nameof(tax.MunicipalitySchedule));
            
            var insertResult = TaxScheduleInsertionResult.InsertionNotAttempted;
            var checkValidity = TaxScheduleValidationResult.Unknown;
            try
            {
                checkValidity = TaxValidator.ValidateTaxSchedule(tax.MunicipalitySchedule);
                logger.Trace("{0} validity status is: {1}", nameof(tax.MunicipalitySchedule), checkValidity);

                if (checkValidity == TaxScheduleValidationResult.Valid)
                {
                    try
                    {
                        // NOTE: if multithreaded, there could be a race condition between the existence check and insertion
                        if (TaxStorage.TaxScheduleExists(tax.MunicipalitySchedule))
                        {
                            insertResult = TaxScheduleInsertionResult.TaxScheduleAlreadyExists;
                        }
                        else
                        {
                            TaxStorage.InsertTaxSchedule(tax);
                            insertResult = TaxScheduleInsertionResult.Success;
                        }
                    }
                    catch (Exception)
                    {
                        insertResult = TaxScheduleInsertionResult.UnknownFailure;
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception occurred in {0} method", nameof(InsertTaxScheduleDetails));
#if DEBUG
                if (Debugger.IsAttached)
                    Debugger.Break();
#endif
            }
            var response = new TaxScheduleActionResult<TaxScheduleInsertionResult>() { ActionResult = insertResult, Validity = checkValidity };
            logger.Trace("Returning {0} response: {1}", nameof(InsertTaxScheduleDetails), response.ToString());
            return response;
        }
    }
}