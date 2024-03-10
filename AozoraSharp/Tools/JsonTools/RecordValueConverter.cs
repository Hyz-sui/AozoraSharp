using System;
using System.Text.Json;
using AozoraSharp.Attributes;
using AozoraSharp.Constants;
using AozoraSharp.Exceptions;
using AozoraSharp.HttpObjects;
using AozoraSharp.HttpObjects.Interfaces;
using AozoraSharp.HttpObjects.Records;

namespace AozoraSharp.Tools.JsonTools;

[CustomJsonConverter]
public class RecordValueConverter : CustomJsonConverter<IRecordValue>
{
    public override IRecordValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var element = document.RootElement;
        var typeName = ParseATTypeName(element);
        return typeName switch
        {
            ATTypeName.FeedPost => Read<Post>(element),
            ATTypeName.GraphFollow => Read<FollowRecord>(element),
            ATTypeName.GraphBlock => Read<BlockRecord>(element),
            // TODO: WIP
            _ => throw new JsonDeserializationNotSupportedException(typeName),
        };
    }

    public override void Write(Utf8JsonWriter writer, IRecordValue value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case Post post: Write(writer, post); break;
            case FollowRecord follow: Write(writer, follow); break;
            case BlockRecord block: Write(writer, block); break;
            // TODO: WIP
            default: throw new JsonSerializationNotSupportedException(value?.GetType());
        }
    }
}

// TODO: hope to be coded by generator in the future?
