using AozoraSharp.Constants;
using AozoraSharp.HttpObjects.Interfaces;
using System.Text.Json.Serialization;

namespace AozoraSharp.HttpObjects;

public record EmbedRecord(RecordStrongReference Record) : Embed
{
    public override string ATType { get; } = ATTypeName.EmbedRecord;
}
