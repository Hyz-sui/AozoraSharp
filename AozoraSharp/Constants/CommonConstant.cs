using System.Text.Json;
using System.Text.Json.Serialization;
using AozoraSharp.Tools.JsonTools;

namespace AozoraSharp.Constants;

public static class CommonConstant
{
    public const string ATTypePropertyName = "$type";

    public const string DefaultPostCollection = "app.bsky.feed.post";

    public const int ListRecordsMaxLimit = 100;

    internal static readonly JsonSerializerOptions DefaultJsonOptions = GenerateJsonOptions();
    private static JsonSerializerOptions GenerateJsonOptions()
    {
        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        foreach (var converter in ConverterRegistration.GetCustomConverters())
        {
            options.Converters.Add(converter);
        }
        return options;
    }

    public readonly static string DefaultPostVia = "AozoraSharp";
}
