using System.Collections.Generic;
using DummyDB.Desktop.ViewModels;

namespace DummyDB.Desktop.Views.Forms;

public partial class TableCreatingForm
{
    public TableCreatingForm(IEnumerable<string> databasesNames)
    {
        InitializeComponent();
        DataContext = new TableCreatingWindowViewModel(this, databasesNames);
    }
}