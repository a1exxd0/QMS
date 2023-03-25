using System.Security.Cryptography.X509Certificates;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Core;
using Microsoft.UI.Xaml.Media.Animation;
using System.Windows;
using QMS.QMSScripts;
using QMS.ViewModels;
using Microsoft.UI.Xaml.Input;
using Windows.UI.Core;
using Windows.System;
using QMS.Networking;
using System.Threading;
using System;
using System.ComponentModel;

namespace QMS.Views;

public sealed partial class MessagingPage : Page
{
    public EventHandler<RoutedEventArgs>? LogoutPressedRecieved; //Event flags
    public EventHandler<RoutedEventArgs>? SendMessageRecieved;
    public EventHandler<KeyEventArgs>? EnterRecieved;
    public QgleAssistant qg = new();

    public MessagingViewModel ViewModel
    {
        get;
    }

    public MessagingPage()
    {
        ViewModel = App.GetService<MessagingViewModel>();
        Resources.Add("LeftBorderWidth", 300);
        Resources.Add("TopBorderHeight", 180);
        //Resources.Add("LeftBorderColour", "#C3C3C3");
        //Resources.Add("PurpleColour", "#C293FF");
        //Resources.Add("LightGrey", "#FFDCDCDE");
        Resources.Add("LeftBorderColour", "#b0e0e6");
        Resources.Add("PurpleColour", "#6495ED");
        Resources.Add("LightGrey", "#f0f8ff");
        LogoutPressedRecieved += LogoutPressedFunction;
        SendMessageRecieved += SendMessageFunction;
        ce.FlagToAddNewRecipient += FlagToAddNewRecipientFunction;
        ce.FlagForSuccessfulRecipient += FlagForSuccessfulRecipientFunction;
        ce.FlagForUnsuccessfulRecipient += FlagForUnsuccessfulRecipientFunction;
        pm.MessageComplete += MessageCompleteFunction;

        InitializeComponent();
        LoggedInAs.Text = "Logged in as\n" + KeyVarFunc.username;
        MessageList newList = new MessageList("Q-gle Assistant");
        KeyVarFunc.queues.Add(newList);
        
        loadNewRecipient();
        InitiateQgleChat();
        new Thread(() => { pm.CheckForMessages(); }).Start();
        ce.StartListeningForConnections();
        
    }
    private async void testFunction()
    {
        new Thread(() => { pm.CheckForMessages(); });
        ce.StartListeningForConnections();

    }
    #region flags incoming messages
    private void FlagToAddNewRecipientFunction(object sender, EventArgsUsername e)
    {
        MessageList? result = KeyVarFunc.queues.Find(delegate (MessageList ml)
        {
            return ml.recieverUsername == KeyVarFunc.desiredRecipient;
        });
        if (result == null) { qg.addUserToQueues(e.username); }
        mh.StartListeningForMessages();
        UpdateDropdowns();

    }
    private void FlagForSuccessfulRecipientFunction(object sender, EventArgsUsername e)
    {
        KeyVarFunc.queues.Find(delegate (MessageList ml)
        {
            return ml.recieverUsername == e.username;
        })!.AddSystemMessage("Successfully connected!");
        mh.StartListeningForMessages();

    }
    private void FlagForUnsuccessfulRecipientFunction(object sender, EventArgsUsername e)
    {
        KeyVarFunc.queues.Find(delegate (MessageList ml)
        {
            return ml.recieverUsername == e.username;
        })!.AddSystemMessage("Connection failed.");
    }
    private void MessageCompleteFunction(object sender, EventArgsMessage e)
    {
        KeyVarFunc.queues.Find(delegate (MessageList ml)
        {
            return ml.recieverUsername == e.username;
        })!.AddRecievedMessage(e.message);
        if (KeyVarFunc.currentEndUser == e.username)
        {
            UpdateSingleMessage();
        }
    }

    #endregion

    #region connection listeners

    #endregion


    #region logout pressed functions
    private void LogoutPressedFunction(object sender, RoutedEventArgs e)
    {
        
        frame.Navigate(typeof(MainPage));
        KeyVarFunc.username = "";
        KeyVarFunc.ClearQueue();
        MessagingLeftBorder.Visibility = Visibility.Collapsed;
        MessagingTopBorder.Visibility = Visibility.Collapsed;
        MessagingBottomBorder.Visibility = Visibility.Collapsed;
        ChatArea.Visibility = Visibility.Collapsed;
        MessageList newList = new MessageList("Q-gle Assistant");
        KeyVarFunc.queues.Add(newList);
    }

    private void LogoutPressed(object sender, RoutedEventArgs e)
    {
        LogoutPressedRecieved?.Invoke(this, e);
    }
    #endregion
    private void OnKeyDownHandler(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            SendMessageRecieved?.Invoke(this, e);
        }
    }

    #region Qgle chats
    private void InitiateQgleChat()
    {
        List<string> response = qg.StartConversation();
        displayNewQgleMessages(response);
    }
    private void RecieveQgleMessage(string s)
    {
        qg.updateLastResponse(s);
        List<string> response = qg.InterpretText();
        displayNewQgleMessages(response);
        UpdateDropdowns();
    }
    private void displayNewQgleMessages(List<string> ls)
    {
        for (var i = 0; i < ls.Count; i++)
        {
            addToQueue(ls[i], 1);
            UpdateSingleMessage();
        }
    }
    #endregion


    #region Chat functionality

    /// <summary>
    /// Works under the assumption a connection is established (this will happen when you type RequestConnection to the bot
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SendMessageFunction(object sender, RoutedEventArgs e)
    {
        var toBeSent = TextToBeSent.Text;
        //IMPLEMENT NETWORK HERE

        //Local message add
        
        addToQueue(toBeSent, 0);
        UpdateSingleMessage();
        TextToBeSent.Text = "";
        if (KeyVarFunc.currentEndUser == "Q-gle Assistant")
        {
            RecieveQgleMessage(toBeSent);
        }
        else
        {
            try
            {
                cf.SendMessage(toBeSent);
            }
            catch
            {

            }
            
        }

    }

    private void SendMessage(object sender, RoutedEventArgs e)
    {
        SendMessageRecieved?.Invoke(this, e);
    }



    /// <summary>
    /// Add a message to queue
    /// </summary>
    /// <param name="s"></param>
    /// <param name="option">0 = sent, 1 = recieved, 2 = system</param>
    private void addToQueue(string s, int option)
    {
        if (option == 0)
        {
            KeyVarFunc.queues.Find(delegate (MessageList ml)
            {
                return ml.recieverUsername == KeyVarFunc.currentEndUser;
            })!.AddSentMessage(s);
        }
        if (option == 1)
        {
            KeyVarFunc.queues.Find(delegate (MessageList ml)
            {
                return ml.recieverUsername == KeyVarFunc.currentEndUser;
            })!.AddRecievedMessage(s);
        }
        if (option == 2)
        {
            KeyVarFunc.queues.Find(delegate (MessageList ml)
            {
                return ml.recieverUsername == KeyVarFunc.currentEndUser;
            })!.AddSystemMessage(s);
        }
    }

    /// <summary>
    /// Load a user up to the GUI
    /// </summary>
    /// <param name="username"></param>
    private void loadNewRecipient()
    {
        string username = KeyVarFunc.currentEndUser;
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
    /// <summary>
    /// Only works with updating most recent message
    /// </summary>
    private void UpdateSingleMessage()
    {
        MessageViewer s = KeyVarFunc.queues.Find(delegate (MessageList ml)
        {
            return ml.recieverUsername == KeyVarFunc.currentEndUser;
        })!.queuedMessages.Last();


        SolidColorBrush greenBrush = new();
        greenBrush.Color = Microsoft.UI.Colors.Green;
        SolidColorBrush blueBrush = new();
        blueBrush.Color = Microsoft.UI.Colors.Blue;
        SolidColorBrush redBrush = new();
        redBrush.Color = Microsoft.UI.Colors.Red;


        if (s.sentOrRecieved == 0)
        {
            Run run = new();
            run.Text = "You: " + s.message + "\n\n";
            run.Foreground = redBrush;
            ChatBox.Inlines.Add(run);
        }
        if (s.sentOrRecieved == 1)
        {
            Run run = new();
            run.Text = KeyVarFunc.currentEndUser + ": " + s.message + "\n\n";
            run.Foreground = blueBrush;
            ChatBox.Inlines.Add(run);
        }
        if (s.sentOrRecieved == 2)
        {
            Run run = new();
            run.Text = "Alert: " + s.message + "\n\n";
            run.Foreground = greenBrush;
            ChatBox.Inlines.Add(run);
        }
    }
    #endregion

    #region Change recipient
    private void UpdateDropdowns()
    {
        RecipientNames.Items.Clear();
        List<string> recipientNames = new List<string>();
        for (var i = 0; i < KeyVarFunc.queues.Count; i++)
        {
            recipientNames.Add(KeyVarFunc.queues[i].recieverUsername);
        }
        for (var j = 0; j < recipientNames.Count; j++)
        {
            MenuFlyoutItem temp = new MenuFlyoutItem();
            temp.Text = recipientNames[j];


            temp.Click += (sender, e) => {
                KeyVarFunc.currentEndUser = temp.Text;
                loadNewRecipient();
            };

            RecipientNames.Items.Add(temp);
        }
        
    }
    #endregion
}
