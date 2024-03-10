using AozoraSharp.HttpObjects.Interfaces;

namespace AozoraSharp.HttpObjects;

public /*abstract*/ record EmbedRecordWithMedia(EmbedRecord Record, IEmbedMedia Media) : Embed;
