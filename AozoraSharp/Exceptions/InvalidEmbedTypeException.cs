using System;

namespace AozoraSharp.Exceptions;

public class InvalidEmbedTypeException : Exception
{
    public InvalidEmbedTypeException(Type expected, Type actual) : base($"Invalid embed type was given. Expected {expected.Name} but got {actual?.Name ?? "null"}.") { }
}
