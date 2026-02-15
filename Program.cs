using Application_task.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext (SQLite)
var connection = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=appdata.db";
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection));

var app = builder.Build();

// Compatibility middleware: rewrite legacy /User2/* requests to /User/* to avoid 404s from cached clients
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? string.Empty;
    if (path.StartsWith("/User2", StringComparison.OrdinalIgnoreCase))
    {
        // rewrite to /User + remainder
        var newPath = "/User" + path.Substring("/User2".Length);
        context.Request.Path = newPath;
    }
    await next();
});

// Ensure database created and seed states
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    if (!db.States.Any())
    {
        db.States.AddRange(new[] {
            new State { Name = "California" },
            new State { Name = "New York" },
            new State { Name = "Texas" },
            new State { Name = "Florida" },
            new State { Name = "Washington" },
            new State { Name = "Illinois" }
        });
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Only enable HTTPS redirection in non-development environments to avoid the "Failed to determine the https port" warning during development.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Index}/{id?}");

app.Run();
