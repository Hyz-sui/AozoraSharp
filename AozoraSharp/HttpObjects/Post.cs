using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using AozoraSharp.HttpObjects.Interfaces;

namespace AozoraSharp.HttpObjects;

public readonly record struct Post(
    string Text,
    DateTime CreatedAt,
    ICollection<string> Langs,
    IEmbed Embed,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    Reply Reply = default,
    string Via = default) : IRecordValue;
