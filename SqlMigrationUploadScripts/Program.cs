// See https://aka.ms/new-console-template for more information
using SqlMigrationUploadScripts;
using System.Configuration;

Console.ForegroundColor = ConsoleColor.Green;
Console.Write("Please Enter the Server instance: ");
var host = Console.ReadLine();
Console.Write("Do you need to provide Sql Credentials? [Y] or [N] ");
var creds = Console.ReadLine();
var userId = "";
var pwd = "";
if (creds.ToLower().Equals("y"))
{
    Console.Write("Please Enter the user id: ");
    userId = Console.ReadLine();
    Console.Write("Enter Password: ");
    pwd = Services.GetPassword();
}

Console.Write("Path for Sql Scripts: ");
var scriptPath = Console.ReadLine();

Console.WriteLine();
Services.CreateSqlTables(host, userId, pwd, scriptPath);
Console.ForegroundColor = ConsoleColor.White;

