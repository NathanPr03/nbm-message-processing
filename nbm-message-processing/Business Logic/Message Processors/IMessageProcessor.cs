namespace nbm_message_processing.Business_Logic.Message_Processors;

public interface IMessageProcessor
{ 
    (string, string) Process(string header, string body);
}