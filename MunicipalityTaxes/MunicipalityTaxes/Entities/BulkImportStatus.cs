using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityTaxes
{
	[DataContract]
	public class BulkImportResponse
	{
		[DataMember]
		public BulkImportStatus Status;
		[DataMember]
		public IEnumerable<KeyValuePair<MunicipalityTaxDetails, TaxScheduleActionResult<TaxScheduleInsertionResult>>> lineItems;
	}

	[DataContract]
	public enum BulkImportStatus
	{
		[EnumMember]
		ParseError = 0,
		[EnumMember]
		UnknownFailure,
		[EnumMember]
		Success,
	}
}
