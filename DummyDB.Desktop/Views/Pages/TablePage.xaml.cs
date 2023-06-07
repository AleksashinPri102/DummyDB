using DummyDB.Desktop.ViewModels;

namespace DummyDB.Desktop.Views.Pages;

public partial class TablePage
{
    public TablePage(string tableName, string dbName)
    {
        InitializeComponent();
        DataContext = new TablePageViewModel(this, tableName, dbName);
    }
}