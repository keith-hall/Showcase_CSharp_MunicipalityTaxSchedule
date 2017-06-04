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
        public MunicipalityTaxSchedule (string municipality, ScheduleFrequency frequency, DateTime begin)
        {
            this.Municipality = municipality;
            this.ScheduleType = frequency;
            this.ScheduleBeginDate = begin;
        }

        [DataMember]
        public readonly string Municipality;
        [DataMember]
        public readonly ScheduleFrequency ScheduleType;
        [DataMember]
        public readonly DateTime ScheduleBeginDate;

        internal string DebuggerDisplay { get { return $"{GetType().Name}: {Municipality}, {ScheduleType}, {ScheduleBeginDate:yyyy.MM.dd}"; } }

        internal bool IsApplicable (DateTime date)
        {
            if (date < this.ScheduleBeginDate)
                return false;

            DateTime? endDate = null;
            switch (this.ScheduleType)
            {
                case ScheduleFrequency.Daily:
                    endDate = this.ScheduleBeginDate.AddDays(1);
                    break;
                case ScheduleFrequency.Weekly:
                    endDate = this.ScheduleBeginDate.AddDays(7);
                    break;
                case ScheduleFrequency.Monthly:
                    endDate = this.ScheduleBeginDate.AddMonths(1);
                    break;
                case ScheduleFrequency.Yearly:
                    endDate = this.ScheduleBeginDate.AddYears(1);
                    break;
            }
            return endDate.HasValue ? date >= this.ScheduleBeginDate && date < endDate : false;
        }

        internal static MunicipalityTaxSchedule MostApplicable(IEnumerable<MunicipalityTaxSchedule> schedules, DateTime at)
        {
            var results = schedules.Where(s => s.IsApplicable(at)).GroupBy(s => s.ScheduleType).OrderByDescending(g => g.Key); // the enum is ordered so that Daily comes after Yearly etc. so Daily overrides all other schedule frequencies
            return results.FirstOrDefault()?.Take(2).SingleOrDefault(); // some enumerable providers don't optimize Single properly
        }

        public override bool Equals (object obj)
        {
            var compare = obj as MunicipalityTaxSchedule;
            if (compare == null)
                return base.Equals(obj);
            return this.Municipality == compare.Municipality && this.ScheduleType == compare.ScheduleType && this.ScheduleBeginDate == compare.ScheduleBeginDate;
        }

        public override int GetHashCode ()
        {
            // the fields used to compute the HashCode must be immutable, otherwise the hashcode could change when inside a HashSet etc. and cause problems
            // https://stackoverflow.com/a/4630550/4473405
            return new { Municipality, ScheduleType, ScheduleBeginDate }.GetHashCode();
        }
    }
}
