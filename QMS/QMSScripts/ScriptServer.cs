using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace QMS.QMSScripts;
/// <summary>
/// Set of functions to recieve data and identify what port it is in
/// </summary>
public class ServerFunctions
{
    //declarations for IP/Socket combinations for every class transferred
    public static readonly IPEndPoint ciReciever = new(IPAddress.Any, 31050);
    public static readonly IPEndPoint miReciever = new(IPAddress.Any, 31051);
    public static readonly IPEndPoint scReciever = new(IPAddress.Any, 31052);
    public static readonly IPEndPoint sqReciever = new(IPAddress.Any, 31053);
    public static readonly IPEndPoint moReciever = new(IPAddress.Any, 31054);
    /// <summary>
    /// Listens for any incoming packet
    /// </summary>
    /// <returns>Returns packet in the form of a ConnectionInitializer</returns>
    public static async Task<ConnectionInitializer> RecieveCI()
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
                return ci;
            }
        }
    }
    /// <summary>
    /// Listens for any incoming packet
    /// </summary>
    /// <returns>Returns packet in the form of a MessageInitializer</returns>
    public static async Task<MessageInitializer> RecieveMI()
    {
        using Socket listener = new(SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(miReciever);//accepts connections from all ip addresses
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
                MessageInitializer? mi = JsonConvert.DeserializeObject<MessageInitializer>
                    (response.Replace(eom, ""));
                return mi;
            }
        }
    }
    /// <summary>
    /// Listens for any incoming packet
    /// </summary>
    /// <returns>Returns packet in the form of SenderCharacters</returns>
    public static async Task<SenderCharacters> RecieveSC()
    {
        using Socket listener = new(SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(scReciever);//accepts connections from all ip addresses
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
                SenderCharacters? sc = JsonConvert.DeserializeObject<SenderCharacters>
                    (response.Replace(eom, ""));
                return sc;
            }
        }
    }
    /// <summary>
    /// Listens for any incoming packet
    /// </summary>
    /// <returns>Returns packet in the form of SenderQubits</returns>
    public static async Task<SenderQubits> RecieveSQ()
    {
        using Socket listener = new(SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(sqReciever);//accepts connections from all ip addresses
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
                SenderQubits? sq = JsonConvert.DeserializeObject<SenderQubits>
                    (response.Replace(eom, ""));
                return sq;
            }
        }
    }
    /// <summary>
    /// Listens for any incoming packer
    /// </summary>
    /// <returns>Returns packet in the form of a MessageObject</returns>
    public static async Task<MessageObject> RecieveMO()
    {
        using Socket listener = new(SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(moReciever);//accepts connections from all ip addresses
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
                MessageObject? mo = JsonConvert.DeserializeObject<MessageObject>
                    (response.Replace(eom, ""));
                return mo;
            }
        }
    }
}
