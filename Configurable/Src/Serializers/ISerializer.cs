namespace Configurable.Serializers
{
    public interface ISerializer
    {
        T Deserialize<T>(string text);
        string Serialize<T>(T obj);
    }
}
