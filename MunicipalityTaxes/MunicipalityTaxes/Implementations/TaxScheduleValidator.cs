using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace MunicipalityTaxes
{
    class TaxScheduleValidator : ITaxScheduleValidator
    {
        public TaxScheduleValidationResult ValidateTaxSchedule (MunicipalityTaxSchedule tax)
        {
            if (string.IsNullOrWhiteSpace(tax.Municipality))
                return TaxScheduleValidationResult.MunicipalityInvalid;

            if (tax.ScheduleBeginDate.TimeOfDay.Ticks > 0)
                return TaxScheduleValidationResult.DateUnsuitableForSchedule;

            if (tax.ScheduleType == ScheduleFrequency.Monthly && tax.ScheduleBeginDate.Day != 1)
                return TaxScheduleValidationResult.DateUnsuitableForSchedule;

            if (tax.ScheduleType == ScheduleFrequency.Weekly && tax.ScheduleBeginDate.DayOfWeek != DayOfWeek.Monday) // culture invariant first day of week hardcoded to Monday
                return TaxScheduleValidationResult.DateUnsuitableForSchedule;

            if (tax.ScheduleType == ScheduleFrequency.Yearly && (tax.ScheduleBeginDate.Day != 1 || tax.ScheduleBeginDate.Month != 1))
                return TaxScheduleValidationResult.DateUnsuitableForSchedule;

            return TaxScheduleValidationResult.Valid;
        }
    }
}
