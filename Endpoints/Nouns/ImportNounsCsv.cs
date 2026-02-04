using FastEndpoints;
using MemoryApp.Contracts;
using MemoryApp.Data;
using MemoryApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Endpoints.Nouns;

public class ImportNounsCsv : Endpoint<ImportNounsCsvRequest, ImportNounsCsvResponse>
{
    public AppDbContext Db { get; set; } = null!;

    public override void Configure()
    {
        Post("/api/v1/nouns/csv");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ImportNounsCsvRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.CsvContent))
        {
            Response = new ImportNounsCsvResponse { Success = false, Errors = new List<string> { "CSV content is empty" } };
            return;
        }

        var lines = req.CsvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var errors = new List<string>();
        var imported = 0;
        var skipped = 0;

        var existingNouns = await Db.Nouns
            .Select(n => n.German)
            .ToHashSetAsync(ct);

        var nounsToAdd = new List<Noun>();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            // Skip header row if present
            if (i == 0 && line.ToLower().Contains("german"))
            {
                continue;
            }

            var parts = ParseCsvLine(line);

            // Expected format: German,Article,TranslationEs,Level,Plural,Notes
            if (parts.Count < 4)
            {
                errors.Add($"Line {i + 1}: Not enough columns (expected at least 4, got {parts.Count})");
                skipped++;
                continue;
            }

            var german = parts[0].Trim();
            if (string.IsNullOrWhiteSpace(german))
            {
                errors.Add($"Line {i + 1}: German word is required");
                skipped++;
                continue;
            }

            if (existingNouns.Contains(german))
            {
                errors.Add($"Line {i + 1}: Noun '{german}' already exists");
                skipped++;
                continue;
            }

            var noun = new Noun
            {
                German = german,
                Article = parts.Count > 1 ? parts[1].Trim().ToLower() : string.Empty,
                TranslationEs = parts.Count > 2 ? parts[2].Trim() : string.Empty,
                Level = parts.Count > 3 ? parts[3].Trim() : "A1",
                Plural = parts.Count > 4 && !string.IsNullOrWhiteSpace(parts[4]) ? parts[4].Trim() : null,
                Notes = parts.Count > 5 && !string.IsNullOrWhiteSpace(parts[5]) ? parts[5].Trim() : null
            };

            nounsToAdd.Add(noun);
            existingNouns.Add(german);
            imported++;
        }

        if (nounsToAdd.Count > 0)
        {
            Db.Nouns.AddRange(nounsToAdd);
            await Db.SaveChangesAsync(ct);
        }

        Response = new ImportNounsCsvResponse
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
}
