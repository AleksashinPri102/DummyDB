using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using DummyDB.Desktop.Views;

namespace DummyDB.Desktop.ViewModels;

public class DbRedactingWindowViewModel
{
    private const string DatabasesFolderPath = "../../../../DummyDb.Core/Databases";
    private readonly DbRedactingWindow _window;
    private string _dbName;
    public ICommand RenameDbCommand { get; init; }
    public ICommand SaveCommand { get; init; }

    public DbRedactingWindowViewModel(DbRedactingWindow window, string dbName)
    {
        _window = window;
        _dbName = dbName;
        window.DatabaseName.Text = dbName;
        InitTables();
        RenameDbCommand = new RelayCommand(RenameDb);
        SaveCommand = new RelayCommand(Save);
    }

    private void InitTables()
    {
        var database = new DirectoryInfo($"{DatabasesFolderPath}//{_dbName}");
        var tables = database.GetDirectories();
        foreach (var table in tables)
        {
            var tablePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            tablePanel.Children.Add(new TextBox { Text = table.Name });
            tablePanel.Children.Add(new Button { Content = "Переименовать", Command = new RelayCommand(_ =>
            {
                Directory.Move(
                    $"{DatabasesFolderPath}/{_dbName}/{table.Name}",
                    $"{DatabasesFolderPath}/{_dbName}/{(tablePanel.Children[0] as TextBox)!.Text}");
            })});
            tablePanel.Children.Add(new Button { Content = "Удалить", Command = new RelayCommand(_ =>
            {
                DeleteDirectory(table, $"{DatabasesFolderPath}/{_dbName}/{table.Name}");
                _window.TablesNames.Children.Remove(tablePanel);
            })});
            _window.TablesNames.Children.Add(tablePanel);
        }
    }

    private static void DeleteDirectory(DirectoryInfo directory, string path)
    {
        foreach (var file in directory.GetFiles())
        {
            file.Delete();
        }
        Directory.Delete(path);
    }

    private void RenameDb(object? o)
    {
        Directory.Move($"{DatabasesFolderPath}/{_dbName}",
            $"{DatabasesFolderPath}/{_window.DatabaseName.Text}");
        _dbName = _window.DatabaseName.Text;
    }

    private void Save(object? o)
    {
        _window.Close();
    }
}