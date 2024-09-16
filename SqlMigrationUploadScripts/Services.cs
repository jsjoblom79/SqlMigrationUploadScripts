using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlMigrationUploadScripts
{
    public class Services
    {
        public static string GetPassword()
        {
            var pwd = "";

            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.Remove(pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else if (i.KeyChar != '\u0000')
                {
                    pwd += i.KeyChar;
                    Console.Write("*");
                }
            }
            return pwd;
        }

        public static void CreateSqlTables(string host, string userId, string password, string scriptPath)
        {
            
            var fileList = Directory.GetFiles(scriptPath);
            var connectionString = "";
            if(string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(password))
            {
                connectionString = $"Server={host};Database=master;Trust Server Certificate=True";
            }
            else
            {
                connectionString = $"Server={host};Database=master;User ID={userId};Password={password};Trust Server Certificate=True";
            }
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                ServerConnection serverConnection = new ServerConnection(connection);
                Server server = new Server(serverConnection);
                if(server.State != SqlSmoState.Existing)
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
                    

                foreach (var file in fileList)
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($"Executing Script: {Path.GetFileName(file)} on {host}");
                    try
                    {
                        server.ConnectionContext.ExecuteNonQuery(File.ReadAllText(file));
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.InnerException);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Script: {Path.GetFileName(file)} has been executed on {host} ");
                }
            }

        }
    }
}
