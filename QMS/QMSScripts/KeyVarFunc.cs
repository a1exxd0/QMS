namespace QMS.QMSScripts;
//Newtonsoft.Json
//System.Data.SQL
//System.Net.Sockets
public class KeyVarFunc
{
    public static string? username { get; set; }
    public static string? senderUsername{ get; set; }
    public static string? desiredRecipient {get; set; }
    public static bool correctRecipient = false;
    public static bool terminate = false;

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

 