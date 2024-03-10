using System;

namespace AozoraSharp.Exceptions;

public sealed class JsonSerializationNotSupportedException : NotSupportedException
{
    public JsonSerializationNotSupportedException(Type encounteredType) : base($"Encountered unknown type during serialization. Type {encounteredType?.Name ?? "null"} is not supported.") { }
}
