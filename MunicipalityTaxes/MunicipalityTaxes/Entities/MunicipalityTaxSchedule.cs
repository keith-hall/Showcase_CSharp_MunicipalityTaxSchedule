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
        public MunicipalityTaxSchedule ()
        {
            
        }

        public MunicipalityTaxSchedule (string municipality, ScheduleFrequency frequency, DateTime begin)
        {
            this.Municipality = municipality;
            this.ScheduleType = frequency;
            this.ScheduleBeginDate = begin;
        }

        [DataMember]
        public string Municipality;
        [DataMember]
        public ScheduleFrequency ScheduleType;
        [DataMember]
        public DateTime ScheduleBeginDate;

        internal string DebuggerDisplay { get { return $"{GetType().Name}: {Municipality}, {ScheduleType}, {ScheduleBeginDate:yyyy.MM.dd}"; } }

        internal bool IsApplicable (DateTime date)
        {
            if (date < this.ScheduleBeginDate)
                return false;

            switch (this.ScheduleType)
            {
                case ScheduleFrequency.Daily:
                    return date >= this.ScheduleBeginDate && date < this.ScheduleBeginDate.AddDays(1);
                case ScheduleFrequency.Weekly:
                    return date >= this.ScheduleBeginDate && date < this.ScheduleBeginDate.AddDays(7);
                case ScheduleFrequency.Monthly:
                    return date >= this.ScheduleBeginDate && date < this.ScheduleBeginDate.AddMonths(1);
                case ScheduleFrequency.Yearly:
                    return date >= this.ScheduleBeginDate && date < this.ScheduleBeginDate.AddYears(1);
            }
            return false;
        }

        internal static MunicipalityTaxSchedule MostApplicable(IEnumerable<MunicipalityTaxSchedule> schedules, DateTime at)
        {
            var results = schedules.Where(s => s.IsApplicable(at)).GroupBy(s => s.ScheduleType).OrderByDescending(g => g.Key); // the enum is ordered so that Daily comes after Yearly etc. so Daily overrides all other schedule frequencies
            return results.First().Take(2).SingleOrDefault(); // some enumerable providers don't optimize Single properly
        }
    }
}
