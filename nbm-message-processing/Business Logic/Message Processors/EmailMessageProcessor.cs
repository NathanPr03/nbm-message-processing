using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace nbm_message_processing.Business_Logic.Message_Processors;

public class EmailMessageProcessor: IMessageProcessor
{
    private const string MessageType = "Email";
    
    private readonly MessageSplitterService _messageSplitterService = new ();
    
    [JsonProperty] private readonly List<string> _quarantineList = new();
    [JsonProperty] private readonly List<SirDto> _sirList = new();
    
    [JsonProperty] private string _header;
    [JsonProperty] private string _sender;
    [JsonProperty] private string _subject;
    [JsonProperty] private string _messageText;

    public (string, string) Process(string header, string body)
    {
        _header = header;
        _sender = _messageSplitterService.ExtractSender(body);
        _subject = _messageSplitterService.ExtractSubject(body);
        string messageText = _messageSplitterService.ExtractMessageText(body);
        
        Validate(_subject, messageText);
        
        if (_subject.StartsWith("SIR"))
        {
            ProcessSir(messageText, _subject);
        }
        
        _messageText = QuarantineUrls(messageText);

        return (MessageType, _messageText);
    }

    public List<SirDto> GetSirList() => _sirList;
    
    private void ProcessSir(string message, string subject)
    {
        string date = subject.Substring(4).Trim();
        
        // This likely does not work!
        string sortCode = message[..9].Trim();
        string natureOfIncident = message[9..].Split(' ')[0];

        var sir = new SirDto(date, sortCode, natureOfIncident);
        _sirList.Add(sir);
    }

    private string QuarantineUrls(string message)
    {
        Regex urlRegex = new Regex(@"http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=]*)?");

        return urlRegex.Replace(message, match =>
        {
            _quarantineList.Add(match.Value);
            return "<URL Quarantined>";
        });
    }

    private void Validate(string subject, string messageText)
    {
        if (subject.Length >= 20)
        {
            throw new ArgumentException("The subject length is too long, it can be 20 characters at most");
        }if (messageText.Length >= 1028)
        {
            throw new ArgumentException("The message text length is too long, it can be 1028 characters at most");
        }
    }
}