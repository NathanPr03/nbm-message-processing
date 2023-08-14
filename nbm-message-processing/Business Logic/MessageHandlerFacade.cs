using System;
using System.Collections.Generic;
using System.Linq;
using nbm_message_processing.Business_Logic.Message_Processors;

namespace nbm_message_processing.Business_Logic;

public class MessageHandlerFacade
{
    private readonly RouterService _routerService = new();
    private readonly JsonWriterService _jsonWriterService = new();

    private readonly List<Message> _messages = new();

    public string AddMessage(string header, string body)
    {
        IMessageProcessor messageProcessor = _routerService.Route(header);
        (string, string) messageInfo = messageProcessor.Process(header, body);
        var messageObj = new Message(messageInfo.Item1, messageInfo.Item2);
        _messages.Add(messageObj);

        return messageInfo.Item2;
    }

    public void WriteToJsonOnSessionFinish()
    {
        var email = _routerService.GetEmailMessageProcessor();
        var tweet = _routerService.GetTweetMessageProcessor();

        var allMessagesDto = new AllMessagesDto(
            _messages, email.GetSirList(), tweet.GetMentions(), tweet.GetHashtags());

        _jsonWriterService.WriteToJson(allMessagesDto);
    }

    public List<SirDto> GetSirList()
    {
        var email = _routerService.GetEmailMessageProcessor();

        return email.GetSirList();
    }

    public List<Tuple<string, int>> GetMentions()
    {
        var tweet = _routerService.GetTweetMessageProcessor();
        var mentions = tweet.GetMentions();

        List<Tuple<string, int>> sortedMentions = mentions
            .Select(kv => new Tuple<string, int>(kv.Key, kv.Value))
            .OrderByDescending(tuple => tuple.Item2)
            .ToList();

        return sortedMentions;
    }

    public List<Tuple<string, int>> GetHashtags()
    {
        var tweet = _routerService.GetTweetMessageProcessor();
        var hashtags = tweet.GetHashtags();

        List<Tuple<string, int>> sortedHashtags = hashtags
            .Select(kv => new Tuple<string, int>(kv.Key, kv.Value))
            .OrderByDescending(tuple => tuple.Item2)
            .ToList();

        return sortedHashtags;
    }

    public void ConvertYmlToJson(string filePath)
    {
        var yml = new YmlToJsonService();
        yml.WriteYmlToJson(filePath);
    }
}