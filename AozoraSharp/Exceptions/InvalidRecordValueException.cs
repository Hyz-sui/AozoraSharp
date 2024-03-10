using System;

namespace AozoraSharp.Exceptions;

public class InvalidRecordValueException : Exception
{
    public InvalidRecordValueException(Type expected, Type actual) : base($"This should never happen, but the record type was {actual?.Name ?? "null"}, expected {expected.Name}.") { }
}
