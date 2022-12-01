using Microsoft.UI.Xaml.Controls;

using QMS.ViewModels;

namespace QMS.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        Resources.Add("LeftBorderWidth", 300);

        InitializeComponent();

    }
}
