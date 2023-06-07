using System.Text;
using DummyDB.Core.Models;

namespace DummyDB.Console;

public static class TableDrawingService
{
    private static Table? _table;
    private static int[]? _columnsLengths;
    
    public static void DrawTable(Table table)
    {
        _table = table;
        _columnsLengths = GetMaxLengths();
        System.Console.Clear();
        System.Console.WriteLine(Header());
        for (var i = 0; i < _table.Rows.Count; i++)
        {
            System.Console.WriteLine(Border());
            System.Console.WriteLine(Row(i));
        }

        System.Console.WriteLine(Border());
    }

    private static string Header()
    {
        var sb = new StringBuilder("| ");
        for (var i = 0; i < _table!.Schema.Columns.Count; i++)
        {
            var name = _table.Schema.Columns[i].Name;
            sb.Append(name);
            for (var j = 0; j < _columnsLengths![i] - name!.Length + 1; j++)
            {
                sb.Append(' ');
            }

            sb.Append(" | ");
        }

        return sb.ToString();
    }

    private static string Border()
    {
        var result = new StringBuilder("|");
        for (var i = 0; i < _table!.Schema.Columns.Count; i++)
        {
            for (var j = 0; j < _columnsLengths![i] + 3; j++)
            {
                result.Append('-');
            }

            result.Append('|');
        }

        return result.ToString();
    }

    private static string Row(int i)
    {
        var result = new StringBuilder("| ");
        for (var j = 0; j < _table!.Schema.Columns.Count; j++)
        {
            var column = _table.Schema.Columns[j];
            var element = _table.Rows[i].Elements[column];
            result.Append(element);
            result.Append(new string(' ', _columnsLengths![j] - element.ToString()!.Length + 1));

            result.Append(" | ");
        }

        return result.ToString();
    }

    private static int[] GetMaxLengths()
    {
        var result = new int[_table!.Schema.Columns.Count];
        for (var i = 0; i < result.Length; i++)
        {
            var temp = _table.Schema.Columns[i];
            var maxLength = _table.Rows[0].Elements[temp].ToString()!.Length;

            for (var j = 1; j < _table.Rows.Count; j++)
            {
                var element = _table.Rows[j].Elements[temp].ToString()!.Length;
                if (element > maxLength)
                {
                    maxLength = element;
                }
            }
            if (maxLength < _table.Schema.Columns[i].Name!.Length)
            {
                maxLength = _table.Schema.Columns[i].Name!.Length;
            }
            result[i] = maxLength;
        }

        return result;
    }
}