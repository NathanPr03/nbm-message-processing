using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace nbm_message_processing.Business_Logic.Message_Processors;

public class TextMessageProcessor: IMessageProcessor
{
    private const string MessageType = "SMS";
    
    private readonly MessageSplitterService _messageSplitterService = new ();
    private readonly TextSpeakReplacer _textSpeakReplacer = new();
    
    [JsonProperty] private string _header;
    [JsonProperty] private string _sender;
    [JsonProperty] private string _messageText;
    
    public (string, string) Process(string header, string body)
    {
        _header = header;
        _sender = _messageSplitterService.ExtractSender(body);
        string dirtyMessageText = _messageSplitterService.ExtractMessageText(body);
        
        Validate(_sender, dirtyMessageText);
        _messageText = _textSpeakReplacer.ReplaceTextSpeak(dirtyMessageText);

        return (MessageType, _messageText);
    }
    
    private void Validate(string sender, string messageText)
    {
        string ukNumberPattern = @"^(\+44|0044|\(0\)|0)?\s?[1-9]{1}\d{1,4}\s?\d{3,4}\s?\d{3,4}$";
        if (!Regex.IsMatch(sender, ukNumberPattern))
        {
            throw new ArgumentException("The sender is not a UK based phone number");
        }if (messageText.Length >= 1028)
        {
            throw new ArgumentException("The message text length is too long, it can be 140 characters at most");
        }
    }
}