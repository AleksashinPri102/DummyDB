using System.IO;
using System.Windows;
using System.Windows.Input;
using DummyDB.Desktop.Views.Forms;

namespace DummyDB.Desktop.ViewModels;

public class DatabaseCreatingViewModel
{
    private const string DatabasesFolderPath = "../../../../DummyDb.Core/Databases";
    private readonly DatabaseCreatingForm _form;
    private readonly MainViewModel _mainViewModel;
    public ICommand CreateDatabaseCommand { get; init; }

    public DatabaseCreatingViewModel(DatabaseCreatingForm form, MainViewModel mainViewModel)
    {
        _form = form;
        _mainViewModel = mainViewModel;
        CreateDatabaseCommand = new RelayCommand(CreateDatabase);
    }

    private void CreateDatabase(object? o)
    {
        var title = _form.DatabaseName.Text;
        if (Directory.Exists($"{DatabasesFolderPath}/{title}"))
        {
            MessageBox.Show("Такая БД уже существует. Пожалуйста, измените название.", "Error", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            Directory.CreateDirectory($"{DatabasesFolderPath}/{title}");
            _mainViewModel.DatabasesNames.Add(title);
            _form.Close();
        }
    }
}