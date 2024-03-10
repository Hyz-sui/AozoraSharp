using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AozoraSharp.Tools.JsonTools;

internal static partial class ConverterRegistration
{
    internal static partial IList<JsonConverter> GetCustomConverters();
}
