using System.Net;
using System.Net.Sockets;
using System.Text;

namespace QMS.QMSScripts;
/// <summary>
/// Set of functions to send data to other side through specific ports
/// </summary>
public class ClientFunctions
{
    //declarations for IP/Socket combinations for every class transferred
    public static readonly IPEndPoint ciSocket = new(DatabaseOptions.ServerIP, 31050);
    public static readonly IPEndPoint miSocket = new(DatabaseOptions.ServerIP, 31051);
    public static readonly IPEndPoint scSocket = new(DatabaseOptions.ServerIP, 31052);
    public static readonly IPEndPoint sqSocket = new(DatabaseOptions.ServerIP, 31053);
    public static readonly IPEndPoint moSocket = new(DatabaseOptions.ServerIP, 31054);

    /// <summary>
    /// Sends string through ConnectionInitialization-specific port
    /// </summary>
    /// <param name="s">string to be sent</param>
    public static async void SendCI(string s)
    {
        //open up a socket object to correct destination
        using Socket client = new(ciSocket.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        //execute while loop as soon as connection is established
        await client.ConnectAsync(ciSocket);
        s = s + "<|EOM|>"; //add endofmessage indicator
        while (true)
        {
            var messageBytes = Encoding.UTF8.GetBytes(s);
            _ = await client.SendAsync(messageBytes, SocketFlags.None); 

            var buffer = new byte[1024];
            var recieved = await client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, recieved);
            if (response == "<|ACK|>")
            {
                //Console.WriteLine("acknowledged");
                break;
            } //file is fully correctly recieved.
        }

        client.Shutdown(SocketShutdown.Both);
    }
    /// <summary>
    /// Sends string through MessageInitialization-specific port
    /// </summary>
    /// <param name="s">string to be sent</param>
    public static async void SendMI(string s)
    {
        //open up a socket object to correct destination
        using Socket client = new(miSocket.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        //execute while loop as soon as connection is established
        await client.ConnectAsync(miSocket);
        s = s + "<|EOM|>"; //add endofmessage indicator
        while (true)
        {
            var messageBytes = Encoding.UTF8.GetBytes(s);
            _ = await client.SendAsync(messageBytes, SocketFlags.None);

            var buffer = new byte[1024];
            var recieved = await client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, recieved);
            if (response == "<|ACK|>")
            {
                //Console.WriteLine("acknowledged");
                break;
            } //file is fully correctly recieved.
        }

        client.Shutdown(SocketShutdown.Both);
    }
    /// <summary>
    /// Sends string through SenderCharacters-specific port
    /// </summary>
    /// <param name="s">string to be sent</param>
    public static async void SendSC(string s)
    {
        //open up a socket object to correct destination
        using Socket client = new(scSocket.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        //execute while loop as soon as connection is established
        await client.ConnectAsync(scSocket);
        s = s + "<|EOM|>"; //add endofmessage indicator
        while (true)
        {
            var messageBytes = Encoding.UTF8.GetBytes(s);
            _ = await client.SendAsync(messageBytes, SocketFlags.None);

            var buffer = new byte[1024];
            var recieved = await client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, recieved);
            if (response == "<|ACK|>")
            {
                //Console.WriteLine("acknowledged");
                break;
            } //file is fully correctly recieved.
        }

        client.Shutdown(SocketShutdown.Both);
    }
    /// <summary>
    /// Sends string through SenderQubits-specific port
    /// </summary>
    /// <param name="s">string to be sent</param>
    public static async void SendSQ(string s)
    {
        //open up a socket object to correct destination
        using Socket client = new(sqSocket.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        //execute while loop as soon as connection is established
        await client.ConnectAsync(sqSocket);
        s = s + "<|EOM|>"; //add endofmessage indicator
        while (true)
        {
            var messageBytes = Encoding.UTF8.GetBytes(s);
            _ = await client.SendAsync(messageBytes, SocketFlags.None);

            var buffer = new byte[1024];
            var recieved = await client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, recieved);
            if (response == "<|ACK|>")
            {
                //Console.WriteLine("acknowledged");
                break;
            } //file is fully correctly recieved.
        }

        client.Shutdown(SocketShutdown.Both);
    }
    /// <summary>
    /// Sends string through MessageObject-specific port
    /// </summary>
    /// <param name="s">string to be sent</param>
    public static async void SendMO(string s)
    {
        //open up a socket object to correct destination
        using Socket client = new(moSocket.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        //execute while loop as soon as connection is established
        await client.ConnectAsync(moSocket);
        s = s + "<|EOM|>"; //add endofmessage indicator
        while (true)
        {
            var messageBytes = Encoding.UTF8.GetBytes(s);
            _ = await client.SendAsync(messageBytes, SocketFlags.None);

            var buffer = new byte[1024];
            var recieved = await client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, recieved);
            if (response == "<|ACK|>")
            {
                //Console.WriteLine("acknowledged");
                break;
            } //file is fully correctly recieved.
        }

        client.Shutdown(SocketShutdown.Both);
    }
}
