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
            //Console.WriteLine("sent ci");//execute once message has sent

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
