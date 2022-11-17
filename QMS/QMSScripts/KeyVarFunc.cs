namespace QMS.QMSScripts;
//Newtonsoft.Json
//System.Data.SQL
//System.Net.Sockets
public class KeyVarFunc
{
    private static string? username;

    public static string Username
    {
        get => username;
        set => username = value;
    }

    //Code for listening
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

 