using System.Text.Json;

namespace LinkvertiseBypass.Handlers.Json;

public class JsonHelper
{
    /// <summary>
    /// Attempts to deserialize a JSON string into an object.
    /// </summary>
    /// <param name="json">The JSON string to be deserialized.</param>
    /// <param name="result">The deserialized object of type T, if successful; otherwise, null.</param>
    /// <typeparam name="T">The type of the class to deserialize into.</typeparam>
    /// <returns>True if the deserialization is successful; otherwise, false.</returns>
    public static bool TryDeserialize<T>(string json, out T? result)
    {
        result = default;
        try
        {
            result =  JsonSerializer.Deserialize<T>(json);
            return true;
        }
        catch(JsonException jsonException)
        {
            Console.WriteLine(jsonException);
            return false;
        }
    }
}