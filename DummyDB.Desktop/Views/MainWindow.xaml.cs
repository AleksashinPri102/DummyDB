using DummyDB.Desktop.ViewModels;

namespace DummyDB.Desktop.Views;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel(this);
    }
}