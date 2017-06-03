using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityTaxes
{
    interface ITaxStorage
    {
        bool TaxScheduleExists (MunicipalityTaxSchedule tax);
        void InsertTaxSchedule (MunicipalityTaxDetails tax);
        void UpdateTaxSchedule (MunicipalityTaxDetails tax);
        void DeleteTaxSchedule (MunicipalityTaxSchedule tax);
        MunicipalityTaxDetails GetTax (string municipality, DateTime at);
    }
}
