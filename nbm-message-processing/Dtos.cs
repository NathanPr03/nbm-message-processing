using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace nbm_message_processing;

public class DeserialisedMessageDto
{
    [YamlMember(Alias = "_messageType")]
    public string MessageType { get; set; }

    [YamlMember(Alias = "_messageText")]
    public string MessageText { get; set; }
}

public class DeserialisedSirDto
{
    [YamlMember(Alias = "Date")]
    public string Date { get; set; }

    [YamlMember(Alias = "SortCode")]
    public string SortCode { get; set; }

    [YamlMember(Alias = "IncidentNature")]
    public string IncidentNature { get; set; }
}

public class RootDto
{
    [YamlMember(Alias = "_messages")]
    public List<DeserialisedMessageDto> Messages { get; set; }

    [YamlMember(Alias = "_sirDtos")]
    public List<DeserialisedSirDto> SirDtos { get; set; }

    [YamlMember(Alias = "_mentions")]
    public Dictionary<string, int> Mentions { get; set; }

    [YamlMember(Alias = "_hashtags")]
    public Dictionary<string, int> Hashtags { get; set; }
}
