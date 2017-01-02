/**********************************************************************************************************************/
/*Class Name    :   MongoDbBackup.cs                                                                                  */
/*Created By    :   Anas Elaiwat                                                                                      */
/*Creation Date :   07/09/2016                                                                                        */
/*Comment       :   This is a windows service that used to backup mongoDB Daily and weekly and it use mongodump.exe   */
/**********************************************************************************************************************/

using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.ServiceProcess;
using System.Timers;

namespace MongoDbBackupService
{
    public partial class MongoDbBackup : ServiceBase
    {
        #region Members
        Timer differentialBackupTimer = new Timer();
        Timer fullBackupTimer = new Timer();
        #endregion

        #region MongoDbBackup
        public MongoDbBackup()
        {
            InitializeComponent();
        }
        #endregion

        #region OnStart
        protected override void OnStart(string[] args)
        {
            if (Program.EnableDebugger) { Debugger.Launch(); }

            #region differentialBackupTimer
            differentialBackupTimer = new Timer { AutoReset = true, Enabled = true, Interval = Program.DifferentialBackupInterval };
            differentialBackupTimer.Elapsed += DifferentialBackupTimerElapsed;
            differentialBackupTimer.Start();
            #endregion

            #region WeeklyTimer
            fullBackupTimer = new Timer { AutoReset = true, Enabled = true, Interval = Program.FullBackupInterval };
            fullBackupTimer.Elapsed += FullBackupTimerElapsed;
            fullBackupTimer.Start();
            #endregion
        }
        #endregion

        #region OnStop
        protected override void OnStop()
        {
            this.differentialBackupTimer.Dispose();
            this.fullBackupTimer.Dispose();
        }
        #endregion

        #region DifferentialBackupTimerElapsed
        private void DifferentialBackupTimerElapsed(object sender, ElapsedEventArgs e)
        {
            StartBackupProcessForAllCollections(Program.DifferentialBackupInterval);
        }
        #endregion

        #region FullBackupTimerElapsed
        private void FullBackupTimerElapsed(object sender, ElapsedEventArgs e)
        {
            StartBackupProcessForAllCollections(Program.FullBackupInterval);
        }
        #endregion

        #region StartBackupProcessForAllCollections
        private void StartBackupProcessForAllCollections(double daysInterval)
        {
            string binDirection = ConfigurationManager.AppSettings["BinDirection"];
            string databaseName = ConfigurationManager.AppSettings["DatabaseName"];
            string outputDirectory = ConfigurationManager.AppSettings["OutputDirectory"] + DateTime.Now.ToString("yyyy-MM-dd(HH.mm.ss.fff)", CultureInfo.InvariantCulture);
            string query = @"""{'Timestamp' : {'$lt' : ISODate('" + DateTime.UtcNow.ToString("s") + "Z" + "'),'$gte' : ISODate('" + DateTime.UtcNow.AddDays(-daysInterval).ToString("s") + "Z" + @"') }}""";
            string mongoDbCollections = ConfigurationManager.AppSettings["MongoDbCollections"];

            foreach (var collectionName in mongoDbCollections.Split(','))
            {
                StartBackupProcess(binDirection, databaseName, collectionName, outputDirectory, query);
            }
        }
        #endregion

        #region StartBackupProcess
        private void StartBackupProcess(string binDirection, string databaseName, string collectionName, string outputDirectory, string query)
        {
            string arguments = " -d " + databaseName +
                               " -c " + collectionName +
                               " -o " + outputDirectory +
                               " --query " + query;
            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = binDirection + @"\mongodump.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = arguments
            };
            Process process = Process.Start(start);
            process.WaitForExit();
        }
        #endregion
    }
}
