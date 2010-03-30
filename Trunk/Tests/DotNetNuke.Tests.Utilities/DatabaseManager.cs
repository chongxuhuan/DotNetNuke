using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Specialized;
using System.Threading;
using Gallio.Framework;

namespace DotNetNuke.Tests.Utilities
{
    public static class DatabaseManager
    {
        public static void DropDatabase(string databaseName)
        {
            // Connect to the SQL Server
            // TODO: Allow the test runner to change the SQL Server!
            Server server = new Server("(local)");

            // Drop the database
            Database db = server.Databases[databaseName];
            if (db != null)
            {
                server.KillDatabase(databaseName);
                while (server.Databases[databaseName] != null)
                {
                    Thread.Sleep(100);
                }
            }
        }

        public static void CopyAndAttachDatabase(TestContext context, string databaseName)
        {
            string destinationPath = CopyDatabase(context, databaseName);

            // Attach the copied database to SQL Server
            AttachDatabase(databaseName, destinationPath);
        }

        public static string GetTestDatabasePath(string databaseName)
        {
            return Path.GetFullPath(String.Format(@"Databases\{0}.mdf", databaseName));
        }

        public static string EnsureDatabaseExists(string databaseName)
        {
            string targetDatabasePath = GetTestDatabasePath(databaseName);
            if (!File.Exists(targetDatabasePath))
            {
                throw new InvalidOperationException(String.Format("Could not find test database {0}. Searched: {1}", databaseName, targetDatabasePath));
            }
            return targetDatabasePath;
        }
        
        private static void AttachDatabase(string databaseName, string databaseFile)
        {
            // Connect to the SQL Server
            // TODO: Allow the test runner to change the SQL Server!
            Server server = new Server("(local)");

            // Attach the database
            server.AttachDatabase(databaseName, new StringCollection()
            {
                databaseFile
            });
            while (server.Databases[databaseName].State != SqlSmoState.Existing)
            {
                Thread.Sleep(100);
            }
        }

        private static string CopyDatabase(TestContext context, string databaseName)
        {
            // Find the target database file
            string targetDatabasePath = EnsureDatabaseExists(databaseName);

            // Create the test database directory if it does not exist
            string testDatabaseDirectory = Path.GetFullPath(String.Format(@"Databases\TestDatabases\{0}", context.Test.FullName));
            if (!Directory.Exists(testDatabaseDirectory))
            {
                Directory.CreateDirectory(testDatabaseDirectory);
            }

            // Copy the database to the test database directory
            string destinationRoot = Path.Combine(testDatabaseDirectory, databaseName);
            string databasePath = String.Concat(destinationRoot, ".mdf");
            File.Copy(targetDatabasePath, databasePath);
            return databasePath;
        }
    }
}
