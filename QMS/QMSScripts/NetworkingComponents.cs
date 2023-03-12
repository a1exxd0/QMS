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
using QMS.QMSScripts;

namespace QMS.Networking;
#region Sent components CI, MI, SC, SQ
/// <summary>
/// Abstract unusable class with virtual method to be overridden for all below components
/// </summary>
public abstract class SentObjects
{
    public virtual string Serialize()
    {
        return ("");
    }
}
/// <summary>
/// Use to establish connection with specific user.
/// 
/// (properties)
/// 1 - sending username
/// 2 - request username
/// 3 - sending username and requesting username
/// </summary>
public class ConnectionInitializer : SentObjects
{

    public int sendOrRetrieveUsername;
    public string? username;

    public ConnectionInitializer(int option, string? Username)
    {
        sendOrRetrieveUsername = option;
        username = Username;
    }
    /// <summary>
    /// Serializes object
    /// </summary>
    /// <returns>string of object properties</returns>
    public override string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
}

/// <summary>
/// Use to establish message details to send to reciever
/// 
/// (properties)
/// messageID - identifier for message
/// messageLength - length of desired message to
///                 indicate if complete on reciever end
/// senderUsername - name of person sending the MessageInitializer
/// </summary>
public class MessageInitializer : SentObjects 
{
    public int messageID;
    public int messageLength;
    public string? senderUsername;

    public MessageInitializer(int messageID, int messageLength)
    {
        this.messageID = messageID;
        this.messageLength = messageLength;
        this.senderUsername = KeyVarFunc.username;
    }
    /// <summary>
    /// Serializes object
    /// </summary>
    /// <returns>string of object properties</returns>
    public override string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
}
/// <summary>
/// To send individual character to reciever
/// 
/// (properties)
/// messageID - identifier for message
/// characterPosition - identifier for where it is in message
///                     incase it comes in the wrong order
/// encodedMessage - for after bitwise XOR has taken place
/// senderUsername - name of person sending the SenderCharacter
/// </summary>
public class SenderCharacter :SentObjects
{
    public int messageID;
    public int characterPosition;
    public int encodedMessage;
    public string? senderUsername;

    public SenderCharacter(int messageID, int characterPosition, int encodedMessage)
    {
        this.messageID = messageID;
        this.characterPosition = characterPosition;
        this.encodedMessage = encodedMessage;
        this.senderUsername = KeyVarFunc.username;
    }
    /// <summary>
    /// Serializes object
    /// </summary>
    /// <returns>string of object properties</returns>
    public override string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
}
/// <summary>
/// To send individual Qubits to reciever
/// 
/// (properties)
/// messageID - identifier for message
/// characterPosition - identifier for where it is in message
///                     incase it comes in the wrong order
/// qubitSystems - to simulate reciever actually receiving the qubits
/// senderUsername - name of person sending the SenderQubits
/// </summary>
public class SenderQubits:SentObjects
{
    public int messageID;
    public int characterPosition;
    public QubitSystem[] qubitSystems;
    public string? senderUsername;

    [JsonConstructor]
    public SenderQubits(int messageID, int characterPosition, QubitSystem[] qubitSystems)
    {
        this.messageID = messageID;
        this.characterPosition = characterPosition;
        this.qubitSystems = qubitSystems;
        this.senderUsername = KeyVarFunc.username;
    }
    /// <summary>
    /// Serializes object
    /// </summary>
    /// <returns>string of object properties</returns>
    public override string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }
}
#endregion
public class ClientFunctions
{
    //declarations for IP/Socket combinations for every class transferred over network
    Dictionary<string, IPEndPoint> socketMatcher = new()
    {
        {"ConnectionInitializer", new(DatabaseOptions.ServerIP, 31050)},
        {"MessageInitializer", new(DatabaseOptions.ServerIP, 31051)},
        {"SenderCharacter", new(DatabaseOptions.ServerIP, 31052)},
        {"SenderQubits", new(DatabaseOptions.ServerIP, 31053)}
    };
    /// <summary>
    /// Sends object across network
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    public async void SendObject<T>(T obj) where T : SentObjects
    {

        string s = obj.Serialize();
        //open up a socket object to correct destination
        using Socket client = new(socketMatcher[obj.GetType().ToString()].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        //execute while loop as soon as connection is established
        await client.ConnectAsync(socketMatcher[obj.GetType().ToString()]);
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
    /// Sends message to server
    /// </summary>
    /// <param name="s">string to be sent</param>
    public async void SendMessage(string s)
    {
        var chararray = SideSender.splitIntoCharacters(s);
        MessageInitializer MI = new(KeyVarFunc.messageIDCounter, chararray.Length);

        SendObject(MI); //To let server know message is coming

        for (int i = 0; i < chararray.Length; i++) //i represents char position
        {
            //deals with setting up correct object properties seperately.
            SenderBitsAndQubits BQ = SideSender.sendCharacterAlgorithm
                                            (chararray[i], i, messageIDcounter);
            SendObject(BQ.qubits);
            SendObject(BQ.bits);
            Task.Delay(600).Wait();
        }
        messageIDcounter++; //increment for unique message IDs
    }
}

