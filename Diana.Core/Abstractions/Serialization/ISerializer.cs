namespace Diana.Core.Abstractions.Serialization;

public interface ISerializer
{
    string Serialize<T>(T obj);

    T? Deserialize<T>(string str);
}
