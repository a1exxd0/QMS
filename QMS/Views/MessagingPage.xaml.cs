using CommunityToolkit.WinUI.UI.Controls;

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

    private void OnViewStateChanged(object sender, ListDetailsViewState e)
    {
        if (e == ListDetailsViewState.Both)
        {
            ViewModel.EnsureItemSelected();
        }
    }
}
