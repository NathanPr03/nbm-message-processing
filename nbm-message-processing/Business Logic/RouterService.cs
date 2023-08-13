using System;
using System.Text.RegularExpressions;
using nbm_message_processing.Business_Logic.Message_Processors;

namespace nbm_message_processing.Business_Logic;

public class RouterService
{
    private readonly TextMessageProcessor _textMessageProcessor = new();
    private readonly EmailMessageProcessor _emailMessageProcessor = new();
    private readonly TweetMessageProcessor _tweetMessageProcessor = new();
    
    public IMessageProcessor Route(string header)
    {
        string pattern = @"^(S|E|T)\d{9}";
        if (!Regex.IsMatch(header, pattern))
        {
            throw new ArgumentException(
                "Invalid header, it must start with either 'S', 'E' or 'T' then followed by 9 numeric characters, like: 'E1234567701'");
        }
        
        switch (header[0])
        {
            case 'S':
                return _textMessageProcessor; 
            case 'E':
                return _emailMessageProcessor;
            case 'T':
                return _tweetMessageProcessor;
            default:
                throw new ArgumentException("Invalid header, it must start with either 'S', 'E' or 'T'");
        }
    }

    public TextMessageProcessor GetTextMessageProcessor() => _textMessageProcessor;
    public EmailMessageProcessor GetEmailMessageProcessor() => _emailMessageProcessor;
    public TweetMessageProcessor GetTweetMessageProcessor() => _tweetMessageProcessor;

}