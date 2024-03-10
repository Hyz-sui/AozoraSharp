using System.Collections;
using System.Collections.Generic;
using AozoraSharp.AozoraObjects;

namespace AozoraSharp;

public sealed class FetchedPosts : IReadOnlyList<AozoraPost>
{
    public string Cursor { get; }
    internal IReadOnlyList<AozoraPost> Inner { get; }

    internal FetchedPosts(string cursor, IReadOnlyList<AozoraPost> posts)
    {
        Cursor = cursor;
        Inner = posts;
    }

    public AozoraPost this[int index] => Inner[index];
    public int Count => Inner.Count;


    public IEnumerator<AozoraPost> GetEnumerator() => Inner.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Inner.GetEnumerator();
}
