using System;

namespace AozoraSharp.HttpObjects;

public readonly record struct ProfileLabel(string Src, string Uri, string Cid, string Val, bool Neg, DateTime Cts);
