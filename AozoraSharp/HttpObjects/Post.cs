using AozoraSharp.HttpObjects.Interfaces;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AozoraSharp.HttpObjects;

public readonly record struct Post
{

    public string Text { get; }
    public string CreatedAt { get; }
    public ICollection<string> Langs { get; }
    [JsonIgnore]
    public IEmbed EmbedSource { get; }
    public Reply? Reply { get; }

    public Post(string text, string createdAt, ICollection<string> langs, IEmbed embed, Reply? reply = null)
    {
        Text = text;
        CreatedAt = createdAt;
        Langs = langs;
        EmbedSource = embed;
        Reply = reply;

        EmbedObject = embed;
    }


    // for polymorphic serialization
    // IEmbedのままだとIEmbedに存在するプロパティしかシリアライズされない．JsonPolymorphicAttributeは.net6では利用できない
    [JsonPropertyName("embed")]
    public object EmbedObject { get; }
}
