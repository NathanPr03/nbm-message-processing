using Newtonsoft.Json;

namespace nbm_message_processing.Business_Logic;

public class SirDto
{
    [JsonProperty]
    public string Date { get; private set; }
    [JsonProperty]
    public string SortCode { get; private set; }
    [JsonProperty]
    public string IncidentNature { get; private set; }
    
    public SirDto(string date, string sortCode, string incidentNature)
    {
        Date = date;
        SortCode = sortCode;
        IncidentNature = incidentNature;
    }
}