using System.Net.Sockets;
using System.Text;

namespace QMS.QMSScripts;
public class ServerFunctions
{
    public static async Task<string> RecieveCI()
    {
        using Socket listener = new(SocketType.Stream, ProtocolType.Tcp);
        listener.Listen(31050);
        var handler = await listener.AcceptAsync();
        while (true)
        {
            var buffer = new byte[1024];
            var recieved = await handler.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, recieved);

            var eom = "<|EOM|>";
            if (response.IndexOf(eom) > -1 /* end of message*/)
            {
                Console.WriteLine("message recieved");
                Console.WriteLine(response.Replace(eom, ""));

                var ackMessage = "<|ACK|>";
                var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
                await handler.SendAsync(echoBytes, 0);
                Console.WriteLine("acknowledgement sent");

                return response.Replace(eom, "");
            }
        }
    }
}
