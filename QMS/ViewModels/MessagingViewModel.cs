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
/// sent is 0, recieved is 1, and error messages are 2 for sentOrRecieved attribute
/// </summary>
public class MessageViewer
{
    public string message;
    public int sentOrRecieved;

    public MessageViewer(string Message, int SentOrRecieved)
    {
        message = Message;
        sentOrRecieved = SentOrRecieved; // sent is true, recieved is false
    }
}
/// <summary>
/// One queue for each person you are talking to
/// 
/// Newest messages at the end of the list
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
    /// Adds a message to the end of the list
    /// </summary>
    /// <param name="messageViewer">Message you want to add</param>
    private void AddMessage(MessageViewer messageViewer)
    {
        queuedMessages.Add(messageViewer);
    }
    /// <summary>
    /// Add message sent by app user to queue
    /// </summary>
    /// <param name="s"></param>
    public void AddSentMessage(string s)
    {
        queuedMessages.Add(new MessageViewer(s, 0));
    }
    /// <summary>
    /// Adds message recieved by app to queue
    /// </summary>
    /// <param name="s"></param>
    public void AddRecievedMessage(string s)
    {
        queuedMessages.Add(new MessageViewer(s, 1));
    }
    /// <summary>
    /// Adds system message to queue
    /// </summary>
    /// <param name="s"></param>
    public void AddSystemMessage(string s)
    {
        queuedMessages.Add(new MessageViewer(s, 2));
    }

    //FILTER NEWLINES AND ANYTHING NOT IN ASCII KEYSET
}
