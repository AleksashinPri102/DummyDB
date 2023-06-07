using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DummyDB.Core.Models;
using DummyDB.Core.Services;
using DummyDB.Desktop.Views.Forms;

namespace DummyDB.Desktop.ViewModels;

public class TableCreatingWindowViewModel
{
    private const string DatabasesFolderPath = "../../../../DummyDb.Core/Databases";
    private readonly TableCreatingForm _form;
    private readonly string[] _columnTypes = { "int", "float", "string", "dataTime", "bool" };
    public ICommand AddColumnCommand { get; init; }
    public ICommand SaveTableCommand { get; init; }

    public TableCreatingWindowViewModel(TableCreatingForm form, IEnumerable<string> databases)
    {
        _form = form;
        _form.DatabaseName.ItemsSource = databases;
        AddColumnCommand = new RelayCommand(AddColumn);
        SaveTableCommand = new RelayCommand(SaveTable);
    }
    
    private void AddColumn(object? o)
    {
        var column = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };
        column.Children.Add(new TextBox { Name = "ColumnName", MinWidth = 40 });
        column.Children.Add(new ComboBox { Name = "ColumnType", ItemsSource = _columnTypes });
        column.Children.Add(new CheckBox { Name = "IsPrimary", IsChecked = false });
        _form.Columns.Children.Add(column);
    }

    private void SaveTable(object? o)
    {
        var flag = CheckColumns();
        if (!flag)
        {
            MessageBox.Show("Не удаётся сохранить. Видимо, где-то ошибка.", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            return;
        }

        var schema = new Schema
        {
            Name = _form.TableName.Text,
            Columns = new List<Column>()
        };
        foreach (var element in _form.Columns.Children)
        {
            var columnView = element as StackPanel;
            var column = new Column
            {
                Name = (columnView!.Children[0] as TextBox)!.Text,
                Type = (columnView.Children[1] as ComboBox)!.SelectedItem as string,
                IsPrimary = (columnView.Children[2] as CheckBox)!.IsChecked ?? false
            };
            schema.Columns.Add(column);
        }
        CreateTable(schema);
    }

    private bool CheckColumns()
    {
        foreach (var element in _form.Columns.Children)
        {
            if (element is StackPanel column)
            {
                if (column.Children[0] is TextBox name && column.Children[1] is ComboBox type && column.Children[2] is CheckBox check)
                {
                    var flag = name.Text != "" && type.SelectedItem is not null && check.IsChecked is not null;
                    if (!flag)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    private void CreateTable(Schema schema)
    {
        try
        {
            var newTablePath =
                $"{DatabasesFolderPath}/{_form.DatabaseName.SelectedItem as string}/{_form.TableName.Text}";
            if (Directory.Exists(newTablePath))
            {
                throw new Exception("Данная таблица уже существует.");
            }
            Directory.CreateDirectory(newTablePath);
            schema.ToJsonFile((_form.DatabaseName.SelectedItem as string)!);
            var table = new Table(schema.Name, schema);
            CsvWritingService.WriteInCsv(table, (_form.DatabaseName.SelectedItem as string)!);
            _form.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}