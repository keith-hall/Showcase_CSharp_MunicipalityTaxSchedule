using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityTaxes
{
    [DataContract]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class MunicipalityTaxSchedule
    {
        [DataMember]
        public string Municipality;
        [DataMember]
        public ScheduleFrequency ScheduleType;
        [DataMember]
        public DateTime ScheduleBeginDate;

        internal string DebuggerDisplay { get { return $"{GetType().Name}: {Municipality}, {ScheduleType}, {ScheduleBeginDate:yyyy.MM.dd}"; } }
    }
}
