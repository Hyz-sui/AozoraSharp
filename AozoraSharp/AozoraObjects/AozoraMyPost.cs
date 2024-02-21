using AozoraSharp.Constants;
using AozoraSharp.HttpObjects;
using AozoraSharp.HttpObjects.Interfaces;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace AozoraSharp.AozoraObjects;

public class AozoraMyPost(
    AozoraMyUser author,
    string text,
    DateTime createdAt,
    string collection,
    string uri,
    string cid,
    IEmbed embed = null) : AozoraPost(author, text, createdAt, collection, uri, cid, embed)
{
    public override AozoraMyUser Author { get; } = author;

    public AozoraMyPost(AozoraMyUser author, Post post, CreateRecordResponse createRecordResponse, string collection) : this(
        author,
        post.Text,
        DateTime.ParseExact(post.CreatedAt, "o", DateTimeFormatInfo.InvariantInfo),
        collection,
        createRecordResponse.Uri,
        createRecordResponse.Cid)
    { }

    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        logger.Debug("Deleting post");
        var request = new DeleteRecordRequest(Author.Did, Collection, Rkey);
        await Author.Client.PostCustomXrpcAsync(ATEndpoint.DeleteRecord, request, cancellationToken);
    }
}
