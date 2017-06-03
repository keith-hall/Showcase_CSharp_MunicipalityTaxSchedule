using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityTaxes
{
    [DataContract]
    public class TaxScheduleInsertionActionResult
    {
        [DataMember]
        public TaxScheduleValidationResult Validity;// { get; }
        [DataMember]
        public TaxScheduleInsertionResult ActionResult;// { get; }

        public TaxScheduleInsertionActionResult (TaxScheduleValidationResult validityStatus, TaxScheduleInsertionResult actionResult)
        {
            Validity = validityStatus;
            ActionResult = actionResult;
        }

        public new string ToString ()
        {
            return $"Validity: {Validity}, {nameof(TaxScheduleInsertionResult)}: {ActionResult}";
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
