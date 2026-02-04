using FastEndpoints;
using MemoryApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MemoryApp.Endpoints.Admin;

public class ExportCsvRequest
{
    public string? Level { get; set; }
}

public class ExportCsv : Endpoint<ExportCsvRequest>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Get("/api/v1/admin/verbs/export");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ExportCsvRequest req, CancellationToken ct)
    {
        var query = Db.Verbs.AsQueryable();

        if (!string.IsNullOrEmpty(req.Level))
        {
            query = query.Where(v => v.Level == req.Level);
        }

        var verbs = await query.OrderBy(v => v.Infinitive).ToListAsync(ct);

        var csv = new StringBuilder();
        csv.AppendLine("Infinitive,TranslationEs,Level,IsSeparable,Prefix,IsIrregular,Auxiliary,PartizipII,PraeteritumIch,Notes");

        foreach (var verb in verbs)
        {
            csv.AppendLine($"{EscapeCsv(verb.Infinitive)},{EscapeCsv(verb.TranslationEs)},{EscapeCsv(verb.Level)},{verb.IsSeparable},{EscapeCsv(verb.Prefix)},{verb.IsIrregular},{EscapeCsv(verb.Auxiliary)},{EscapeCsv(verb.PartizipII)},{EscapeCsv(verb.PraeteritumIch)},{EscapeCsv(verb.Notes)}");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        var fileName = string.IsNullOrEmpty(req.Level)
            ? $"verbs_export_{DateTime.UtcNow:yyyyMMdd}.csv"
            : $"verbs_{req.Level}_export_{DateTime.UtcNow:yyyyMMdd}.csv";

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
