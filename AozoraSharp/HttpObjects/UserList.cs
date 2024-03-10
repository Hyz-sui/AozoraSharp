using System;

namespace AozoraSharp.HttpObjects;

public readonly record struct UserList(string Uri, string Cid, string Name, string Purpose, string Avatar, ListViewer Viewer, DateTime IndexedAt);
