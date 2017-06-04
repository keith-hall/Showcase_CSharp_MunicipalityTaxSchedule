using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace MunicipalityTaxesWindowsService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller ()
        {
            InitializeComponent();
        }

        private void serviceInstaller1_BeforeInstall (object sender, InstallEventArgs e)
        {
            // https://msdn.microsoft.com/en-us/library/zt39148a(v=vs.110).aspx#BK_StartupParameters
            string parameter = "\"MunicipalityTaxes.MunicipalityTaxesService\"";
            Context.Parameters["assemblypath"] = "\"" + Context.Parameters["assemblypath"] + "\" " + parameter;
        }
    }
}
