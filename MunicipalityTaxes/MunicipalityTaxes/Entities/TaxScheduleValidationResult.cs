using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityTaxes
{
    [DataContract]
    public enum TaxScheduleValidationResult
    {
        [EnumMember]
        Unknown = 0,
        [EnumMember]
        Valid,
        [EnumMember]
        DateUnsuitableForSchedule,
        [EnumMember]
        MunicipalityInvalid,
    }
}
