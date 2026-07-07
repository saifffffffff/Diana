namespace Diana.Server.Serialization;

class JsonSerializer : ISerializer
{
    public T? Deserialize<T>(string str) => System.Text.Json.JsonSerializer.Deserialize<T>(str);
    
        
    

    public string Serialize<T>(T obj) => System.Text.Json.JsonSerializer.Serialize(obj);
    
}
