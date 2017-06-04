using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using NLog;

namespace MunicipalityTaxes
{
    [
    ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single
    )
    ]
    public class MunicipalityTaxesService : IMunicipalityTaxesService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        internal ITaxStorage TaxStorage;
        internal ITaxScheduleValidator TaxValidator;

        public MunicipalityTaxesService ()
        {
            TaxValidator = new TaxScheduleValidator();
            TaxStorage = new InMemoryTaxStorageProvider();

            var className = ConfigurationManager.AppSettings["ITaxScheduleValidator"] ?? typeof(TaxScheduleValidator).FullName;
            var instanceType = Type.GetType(className);
            //if (instanceType as ITaxScheduleValidator == null)
            if (instanceType == null || !instanceType.GetInterfaces().Contains(typeof(ITaxScheduleValidator)))
            {
                // technically this is an internal error we shouldn't show, but let's say the restriction is only for the public facing API
                throw new ConfigurationErrorsException($"Type {className} not found or not valid, please check AppSettings configuration key value pair 'ITaxScheduleValidator'");
            }
            try
            {
                var instance = Activator.CreateInstance(instanceType);
                TaxValidator = (ITaxScheduleValidator)instance;
            } catch (Exception ex)
            {
                logger.Error(ex, "Unable to construct type {0}", className);
                throw new ConfigurationErrorsException($"Unable to construct ITaxScheduleValidator Type {className}", ex);
            }
        }

        public double? GetTax (string muncipality, DateTime at)
        {
            logger.Trace("{0} request received with parameters: {1}: {2}, {2}: {3}", nameof(GetTax), nameof(muncipality), muncipality, nameof(at), at);
            double? taxAmount = null;
            try
            {
                var tax = TaxStorage.GetTax(muncipality, at);
                taxAmount = tax?.TaxAmount;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception occurred in {0} method", nameof(GetTax));
#if DEBUG
                if (Debugger.IsAttached)
                    Debugger.Break();
#endif
            }
            logger.Trace("Returning {0} response: {1}", nameof(GetTax), taxAmount);
            return taxAmount;
        }

        public TaxScheduleActionResult<TaxScheduleInsertionResult> InsertTaxScheduleDetails (MunicipalityTaxDetails tax)
        {
            logger.Trace("{0} request received with parameters: {1}: {2}", nameof(InsertTaxScheduleDetails), nameof(tax), tax?.DebuggerDisplay);
            if (tax == null)
                throw new ArgumentNullException(nameof(tax));
            if (tax.MunicipalitySchedule == null)
                throw new ArgumentNullException(nameof(tax.MunicipalitySchedule));
            
            var insertResult = TaxScheduleInsertionResult.InsertionNotAttempted;
            var checkValidity = TaxScheduleValidationResult.Unknown;
            try
            {
                checkValidity = TaxValidator.ValidateTaxDetails(tax);
                logger.Trace("{0} validity status is: {1}", nameof(tax), checkValidity);

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
            var response = new TaxScheduleActionResult<TaxScheduleInsertionResult>(checkValidity, insertResult);
            logger.Trace("Returning {0} response: {1}", nameof(InsertTaxScheduleDetails), response.ToString());
            return response;
        }

        public TaxScheduleActionResult<TaxScheduleUpdateResult> UpdateTaxScheduleDetails (MunicipalityTaxDetails tax)
        {
            logger.Trace("{0} request received with parameters: {1}: {2}", nameof(UpdateTaxScheduleDetails), nameof(tax), tax?.DebuggerDisplay);
            if (tax == null)
                throw new ArgumentNullException(nameof(tax));
            if (tax.MunicipalitySchedule == null)
                throw new ArgumentNullException(nameof(tax.MunicipalitySchedule));

            var updateResult = TaxScheduleUpdateResult.UpdateNotAttempted;
            var checkValidity = TaxScheduleValidationResult.Unknown;
            try
            {
                checkValidity = TaxValidator.ValidateTaxDetails(tax);
                logger.Trace("{0} validity status is: {1}", nameof(tax), checkValidity);

                if (checkValidity == TaxScheduleValidationResult.Valid)
                {
                    try
                    {
                        // NOTE: if multithreaded, there could be a race condition between the existence check and updating, maybe some other thread will delete it meanwhile
                        //       - we could use locks to prevent this
                        if (!TaxStorage.TaxScheduleExists(tax.MunicipalitySchedule))
                        {
                            updateResult = TaxScheduleUpdateResult.ExistingTaxScheduleNotFound;
                        }
                        else
                        {
                            TaxStorage.UpdateTaxSchedule(tax);
                            updateResult = TaxScheduleUpdateResult.Success;
                        }
                    }
                    catch (Exception)
                    {
                        updateResult = TaxScheduleUpdateResult.UnknownFailure;
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception occurred in {0} method", nameof(UpdateTaxScheduleDetails));
#if DEBUG
                if (Debugger.IsAttached)
                    Debugger.Break();
#endif
            }
            var response = new TaxScheduleActionResult<TaxScheduleUpdateResult>(checkValidity, updateResult);
            logger.Trace("Returning {0} response: {1}", nameof(UpdateTaxScheduleDetails), response.ToString());
            return response;
        }
    }
}