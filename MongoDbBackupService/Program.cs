using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbBackupService
{
    static class Program
    {
        internal static bool EnableDebugger;
        internal static double DifferentialBackupInterval;
        internal static double FullBackupInterval;
        static void Main()
        {
            EnableDebugger = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableDebugger"]);
            DifferentialBackupInterval = Convert.ToDouble(ConfigurationManager.AppSettings["DifferentialBackupInterval"])* 10000;// 60000;
            FullBackupInterval= Convert.ToDouble(ConfigurationManager.AppSettings["FullBackupInterval"]) * 600000;
            var ServicesToRun = new ServiceBase[]
            {
                new MongoDbBackup()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
