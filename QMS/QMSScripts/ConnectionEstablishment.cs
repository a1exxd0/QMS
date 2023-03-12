using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using QMS.Views;
using global::QMS.QMSScripts;

namespace QMS.Networking;

#region Starting and recieving connection requests
public class ConnectionEstablishment
{
    ClientFunctions cf = new();
    /// <summary>
    /// Starts listening for incoming connections
    /// </summary>
    /// <returns></returns>
    public async Task StartListeningForConnections()
    {
        ProcessCI processCI = new ProcessCI();
        processCI.MessageRecieved += processCI_MessageRecieved;
        processCI.RecieveCI();

    }
    /// <summary>
    /// When event invoked, carry out checks as appropriate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void processCI_MessageRecieved(object sender, EventArgsCI e)
    {
        ConnectionInitializer ci;
        
        switch (e.CI.sendOrRetrieveUsername)
        {
            case 1: //when sender confirms recipient username matches desired
                KeyVarFunc.senderUsername = e.CI.username; // COULD THIS BE UNNECESSARY? ignore for now
                KeyVarFunc.correctRecipient = true;


                //NEED TO SORT OUT BELOW BIT
                MessageHandler.StartListeningForMessages();
                break;

            case 2: //when recipient recieves request for username
                ci = new(3, KeyVarFunc.username);

                cf.SendObject(ci);
                break;

            case 3: //when sender recieves requested username, check if correct
                if (e.CI.username == KeyVarFunc.desiredRecipient)
                {
                    KeyVarFunc.desiredRecipient = e.CI.username;
                    KeyVarFunc.correctRecipient = true;
                    ci = new(1, KeyVarFunc.username);
                    cf.SendObject(ci);
                    //MessageHandler.StartListeningForMessages(); possibly unnecessary
                }
                else
                {
                    KeyVarFunc.correctRecipient = false;
                    ci = new(4, "");
                    cf.SendObject(ci);
                }
                break;

            case 4: //when sender confirms wrong recipient
                KeyVarFunc.correctRecipient = false;
                MessagingPage.correctTemporaryCIRecieved = 1;
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
    public void RequestSend(string username)
    {
        KeyVarFunc.desiredRecipient = username;
        ConnectionInitializer ci = new(2, "");
        cf.SendObject(ci);
    }
}
#endregion