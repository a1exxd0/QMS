using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
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
        testFunction();
    }
    private void LogoutPressedFunction(object sender, RoutedEventArgs e)
    {
        
        frame.Navigate(typeof(MainPage));
        KeyVarFunc.username = "";
        KeyVarFunc.ClearQueue();
        MessagingLeftBorder.Visibility = Visibility.Collapsed;
        MessagingTopBorder.Visibility = Visibility.Collapsed;
        MessagingBottomBorder.Visibility = Visibility.Collapsed;
        ChatArea.Visibility = Visibility.Collapsed;
    }

    private void LogoutPressed(object sender, RoutedEventArgs e)
    {
        LogoutPressedRecieved?.Invoke(this, e);
    }
    //test function 1
    private void testFunction()
    {
        MessageViewer temp = new MessageViewer("hey there", true);
        MessageQueue newQueue = new MessageQueue("Q-gle Assistant");
        KeyVarFunc.queues.Add(newQueue);

        KeyVarFunc.currentEndUser = "Q-gle Assistant";

        addToQueue(temp, "Q-gle Assistant");
        var toBeDisplayed = "";
        string temp2;

        //Predicate<MessageQueue> userFinder = matchUser;
        //MUST NOT ALLOW DUPLICATE USERNAMES IN QUEUE
        temp2 = KeyVarFunc.queues.Find(delegate (MessageQueue mq)
        {
            return mq.recieverUsername == KeyVarFunc.currentEndUser;
        })!.queuedMessages.ElementAt(0).message + toBeDisplayed;

        SolidColorBrush greenBrush = new();
        greenBrush.Color = Microsoft.UI.Colors.Green;
        SolidColorBrush redBrush= new();
        redBrush.Color= Microsoft.UI.Colors.Red;
        Run run = new();

        run.Text = temp2;
        run.Foreground = greenBrush;
        ChatBox.Inlines.Add(run);

        Run run2 = new();
        run2.Text = "\n\n\nhi to you too";
        run2.Foreground = redBrush;
        ChatBox.Inlines.Add(run2);
        
    }
    //test function 2
    private void addToQueue(MessageViewer mv, string username)
    {
        KeyVarFunc.queues.Find(delegate (MessageQueue mq)
        {
            return mq.recieverUsername == KeyVarFunc.currentEndUser;
        })!.AddMessage(mv);
    }

    private bool matchUser(string username, string testcase)
    {
        if (username == testcase) { return true; } return false;
    }
}
