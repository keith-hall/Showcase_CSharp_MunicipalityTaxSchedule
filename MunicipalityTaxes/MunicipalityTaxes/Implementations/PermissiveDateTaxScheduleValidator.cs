using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace MunicipalityTaxes
{
    class PermissiveDateTaxScheduleValidator : ITaxScheduleValidator
    {
        public TaxScheduleValidationResult ValidateTaxDetails (MunicipalityTaxDetails tax)
        {
            if (tax.TaxAmount < 0)
                return TaxScheduleValidationResult.TaxAmountInvalid;
            return ValidateTaxSchedule(tax.MunicipalitySchedule);
        }

        public TaxScheduleValidationResult ValidateTaxSchedule (MunicipalityTaxSchedule tax)
        {
            if (string.IsNullOrWhiteSpace(tax.Municipality))
                return TaxScheduleValidationResult.MunicipalityInvalid;

            if (tax.ScheduleBeginDate.TimeOfDay.Ticks > 0)
                return TaxScheduleValidationResult.DateUnsuitableForSchedule;
            
            return TaxScheduleValidationResult.Valid;
        }
    }
}
