using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace MongoDbBackupService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            var process = new ServiceProcessInstaller { Account = ServiceAccount.LocalSystem };
            var service = new ServiceInstaller { ServiceName = "MongoDbBackup" };
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
