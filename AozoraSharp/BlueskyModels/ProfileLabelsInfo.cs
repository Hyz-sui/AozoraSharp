using System;
using System.Collections.Generic;
using AozoraSharp.HttpObjects;

namespace AozoraSharp.BlueskyModels;

public sealed class ProfileLabelsInfo
{
    internal ProfileLabel[] RawLabels { get; }
    public IReadOnlyList<Label> Labels => InternalLabels;
    internal List<Label> InternalLabels { get; }

    public ProfileLabelsInfo(ProfileLabel[] labels)
    {
        RawLabels = labels;

        InternalLabels = new(labels.Length);
        foreach (var label in labels)
        {
            InternalLabels.Add(new(label));
        }
    }

    public readonly struct Label(ProfileLabel label)
    {
        public string SourceDid { get; } = label.Src;
        public string Uri { get; } = label.Uri;
        public string Cid { get; } = label.Cid;
        public string Value { get; } = label.Val;
        public bool IsNegationLabel { get; } = label.Neg;
        public DateTime CreatedAt { get; } = label.Cts;
    }
}
