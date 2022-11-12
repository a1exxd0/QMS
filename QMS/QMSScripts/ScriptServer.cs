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
                Console.WriteLine("message recieved");

                var ackMessage = "<|ACK|>";
                var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
                await handler.SendAsync(echoBytes, 0);
                Console.WriteLine("acknowledgement sent");
                //convert to object and return
                ConnectionInitializer? ci = JsonConvert.DeserializeObject<ConnectionInitializer>
                    (response.Replace(eom, "")); 
                return ci;
            }
        }
    }
}
