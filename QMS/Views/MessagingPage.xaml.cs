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
        Resources.Add("LeftBorderWidth", 300);
        Resources.Add("TopBorderHeight", 180);
        Resources.Add("LeftBorderColour", "#C3C3C3");
        Resources.Add("PurpleColour", "#C293FF");
        Resources.Add("LightGrey", "#FFDCDCDE");


        InitializeComponent();
    }
}
