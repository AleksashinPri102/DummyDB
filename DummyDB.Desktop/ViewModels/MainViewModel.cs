using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using DummyDB.Desktop.Views;
using DummyDB.Desktop.Views.Forms;
using DummyDB.Desktop.Views.Pages;

namespace DummyDB.Desktop.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private const string DatabasesFolderPath = "../../../../DummyDB.Core/Databases";
    private readonly MainWindow _window;
    private string _selectedDatabase = null!;

    public string SelectedDatabase
    {
        get => _selectedDatabase;
        set
        {
            if (_selectedDatabase == value) return;
            _selectedDatabase = value;
            _window.TableChoosingBox.ItemsSource = new ObservableCollection<string>(InitTables());
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedDatabase"));
            }
        }
    }
    public ObservableCollection<string> DatabasesNames { get; set; }
    public ICommand ShowTablePageCommand { get; init; }
    public ICommand ShowMetaDataCommand { get; init; }
    public ICommand ShowDbsMetaDataCommand { get; init; }
    public ICommand CreateDatabaseCommand { get; init; }
    public ICommand CreateTableCommand { get; init; }
    public ICommand DbRedactingCommand { get; init; }
    public ICommand RefreshCommand { get; init; }
    public ICommand OpenChangingMenuCommand { get; init; }

    public MainViewModel(MainWindow window)
    {
        _window = window;
        DatabasesNames = new ObservableCollection<string>(InitDatabases());
        ShowTablePageCommand = new RelayCommand(ShowTablePage);
        ShowMetaDataCommand = new RelayCommand(ShowMetaData);
        ShowDbsMetaDataCommand = new RelayCommand(ShowDbsMetaData);
        CreateDatabaseCommand = new RelayCommand(CreateDatabase);
        CreateTableCommand = new RelayCommand(CreateTable);
        DbRedactingCommand = new RelayCommand(RedactDb);
        RefreshCommand = new RelayCommand(Refresh);
        OpenChangingMenuCommand = new RelayCommand(OpenChangingMenu);
    }

    private IEnumerable<string> InitDatabases()
    {
        var databasesFolder = new DirectoryInfo(DatabasesFolderPath);
        var databases = databasesFolder.GetDirectories();
        return databases
            .Select(d => d.Name);
    }

    private IEnumerable<string> InitTables()
    {
        var database = new DirectoryInfo($"{DatabasesFolderPath}//{_selectedDatabase}");
        var tables = database.GetDirectories();
        return tables
            .Where(table => table.GetFiles().Length == 2)
            .Select(table => table.Name);
    }
    
    private void ShowTablePage(object? o)
    {
        if (_window.TableChoosingBox.SelectedItem is string tableName)
        {
            _window.MainFrame.Content = new TablePage(tableName, _selectedDatabase);
        }
        else
        {
            MessageBox.Show("Пожалуйста, выберите таблицу.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ShowMetaData(object? o)
    {
        if (_window.MainFrame.Content is TablePage)
        {
            var chosenTable = _window.TableChoosingBox.SelectedItem as string;
            var metaDataWindow = new TableMetaDataWindow(chosenTable!, _selectedDatabase);
            metaDataWindow.Show();
        }
        else
        {
            MessageBox.Show("No-таблица = no-метаданные", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static void ShowDbsMetaData(object? o)
    {
        var metaDataWindow = new DbMetaDataWindow();
        metaDataWindow.Show();
    }

    private void CreateDatabase(object? o)
    {
        var window = new DatabaseCreatingForm(this);
        window.Show();
    }

    private void CreateTable(object? o)
    {
        var window = new TableCreatingForm(DatabasesNames);
        window.Show();
    }

    private void RedactDb(object? o)
    {
        if (_selectedDatabase != null)
        {
            var window = new DbRedactingWindow(_selectedDatabase);
            window.Show();
        }
        else
        {
            MessageBox.Show("Пожалуйста, выберите Базу данных", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Refresh(object? o)
    {
        _window.DatabaseChoosingBox.ItemsSource = new ObservableCollection<string>(InitDatabases());
        _window.DatabaseChoosingBox.SelectedItem = null;
        _window.TableChoosingBox.SelectedItem = null;
        _window.MainFrame.Content = null;
    }

    private void OpenChangingMenu(object? o)
    {
        if (_window.TableChoosingBox.SelectedItem is not null)
        {
            var window =
                new TableRedactingWindow((_window.TableChoosingBox.SelectedItem as string)!,
                    _selectedDatabase);
            window.Show();
        }
        else
        {
            MessageBox.Show("Пожалуйста, выберите таблицу.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
}