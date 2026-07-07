namespace Diana.Server.Serialization;

interface ISerializer
{
    string Serialize<T>(T obj);

    T? Deserialize<T>(string str);
}
