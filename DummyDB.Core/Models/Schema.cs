using Newtonsoft.Json;

namespace DummyDB.Core.Models;

public class Schema
{
    [JsonProperty(PropertyName = "name")] 
    public string Name { get; set; } = null!;

    [JsonProperty(PropertyName = "columns")]
    public List<Column> Columns { get; set; }

    public Schema()
    {
        Columns = new List<Column>();
    }

    public static Schema? GetFromJsonFile(string path)
    {
        var fileText = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<Schema>(fileText);
    }

    public void ToJsonFile(string dbName)
    {
        var serializedSchema = JsonConvert.SerializeObject(this, Formatting.Indented);
        var stream = File.Create($"../../../../DummyDb.Core/Databases/{dbName}/{Name}/{Name}.json");
        stream.Close();
        File.WriteAllText($"../../../../DummyDb.Core/Databases/{dbName}/{Name}/{Name}.json", serializedSchema);
    }
}