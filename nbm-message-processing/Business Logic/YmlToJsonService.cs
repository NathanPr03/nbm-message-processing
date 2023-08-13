using System.IO;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace nbm_message_processing.Business_Logic;

public class YmlToJsonService
{
    public void WriteYmlToJson(string yamlFilePath)
    {
        string jsonFilePath = @"../../../converted.json";
        
        string yamlContent = File.ReadAllText(yamlFilePath);

        var deserializer = new DeserializerBuilder().Build();

        var yamlObject = deserializer.Deserialize<object>(yamlContent);

        var jsonContent = JsonConvert.SerializeObject(yamlObject, Formatting.Indented);

        File.WriteAllText(jsonFilePath, jsonContent);
    }
}