using AozoraSharp.Constants;

namespace AozoraSharp.HttpObjects;

public record EmbedRecord(RecordStrongReference Record) : Embed
{
    public override string ATType { get; } = ATTypeName.EmbedRecord;
}
