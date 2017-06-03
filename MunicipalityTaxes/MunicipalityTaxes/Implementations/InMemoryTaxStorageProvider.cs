using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityTaxes
{
    class InMemoryTaxStorageProvider : ITaxStorage
    {
        private List<MunicipalityTaxDetails> database;

        public InMemoryTaxStorageProvider ()
        {
            database = new List<MunicipalityTaxDetails>();
        }

        public void InsertTaxSchedule (MunicipalityTaxDetails tax)
        {
            // double check that the tax doesn't already exist in the database
            // (on a SQL Server DB with primary key / unique index contraints etc. it'd throw an error for us)
            if (TaxScheduleExists(tax.MunicipalitySchedule))
                throw new InvalidOperationException($"Tax schedule '{tax.MunicipalitySchedule.DebuggerDisplay}' already exists, unable to insert it again."); // hmm, Invalid Operation or Invalid Argument? let's go with operation, because if it didn't already exist, it'd be a valid argument
            database.Add(tax);
        }

        public void DeleteTaxSchedule (MunicipalityTaxSchedule tax)
        {
            var existing = FindTaxSchedule(tax);
            if (existing == null)
                throw new InvalidOperationException($"Tax schedule '{tax.DebuggerDisplay}' not found, unable to delete it.");
            database.Remove(existing);
        }

        public MunicipalityTaxDetails GetTax (string municipality, DateTime at)
        {
            var results = database.Where(tax => tax.MunicipalitySchedule.Municipality == municipality);
            return FindTaxSchedule(MunicipalityTaxSchedule.MostApplicable(results.Select(r => r.MunicipalitySchedule), at)); // not the most efficient way to do it ever, but more generic
        }

        internal MunicipalityTaxDetails FindTaxSchedule (MunicipalityTaxSchedule tax)
        {
            return database.FirstOrDefault(t => t.MunicipalitySchedule.Equals(tax));
        }

        public bool TaxScheduleExists (MunicipalityTaxSchedule tax)
        {
            return FindTaxSchedule(tax) != null;
        }

        public void UpdateTaxSchedule (MunicipalityTaxDetails tax)
        {
            var existing = FindTaxSchedule(tax.MunicipalitySchedule);
            if (existing == null)
                throw new InvalidOperationException($"Tax schedule '{tax.MunicipalitySchedule.DebuggerDisplay}' not found, unable to update it.");
            // we know that the only thing that differs is the TaxAmount
            // so there is no need to removing the existing record from the DB and add the new one, or update all fields
            existing.TaxAmount = tax.TaxAmount;
        }
    }
}
