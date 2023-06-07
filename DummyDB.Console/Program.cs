using DummyDB.Core.Models;
using DummyDB.Core.Services;

namespace DummyDB.Console;

public class Program
{
    private const string DatabasesFolderPath = "../../../../DummyDB.Core/Databases";
    
    public static void Main()
    {
        System.Console.OutputEncoding = System.Text.Encoding.UTF8;
        var dbName = GetDbName();
        var tableName = GetTableName();

        try
        {
            var data = GetData(tableName!, dbName!);
            var schema = Schema.GetFromJsonFile($"{DatabasesFolderPath}//{dbName}//{tableName}//{tableName}.json");
            var table = TableCreatingService.CreateTable(tableName!, schema!, data);
            
            TableDrawingService.DrawTable(table);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private static string[] GetData(string fileName, string dbName)
    {
        var dataPath = $"{DatabasesFolderPath}//{dbName}//{fileName}//{fileName}.csv";
        var schemaPath = $"{DatabasesFolderPath}//{dbName}//{fileName}//{fileName}.json";
        return CsvReadingService.ReadFromCsv(dataPath, schemaPath)!;
    }

    private static void ShowError(Exception ex)
    {
        System.Console.Clear();
        System.Console.WriteLine(ex.Message);
        System.Console.ResetColor();
        System.Console.ReadKey();
    }

    private static string? GetDbName()
    {
        System.Console.Write("Введите название БД: ");
        return System.Console.ReadLine();
    }

    private static string? GetTableName()
    {
        System.Console.Write("Введите название таблицы: ");
        return System.Console.ReadLine();
    }
}