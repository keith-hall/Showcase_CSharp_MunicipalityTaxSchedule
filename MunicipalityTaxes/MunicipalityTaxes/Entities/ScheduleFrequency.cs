using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityTaxes
{
    [DataContract]
    public enum ScheduleFrequency
    {
        [EnumMember]
        Yearly = 0,
        [EnumMember]
        Monthly,
        [EnumMember]
        Weekly,
        [EnumMember]
        Daily,
    }
}
