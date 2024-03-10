using System;
using AozoraSharp.HttpObjects.Interfaces;

namespace AozoraSharp.HttpObjects.Records;

public readonly record struct BlockRecord(string Subject, DateTime CreatedAt) : IRecordValue;
