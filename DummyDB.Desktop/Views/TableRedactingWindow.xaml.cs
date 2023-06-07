using DummyDB.Desktop.ViewModels;

namespace DummyDB.Desktop.Views;

public partial class TableRedactingWindow
{
    public TableRedactingWindow(string tableName, string dbName)
    {
        InitializeComponent();
        DataContext = new TableRedactingWindowViewModel(this, tableName, dbName);
    }
}