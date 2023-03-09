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
        MessageViewer temp = new MessageViewer("hey there", 0);
        MessageViewer temp1 = new MessageViewer("hi to you too", 1);
        MessageViewer temp2 = new MessageViewer("Dont input anything bad!", 2);
        MessageList newList = new MessageList("Q-gle Assistant");
        KeyVarFunc.queues.Add(newList);

        KeyVarFunc.currentEndUser = "Q-gle Assistant";

        addToQueue(temp, "Q-gle Assistant");
        addToQueue(temp1, "Q-gle Assistant");
        addToQueue(temp2, "Q-gle Assistant");

        loadNewRecipient("Q-gle Assistant");

    }
    //test function 2
    private void addToQueue(MessageViewer mv, string username)
    {
        KeyVarFunc.queues.Find(delegate (MessageList ml)
        {
            return ml.recieverUsername == KeyVarFunc.currentEndUser;
        })!.AddMessage(mv);
    }

    /// <summary>
    /// Load a user up to the GUI
    /// </summary>
    /// <param name="username"></param>
    private void loadNewRecipient(string username)
    {
        SolidColorBrush greenBrush = new();
        greenBrush.Color = Microsoft.UI.Colors.Green;
        SolidColorBrush blueBrush = new();
        blueBrush.Color = Microsoft.UI.Colors.Blue;
        SolidColorBrush redBrush = new();
        redBrush.Color = Microsoft.UI.Colors.Red;

        NameBox.Text= username;
        ChatBox.Inlines.Clear();//resets textbox contents
        MessageList mList = KeyVarFunc.queues.Find(delegate (MessageList ml)
        {
            return ml.recieverUsername == username;
        })!; //get appropriate list

        var initialSize = mList.queuedMessages.Count();
        for (var i = 0; i < initialSize; i++)
        {
            MessageViewer tempMessage = mList.queuedMessages[i];

            if (tempMessage.sentOrRecieved == 0)
            {
                Run run = new();
                run.Text = "You: " + tempMessage.message + "\n\n";
                run.Foreground = redBrush;
                ChatBox.Inlines.Add(run);
            }
            if (tempMessage.sentOrRecieved == 1)
            {
                Run run = new();
                run.Text = username + ": " + tempMessage.message + "\n\n";
                run.Foreground = blueBrush;
                ChatBox.Inlines.Add(run);
            }
            if (tempMessage.sentOrRecieved == 2)
            {
                Run run = new();
                run.Text = "Alert: " + tempMessage.message + "\n\n";
                run.Foreground = greenBrush;
                ChatBox.Inlines.Add(run);
            }
        }
        
    }
}
