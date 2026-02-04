using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using MemoryApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Admin;

public class ImportCsv : Endpoint<ImportCsvRequest, ImportCsvResponse>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Post("/api/v1/admin/verbs/csv");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ImportCsvRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.CsvContent))
        {
            Response = new ImportCsvResponse { Success = false, Errors = new List<string> { "CSV content is empty" } };
            return;
        }

        var lines = req.CsvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var errors = new List<string>();
        var imported = 0;
        var skipped = 0;

        var existingInfinitives = await Db.Verbs
            .Select(v => v.Infinitive)
            .ToHashSetAsync(ct);

        var verbsToAdd = new List<Verb>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Skip header row if present
            if (i == 0 && line.ToLower().Contains("infinitive"))
            {
                continue;
            }

            var parts = ParseCsvLine(line);

            // Expected format: Infinitive,TranslationEs,Level,IsSeparable,Prefix,IsIrregular,Auxiliary,PartizipII,PraeteritumIch,Notes
            if (parts.Count < 8)
            {
                errors.Add($"Line {i + 1}: Not enough columns (expected at least 8, got {parts.Count})");
                skipped++;
                continue;
            }

            var infinitive = parts[0].Trim();
            if (string.IsNullOrWhiteSpace(infinitive))
            {
                errors.Add($"Line {i + 1}: Infinitive is required");
                skipped++;
                continue;
            }

            if (existingInfinitives.Contains(infinitive))
            {
                errors.Add($"Line {i + 1}: Verb '{infinitive}' already exists");
                skipped++;
                continue;
            }

            var verb = new Verb
            {
                Infinitive = infinitive,
                TranslationEs = parts.Count > 1 ? parts[1].Trim() : string.Empty,
                Level = parts.Count > 2 ? parts[2].Trim() : "A1",
                IsSeparable = parts.Count > 3 && ParseBool(parts[3]),
                Prefix = parts.Count > 4 && !string.IsNullOrWhiteSpace(parts[4]) ? parts[4].Trim() : null,
                IsIrregular = parts.Count > 5 && ParseBool(parts[5]),
                Auxiliary = parts.Count > 6 ? parts[6].Trim() : "haben",
                PartizipII = parts.Count > 7 ? parts[7].Trim() : string.Empty,
                PraeteritumIch = parts.Count > 8 && !string.IsNullOrWhiteSpace(parts[8]) ? parts[8].Trim() : null,
                Notes = parts.Count > 9 && !string.IsNullOrWhiteSpace(parts[9]) ? parts[9].Trim() : null
            };

            verbsToAdd.Add(verb);
            existingInfinitives.Add(infinitive);
            imported++;
        }

        if (verbsToAdd.Count > 0)
        {
            Db.Verbs.AddRange(verbsToAdd);
            await Db.SaveChangesAsync(ct);
        }

        Response = new ImportCsvResponse
        {
            Success = true,
            ImportedCount = imported,
            SkippedCount = skipped,
            Errors = errors
        };
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = "";
        var inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }
        result.Add(current);
        return result;
    }

    private static bool ParseBool(string value)
    {
        var lower = value.Trim().ToLower();
        return lower == "true" || lower == "1" || lower == "yes" || lower == "si" || lower == "sí";
    }
}
