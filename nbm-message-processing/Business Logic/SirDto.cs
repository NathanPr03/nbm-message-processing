using Newtonsoft.Json;

namespace nbm_message_processing.Business_Logic;

public class SirDto
{
    [JsonProperty]
    private string Date { get; }
    [JsonProperty]
    private string SortCode { get; }
    [JsonProperty]
    private string IncidentNature { get; }
    
    public SirDto(string date, string sortCode, string incidentNature)
    {
        Date = date;
        SortCode = sortCode;
        IncidentNature = incidentNature;
    }
}