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
#region processing recieved messages

/// <summary>
/// EventArgs child class containing property for what I want passed through program
/// </summary>
public class EventArgsMI : EventArgs
{
    public MessageInitializer MI
    {
        get; set;
    }
}
/// <summary>
/// Class to raise event when MessageInitializer is recieved
/// </summary>
public class ProcessMI
{
    public EventHandler<EventArgsMI>? MessageRecieved; //Event flag
    public static readonly IPEndPoint miReciever = new(IPAddress.Any, 31051); //port to listen to
    /// <summary>
    /// Listens to port
    /// </summary>
    /// <returns>Returns on completion</returns>
    public async Task RecieveMI()
    {
        using Socket listener = new(SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(miReciever);//accepts connections from all ip addresses
                                  //listening from port 31051
        listener.Listen(100);
        while (true)
        {
            var handler = await listener.AcceptAsync(); //accept connection
            while (true)
            {
                var buffer = new byte[2048];
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
                    MessageInitializer? mi = JsonConvert.DeserializeObject<MessageInitializer>
                        (response.Replace(eom, ""));
                    EventArgsMI data = new();
                    data.MI = mi;

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
    protected virtual void OnComplete(EventArgsMI e)
    {
        MessageRecieved?.Invoke(this, e);
        //invoke event
    }
}
/// <summary>
/// EventArgs child class containing property for what I want passed through program
/// </summary>
public class EventArgsSC : EventArgs
{
    public SenderCharacter SC
    {
        get; set;
    }
}
/// <summary>
/// Class to raise event when SenderCharacters is recieved
/// </summary>
public class ProcessSC
{
    public EventHandler<EventArgsSC>? MessageRecieved; //Event flag
    public static readonly IPEndPoint scReciever = new(IPAddress.Any, 31052); //port to listen to
    /// <summary>
    /// Listens to port
    /// </summary>
    /// <returns>Returns on completion</returns>
    public async Task RecieveSC()
    {
        using Socket listener = new(SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(scReciever);//accepts connections from all ip addresses
                                  //listening from port 31052
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
                    SenderCharacter? sc = JsonConvert.DeserializeObject<SenderCharacter>
                        (response.Replace(eom, ""));
                    EventArgsSC data = new();
                    data.SC = sc;

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
    protected virtual void OnComplete(EventArgsSC e)
    {
        MessageRecieved?.Invoke(this, e);
        //invoke event
    }
    public static async Task CheckMOExists(int i)
    {
        while (true)
        {
            if (SideReciever.messageIDQueue.Contains(i))
            {
                break;
            }
            else
            {
                Task.Delay(1000).Wait();
            }
        }
    }
}
/// <summary>
/// EventArgs child class containing property for what I want passed through program
/// </summary>
public class EventArgsSQ : EventArgs
{
    public SenderQubits SQ
    {
        get; set;
    }
}
/// <summary>
/// Class to raise event when SenderCharacters is recieved
/// </summary>
public class ProcessSQ
{
    public EventHandler<EventArgsSQ>? MessageRecieved; //Event flag
    public static readonly IPEndPoint sqReciever = new(IPAddress.Any, 31053); //port to listen to
    /// <summary>
    /// Listens to port
    /// </summary>
    /// <returns>Returns on completion</returns>
    public async Task RecieveSQ()
    {
        using Socket listener = new(SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(sqReciever);//accepts connections from all ip addresses
                                  //listening from port 31053
        listener.Listen(100);
        while (true)
        {
            var handler = await listener.AcceptAsync(); //accept connection
            while (true)
            {
                var buffer = new byte[32768];
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
                    SenderQubits? sq = JsonConvert.DeserializeObject<SenderQubits>
                        (response.Replace(eom, ""));
                    EventArgsSQ data = new();
                    data.SQ = sq;

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
    protected virtual void OnComplete(EventArgsSQ e)
    {
        MessageRecieved?.Invoke(this, e);
        //invoke event
    }
}

#endregion
#region handling messages
public class MessageHandler
{

    /// <summary>
    /// Run to begin listening to messages
    /// </summary>
    /// <returns></returns>
    public async Task StartListeningForMessages()
    {
        //Instantiate processes to be used
        ProcessMI processMI = new ProcessMI();
        ProcessSC processSC = new ProcessSC();
        ProcessSQ processSQ = new ProcessSQ();

        //Subscribing to publishers
        processMI.MessageRecieved += processMI_MessageRecieved;
        processSC.MessageRecieved += processSC_MessageRecieved;
        processSQ.MessageRecieved += processSQ_MessageRecieved;

        //Disposal service when terminate is true
        var cts = new CancellationTokenSource(700);
        var options = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 5,
            CancellationToken = cts.Token
        };
        try
        {
            Parallel.Invoke(
            () => processMI.RecieveMI(),
            () => processSC.RecieveSC(),
            () => processSQ.RecieveSQ()
            );
        }
        catch (OperationCanceledException)
        {

        }
        //CancelListen cl = new CancelListen();
        //await cl.CheckTerminate();
        //cts.Dispose();
    }

    /// <summary>
    /// What to execute when invoker is called
    /// 
    /// For MessageInitializer object
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">Event argument</param>
    public void processMI_MessageRecieved(object sender, EventArgsMI e)
    {
        int messageID = e.MI.messageID;
        int messageLength = e.MI.messageLength;
        string username = e.MI.senderUsername;
        MessageObject mo = new MessageObject(messageID, messageLength, username);
        SideReciever.messageObjectList.Add(mo);
        SideReciever.messageIDQueue.Enqueue(messageID);
    }
    /// <summary>
    /// What to execute when invoker is called
    /// 
    /// For SenderCharacters object
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">Event argument</param>
    public async void processSC_MessageRecieved(object sender, EventArgsSC e)
    {
        int messageID = e.SC.messageID;
        int characterPosition = e.SC.characterPosition;
        int encodedValue = e.SC.encodedMessage;

        await ProcessSC.CheckMOExists(messageID);
        Task.Delay(500).Wait();
        int indexSQ = SideReciever.CheckExistsSQ(messageID, characterPosition);
        if (indexSQ == -1) //not in list
        {
            SideReciever.SenderCharactersList.Add(e.SC);
        }
        else
        {
            //Measure qubits for key 
            int result = SideReciever.measureEightQubitsLast
                            (SideReciever.SenderQubitsList[indexSQ].qubitSystems);


            int i = SideReciever.FindIndexMO(messageID);
            SideReciever.messageObjectList[i].messageContents[characterPosition]
                = (char)(encodedValue ^ result); //apply XORS

            SideReciever.SenderQubitsList.RemoveAt(indexSQ);
            //Removes from list for efficiency
            SideReciever.ChangeMessageStatus(messageID);
            //change to complete if character array is filled out
        }
    }

    /// <summary>
    /// What to execute when invoker is called
    /// 
    /// For SenderQubits object
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">Event argument</param>
    public async void processSQ_MessageRecieved(object sender, EventArgsSQ e)
    {
        int messageID = e.SQ.messageID;
        int characterPosition = e.SQ.characterPosition;
        QubitSystem[] systems = e.SQ.qubitSystems;

        await ProcessSC.CheckMOExists(messageID);
        Task.Delay(500).Wait();
        int indexSC = SideReciever.CheckExistsSC(messageID, characterPosition);
        if (indexSC == -1) //not in list
        {
            SideReciever.SenderQubitsList.Add(e.SQ);
        }
        else
        {
            //Measure qubits for key 
            int result = SideReciever.measureEightQubitsLast(e.SQ.qubitSystems);

            int encodedMessage = SideReciever.SenderCharactersList[indexSC].encodedMessage;
            int i = SideReciever.FindIndexMO(messageID);
            SideReciever.messageObjectList[i].messageContents[characterPosition]
                = (char)(encodedMessage ^ result); //apply XORS

            SideReciever.SenderCharactersList.RemoveAt(indexSC);
            //Removes from list for efficiency
            SideReciever.ChangeMessageStatus(messageID);
            //change to complete if character array is filled out
        }
    }

}
#endregion
#region Holding messages in 1 place
public class SideReciever
{
    public static Queue<int> messageIDQueue = new();
    public static List<MessageObject> messageObjectList = new();
    public static List<SenderCharacter> SenderCharactersList = new();
    public static List<SenderQubits> SenderQubitsList = new();

    /// <summary>
    /// Iterates through to find matching SC/SQ pair
    /// </summary>
    /// <param name="messageID">unique ID</param>
    /// <param name="characterPosition">position in message</param>
    /// <returns>-1 if doesnt exist, else index</returns>
    public static int CheckExistsSC(int messageID, int characterPosition)
    {
        var i = 0;
        foreach (var SC in SenderCharactersList)
        {
            if ((SC.messageID == messageID) && (SC.characterPosition == characterPosition))
            {
                return i;
            }
            i++;
        }
        return -1;
    }
    /// <summary>
    /// Iterates through to find matching SC/SQ pair
    /// </summary>
    /// <param name="messageID">unique ID</param>
    /// <param name="characterPosition">position in message</param>
    /// <returns> -1 if doesnt exist, else index</returns>
    public static int CheckExistsSQ(int messageID, int characterPosition)
    {
        var i = 0;
        foreach (var SQ in SenderQubitsList)
        {
            if ((SQ.messageID == messageID) && (SQ.characterPosition == characterPosition))
            {
                return i;
            }
            i++;
        }
        return -1;
    }
    /// <summary>
    /// Finds a message object in list of message objects based on messageID
    /// </summary>
    /// <param name="messageID">unique identifier</param>
    /// <returns>index of object</returns>
    public static int FindIndexMO(int messageID)
    {
        var i = 0;
        foreach (var MO in messageObjectList)
        {
            if (MO.messageID == messageID)
            {
                return i;
            }
            else { i++; }
        }
        return -1;
    }
    /// <summary>
    /// If all characters are filled out, change message status to complete
    /// </summary>
    /// <param name="messageID">unique identifier for message</param>
    public static void ChangeMessageStatus(int messageID)
    {
        var i = FindIndexMO(messageID);
        var complete = true;
        foreach (char c in messageObjectList[i].messageContents)
        {
            if (c == '\0')
            {
                complete = false;
                break;
            }
        }
        messageObjectList[i].messageFinishedStatus = complete;
    }
    public static int measureEightQubitsLast(QubitSystem[] qubits)
    {
        int result = 0;
        for (int i = 0; i < qubits.Length; i++)
        {
            int temp = qubits[i].measurement(4);
            int multiplier = QubitSystem.intPow(2, qubits.Length - i - 1);
            result += multiplier * temp;
        }

        return result;
    }
}
#endregion
#region processing messages when complete
/// <summary>
/// EventArgs child class containing property for what I want passed through program
/// </summary>
public class EventArgsMessage : EventArgs
{
    public string message
    {
        get; set;
    }
    public string username
    {
        get; set;
    }
}
/// <summary>
/// Class to raise flag when there is a complete message
/// </summary>
public class ProcessMessage
{
    public EventHandler<EventArgsMessage>? MessageComplete; //Event flag

    /// <summary>
    /// Repeatedly checks message queue for objects that are complete
    /// </summary>
    /// <returns></returns>
    public async Task CheckForMessages()
    {
        while (true)
        {
            Task.Delay(2000).Wait();
            try
            {
                int messageID = SideReciever.messageIDQueue.Peek();
                int indexMessage = SideReciever.FindIndexMO(messageID);
                if (SideReciever.messageObjectList[indexMessage].messageFinishedStatus)
                {
                    EventArgsMessage data = new EventArgsMessage();
                    data.message = new string(SideReciever.messageObjectList[indexMessage].messageContents);
                    data.username = SideReciever.messageObjectList[indexMessage].username;

                    //clear up clutter in data structures
                    SideReciever.messageIDQueue.Dequeue();
                    SideReciever.messageObjectList.RemoveAt(indexMessage);
                    OnComplete(data);
                }
            }
            catch
            {

            }

        }
    }
    /// <summary>
    /// Invoker function when message is found that is complete
    /// MUST STORE IN STACK 
    /// </summary>
    /// <param name="e">Event argument</param>
    protected virtual void OnComplete(EventArgsMessage e)
    {
        MessageComplete?.Invoke(this, e);
        //invoke event
    }
}
#endregion