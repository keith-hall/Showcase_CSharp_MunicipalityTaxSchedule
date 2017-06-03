using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityTaxes
{
    [DataContract]
    public class TaxScheduleActionResult<T>
    {
        [DataMember]
        public TaxScheduleValidationResult Validity;
        [DataMember]
        public T ActionResult;

        public new string ToString()
        {
            return $"Validity: {Validity}, {typeof(T).Name}: {ActionResult}";
        }
    }

    [DataContract]
    public enum TaxScheduleInsertionResult
    {
        [EnumMember]
        InsertionNotAttempted,
        [EnumMember]
        Success,
        [EnumMember]
        TaxScheduleAlreadyExists,
        [EnumMember]
        UnknownFailure,
    }
}
