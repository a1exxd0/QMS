//using Newtonsoft.Json;


//namespace QMS.QMSScripts;

///// <summary>
///// Use to establish connection with specific user.
///// 
///// (properties)
///// 1 - sending username
///// 2 - request username
///// 3 - sending username and requesting username
///// </summary>
//public class ConnectionInitializer
//{

//    public int sendOrRetrieveUsername;
//    public string? username;

//    public ConnectionInitializer(int option, string? Username)
//    {
//        sendOrRetrieveUsername = option;
//        username = Username;
//    }
//    /// <summary>
//    /// Serializes object
//    /// </summary>
//    /// <returns>string of object properties</returns>
//    public string CISerialize()
//    {
//        var connectionInitializer = new ConnectionInitializer
//        (
//            sendOrRetrieveUsername,
//            username
//        );

//        return JsonConvert.SerializeObject(connectionInitializer);
//    }
//}

///// <summary>
///// Use to establish message details to send to reciever
///// 
///// (properties)
///// messageID - identifier for message
///// messageLength - length of desired message to
/////                 indicate if complete on reciever end
///// </summary>
//public class MessageInitializer
//{
//    public uint messageID;
//    public uint messageLength;

//    public MessageInitializer(uint messageID, uint messageLength)
//    {
//        this.messageID = messageID;
//        this.messageLength = messageLength;
//    }
//    /// <summary>
//    /// Serializes object
//    /// </summary>
//    /// <returns>string of object properties</returns>
//    public string MISerialize()
//    {
//        var messageInitializer = new MessageInitializer
//        (
//            messageID,
//            messageLength
//        );
//        return JsonConvert.SerializeObject(messageInitializer);
//    }
//}
///// <summary>
///// To send individual character to reciever
///// 
///// (properties)
///// messageID - identifier for message
///// characterPosition - identifier for where it is in message
/////                     incase it comes in the wrong order
///// encodedMessage - for after bitwise XOR has taken place
///// </summary>
//public class SenderCharacters
//{
//    public uint messageID;
//    public uint characterPosition;
//    public int encodedMessage;

//    public SenderCharacters(uint messageID, uint characterPosition, int encodedMessage)
//    {
//        this.messageID = messageID;
//        this.characterPosition = characterPosition;
//        this.encodedMessage = encodedMessage;
//    }
//    /// <summary>
//    /// Serializes object
//    /// </summary>
//    /// <returns>string of object properties</returns>
//    public string SCSerialize()
//    {
//        var senderCharacters = new SenderCharacters
//        (
//            messageID,
//            characterPosition,
//            encodedMessage
//        );

//        return JsonConvert.SerializeObject(senderCharacters);
//    }
//}
///// <summary>
///// To send individual Qubits to reciever
///// 
///// (properties)
///// messageID - identifier for message
///// characterPosition - identifier for where it is in message
/////                     incase it comes in the wrong order
///// qubitSystems - to simulate reciever actually receiving the qubits
///// </summary>
//public class SenderQubits
//{
//    public uint messageID;
//    public uint characterPosition;
//    public QubitSystem[] qubitSystems;

//    [JsonConstructor]
//    public SenderQubits(uint messageID, uint characterPosition, QubitSystem[] qubitSystems)
//    {
//        this.messageID = messageID;
//        this.characterPosition = characterPosition;
//        this.qubitSystems = qubitSystems;
//    }
//    /// <summary>
//    /// Serializes object
//    /// </summary>
//    /// <returns>string of object properties</returns>
//    public string SQSerialize()
//    {
//        for (int i = 0; i < qubitSystems.Length; i++)
//        {

//        }
//        var senderQubits = new SenderQubits
//            (
//                messageID,
//                characterPosition,
//                qubitSystems
//            );
//        return JsonConvert.SerializeObject(senderQubits);
//    }
//}
///// <summary>
///// Class to store a qubit system and measurement results at the same time
///// </summary>
//public class ByteQubitsPair
//{
//    public int bits { get; set; }
//    public QubitSystem[]? qubits { get; set; }
//}
///// <summary>
///// Class to store objects created from a single character input
///// </summary>
//public class SenderBitsAndQubits
//{
//    public SenderCharacters? bits { get; set; }
//    public SenderQubits? qubits { get; set; }
//}
//public class SideSender
//{
//    /// <summary>
//    /// Split string into character array
//    /// </summary>
//    /// <param name="s">string you want to split</param>
//    /// <returns>character array</returns>
//    public static char[] splitIntoCharacters(string s)
//    {
//        char[] returned = new char[s.Length];
//        int i = 0;
//        foreach (char c in s)
//        {
//            returned[i] = c; i++;
//        }
//        return returned;
//    }
//    /// <summary>
//    /// Use when you have a character you want to send
//    /// </summary>
//    /// <param name="c">character</param>
//    /// <param name="pos">character position</param>
//    /// <param name="messageID">ID for message</param>
//    /// <returns>Object containing SenderCharacters & SenderQubits</returns>
//    public static SenderBitsAndQubits sendCharacterAlgorithm(char c, uint pos, uint messageID)
//    {

//        QubitSystem[] system = instantiateQubits();
//        ByteQubitsPair measurementResult = measureEightQubits(system);

//        int appliedXORInt = c ^ measurementResult.bits;
//        SenderCharacters charSent = new SenderCharacters(messageID, pos, appliedXORInt);
//        SenderQubits qubitsSent = new SenderQubits(messageID, pos, measurementResult.qubits);

//        SenderBitsAndQubits returned = new SenderBitsAndQubits();
//        returned.bits = charSent;
//        returned.qubits = qubitsSent;

//        return returned;
//    }
//    /// <summary>
//    /// Measures 8 qubits and adjusts accordingly
//    /// 
//    /// For measuring qubit 1 of all sets
//    /// </summary>
//    /// <param name="qubits">Qubit array</param>
//    /// <returns>Object containing 8 qubits array and measurement result</returns>
//    public static ByteQubitsPair measureEightQubits(QubitSystem[] qubits)
//    {
//        int result = 0;
//        for (int i = 0; i < qubits.Length; i++)
//        {
//            int temp = qubits[i].measurement(1);
//            int multiplier = QubitSystem.intPow(2, qubits.Length - i - 1);
//            result += multiplier * temp;
//        }

//        ByteQubitsPair returned = new ByteQubitsPair();
//        returned.bits = result;
//        returned.qubits = qubits;

//        return returned;
//    }
//    /// <summary>
//    /// Sets up entanglement-swappable qubits.
//    /// </summary>
//    /// <returns>Qubit states</returns>
//    public static QubitSystem[] instantiateQubits()
//    {
//        QubitSystem[] returned = new QubitSystem[8];
//        for (int i = 0; i < 8; i++)
//        {
//            QubitSystem EntSwap = new QubitSystem(4);
//            EntSwap.applyGateToOneQubit(1, QubitModule.H);
//            EntSwap.applyGateToOneQubit(3, QubitModule.H);

//            EntSwap.applyCNOT(1);
//            EntSwap.applyCNOT(3);
//            EntSwap.applyCNOT(2);
//            EntSwap.applyGateToOneQubit(2, QubitModule.H);
//            EntSwap.applyGateToOneQubit(4, QubitModule.bellStateAnalyser
//                (EntSwap.measurement(2), EntSwap.measurement(3)));
//            returned[i] = EntSwap;

//        }

//        return returned;
//    }
//    /*
//    static void Main()
//    {
//        QubitSystem[] qs = new QubitSystem[2];
//        qs[0] = new QubitSystem(2);
//        qs[0].applyGateToOneQubit(1, QubitModule.H);
//        qs[1] = new QubitSystem(3);
//        SenderQubits sq = new SenderQubits(1, 3, qs);
//        string stuff = sq.SQSerialize();

//        //Console.WriteLine(stuff);
//        SenderQubits result = JsonConvert.DeserializeObject<SenderQubits>(stuff);
//        QubitSystem.PrintDiracVector(result.qubitSystems[0].getDiracVector());
//        //ConnectionInitializer? dp = JsonConvert.DeserializeObject<ConnectionInitializer>(stuff);

//        SenderBitsAndQubits testvar = sendCharacterAlgorithm('b', 2, 1);
//        Console.WriteLine(testvar.bits.SCSerialize());
//        Console.WriteLine(testvar.qubits.SQSerialize());
//    } */
//}
