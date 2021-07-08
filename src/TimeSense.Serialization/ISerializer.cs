namespace TimeSense.Serialization
{
    public interface ISerializer
    {
        string Serialize<T>(T input) where T : class;
        T Deserialize<T>(string input);
    }
}
