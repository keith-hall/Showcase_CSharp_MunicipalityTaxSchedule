using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
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
            TaxValidator = ConstructTypeImplementingInterface<ITaxScheduleValidator>("ITaxScheduleValidator", typeof(TaxScheduleValidator));

            TaxStorage = ConstructTypeImplementingInterface<ITaxStorage>("ITaxStorage", typeof(InMemoryTaxStorageProvider));
        }

        internal static T ConstructTypeImplementingInterface<T>(string configSettingName, Type defaultType)
        {
            var className = ConfigurationManager.AppSettings[configSettingName];
            var instanceType = className == null ? defaultType : Type.GetType(className);
            if (instanceType == null || !instanceType.GetInterfaces().Contains(typeof(T)))
            {
                // technically this is an internal error we shouldn't show, but let's say the restriction is only for the public facing API
                throw new ConfigurationErrorsException($"Type {className} not found or not valid, please check AppSettings configuration key value pair '{configSettingName}'");
            }
            try
            {
                var instance = Activator.CreateInstance(instanceType);
                return (T)instance;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Unable to construct type {0}", className);
                throw new ConfigurationErrorsException($"Unable to construct {typeof(T).Name} Type {className}", ex);
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
                throw new ArgumentNullException(nameof(tax)); // argument exceptions are not internal errors, but errors with the caller, so we can throw these
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
                        throw; // this will be re-caught further down
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

        public BulkImportResponse InsertTaxScheduleDetailsFromFile(string path)
        {
            logger.Trace("{0} request received with parameters: {1}: {2}", nameof(InsertTaxScheduleDetailsFromFile), nameof(path), path);
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            var responseCode = BulkImportStatus.UnknownFailure;
            List<KeyValuePair<MunicipalityTaxDetails, TaxScheduleActionResult<TaxScheduleInsertionResult>>> results = null;
            try
            {
                if (!File.Exists(path))
                {
                    logger.Error("File '{0}' does not exist", path);
                    responseCode = BulkImportStatus.ParseError;
                }
                else
                {
                    // TODO: move to a separate provider class for IoC, then can have e.g. an XML file parser, a CSV file parser etc.

                    responseCode = BulkImportStatus.Success;
                    var parsedItems = new List<MunicipalityTaxDetails>();
                    // parse the file
                    foreach (var line in File.ReadLines(path))
                    {
                        var items = line.Split('|');
                        if (items.Length != 4)
                        {
                            logger.Error("Error parsing file - line contains only {0} items, expected 4", items.Length); // TODO: include the line number of the failure
                            responseCode = BulkImportStatus.ParseError;
                            break;
                        }
                        try
                        {
                            var municipality = items[0];
                            var frequency = (ScheduleFrequency)Enum.Parse(typeof(ScheduleFrequency), items[1]);
                            var begin = DateTime.Parse(items[2]);
                            var amount = double.Parse(items[3]);
                            var tax = new MunicipalityTaxDetails() { MunicipalitySchedule = new MunicipalityTaxSchedule(municipality, frequency, begin), TaxAmount = amount };
                            parsedItems.Add(tax);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, "Error parsing file"); // TODO: include the line number of the failure
                            throw;
                        }
                    }
                    if (responseCode == BulkImportStatus.Success)
                    {
                        // there are two approaches we could take here, either don't insert any tax schedules if any one fails to validate, or insert those that pass validation
                        // we are going to go with insert those that pass validation for simplicity and code re-use
                        // however, we want to be able to show what schedules were parsed and their status in case of an error, so we build the initial list here and then update the status for each item individually
                        results = parsedItems.Select(t => new KeyValuePair<MunicipalityTaxDetails, TaxScheduleActionResult<TaxScheduleInsertionResult>>(t, new TaxScheduleActionResult<TaxScheduleInsertionResult>(TaxScheduleValidationResult.Unknown, TaxScheduleInsertionResult.InsertionNotAttempted))).ToList();
                        foreach (var item in results)
                        {
                            var result = InsertTaxScheduleDetails(item.Key);
                            item.Value.Validity = result.Validity;
                            item.Value.ActionResult = result.ActionResult;
                        }
                    }
                }
            } catch (Exception ex)
            {
                logger.Error(ex, "Exception occurred in {0} method", nameof(InsertTaxScheduleDetailsFromFile));
                responseCode = BulkImportStatus.UnknownFailure;
            }
            return new BulkImportResponse() { Status = responseCode, lineItems = results };
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
                        //       - we could use locks or (depending on the provider) a specific transaction type to prevent this
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
                        throw; // this will be re-caught further down
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