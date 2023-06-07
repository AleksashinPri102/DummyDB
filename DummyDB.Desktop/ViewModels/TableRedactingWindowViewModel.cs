using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DummyDB.Core.Models;
using DummyDB.Core.Services;
using DummyDB.Desktop.Views;

namespace DummyDB.Desktop.ViewModels;

class TableRedactingWindowViewModel
{
    private const string DatabasesFolderPath = "../../../../DummyDB.Core/Databases";
    private readonly TableRedactingWindow _window;
    private readonly string _oldTableName;
    private readonly string _dbName;
    private readonly Table _beginTable;
    private readonly string[] _types = { "int", "float", "string", "dateTime", "bool" };
    private readonly Schema _schema;
    private bool _isCorrect;
    private string _tableName;
    
    public ObservableCollection<RowViewModel> Rows { get; set; } = new();
    public RowViewModel SelectedRow { get; set; } = null!;
    public string TableName { get; set; }
    public ICommand AddColumnCommand { get; init; }
    public ICommand AddRowCommand { get; init; }
    public ICommand CheckChangesCommand { get; init; }
    public ICommand SaveAllChangesCommand { get; init; }
    public ICommand DeleteRowCommand { get; init; }
    public ICommand RenameTableCommand { get; init; }

    public TableRedactingWindowViewModel(TableRedactingWindow window, string tableName, string dbName)
    {
        _window = window;
        _oldTableName = tableName;
        _dbName = dbName;
        TableName = _oldTableName;
        _tableName = _oldTableName;
        AddColumnCommand = new RelayCommand(AddColumn);
        AddRowCommand = new RelayCommand(AddRow);
        CheckChangesCommand = new RelayCommand(CheckChanges);
        SaveAllChangesCommand = new RelayCommand(SaveAllChanges);
        DeleteRowCommand = new RelayCommand(DeleteRow);
        RenameTableCommand = new RelayCommand(RenameTable);
        _beginTable = Init();
        _schema = _beginTable.Schema;
        InitColumns();
        InitRows();
    }
    
    private Table Init()
    {
        var dataPath = $"{DatabasesFolderPath}//{_dbName}//{_oldTableName}//{_oldTableName}.csv";
        var schemaPath = $"{DatabasesFolderPath}//{_dbName}//{_oldTableName}//{_oldTableName}.json";
        
        var data = CsvReadingService.ReadFromCsv(dataPath, schemaPath);
        var schema = Schema.GetFromJsonFile($"{DatabasesFolderPath}//{_dbName}//{_oldTableName}//{_oldTableName}.json");
        
        return TableCreatingService.CreateTable(_oldTableName, schema!, data!);
    }

    private void InitColumns()
    {
        _window.TableGrid.Columns.Clear();
        _window.Columns.Children.Clear();
        for (var i = 0; i < _schema.Columns.Count; i++)
        {
            var tableTextColumn = new DataGridTextColumn
            {
                Header = _schema.Columns[i].Name,
                Binding = new Binding($"Elements[{i}]")
            };
            
            _window.TableGrid.Columns.Add(tableTextColumn);

            var column = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            column.Children.Add(new TextBox { Text = _schema.Columns[i].Name, MinWidth = 50 });
            column.Children.Add(new ComboBox
                { ItemsSource = _types, SelectedItem = _schema.Columns[i].Type });
            column.Children.Add(new CheckBox { IsChecked = _schema.Columns[i].IsPrimary });
            var index = i;
            column.Children.Add(new Button
            {
                Content = "Удалить", Command = new RelayCommand(_ =>
                {
                    _window.Columns.Children.Remove(column);
                    _schema.Columns.RemoveAt(index);
                    _window.TableGrid.Columns.RemoveAt(index);
                    foreach (var row in Rows)
                    {
                        row.Elements.RemoveAt(index);
                    }
                })
            });
            column.Children.Add(new Button
            {
                Content = "Сохранить", Command = new RelayCommand(_ =>
                {
                    _schema.Columns[index].Name = ((_window.Columns.Children[index] as StackPanel)!.Children[0] as TextBox)!.Text;
                    _schema.Columns[index].Type =
                        ((_window.Columns.Children[index] as StackPanel)!.Children[1] as ComboBox)!.SelectedItem as string;
                    _schema.Columns[index].IsPrimary = (bool)((_window.Columns.Children[index] as StackPanel)!.Children[2] as CheckBox)!.IsChecked!;
                    _window.TableGrid.Columns.Add(new DataGridTextColumn
                    {
                        Header = (column.Children[0] as TextBox)!.Text,
                        Binding = new Binding($"Elements[{index}]")
                    });
                })
            });
            _window.Columns.Children.Add(column);
        }
    }

    private void InitRows()
    {
        if (Rows.Count != 0) return;
        var data = _beginTable.Rows
            .Select(row => new RowViewModel { Elements = row.Elements.Values.ToList() })
            .ToList();
        Rows = new ObservableCollection<RowViewModel>(data);
    }

    private void AddColumn(object? o)
    {
        var column = new Column { Name = "", Type = "" };
        _schema.Columns.Add(column);
        var columnPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };
        columnPanel.Children.Add(new TextBox { Text = "", MinWidth = 50 });
        columnPanel.Children.Add(new ComboBox
            { ItemsSource = _types });
        columnPanel.Children.Add(new CheckBox { IsChecked = false });
        var index = _schema.Columns.IndexOf(column);
        columnPanel.Children.Add(new Button
        {
            Content = "Удалить", Command = new RelayCommand(_ =>
            {
                _window.Columns.Children.Remove(columnPanel);
                _schema.Columns.Remove(column);
                _window.TableGrid.Columns.RemoveAt(index);
                foreach (var row in Rows)
                {
                    row.Elements.RemoveAt(index);
                }
            })
        });
        columnPanel.Children.Add(new Button
        {
            Content = "Сохранить", Command = new RelayCommand(_ =>
            {
                column.Name = ((_window.Columns.Children[index] as StackPanel)!.Children[0] as TextBox)!.Text;
                column.Type =
                    ((_window.Columns.Children[index] as StackPanel)!.Children[1] as ComboBox)!.SelectedItem as string;
                column.IsPrimary = (bool)((_window.Columns.Children[index] as StackPanel)!.Children[2] as CheckBox)!.IsChecked!;
                _window.TableGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = (columnPanel.Children[0] as TextBox)!.Text,
                    Binding = new Binding($"Elements[{index}]")
                });
                _schema.Columns[index] = column;
            })
        });
        _window.Columns.Children.Add(columnPanel);

        foreach (var row in Rows)
        {
            row.Elements.Add("");
        }
    }

    private void AddRow(object? o)
    {
        var adapter = new RowViewModel(_schema.Columns.Count);
        Rows.Add(adapter);
    }

    private void CheckChanges(object? o)
    {
        var strings = new List<string>();
        var columns = _schema.Columns
            .Select(column => column.Name)
            .ToList();
        strings.Add(string.Join(';', columns));
        strings.AddRange(Rows
            .Select(adapter => adapter.Elements
                .Select(element => element.ToString())
                .ToList())
            .Select(row => string.Join(';', row)));

        _isCorrect = JsonValidationService.CheckBySchema(_schema, strings.ToArray());
        string response;
        if (_isCorrect)
        {
            InitColumns();
            response = "Данные корректны.";
            MessageBox.Show(response, "Succes", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
        else
        {
            response = "Данные не корректны";
            MessageBox.Show(response, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void RenameTable(object? o)
    {
        if (_window.TableName.Text != "")
        {
            _tableName = _window.TableName.Text;
        }
        else
        {
            MessageBox.Show("Введите имя таблицы.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void DeleteRow(object? o)
    {
        if (_window.IndexForDelete.Visibility == Visibility.Collapsed)
        {
            _window.IndexForDelete.Visibility = Visibility.Visible;
        }
        else
        {
            try
            {
                _ = int.TryParse(_window.IndexForDelete.Text, out var index);
                Rows.RemoveAt(index - 1);
                _window.IndexForDelete.Visibility = Visibility.Collapsed;
            }
            catch
            {
                MessageBox.Show("Пожалуйста, введите число.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }

    private void SaveAllChanges(object? o)
    {
        _schema.Name = _tableName;
        var table = new Table(_tableName, _schema);
        foreach (var rowAdapter in Rows)
        {
            var row = new Dictionary<Column, object>();
            for (var j = 0; j < _schema.Columns.Count; j++)
            {
                row[_schema.Columns[j]] = rowAdapter.Elements[j];
            }

            table.Rows.Add(new Row { Elements = row });
        }
        try
        {
            if (_tableName != _oldTableName)
            {
                var tableFolder = new DirectoryInfo($"{DatabasesFolderPath}/{_dbName}/{_oldTableName}");
                foreach (var file in tableFolder.GetFiles())
                {
                    file.MoveTo($"{DatabasesFolderPath}/{_dbName}/{_oldTableName}/{string.Concat(_tableName, file.Extension)}");
                }
                tableFolder.MoveTo($"{DatabasesFolderPath}/{_dbName}/{_tableName}");
            }
            CsvWritingService.WriteInCsv(table, _dbName);
            _schema.ToJsonFile(_dbName);
            _window.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}