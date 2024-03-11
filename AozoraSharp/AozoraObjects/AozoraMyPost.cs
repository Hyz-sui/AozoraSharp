using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using AozoraSharp.Constants;
using AozoraSharp.HttpObjects;
using AozoraSharp.HttpObjects.Interfaces;

namespace AozoraSharp.AozoraObjects;

public class AozoraMyPost(
    AozoraMyUser myUser,
    AozoraMyUser author,
    string text,
    DateTime createdAt,
    string uri,
    string cid,
    IEmbed embed = null) : AozoraPost(myUser, author, text, createdAt, uri, cid, embed)
{
    public override AozoraMyUser Author { get; } = author;

    public AozoraMyPost(AozoraMyUser myUser, AozoraMyUser author, Post post, CreateRecordResponse createRecordResponse) : this(
        myUser,
        author,
        post.Text,
        DateTime.ParseExact(post.CreatedAt, "o", DateTimeFormatInfo.InvariantInfo),
        createRecordResponse.Uri,
        createRecordResponse.Cid)
    { }

    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        logger.Debug("Deleting post");
        var request = new DeleteRecordRequest(Author.Did, CommonConstant.DefaultPostCollection, Rkey);
        await Author.Client.PostCustomXrpcAsync(ATEndpoint.DeleteRecord, request, cancellationToken);
    }
}
