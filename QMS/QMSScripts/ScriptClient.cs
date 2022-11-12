using System.Net;
using System.Net.Sockets;
using System.Text;

namespace QMS.QMSScripts;
public class ClientFunctions
{
    public static readonly IPEndPoint ciSocket = new(DatabaseOptions.ServerIP, 31050);
    public static async void SendCI(string s)
    {
        using Socket client = new(ciSocket.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        await client.ConnectAsync(ciSocket);
        s = s + "<|EOM|>";
        while (true)
        {
            var messageBytes = Encoding.UTF8.GetBytes(s);
            _ = await client.SendAsync(messageBytes, SocketFlags.None);
            Console.WriteLine("sent ci");

            var buffer = new byte[1024];
            var recieved = await client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, recieved);
            if (response == "<|ACK|>")
            {
                Console.WriteLine("acknowledged");
                break;
            }
        }

        client.Shutdown(SocketShutdown.Both);
    }
}
