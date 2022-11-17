using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace QMS.QMSScripts;

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
    public SenderCharacters SC
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
                    SenderCharacters? sc = JsonConvert.DeserializeObject<SenderCharacters>
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
    public static async Task CheckMOExists(uint i)
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
                var buffer = new byte[16384];
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

public class MessageHandler
{
    /// <summary>
    /// Run to begin listening to messages
    /// </summary>
    /// <returns></returns>
    public static async Task StartListeningForMessages()
    {
        //Instantiate processes to be used
        ProcessMI processMI = new ProcessMI();
        ProcessSC processSC = new ProcessSC();
        ProcessSQ processSQ= new ProcessSQ();

        //Subscribing to publishers
        processMI.MessageRecieved += processMI_MessageRecieved;
        processSC.MessageRecieved += processSC_MessageRecieved;
        processSQ.MessageRecieved += processSQ_MessageRecieved;
        Thread t = new Thread(new ThreadStart(() => {
            Parallel.Invoke(
            () => processMI.RecieveMI(),
            () => processSC.RecieveSC(),
            () => processSQ.RecieveSQ()
            );
        }));
        t.Start();

    }
    /// <summary>
    /// What to execute when invoker is called
    /// 
    /// For MessageInitializer object
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">Event argument</param>
    public static void processMI_MessageRecieved(object sender, EventArgsMI e)
    {
        uint messageID = e.MI.messageID;
        uint messageLength = e.MI.messageLength;
        MessageObject mo = new MessageObject(messageID, messageLength);
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
    public static async void processSC_MessageRecieved(object sender, EventArgsSC e)
    {
        uint messageID = e.SC.messageID;
        uint characterPosition = e.SC.characterPosition;
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
    public static async void processSQ_MessageRecieved(object sender, EventArgsSQ e)
    {
        uint messageID = e.SQ.messageID;
        uint characterPosition = e.SQ.characterPosition;
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
/// <summary>
/// EventArgs child class containing property for what I want passed through program
/// </summary>
public class EventArgsMessage : EventArgs
{
    public string message
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
                uint messageID = SideReciever.messageIDQueue.Peek();
                int indexMessage = SideReciever.FindIndexMO(messageID);
                if (SideReciever.messageObjectList[indexMessage].messageFinishedStatus)
                {
                    EventArgsMessage data = new EventArgsMessage();
                    data.message = new string(SideReciever.messageObjectList[indexMessage].messageContents);

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
    /// Invoker function
    /// </summary>
    /// <param name="e">Event argument</param>
    protected virtual void OnComplete(EventArgsMessage e)
    {
        MessageComplete?.Invoke(this, e);
        //invoke event
    }
}