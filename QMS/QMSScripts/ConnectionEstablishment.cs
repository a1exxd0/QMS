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
public class EventArgsUsername : EventArgs
{
    public string? username
    {
        get; set;
    }
}
public class ConnectionEstablishment
{
    /// <summary>
    /// Flag appears when the reciever of the CI returns the correct username
    /// Likely progression to have Qgle message and say a cconnection has been established with you
    /// HERE AFTER EVENT IS CALLED START LISTENING
    /// </summary>
    public EventHandler<EventArgsUsername>? FlagToAddNewRecipient;
    /// <summary>
    /// If the sender recieves an incorrect username in return
    /// </summary>
    public EventHandler<EventArgsUsername>? FlagForUnsuccessfulRecipient;
    /// <summary>
    /// If the sender recieves the correct username
    /// HERE AFTER EVENT IS CALLED START LISTENING
    /// </summary>
    public EventHandler<EventArgsUsername>? FlagForSuccessfulRecipient;

    private readonly ClientFunctions cf = new();
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
                //now recipient adds sender to queues
                EventArgsUsername e1 = new EventArgsUsername(); e1.username = e.CI.username;
                FlagToAddNewRecipient?.Invoke(this, e1);


                break;

            case 2: //when recipient recieves request for username
                ci = new(3, KeyVarFunc.username);
                cf.SendObject(ci);
                break;

            case 3: //when sender recieves requested username, check if correct
                if (e.CI.username == KeyVarFunc.desiredRecipient)
                {
                    //sender end - correct recipient found
                    ci = new(1, KeyVarFunc.username);
                    cf.SendObject(ci);

                    EventArgsUsername e3 = new EventArgsUsername(); e3.username = e.CI.username;
                    FlagForSuccessfulRecipient?.Invoke(this, e3);

                }
                else
                {
                    //sender end - incorrect recipient
                    ci = new(4, "");
                    cf.SendObject(ci);
                    EventArgsUsername e4 = new EventArgsUsername(); e4.username = KeyVarFunc.lastRequestedUsername;
                    FlagForUnsuccessfulRecipient?.Invoke(this, e4);
                }
                break;

            case 4: //when sender confirms wrong recipient
                    //recipient does the below

                break;

            default: //when terminate command is recieved

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
#region event handling for ConnectionInitializer
/// <summary>
/// EventArgs child class containing property for what I want passed through program
/// </summary>
public class EventArgsCI : EventArgs
{
    public ConnectionInitializer CI
    {
        get; set;
    }
}
/// <summary>
/// Class to raise event when ConnectionInitializer is recieved
/// </summary>
public class ProcessCI
{
    public EventHandler<EventArgsCI>? MessageRecieved; //Event flag
    public static readonly IPEndPoint ciReciever = new(IPAddress.Any, 31050); //port to listen to
    /// <summary>
    /// Listens to port
    /// </summary>
    /// <returns>Returns on completion</returns>
    public async Task RecieveCI()
    {
        using Socket listener = new(SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(ciReciever);//accepts connections from all ip addresses
                                  //listening from port 31050
        listener.Listen(100);
        while (true)
        {
            var handler = await listener.AcceptAsync(); //accept connection
            while (true)
            {
                var buffer = new byte[4096];
                var recieved = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, recieved);
                var eom = "<|EOM|>";
                if (response.IndexOf(eom) > -1 /* end of message detected*/)
                {
                    //Console.WriteLine("message recieved");

                    var ackMessage = "<|ACK|>";
                    var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
                    await handler.SendAsync(echoBytes, 0);
                    //Console.WriteLine("acknowledgement sent");
                    //convert to object and return
                    ConnectionInitializer? ci = JsonConvert.DeserializeObject<ConnectionInitializer>
                        (response.Replace(eom, ""));
                    EventArgsCI data = new();
                    data.CI = ci; //creating event argument for event flag

                    OnComplete(data);
                    break;
                }
            }
        }
    }
    /// <summary>
    /// Invoker function
    /// </summary>
    /// <param name="e">Event argument</param>
    protected virtual void OnComplete(EventArgsCI e)
    {
        MessageRecieved?.Invoke(this, e);
        //invoke event
    }
}
#endregion