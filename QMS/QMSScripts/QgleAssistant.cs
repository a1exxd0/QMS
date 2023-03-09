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
        StatementFromBot.Add(2, "Hey there!");
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
    public List<string> InterpretText()
    {
        if (lastResponse.ToLower().Contains("connect") && lastQuestionAskedByBot != 1)
        {
            lastQuestionAskedByBot=1;
            return(new List<string>() { QuestionsFromBot[1] });
        }
        else if ((lastResponse.ToLower().Contains("hey") || lastResponse.ToLower().Contains("hi") || lastResponse.ToLower().Contains("hello"))
            && lastQuestionAskedByBot != 1)
        {
            lastStatementFromBot = 2;
            return (new List<string>() { StatementFromBot[2] });
        }
        else if (lastQuestionAskedByBot== 1)
        {
            lastQuestionAskedByBot = -1;
            return (new List<string>() { "not yet implemented" });
        }
        else
        {
            lastStatementFromBot = 3;
            return new List<string>() { StatementFromBot[3] };
        }
    }
}
