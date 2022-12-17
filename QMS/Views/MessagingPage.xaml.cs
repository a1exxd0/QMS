using Microsoft.UI.Xaml.Controls;

using QMS.ViewModels;

namespace QMS.Views;

public sealed partial class MessagingPage : Page
{
    public MessagingViewModel ViewModel
    {
        get;
    }

    public MessagingPage()
    {
        ViewModel = App.GetService<MessagingViewModel>();
        InitializeComponent();
    }
}
