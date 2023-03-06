namespace QMS.QMSScripts;
using QMS.ViewModels;
//Newtonsoft.Json
//System.Data.SQL
//System.Net.Sockets
public class KeyVarFunc
{
    //username of person on application on this device
    public static string? username { get; set; }
    //username of person sending the CI to this device
    public static string? senderUsername{ get; set; } // COULD THIS BE UNNECESSARY? ignore for now
    //username of the person wanted to be contacted! remember to update
    public static string? desiredRecipient {get; set; }
    public static bool correctRecipient = false; //unnecessary?
    //kill a connection (needs to be tested)
    public static bool terminate = false;

    //container for messages
    public static List<MessageList> queues = new();
    /// <summary>
    /// Deletes all items from the queue
    /// </summary>
    public static void ClearQueue()
    {
        queues.Clear();
    }
    



    //Defaulted to Q-gle Assistant and trcks what chat should be displayed
    public static string currentEndUser = "Q-gle Assistant";


    //Code for listening for messages
    //public static void Main()
    //{
    //    MessageHandler.StartListeningForMessages();
    //    ProcessMessage pm = new ProcessMessage();
    //    pm.MessageComplete += pm_complete;
    //    pm.CheckForMessages();
    //    Console.ReadLine();
    //}
    //public static void pm_complete(object sender, EventArgsMessage e)
    //{
    //    Console.WriteLine(e.message);
    //}

}


//needs to be placed in button
public class TerminateButton
{
    public static void OnPress()
    {
        //on button click
        KeyVarFunc.terminate = true;
    }
}

 