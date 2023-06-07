using DummyDB.Desktop.ViewModels;

namespace DummyDB.Desktop.Views;

public partial class DbMetaDataWindow
{
    public DbMetaDataWindow()
    {
        InitializeComponent();
        DataContext = new DbMetaDataViewModel(this);
    }
}