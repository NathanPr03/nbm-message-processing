using System.Collections.Generic;
using Newtonsoft.Json;

namespace nbm_message_processing.Business_Logic;

public class AllMessagesDto
{
    [JsonProperty]
    private readonly List<Message> _messages;
    [JsonProperty]
    private readonly List<SirDto> _sirDtos;
    [JsonProperty]
    private readonly Dictionary<string, int> _mentions;
    [JsonProperty]
    private readonly Dictionary<string, int> _hashtags;

    public AllMessagesDto(List<Message> messages, List<SirDto> sirDtos, Dictionary<string, int> mentions, Dictionary<string, int> hashtags)
    {
        _messages = messages;
        _sirDtos = sirDtos;
        _mentions = mentions;
        _hashtags = hashtags;
    }
}

public record Message()
{
    [JsonProperty]
    private readonly string _messageType;
    [JsonProperty]
    private readonly string _messageText;

    public Message(string messageType, string messageText) : this()
    {
        _messageType = messageType;
        _messageText = messageText;
    }
}