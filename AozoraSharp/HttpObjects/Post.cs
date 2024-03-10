using System.Collections.Generic;
using System.Text.Json.Serialization;
using AozoraSharp.HttpObjects.Interfaces;

namespace AozoraSharp.HttpObjects;

public readonly record struct Post(
    string Text,
    string CreatedAt,
    ICollection<string> Langs,
    IEmbed Embed,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    Reply Reply = default,
    string Via = default) : IRecordValue;
