using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityTaxes
{
    interface ITaxScheduleValidator
    {
        TaxScheduleValidationResult ValidateTaxSchedule (MunicipalityTaxSchedule taxSchedule);
    }
}
