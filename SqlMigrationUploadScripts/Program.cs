// See https://aka.ms/new-console-template for more information
using SqlMigrationUploadScripts;
using Serilog;
using Serilog.Sinks.File;

Console.ForegroundColor = ConsoleColor.Green;
Console.Write("Please Enter the Server instance: ");
string host = Console.ReadLine();
Console.Write("Do you need to provide Sql Credentials? [Y] or [N] ");
string creds = Console.ReadLine();
string userId = "";
string pwd = "";
if (creds.ToLower().Equals("y"))
{
    Console.Write("Please Enter the user id: ");
    userId = Console.ReadLine();
    Console.Write("Enter Password: ");
    pwd = Services.GetPassword();
    Console.WriteLine();
}

Console.Write("Path for Sql Scripts: ");
var scriptPath = Console.ReadLine();

Log.Logger = new LoggerConfiguration()
    .WriteTo.File($"{scriptPath}\\Logs\\SqlMigrationScript_.logs", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Warning()
    .CreateLogger();
SqlServerFunctions.ProcessServerScripts(host, userId, pwd, scriptPath);
Console.ForegroundColor = ConsoleColor.White;

