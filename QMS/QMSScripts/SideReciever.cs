using Newtonsoft.Json;
using System.Collections.Generic;

namespace QSim.Scripts
{
    public class MessageObject
    {
        public uint messageID;
        internal uint messageLength;
        public char[] messageContents;
        public bool messageFinishedStatus;

        /// <summary>
        /// Only to be used once a MessageInitializer has been recieved
        /// </summary>
        /// <param name="messageID">Unique identifier for message</param>
        /// <param name="messageLength">Length of message</param>
        public MessageObject(uint messageID, uint messageLength)
        {
            this.messageID = messageID;
            this.messageLength = messageLength;
            messageContents = new char[messageLength];
            messageFinishedStatus = false;
        }
        /// <summary>
        /// Serializes object
        /// </summary>
        /// <returns>string of object properties</returns>
        public string MOSerialize()
        {
            var messageObject = new MessageObject
            (
                messageID,
                messageLength
            );
            messageObject.messageContents = messageContents;
            messageObject.messageFinishedStatus = messageFinishedStatus;

            return JsonConvert.SerializeObject(messageObject);
        }
    }
    public class SideReciever
    {
        public static Queue<int> messageIDQueue = new Queue<int>();
        public static List<MessageObject> messageObjectList = new List<MessageObject>();
        public static List<SenderCharacters> SenderCharactersList = new List<SenderCharacters>();
        public static List<SenderQubits> SenderQubitsList = new List<SenderQubits>();

        
    }
}
