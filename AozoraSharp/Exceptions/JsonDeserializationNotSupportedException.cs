using System;

namespace AozoraSharp.Exceptions;

public sealed class JsonDeserializationNotSupportedException : NotSupportedException
{
    public JsonDeserializationNotSupportedException(string encounteredTypeName) : base($"Encountered unknown type during deserialization. Type {encounteredTypeName} is not supported.") { }
}
