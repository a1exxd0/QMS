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
public class SenderCharacter : SentObjects
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
public class SenderQubits : SentObjects
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
#region ByteQubitsPair & SenderBits&Qubits
public class ByteQubitsPair
{
    public int bits
    {
        get; set;
    }
    public QubitSystem[]? qubits
    {
        get; set;
    }
}
/// <summary>
/// Class to store objects created from a single character input
/// </summary>
public class SenderBitsAndQubits
{
    public SenderCharacter? bits
    {
        get; set;
    }
    public SenderQubits? qubits
    {
        get; set;
    }
}
#endregion
#region ClientFunctions: components for actually sending across network
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
    /// DOES NOT UPDATE QUEUES - only sends the message
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
            SenderBitsAndQubits BQ = SideSender.sendCharacterAlgorithm //may need adjusting
                                            (chararray[i], i, KeyVarFunc.messageIDCounter);
            SendObject(BQ.qubits);
            SendObject(BQ.bits);
            Task.Delay(600).Wait();
        }
        KeyVarFunc.messageIDCounter++; //increment for unique message IDs
    }
}

#endregion
#region algorithm for encrypting singular characters
public class SideSender
{
    /// <summary>
    /// Use when you have a character you want to send
    /// </summary>
    /// <param name="c">character</param>
    /// <param name="pos">character position</param>
    /// <param name="messageID">ID for message</param>
    /// <returns>Object containing SenderCharacters & SenderQubits</returns>
    public static SenderBitsAndQubits sendCharacterAlgorithm(char c, int pos, int messageID)
    {

        QubitSystem[] system = instantiateQubits();
        ByteQubitsPair measurementResult = measureEightQubits(system);

        int appliedXORInt = c ^ measurementResult.bits;
        SenderCharacter charSent = new SenderCharacter(messageID, pos, appliedXORInt);
        SenderQubits qubitsSent = new SenderQubits(messageID, pos, measurementResult.qubits);

        SenderBitsAndQubits returned = new SenderBitsAndQubits();
        returned.bits = charSent;
        returned.qubits = qubitsSent;

        return returned;
    }
    #region sub-functions for encrypting/decrypting characters
    /// <summary>
    /// Split string into character array
    /// </summary>
    /// <param name="s">string you want to split</param>
    /// <returns>character array</returns>
    public static char[] splitIntoCharacters(string s)
    {
        char[] returned = new char[s.Length];
        int i = 0;
        foreach (char c in s)
        {
            returned[i] = c; i++;
        }
        return returned;
    }

    /// <summary>
    /// Measures 8 qubits and adjusts accordingly
    /// 
    /// For measuring qubit 1 of all sets
    /// </summary>
    /// <param name="qubits">Qubit array</param>
    /// <returns>Object containing 8 qubits array and measurement result</returns>
    public static ByteQubitsPair measureEightQubits(QubitSystem[] qubits)
    {
        int result = 0;
        for (int i = 0; i < qubits.Length; i++)
        {
            int temp = qubits[i].measurement(1);
            int multiplier = QubitSystem.intPow(2, qubits.Length - i - 1);
            result += multiplier * temp;
        }

        ByteQubitsPair returned = new ByteQubitsPair();
        returned.bits = result;
        returned.qubits = qubits;

        return returned;
    }
    /// <summary>
    /// Sets up entanglement-swappable qubits.
    /// </summary>
    /// <returns>Qubit states</returns>
    public static QubitSystem[] instantiateQubits()
    {
        QubitSystem[] returned = new QubitSystem[8];
        for (int i = 0; i < 8; i++)
        {
            QubitSystem EntSwap = new QubitSystem(4);
            EntSwap.applyGateToOneQubit(1, QubitModule.H);
            EntSwap.applyGateToOneQubit(3, QubitModule.H);

            EntSwap.applyCNOT(1);
            EntSwap.applyCNOT(3);
            EntSwap.applyCNOT(2);
            EntSwap.applyGateToOneQubit(2, QubitModule.H);
            EntSwap.applyGateToOneQubit(4, QubitModule.bellStateAnalyser
                (EntSwap.measurement(2), EntSwap.measurement(3)));
            returned[i] = EntSwap;

        }

        return returned;
    }
    #endregion
}
#endregion
#region MessageObject
public class MessageObject
{
    public int messageID;
    internal int messageLength;
    public char[] messageContents;
    public bool messageFinishedStatus;
    public string username;

    /// <summary>
    /// Only to be used once a MessageInitializer has been recieved
    /// </summary>
    /// <param name="messageID">Unique identifier for message</param>
    /// <param name="messageLength">Length of message</param>
    public MessageObject(int messageID, int messageLength, string username)
    {
        this.messageID = messageID;
        this.messageLength = messageLength;
        messageContents = new char[messageLength];
        messageFinishedStatus = false;
        this.username = username;
    }
}
#endregion