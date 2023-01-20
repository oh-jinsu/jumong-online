using Newtonsoft.Json;

public static class Deserializer
{
    public static T FromJson<T>(string text)
    {
        return JsonConvert.DeserializeObject<T>(text);
    }
}
