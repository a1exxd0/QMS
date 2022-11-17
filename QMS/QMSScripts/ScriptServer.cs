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
        var handler = await listener.AcceptAsync(); //accept connection
        while (true)
        {
            var buffer = new byte[1024];
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
        var handler = await listener.AcceptAsync(); //accept connection
        while (true)
        {
            var buffer = new byte[1024];
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
        var handler = await listener.AcceptAsync(); //accept connection
        while (true)
        {
            var buffer = new byte[1024];
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
    public async Task RecieveSC()
    {
        using Socket listener = new(SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(sqReciever);//accepts connections from all ip addresses
                                  //listening from port 31053
        listener.Listen(100);
        var handler = await listener.AcceptAsync(); //accept connection
        while (true)
        {
            var buffer = new byte[1024];
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