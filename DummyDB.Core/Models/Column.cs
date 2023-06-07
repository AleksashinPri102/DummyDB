using Newtonsoft.Json;

namespace DummyDB.Core.Models;

public class Column
{
    [JsonProperty(PropertyName = "name")]
    public string? Name { get; set; }
    
    [JsonProperty(PropertyName = "type")]
    public string? Type { get; set; }

    [JsonProperty(PropertyName = "IsPrimary")]
    public bool IsPrimary { get; set; }
}