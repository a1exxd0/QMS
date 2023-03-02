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
    public string Message;
    public bool SentOrRecieved;

    public MessageViewer(string message, bool sentOrRecieved)
    {
        Message = message;
        SentOrRecieved = sentOrRecieved; // sent is true, recieved is false
    }
}
/// <summary>
/// One queue for each person you are talking to
/// </summary>
public class MessageQueue
{  
    public Queue<MessageViewer> queuedMessages= new();
    public string recieverUsername;
    private static readonly int maxMessages = 10;
    public MessageQueue(string RecieverUsername)
    {  
        recieverUsername= RecieverUsername;
    }

    /// <summary>
    /// Adds a message to the queue
    /// </summary>
    /// <param name="messageViewer">Message you want to send</param>
    /// <returns>true if a message is popped, false if not</returns>
    public bool AddMessage(MessageViewer messageViewer)
    {
        if (queuedMessages.Count == maxMessages)
        {
            queuedMessages.Dequeue();
            queuedMessages.Enqueue(messageViewer);
            return true;
        }
        else
        {
            queuedMessages.Enqueue(messageViewer);
            return false;
        }
    }
}
