using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using DummyDB.Core.Models;
using DummyDB.Core.Services;
using DummyDB.Desktop.Views.Pages;

namespace DummyDB.Desktop.ViewModels;

public class TablePageViewModel
{
    private const string DatabasesFolderPath = "../../../../DummyDB.Core/Databases";
    private readonly TablePage _page;
    private readonly Table _table;

    public TablePageViewModel(TablePage page, string tableName, string dbName)
    {
        _page = page;
        _table = Init(tableName, dbName);
        InitRows();
        InitColumns();
    }

    private static Table Init(string tableName, string dbName)
    {
        var dataPath = $"{DatabasesFolderPath}//{dbName}//{tableName}//{tableName}.csv";
        var schemaPath = $"{DatabasesFolderPath}//{dbName}//{tableName}//{tableName}.json";
        
        var data = CsvReadingService.ReadFromCsv(dataPath, schemaPath);
        var schema = Schema.GetFromJsonFile($"{DatabasesFolderPath}//{dbName}//{tableName}//{tableName}.json");
        
        return TableCreatingService.CreateTable(tableName, schema!, data!);
    }

    private void InitColumns()
    {
        for (var i = 0; i < _table.Schema.Columns.Count; i++)
        {
            var tableTextColumn = new DataGridTextColumn
            {
                Header = _table.Schema.Columns[i].Name,
                Binding = new System.Windows.Data.Binding($"Elements[{i}]")
            };

            _page.Table.Columns.Add(tableTextColumn);
        }
    }

    private void InitRows()
    {
        var data = new ObservableCollection<RowViewModel>();
        foreach (var row in _table.Rows)
        {
            data.Add(new RowViewModel { Elements = row.Elements.Values.ToList() });
        }

        _page.Table.ItemsSource = data;
    }
}