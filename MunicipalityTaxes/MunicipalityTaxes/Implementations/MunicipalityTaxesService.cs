using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MunicipalityTaxes
{
    public class MunicipalityTaxesService : IMunicipalityTaxesService
    {
        public MunicipalityTaxDetails GetDataUsingDataContract (MunicipalityTaxDetails composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException(nameof(composite));
            }
            return composite;
        }
    }
}
