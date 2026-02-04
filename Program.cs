using MemoryApp.Components;
using MemoryApp.Data;
using Microsoft.EntityFrameworkCore;
using FastEndpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add HttpClient for API calls from Blazor components
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(sp =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var request = httpContextAccessor.HttpContext?.Request;
    var baseUri = request != null
        ? $"{request.Scheme}://{request.Host}"
        : "http://localhost:5296";
    return new HttpClient { BaseAddress = new Uri(baseUri) };
});

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=memoryapp.db"));

// Add services
builder.Services.AddScoped<MemoryApp.Services.UserService>();
builder.Services.AddScoped<MemoryApp.Services.SrsService>();
builder.Services.AddScoped<MemoryApp.Services.MatchingSessionService>();
builder.Services.AddScoped<MemoryApp.Services.MatchingAnswerService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<MemoryApp.Services.AiFeedbackService>();

// Add FastEndpoints
builder.Services.AddFastEndpoints();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
    await DbSeeder.SeedAsync(db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

// Map FastEndpoints
app.UseFastEndpoints();

// Minimal API endpoints
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
