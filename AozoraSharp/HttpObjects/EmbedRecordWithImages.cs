namespace AozoraSharp.HttpObjects;

public record EmbedRecordWithImages : EmbedRecordWithMedia
{
    public override EmbedImages Media { get; }

    public EmbedRecordWithImages(EmbedRecord record, EmbedImages media) : base(record, media)
    {
        Media = media;
    }
}
