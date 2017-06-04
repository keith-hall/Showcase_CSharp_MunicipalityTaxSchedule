using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using MunicipalityTaxes;

namespace MunicipalityTaxesWindowsService
{
    public partial class MunicipalityTaxesWindowsService : ServiceBase
    {
        internal ServiceHost WcfService;

        public MunicipalityTaxesWindowsService (string[] args)
        {
            InitializeComponent();
            var useType = typeof(MunicipalityTaxesService);
            if (args != null && args.Any())
            {
                // use dependency injection / IoC for any type implementing IMunicipalityTaxesService
                var specifiedType = Type.GetType(args[0]);
                if (specifiedType != null && specifiedType.GetInterfaces().Contains(typeof(IMunicipalityTaxesService)))
                    useType = specifiedType;
                // TODO: else complain
            }
            WcfService = new ServiceHost(useType);
        }

        protected override void OnStart (string[] args)
        {
            WcfService.Open();
        }

        protected override void OnStop ()
        {
            WcfService.Close();
        }
    }
}
