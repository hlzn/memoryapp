using FastEndpoints;
using MemoryApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MemoryApp.Endpoints.Nouns;

public class ExportNounsCsvRequest
{
    public string? Level { get; set; }
}

public class ExportNounsCsv : Endpoint<ExportNounsCsvRequest>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Get("/api/v1/nouns/export");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ExportNounsCsvRequest req, CancellationToken ct)
    {
        var query = Db.Nouns.AsQueryable();

        if (!string.IsNullOrEmpty(req.Level))
        {
            query = query.Where(n => n.Level == req.Level);
        }

        var nouns = await query.OrderBy(n => n.German).ToListAsync(ct);

        var csv = new StringBuilder();
        csv.AppendLine("German,Article,TranslationEs,Level,Plural,Notes");

        foreach (var noun in nouns)
        {
            csv.AppendLine($"{EscapeCsv(noun.German)},{EscapeCsv(noun.Article)},{EscapeCsv(noun.TranslationEs)},{EscapeCsv(noun.Level)},{EscapeCsv(noun.Plural)},{EscapeCsv(noun.Notes)}");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        var fileName = string.IsNullOrEmpty(req.Level)
            ? $"nouns_export_{DateTime.UtcNow:yyyyMMdd}.csv"
            : $"nouns_{req.Level}_export_{DateTime.UtcNow:yyyyMMdd}.csv";

        HttpContext.Response.ContentType = "text/csv";
        HttpContext.Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"");
        await HttpContext.Response.Body.WriteAsync(bytes, ct);
    }

    private static string EscapeCsv(string? value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }
}
