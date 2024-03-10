using AozoraSharp.HttpObjects.Interfaces;

namespace AozoraSharp.HttpObjects;

public readonly record struct Record(string Uri, string Cid, IRecordValue Value);
