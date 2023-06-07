using DummyDB.Desktop.ViewModels;

namespace DummyDB.Desktop.Views.Forms;

public partial class DatabaseCreatingForm
{
    public DatabaseCreatingForm(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = new DatabaseCreatingViewModel(this, viewModel);
    }
}