using Links.Api;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Links")
    ?? throw new InvalidOperationException("Connection string 'Links' is not configured.");

builder.Services.AddDbContext<LinksDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<LinksDbContext>();

var app = builder.Build();

// Learning shortcut (see "Decisions made for you"): create the tables if the database has none.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LinksDbContext>();
    await db.Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "Links service is running");
app.MapHealthChecks("/health");

app.MapPost("/links", async (CreateLinkRequest request, LinksDbContext db, CancellationToken ct) =>
{
    if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var uri)
        || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["url"] = ["Use an absolute http:// or https:// URL."]
        });
    }

    // A new code can collide with an existing one. The primary key rejects the
    // duplicate, and we try again with another code.
    for (var attempt = 1; ; attempt++)
    {
        var link = new Link { Code = ShortCode.New(), Url = uri.OriginalString, CreatedAt = DateTimeOffset.UtcNow };
        db.Links.Add(link);
        try
        {
            await db.SaveChangesAsync(ct);
            return Results.Created($"/links/{link.Code}", LinkResponse.From(link));
        }
        catch (DbUpdateException ex) when (attempt < 3
            && ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            db.Entry(link).State = EntityState.Detached;
        }
    }
});

app.MapGet("/links/{code}", async (string code, LinksDbContext db, CancellationToken ct) =>
{
    var link = await db.Links.AsNoTracking().FirstOrDefaultAsync(l => l.Code == code, ct);
    return link is null ? Results.NotFound() : Results.Ok(LinkResponse.From(link));
});

app.MapPost("/links/{code}/disable", async (string code, LinksDbContext db, CancellationToken ct) =>
{
    var link = await db.Links.FirstOrDefaultAsync(l => l.Code == code, ct);
    if (link is null)
    {
        return Results.NotFound();
    }

    link.DisabledAt ??= DateTimeOffset.UtcNow; // disabling twice keeps the first time
    await db.SaveChangesAsync(ct);
    return Results.NoContent();
});

app.Run();