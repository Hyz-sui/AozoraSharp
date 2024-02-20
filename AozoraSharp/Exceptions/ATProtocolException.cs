using AozoraSharp.HttpObjects;
using System;

namespace AozoraSharp.Exceptions;

/// <summary>
/// Represents errors that were thrown by the server.
/// </summary>
public class ATProtocolException : Exception
{
    public ATProtocolException(ErrorResponse errorResponse) : base($"An error was thrown by atproto. {errorResponse}") { }
}
