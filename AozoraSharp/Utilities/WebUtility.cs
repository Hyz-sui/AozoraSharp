using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using AozoraSharp.Embeds;
using AozoraSharp.Tools.FileTools;
using MimeMapping;

namespace AozoraSharp.Utilities;

public static class WebUtility
{
    public static async Task<ExternalInfo> FetchExternalInfoFromUriAsync(string uri, HttpClient client)
    {
        // リンク先のhtmlからmetaタグを探す
        using var documentStream = await client.GetStreamAsync(uri);
        var parser = new HtmlParser();
        var head = await parser.ParseHeadAsync(documentStream);
        var metas = head.QuerySelectorAll("meta");

        var title = "";
        var description = "";
        string imageUri = null;
        // メタタグの中から
        foreach (var meta in metas)
        {
            // <meta property=なんとか ...>となっているものを探して，なんとかをpropertyに代入
            if (meta.GetAttribute(PropertyAttribute) is { } property)
            {
                // <meta property=なんとか content=ほにゃらら>のほにゃららをcontentに代入
                var content = meta.GetAttribute(ContentAttribute);
                // メタタグにcontentがなかったら欲しいものではないので次へ
                if (content == null)
                {
                    continue;
                }
                // propertyの値によって，contentの値を代入すべき変数を判定して代入
                switch (property)
                {
                    case OGTitleProperty: title = content; break;
                    case OGDescriptionProperty: description = content; break;
                    case OGImageProperty: imageUri = content; break;
                }
            }
        }

        // サムネイルがなかったらサムネイルなしのWebサイトカードになる
        if (imageUri == null)
        {
            return new(uri, title, description);
        }

        // サムネイルを一時ファイルにダウンロード
        using var imageResponse = await client.GetAsync(imageUri);
        var mimeType = imageResponse.Content.Headers.ContentType.MediaType;
        var extension = MimeUtility.GetExtensions(mimeType)[0];
        var imageBytes = await imageResponse.Content.ReadAsByteArrayAsync();
        var tempFile = TempManager.Instance.CreateTempFile(extension);
        var thumbPath = tempFile.Path;
        using var thumbWriter = new FileStream(thumbPath, FileMode.Create, FileAccess.Write, FileShare.Read);
        await thumbWriter.WriteAsync(imageBytes);

        return new(uri, title, description, thumbPath);
    }
    public static async Task<ExternalInfo> FetchExternalInfoFromUriAsync(string uri)
    {
        using var client = new HttpClient();
        return await FetchExternalInfoFromUriAsync(uri, client);
    }

    private const string PropertyAttribute = "property";
    private const string ContentAttribute = "content";
    private const string OGTitleProperty = "og:title";
    private const string OGDescriptionProperty = "og:description";
    private const string OGImageProperty = "og:image";
}
