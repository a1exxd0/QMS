using CommunityToolkit.Mvvm.ComponentModel;

namespace QMS.ViewModels;

public class MessagingViewModel : ObservableRecipient
{
    public MessagingViewModel()
    {
    }
  
}
/// <summary>
/// Class to hold messages when recieved or sent by the application
/// </summary>
public class MessageViewer
{
    public string message;
    public bool sentOrRecieved;

    public MessageViewer(string Message, bool SentOrRecieved)
    {
        message = Message;
        sentOrRecieved = SentOrRecieved; // sent is true, recieved is false
    }
}
/// <summary>
/// One queue for each person you are talking to
/// </summary>
public class MessageList
{
    public List<MessageViewer> queuedMessages = new();
    public string recieverUsername;
    public MessageList(string RecieverUsername)
    {  
        recieverUsername= RecieverUsername;
    }

    /// <summary>
    /// Adds a message to the list
    /// </summary>
    /// <param name="messageViewer">Message you want to add</param>
    public void AddMessage(MessageViewer messageViewer)
    {
        queuedMessages.Add(messageViewer);
    }
}
