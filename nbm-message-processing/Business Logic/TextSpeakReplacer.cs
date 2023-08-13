using System;
using System.Collections.Generic;
using System.IO;

namespace nbm_message_processing.Business_Logic;

public class TextSpeakReplacer
{
    private readonly Dictionary<string,string> _abbreviationDict = new ();
    
    public TextSpeakReplacer()
    {
        LoadAbbreviations();
    }
    
    public string ReplaceTextSpeak(string messageText)
    {
        string[] words = messageText.Split(' ');

        for (int i = 0; i < words.Length; i++)
        {
            if (_abbreviationDict.ContainsKey(words[i]))
            {
                words[i] = words[i] + " <" + _abbreviationDict[words[i]] + ">";
            }
        }

        string cleanedMessageText = String.Join(' ', words);
        return cleanedMessageText;
    }

    private void LoadAbbreviations()
    {
        using var reader = new StreamReader(@"../../../Business Logic/textwords.csv");
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line == null)
            {
                continue;
            }
            var values = line.Split(',');

            _abbreviationDict.Add(values[0], values[1]);
        }
    }
}