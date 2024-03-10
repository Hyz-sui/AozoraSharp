using System;
using System.Text.Json;
using AozoraSharp.Attributes;
using AozoraSharp.Constants;
using AozoraSharp.Exceptions;
using AozoraSharp.HttpObjects;
using AozoraSharp.HttpObjects.Interfaces;

namespace AozoraSharp.Tools.JsonTools.Converters;

[CustomJsonConverter]
public sealed class EmbedMediaConverter : CustomJsonConverter<IEmbedMedia>
{
    public override IEmbedMedia Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var element = document.RootElement;
        var typeName = ParseATTypeName(element);
        return typeName switch
        {
            ATTypeName.EmbedImages => Read<EmbedImages>(element),
            ATTypeName.EmbedExternal => Read<EmbedExternal>(element),
            _ => throw new JsonDeserializationNotSupportedException(typeName),
        };
    }

    public override void Write(Utf8JsonWriter writer, IEmbedMedia value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case EmbedImages embedImages: WriteWithATTypeName(writer, embedImages, ATTypeName.EmbedImages); break;
            case EmbedExternal embedExternal: WriteWithATTypeName(writer, embedExternal, ATTypeName.EmbedExternal); break;
            default: throw new JsonSerializationNotSupportedException(value.GetType());
        }
    }
}
