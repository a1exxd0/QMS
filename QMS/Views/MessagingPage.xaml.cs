using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using QMS.QMSScripts;
using QMS.ViewModels;

namespace QMS.Views;

public sealed partial class MessagingPage : Page
{
    public EventHandler<RoutedEventArgs>? LogoutPressedRecieved; //Event flags
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
        LogoutPressedRecieved += LogoutPressedFunction;

        InitializeComponent();
        LoggedInAs.Text = "Logged in as\n" + KeyVarFunc.username;
    }
    private void LogoutPressedFunction(object sender, RoutedEventArgs e)
    {
        
        frame.Navigate(typeof(MainPage));
        KeyVarFunc.username = "";
        MessagingLeftBorder.Visibility = Visibility.Collapsed;
        MessagingTopBorder.Visibility = Visibility.Collapsed;
        MessagingBottomBorder.Visibility = Visibility.Collapsed;
    }

    private void LogoutPressed(object sender, RoutedEventArgs e)
    {
        LogoutPressedRecieved?.Invoke(this, e);
    }
}
