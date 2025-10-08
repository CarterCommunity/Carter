using System.ComponentModel.DataAnnotations;
using Carter;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCarter();
var app = builder.Build();
app.MapCarter();

app.Run();

public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }

    public string DbPath { get; }

    public BloggingContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "blogging.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}

public class Blog
{
    public int BlogId { get; set; }

    [MaxLength(255)]
    public string Url { get; set; } = null!;

    public List<Post> Posts { get; } = new();
}

public class Post
{
    public int PostId { get; set; }

    [MaxLength(50)]
    public string Title { get; set; } = null!;

    [MaxLength(1000)]
    public string Content { get; set; } = null!;

    public int BlogId { get; set; }

    public Blog Blog { get; set; } = new();
}
