using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MunicipalityTaxes
{
    [ServiceContract]
    public interface IMunicipalityTaxesService
    {
        [OperationContract]
        TaxScheduleActionResult<TaxScheduleInsertionResult> InsertTaxScheduleDetails (MunicipalityTaxDetails tax);

        /// <summary>
        /// Get the specific municipality tax that applies on the given date
        /// </summary>
        /// <param name="Muncipality">The name of the muncipality to check the tax of</param>
        /// <param name="at">The date on which to get the taxes for</param>
        /// <returns>The tax that applies/applied for the given <paramref name="Muncipality"/> on the given date, or <code>null</code> if no schedules exist that match the given input parameters</returns>
        [OperationContract]
        double? GetTax (string Muncipality, DateTime at);
    }

}
