using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.QMSScripts;
public class QgleAssistant
{
    /// <summary>
    /// Keeps track of conversation
    /// </summary>
    private int lastQuestionAskedByBot = -1;
    private int lastStatementFromBot = -1;
    private string lastQuestionAskedByUser = "";
    private string lastResponse = "";

    private static readonly Dictionary<int, string> QuestionsFromBot= new();
    private static readonly Dictionary<int, string> StatementFromBot = new();
    public QgleAssistant()
    {
        QuestionsFromBot.Add(0, "How can I help?");
        QuestionsFromBot.Add(1, "Who would you like to connect to?");

        StatementFromBot.Add(0, "Connection successful!");
        StatementFromBot.Add(1, "Connection failed :(");
        StatementFromBot.Add(2, "Hey there, " + KeyVarFunc.username + "!");
        StatementFromBot.Add(3, "I'm sorry, I didn't understand what you were saying.\n Please reword! I'm not very smart.");
    }

    public void updateLastResponse(string response)
    {
        lastResponse=response;
    }


    /// <summary>
    /// Make the bot ask Hey! How can I help?
    /// </summary>
    /// <returns></returns>
    public List<string> StartConversation()
    {
        lastQuestionAskedByBot=0;
        lastStatementFromBot=2;
        return (new List<string> { StatementFromBot[2], QuestionsFromBot[0] });
    }
    /// <summary>
    /// Call to get the bot to read lastResponse
    /// </summary>
    /// <returns></returns>
    public List<string> InterpretText()
    {
        List<string> connectSynonyms = new List<string>() { "connect", "establish", "talk"};
        List<string> greetingSynonyms = new List<string>() { "hey", "hi", "hello", "greet" };



        if (checkInList(connectSynonyms, lastResponse) && lastQuestionAskedByBot != 1)
        {
            //if user wants to connect
            lastQuestionAskedByBot=1;
            return(new List<string>() { QuestionsFromBot[1] });
        }
        else if (checkInList(greetingSynonyms, lastResponse) && lastQuestionAskedByBot != 1)
        {
            //if user says hi
            lastStatementFromBot = 2;
            return (new List<string>() { StatementFromBot[2] });
        }
        else if (lastQuestionAskedByBot== 1)
        {
            //connecting user to another user
            lastQuestionAskedByBot = -1;
            bool tempResult = establishConnection(lastResponse);
            if (tempResult)
            {
                //success
                lastStatementFromBot = 0;
                return (new List<string> { StatementFromBot[0] });
            }
            else
            {
                //failed
                lastStatementFromBot = 1;
                return (new List<string> { StatementFromBot[1] });
            }
        }
        else
        {
            //incomprehensible
            lastStatementFromBot = 3;
            return new List<string>() { StatementFromBot[3] };
        }
    }
    private bool checkInList(List<string> list, string s)
    {
        for(var i = 0; i< list.Count; i++)
        {
            if (s.ToLower().Contains(list[i]))
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// function to connect with another user
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    private bool establishConnection(string username)
    {
        try
        {
            //networking stuff lets say it works

            KeyVarFunc.queues.Add(new ViewModels.MessageList(username));
            return true;
        }
        catch
        {
            //it doesnt work and program throws error (then doesn't break the app and moves on!
            return false;
        }
    }
}
