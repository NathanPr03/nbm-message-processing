using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace nbm_message_processing.Business_Logic.Message_Processors;

public class TweetMessageProcessor : IMessageProcessor
{
    private const string MessageType = "Tweet";
    
    private readonly IMessageSplitterService _messageSplitterService;
    private readonly ITextSpeakReplacer _textSpeakReplacer;

    [JsonProperty] private readonly Dictionary<string, int> _mentions = new(); // These two should be maps to preserve a count!!
    [JsonProperty] private readonly Dictionary<string, int> _hashtags = new();

    [JsonProperty] private string _header;
    [JsonProperty] private string _sender;
    [JsonProperty] private string _messageText;
    
    public TweetMessageProcessor()
    {
        _messageSplitterService = new MessageSplitterService();
        _textSpeakReplacer = new TextSpeakReplacer();
    }

    public TweetMessageProcessor(IMessageSplitterService messageSplitterService, ITextSpeakReplacer textSpeakReplacer)
    {
        _messageSplitterService = messageSplitterService;
        _textSpeakReplacer = textSpeakReplacer;
    }
    
    public (string, string) Process(string header, string body)
    {
        _header = header;
        _sender = _messageSplitterService.ExtractSender(body);
        string dirtyMessageText = _messageSplitterService.ExtractMessageText(body);
        Validate(_sender, dirtyMessageText);

        _messageText = _textSpeakReplacer.ReplaceTextSpeak(dirtyMessageText);

        CountMentions(body);
        CountHashtags(body);

        return (MessageType, _messageText);
    }

    public Dictionary<string, int> GetMentions() => _mentions;

    public Dictionary<string, int> GetHashtags() => _hashtags;

    private void CountMentions(string body)
    {
        string pattern = @"@\w+"; // Match '@' followed by one or more word characters

        MatchCollection matches = Regex.Matches(body, pattern);

        int matchCount = 0;
        foreach (Match match in matches)
        {
            if (matchCount == 0)
            {
                matchCount++;
                continue;
            }

            var value = match.Value;
            
            if (_mentions.ContainsKey(value))
            {
                _mentions[value] += 1;
            }
            else
            {
                _mentions[value] = 1;
            }
            matchCount++;
        }
    }

    private void CountHashtags(string body)
    {
        string pattern = @"#\w+"; // Match '#' followed by one or more word characters

        MatchCollection matches = Regex.Matches(body, pattern);

        foreach (Match match in matches)
        {
            var value = match.Value;
            
            if (_hashtags.ContainsKey(value))
            {
                _hashtags[value] += 1;
            }
            else
            {
                _hashtags[value] = 1;
            }
        }
    }

    private void Validate(string sender, string messageText)
    {
        if (sender[0] != '@')
        {
            throw new ArgumentException("The sender must start with an '@'");
        }

        if (sender.Length > 16)
        {
            throw new ArgumentException(
                "The sender length is too long, it can be 16 characters at most (including the '@')");
        }

        if (messageText.Length >= 1028)
        {
            throw new ArgumentException("The message text length is too long, it can be 140 characters at most");
        }
    }
}