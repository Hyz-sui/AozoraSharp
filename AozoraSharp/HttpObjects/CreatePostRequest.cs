namespace AozoraSharp.HttpObjects;

public readonly record struct CreatePostRequest(string Repo, string Collection, Post Record);
