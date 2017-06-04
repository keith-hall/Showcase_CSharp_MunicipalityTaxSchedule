using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ExampleClient.MunicipalityTaxesServiceReference;

namespace ExampleClient
{
    class Program
    {
        static void Main (string[] args)
        {
            var cf = new ChannelFactory<IMunicipalityTaxesService>("BasicHttpBinding_IMunicipalityTaxesService");
            var c = cf.CreateChannel();

            var response = c.InsertTaxScheduleDetails(new MunicipalityTaxDetails { MunicipalitySchedule = new MunicipalityTaxSchedule("Vilnius", ScheduleFrequency.Yearly, new DateTime(2016, 01, 01)), TaxAmount = 0.2 });
            response = c.InsertTaxScheduleDetails(new MunicipalityTaxDetails { MunicipalitySchedule = new MunicipalityTaxSchedule("Vilnius", ScheduleFrequency.Monthly, new DateTime(2016, 05, 01)), TaxAmount = 0.4 });
            response = c.InsertTaxScheduleDetails(new MunicipalityTaxDetails { MunicipalitySchedule = new MunicipalityTaxSchedule("Vilnius", ScheduleFrequency.Daily, new DateTime(2016, 01, 01)), TaxAmount = 0.1 });
            response = c.InsertTaxScheduleDetails(new MunicipalityTaxDetails { MunicipalitySchedule = new MunicipalityTaxSchedule("Vilnius", ScheduleFrequency.Daily, new DateTime(2016, 12, 25)), TaxAmount = 0.1 });

            Console.Write("The tax applicable on 2016.01.01 is: ");
            Console.WriteLine(c.GetTax("Vilnius", new DateTime(2016, 1, 1)));
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
