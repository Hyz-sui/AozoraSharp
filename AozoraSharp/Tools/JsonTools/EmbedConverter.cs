using System;
using System.Text.Json;
using AozoraSharp.Attributes;
using AozoraSharp.Constants;
using AozoraSharp.Exceptions;
using AozoraSharp.HttpObjects;
using AozoraSharp.HttpObjects.Interfaces;

namespace AozoraSharp.Tools.JsonTools;

[CustomJsonConverter]
public class EmbedConverter : CustomJsonConverter<IEmbed>
{
    public override IEmbed Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var element = document.RootElement;
        var typeName = ParseATTypeName(element);
        return typeName switch
        {
            ATTypeName.EmbedImages => Read<EmbedImages>(element),
            ATTypeName.EmbedRecord => Read<EmbedRecord>(element),
            ATTypeName.EmbedExternal => Read<EmbedExternal>(element),
            ATTypeName.EmbedRecordWithMedia => Read<EmbedRecordWithMedia>(element),
            _ => throw new JsonDeserializationNotSupportedException(typeName),
        };
    }

    public override void Write(Utf8JsonWriter writer, IEmbed value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case EmbedImages embedImages: WriteWithATTypeName(writer, embedImages, ATTypeName.EmbedImages); break;
            case EmbedRecord embedRecord: WriteWithATTypeName(writer, embedRecord, ATTypeName.EmbedRecord); break;
            case EmbedExternal embedExternal: WriteWithATTypeName(writer, embedExternal, ATTypeName.EmbedExternal); break;
            case EmbedRecordWithMedia embedRecordWithMedia: WriteWithATTypeName(writer, embedRecordWithMedia, ATTypeName.EmbedRecordWithMedia); break;
            default: throw new JsonSerializationNotSupportedException(value.GetType());
        }
    }

}
