using System;
using System.IO;
using Newtonsoft.Json;

namespace nbm_message_processing.Business_Logic;

public class JsonWriterService
{
    public void WriteToJson(Object objectToWrite)
    {
        string jsonString = JsonConvert.SerializeObject(objectToWrite);
        Console.WriteLine(jsonString);
        
        string filePath = @"../../../serialised.json";
        
        File.WriteAllText(filePath, jsonString);
    }
}