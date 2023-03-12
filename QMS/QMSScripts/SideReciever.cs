//using Newtonsoft.Json;
//using System.Collections.Generic;

//namespace QMS.QMSScripts;

//public class MessageObject
//{
//    public uint messageID;
//    internal uint messageLength;
//    public char[] messageContents;
//    public bool messageFinishedStatus;

//    /// <summary>
//    /// Only to be used once a MessageInitializer has been recieved
//    /// </summary>
//    /// <param name="messageID">Unique identifier for message</param>
//    /// <param name="messageLength">Length of message</param>
//    public MessageObject(uint messageID, uint messageLength)
//    {
//        this.messageID = messageID;
//        this.messageLength = messageLength;
//        messageContents = new char[messageLength];
//        messageFinishedStatus = false;
//    }
//    /// <summary>
//    /// Serializes object
//    /// </summary>
//    /// <returns>string of object properties</returns>
//    public string MOSerialize()
//    {
//        var messageObject = new MessageObject
//        (
//            messageID,
//            messageLength
//        );
//        messageObject.messageContents = messageContents;
//        messageObject.messageFinishedStatus = messageFinishedStatus;

//        return JsonConvert.SerializeObject(messageObject);
//    }
//}
//public class SideReciever
//{
//    public static Queue<uint> messageIDQueue = new();
//    public static List<MessageObject> messageObjectList = new();
//    public static List<SenderCharacters> SenderCharactersList = new();
//    public static List<SenderQubits> SenderQubitsList = new();

//    /// <summary>
//    /// Iterates through to find matching SC/SQ pair
//    /// </summary>
//    /// <param name="messageID">unique ID</param>
//    /// <param name="characterPosition">position in message</param>
//    /// <returns>-1 if doesnt exist, else index</returns>
//    public static int CheckExistsSC(uint messageID, uint characterPosition)
//    {
//        var i = 0;
//        foreach (var SC in SenderCharactersList)
//        {
//            if ((SC.messageID == messageID) && (SC.characterPosition == characterPosition))
//            {
//                return i;
//            }
//            i++;
//        }
//        return -1;
//    }
//    /// <summary>
//    /// Iterates through to find matching SC/SQ pair
//    /// </summary>
//    /// <param name="messageID">unique ID</param>
//    /// <param name="characterPosition">position in message</param>
//    /// <returns> -1 if doesnt exist, else index</returns>
//    public static int CheckExistsSQ(uint messageID, uint characterPosition)
//    {
//        var i = 0;
//        foreach (var SQ in SenderQubitsList)
//        {
//            if ((SQ.messageID == messageID) && (SQ.characterPosition == characterPosition))
//            {
//                return i;
//            }
//            i++;
//        }
//        return -1;
//    }
//    /// <summary>
//    /// Finds a message object in list of message objects based on messageID
//    /// </summary>
//    /// <param name="messageID">unique identifier</param>
//    /// <returns>index of object</returns>
//    public static int FindIndexMO(uint messageID)
//    {
//        var i = 0;
//        foreach (var MO in messageObjectList)
//        {
//            if (MO.messageID == messageID)
//            {
//                return i;
//            }
//            else { i++; }
//        }
//        return -1;
//    }
//    /// <summary>
//    /// If all characters are filled out, change message status to complete
//    /// </summary>
//    /// <param name="messageID">unique identifier for message</param>
//    public static void ChangeMessageStatus(uint messageID)
//    {
//        var i = FindIndexMO(messageID);
//        var complete = true;
//        foreach (char c in messageObjectList[i].messageContents)
//        {
//            if (c == '\0')
//            {
//                complete = false;
//                break;
//            }
//        }
//        messageObjectList[i].messageFinishedStatus = complete;
//    }
//    /// <summary>
//    /// Measures 8 qubits and adjusts accordingly
//    /// 
//    /// For measuring qubit 4 of all sets
//    /// </summary>
//    /// <param name="qubits">Qubit array</param>
//    /// <returns>Object containing 8 qubits array and measurement result</returns>
//    public static int measureEightQubitsLast(QubitSystem[] qubits)
//    {
//        int result = 0;
//        for (int i = 0; i < qubits.Length; i++)
//        {
//            int temp = qubits[i].measurement(4);
//            int multiplier = QubitSystem.intPow(2, qubits.Length - i - 1);
//            result += multiplier * temp;
//        }

//        return result;
//    }
//}
