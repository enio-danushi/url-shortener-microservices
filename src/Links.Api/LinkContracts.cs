namespace Links.Api;

public record CreateLinkRequest(string? Url);

public record LinkResponse(string Code, string Url, DateTimeOffset CreatedAt, DateTimeOffset? DisabledAt)
{
    public static LinkResponse From(Link link) => 
        new(link.Code, link.Url, link.CreatedAt, link.DisabledAt);
}