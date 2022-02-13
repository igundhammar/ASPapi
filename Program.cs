using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MusicDb>(options => options.UseSqlite("Data Source=MusicDb.db"));

var app = builder.Build();

app.MapGet("/", () => "Welcome to the Music API!");

app.MapGet("/music", async (MusicDb db) =>
    await db.Music.ToListAsync());

app.MapGet("music/{id}", async (int id, MusicDb db) =>
    await db.Music.FindAsync(id)
        is Music song
        ? Results.Ok(song)
        : Results.NotFound()
);

app.MapPost("/music", async(Music song, MusicDb db) =>
{
    db.Music.Add(song);
    await db.SaveChangesAsync();
    return Results.Created("New Music created! ", song);
});

app.MapPut("/music/{id}", async (int id, Music inputSong, MusicDb db) =>
{
    var song = await db.Music.FindAsync(id);
    if (song is null) return Results.NotFound();
    if (inputSong.Artist != null)
    {
        song.Artist = inputSong.Artist;
    }

    if (inputSong.Title != null)
    {
        song.Title = inputSong.Title;
    }

    if (inputSong.Length != null)
    {
        song.Length = inputSong.Length;
    }

    if (inputSong.Category != null)
    {
        song.Category = inputSong.Category;
    }
    await db.SaveChangesAsync();
    return Results.Created("Updated song with id: ", id);
});

app.MapDelete("/music/{id}", async (int id, MusicDb db) =>
{
    if (await db.Music.FindAsync(id) is Music song)
    {
        db.Music.Remove(song);
        await db.SaveChangesAsync();
        return Results.Ok(song);
    }

    return Results.NotFound();
});

app.Run();

class Music
{
    public int Id { get; set; }
    public string? Artist { get; set; }
    public string? Title { get; set; }
    public int? Length { get; set; }
    public string? Category { get; set; }
}

class MusicDb : DbContext
{
    public MusicDb(DbContextOptions<MusicDb> options) : base(options) { }
    public DbSet<Music> Music => Set<Music>();
}