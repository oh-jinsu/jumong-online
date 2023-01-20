using Newtonsoft.Json;

public static class Serializer
{
    public static string ToJson(object o)
    {
        return JsonConvert.SerializeObject(o);
    }
}
