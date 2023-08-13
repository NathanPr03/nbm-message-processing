using System;

namespace nbm_message_processing.Business_Logic.Message_Processors;

public class MessageSplitterService
{
    public string ExtractSender(string message)
    {
        string firstSearchTerm = "Subject:";
        string secondSearchTerm = "Message Text:";
        string removalTerm = "Sender:";
        int index = message.IndexOf(firstSearchTerm, StringComparison.Ordinal);
        if (index == -1)
        {
            index = message.IndexOf(secondSearchTerm, StringComparison.Ordinal);   
        }

        return message[removalTerm.Length..index].Trim();
    }

    public string ExtractMessageText(string message)
    {
        string searchTerm = "Message Text:";
        int index = message.IndexOf(searchTerm, StringComparison.Ordinal);
        
        return message[(index + searchTerm.Length)..].Trim();
    }


    public string ExtractSubject(string message)
    {
        string searchTerm = "Subject:";
        string stopTerm = "Message Text:";
        
        int index = message.IndexOf(searchTerm, StringComparison.Ordinal);
        int stopIndex = message.IndexOf(stopTerm, StringComparison.Ordinal);
        
        return message[(index + searchTerm.Length + 1)..stopIndex];
    }
}