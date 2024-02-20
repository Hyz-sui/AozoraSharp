namespace AozoraSharp.HttpObjects;

public record EmbedRecordWithExternal : EmbedRecordWithMedia
{
    public override EmbedExternal Media { get; }

    public EmbedRecordWithExternal(EmbedRecord record, EmbedExternal media) : base(record, media)
    {
        Media = media;
    }
}
