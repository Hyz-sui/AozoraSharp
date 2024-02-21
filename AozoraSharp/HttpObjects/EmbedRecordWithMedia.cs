using AozoraSharp.Constants;

namespace AozoraSharp.HttpObjects;

public abstract record EmbedRecordWithMedia : Embed
{
    public EmbedRecord Record { get; }
    public abstract Embed Media { get; }

    public EmbedRecordWithMedia(EmbedRecord record)
    {
        Record = record;
    }

    public override sealed string ATType { get; } = ATTypeName.EmbedRecordWithMedia;
}
