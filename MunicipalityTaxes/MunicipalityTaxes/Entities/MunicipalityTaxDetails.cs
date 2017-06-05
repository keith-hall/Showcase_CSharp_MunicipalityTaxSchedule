﻿using System;
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
    public class MunicipalityTaxDetails
    {
        [DataMember(IsRequired = true, Order = 0)]
        public MunicipalityTaxSchedule MunicipalitySchedule;
        [DataMember(IsRequired = true, Order = 1)]
        public double TaxAmount;

        internal string DebuggerDisplay { get { return MunicipalitySchedule?.DebuggerDisplay + $", {TaxAmount}"; } }
    }
}
