using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using DummyDB.Core.Models;
using DummyDB.Desktop.Views;

namespace DummyDB.Desktop.ViewModels;

public class DbMetaDataViewModel
{
    private const string DatabasesFolderPath = "../../../../DummyDB.Core/Databases";

    public DbMetaDataViewModel(DbMetaDataWindow window)
    {
        window.Databases.ItemsSource = InitDbs();
    }
    
    private static IEnumerable<TreeViewItem> InitDbs()
    {
        var databases = new DirectoryInfo(DatabasesFolderPath);
        return databases
            .GetDirectories()
            .Select(database => new TreeViewItem { Header = database.Name, ItemsSource = InitTables(database) })
            .ToList();
    }

    private static IEnumerable<TreeViewItem> InitTables(DirectoryInfo database)
    {
        return database
            .GetDirectories()
            .Select(table => new TreeViewItem { Header = table.Name, ItemsSource = InitColumns(table.Name, database) })
            .ToList();
    }

    private static IEnumerable<TreeViewItem> InitColumns(string name, DirectoryInfo database)
    {
        var schema = Schema.GetFromJsonFile($"{DatabasesFolderPath}//{database.Name}//{name}//{name}.json") 
                     ?? throw new ArgumentException("Нет такой таблицы");
        return schema.Columns
            .Select(column => new TreeViewItem { Header = $"Name: {column.Name}, Type: {column.Type}, IsPrimary: {column.IsPrimary}" })
            .ToList();
    }
}