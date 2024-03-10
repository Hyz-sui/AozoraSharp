using System.Collections.Generic;

namespace AozoraSharp.HttpObjects;

public readonly record struct FetchPostsResponse(string Cursor, IList<Record> Records);
