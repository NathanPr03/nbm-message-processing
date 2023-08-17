namespace nbm_message_processing.Business_Logic.Message_Processors;

public interface IMessageSplitterService
{
    public string ExtractSender(string message);

    public string ExtractMessageText(string message);

    public string ExtractSubject(string message);
}