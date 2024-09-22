using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlMigrationUploadScripts
{
    public class SqlServerFunctions
    {

        public static void ProcessServerScripts(string host, string userId, string password, string scriptPath)
        {
            string connectionString = string.Empty;

            if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(password))
            {
                connectionString = $"Server={host};Database=master;Integrated Security=True;Trust Server Certificate=True";
            }
            else
            {
                connectionString = $"Server={host};Database=master;User ID={userId};Password={password};Trust Server Certificate=True";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                ServerConnection serverConnection = new ServerConnection(connection);
                Server server = new Server(serverConnection);
                if (server.State != SqlSmoState.Existing)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Connection to server: ");
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nConnection to the server: {host} is {server.State}");
                }

                List<string> databaseFiles = Directory.GetFiles($"{scriptPath}\\{DirectoryFolders.DatabaseScripts}").ToList();
                List<string> tableFiles = Directory.GetFiles($"{scriptPath}\\{DirectoryFolders.TableScripts}").ToList();
                List<string> viewFiles = Directory.GetFiles($"{scriptPath}\\{DirectoryFolders.ViewScripts}").ToList();
                List<string> functionFiles = Directory.GetFiles($"{scriptPath}\\{DirectoryFolders.FunctionScripts}").ToList();
                List<string> procedureFiles = Directory.GetFiles($"{scriptPath}\\{DirectoryFolders.StoredProcedureScripts}").ToList();


                foreach (var file in databaseFiles)
                {
                    //Create Database First
                    RunSqlScript(server, file);

                    var filename = Path.GetFileName(file).Split("~");
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    //run Table file scripts
                    var tableName = filename[0];
                    var tables = tableFiles.Where(d => d.Contains(tableName)).ToList();
                    RunSqlScript(server, tables);
                    //run View File Scripts
                    RunSqlScript(server, viewFiles.Where(d => d.Contains(tableName)).ToList());
                    //run Function file Scripts
                    RunSqlScript(server, functionFiles.Where(d => d.Contains(tableName)).ToList());
                    //run Procedure File Scripts
                    RunSqlScript(server, procedureFiles.Where(d => d.Contains(tableName)).ToList());
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"All Scripts for {tableName} have been executed on {server.Name} ");
                }
            }
        }
        public static void RunSqlScript(Server server, List<string> fileNames)
        {
            foreach(var file in fileNames)
            {
                try
                {
                    if (server.State == SqlSmoState.Existing)
                    {
                        server.ConnectionContext.ExecuteNonQuery(File.ReadAllText($"{file}"));
                        Console.WriteLine($"Script: {Path.GetFileName(file)} has been executed on {server.Name}");
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Log.Error(ex.InnerException, ex.Message);
                    Console.WriteLine(ex.InnerException);
                    Console.ForegroundColor = ConsoleColor.Green;
                }
            }
        }
        public static void RunSqlScript(Server server, string database)
        {
            try
            {
                server.ConnectionContext.ExecuteNonQuery(File.ReadAllText($"{database}"));
                Console.WriteLine($"Script: {Path.GetFileName(database)} has been executed on {server.Name}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Log.Error(ex.InnerException, ex.Message);
                Console.WriteLine(ex.InnerException);
                Console.ForegroundColor= ConsoleColor.Green;
            }
        }
    }
}
