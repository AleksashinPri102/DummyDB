using DummyDB.Core.Models;

namespace DummyDB.Core.Services;

public static class CsvReadingService
{
    public static string[]? ReadFromCsv(string dataFilePath, string schemaFilePath)
    {
        var dataFromCsv = File.ReadAllLines(dataFilePath);
        var schema = Schema.GetFromJsonFile(schemaFilePath);

        try
        {
            return JsonValidationService.CheckBySchema(schema!, dataFromCsv) ? dataFromCsv[1..] : throw new FormatException("Невозможно считать данные из файла, так как они не соответствуют схеме таблицы.");
        }
        catch (Exception ex)
        {
            WriteExceptionMessage(ex);
        }

        return null;
    }

    private static void WriteExceptionMessage(Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Clear();
        Console.WriteLine($"Ошибка: {ex.Message}");
        Console.ResetColor();
    }
}