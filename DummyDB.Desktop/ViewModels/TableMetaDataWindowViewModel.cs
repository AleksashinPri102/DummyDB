using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using DummyDB.Core.Models;
using DummyDB.Desktop.Views;

namespace DummyDB.Desktop.ViewModels;

public class TableMetaDataWindowViewModel
{
    private const string DatabasesFolderPath = "../../../../DummyDb.Core/Databases";
    private readonly Schema _schema;
    
    public List<TreeViewItem> ColumnsData { get; init; }

    public TableMetaDataWindowViewModel(TableMetaDataWindow window, string schemaName, string dbName)
    {
        window.TableName.Text = schemaName;
        _schema = Schema.GetFromJsonFile($"{DatabasesFolderPath}//{dbName}//{schemaName}//{schemaName}.json")!;
        ColumnsData = InitMetaDataView();
    }

    private List<TreeViewItem> InitMetaDataView()
    {
        return _schema.Columns
            .Select(column => new TreeViewItem { Header = $"name: {column.Name}, type: {column.Type}, isPrimary: {column.IsPrimary}" })
            .ToList();
    }
}