using DummyDB.Core.Models;

namespace DummyDB.Core.Services;

public static class JsonValidationService
{
    public static bool CheckBySchema(Schema schema, string[] data)
    {
        var areColumnsNamesOk = CheckColumnsNames(schema, data[0].Split(';'));
        for (var i = 1; i < data.Length; i++)
        {
            var flag = CheckColumnsTypes(schema, data[i].Split(';'));
            if (!flag)
            {
                return false;
            }
        }

        return areColumnsNamesOk;
    }

    private static bool CheckColumnsNames(Schema schema, string[] names)
    {
        if (names.Where((columnName, i) => columnName != schema.Columns[i].Name).Any())
        {
            throw new FormatException("Названия столбцов в таблице не совпадают со схемой.");
        }

        return true;
    }

    private static bool CheckColumnsTypes(Schema schema, string[] row)
    {
        var result = false;
        for (var i = 0; i < row.Length; i++)
        {
            result = schema.Columns[i].Type switch
            {
                "int" => int.TryParse(row[i], out _),
                "bool" => bool.TryParse(row[i], out _),
                "float" => float.TryParse(row[i], out _),
                "dateTime" => DateTime.TryParse(row[i], out _),
                "string" => true,
                _ => throw new ArgumentException($"В схеме неверно указан тип данных столбца под номером {i + 1}.")
            };
        }

        return result;
    }
}