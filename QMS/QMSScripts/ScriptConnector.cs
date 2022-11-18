using Newtonsoft.Json;

namespace QMS.QMSScripts;

public class ConnectionRequestHandler
{
    /// <summary>
    /// Starts listening for incoming connections
    /// </summary>
    /// <returns></returns>
    public static async Task StartListeningForConnections()
    {
        ProcessCI processCI= new ProcessCI();
        processCI.MessageRecieved += processCI_MessageRecieved;
        processCI.RecieveCI();

    }
    /// <summary>
    /// When event invoked, carry out checks as appropriate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public static async void processCI_MessageRecieved(object sender, EventArgsCI e)
    {
        ConnectionInitializer ci;
        switch (e.CI.sendOrRetrieveUsername)
        {
            case 1: //when sender confirms recipient username matches desired
                KeyVarFunc.senderUsername = e.CI.username;
                KeyVarFunc.correctRecipient = true;
                MessageHandler.StartListeningForMessages();
                break;

            case 2: //when recipient recieves request for username
                ci = new(3, KeyVarFunc.username);

                ClientFunctions.SendCI(ci.CISerialize());
                break;

            case 3: //when sender recieves requested username, check if correct
                if (e.CI.username == KeyVarFunc.desiredRecipient)
                {
                    KeyVarFunc.desiredRecipient = e.CI.username;
                    KeyVarFunc.correctRecipient = true;
                    ci = new(1, KeyVarFunc.username);
                    ClientFunctions.SendCI(ci.CISerialize());
                    MessageHandler.StartListeningForMessages();
                }
                else
                {
                    KeyVarFunc.correctRecipient = false;
                    ci = new(4, "");
                    ClientFunctions.SendCI(ci.CISerialize());
                }
                break;

            case 4: //when sender confirms wrong recipient
                KeyVarFunc.correctRecipient = false;
                break;

            default: //when terminate command is recieved
                KeyVarFunc.terminate = true;
                KeyVarFunc.correctRecipient = false;
                break;
        }
    }
    /// <summary>
    /// Establishes end user
    /// </summary>
    /// <param name="username">Desired username</param>
    /// <returns></returns>
    public static void RequestSend(string username)
    {
        KeyVarFunc.desiredRecipient = username;
        ConnectionInitializer ci = new(2, "");
        ClientFunctions.SendCI(ci.CISerialize());
    }
}