using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using AozoraSharp.Constants;

namespace AozoraSharp.Tools.JsonTools;

public abstract class CustomJsonConverter<TObject> : JsonConverter<TObject>
{
    protected string ParseATTypeName(JsonElement element)
    {
        var typeElement = element.GetProperty(CommonConstant.ATTypePropertyName);
        return typeElement.GetString();
    }

    protected void Write<TAs>(Utf8JsonWriter writer, TAs value) where TAs : TObject
    {
        JsonSerializer.Serialize(writer, value, CommonConstant.DefaultJsonOptions);
    }
    protected void WriteWithATTypeName<TValue>(Utf8JsonWriter writer, TValue value, string typeName)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }
        // TODO: needs to be refactored
        // Highly bad performance. Should be coded by generator for each type.
        writer.WriteStartObject();
        writer.WriteString("$type", typeName);
        var valueType = value.GetType();
        var properties = valueType.GetProperties();
        // TODO: fix
        foreach (var property in properties.DistinctBy(p => p.Name))
        {
            var attribute = property.GetCustomAttribute<JsonPropertyNameAttribute>(true);
            var propertyName = CommonConstant.DefaultJsonOptions.PropertyNamingPolicy.ConvertName(attribute?.Name ?? property.Name);
            writer.WritePropertyName(propertyName);
            var propertyType = property.PropertyType;
            var propertyValue = property.GetValue(value);
            writer.WriteRawValue(JsonSerializer.Serialize(propertyValue, propertyType, CommonConstant.DefaultJsonOptions));
        }
        writer.WriteEndObject();
    }

    protected TAs Read<TAs>(JsonElement element)
    {
        return JsonSerializer.Deserialize<TAs>(element, CommonConstant.DefaultJsonOptions);
    }
}
