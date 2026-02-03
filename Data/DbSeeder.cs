using MemoryApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MemoryApp.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Check if already seeded
        if (await context.Verbs.AnyAsync())
        {
            return;
        }

        // Seed Verbs
        var verbs = new List<Verb>
        {
            new Verb
            {
                Infinitive = "sein",
                TranslationEs = "ser / estar",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = true,
                Auxiliary = "sein",
                PartizipII = "gewesen",
                PraeteritumIch = "war",
                Notes = "Verbo irregular muy importante"
            },
            new Verb
            {
                Infinitive = "haben",
                TranslationEs = "tener / haber",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = true,
                Auxiliary = "haben",
                PartizipII = "gehabt",
                PraeteritumIch = "hatte",
                Notes = "Verbo irregular muy importante"
            },
            new Verb
            {
                Infinitive = "gehen",
                TranslationEs = "ir / caminar",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = true,
                Auxiliary = "sein",
                PartizipII = "gegangen",
                PraeteritumIch = "ging",
                Notes = "Verbo de movimiento, usa sein"
            },
            new Verb
            {
                Infinitive = "kommen",
                TranslationEs = "venir",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = true,
                Auxiliary = "sein",
                PartizipII = "gekommen",
                PraeteritumIch = "kam",
                Notes = "Verbo de movimiento"
            },
            new Verb
            {
                Infinitive = "machen",
                TranslationEs = "hacer",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = false,
                Auxiliary = "haben",
                PartizipII = "gemacht",
                PraeteritumIch = "machte"
            },
            new Verb
            {
                Infinitive = "sagen",
                TranslationEs = "decir",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = false,
                Auxiliary = "haben",
                PartizipII = "gesagt",
                PraeteritumIch = "sagte"
            },
            new Verb
            {
                Infinitive = "können",
                TranslationEs = "poder",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = true,
                Auxiliary = "haben",
                PartizipII = "gekonnt",
                PraeteritumIch = "konnte",
                Notes = "Verbo modal"
            },
            new Verb
            {
                Infinitive = "wollen",
                TranslationEs = "querer",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = true,
                Auxiliary = "haben",
                PartizipII = "gewollt",
                PraeteritumIch = "wollte",
                Notes = "Verbo modal"
            },
            new Verb
            {
                Infinitive = "müssen",
                TranslationEs = "deber / tener que",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = true,
                Auxiliary = "haben",
                PartizipII = "gemusst",
                PraeteritumIch = "musste",
                Notes = "Verbo modal"
            },
            new Verb
            {
                Infinitive = "sehen",
                TranslationEs = "ver",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = true,
                Auxiliary = "haben",
                PartizipII = "gesehen",
                PraeteritumIch = "sah"
            },
            new Verb
            {
                Infinitive = "sprechen",
                TranslationEs = "hablar",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = true,
                Auxiliary = "haben",
                PartizipII = "gesprochen",
                PraeteritumIch = "sprach",
                Notes = "Cambia vocal: sprechen-spricht"
            },
            new Verb
            {
                Infinitive = "essen",
                TranslationEs = "comer",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = true,
                Auxiliary = "haben",
                PartizipII = "gegessen",
                PraeteritumIch = "aß"
            },
            new Verb
            {
                Infinitive = "trinken",
                TranslationEs = "beber",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = true,
                Auxiliary = "haben",
                PartizipII = "getrunken",
                PraeteritumIch = "trank"
            },
            new Verb
            {
                Infinitive = "schlafen",
                TranslationEs = "dormir",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = true,
                Auxiliary = "haben",
                PartizipII = "geschlafen",
                PraeteritumIch = "schlief"
            },
            new Verb
            {
                Infinitive = "aufstehen",
                TranslationEs = "levantarse",
                Level = "A1",
                Prefix = "auf",
                IsSeparable = true,
                IsIrregular = true,
                Auxiliary = "sein",
                PartizipII = "aufgestanden",
                PraeteritumIch = "stand auf",
                Notes = "Verbo separable: ich stehe auf"
            },
            new Verb
            {
                Infinitive = "ankommen",
                TranslationEs = "llegar",
                Level = "A2",
                Prefix = "an",
                IsSeparable = true,
                IsIrregular = true,
                Auxiliary = "sein",
                PartizipII = "angekommen",
                PraeteritumIch = "kam an",
                Notes = "Verbo separable de movimiento"
            },
            new Verb
            {
                Infinitive = "anfangen",
                TranslationEs = "comenzar",
                Level = "A2",
                Prefix = "an",
                IsSeparable = true,
                IsIrregular = true,
                Auxiliary = "haben",
                PartizipII = "angefangen",
                PraeteritumIch = "fing an",
                Notes = "Verbo separable"
            },
            new Verb
            {
                Infinitive = "warten",
                TranslationEs = "esperar",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = false,
                Auxiliary = "haben",
                PartizipII = "gewartet",
                PraeteritumIch = "wartete",
                Notes = "Suele usarse con 'auf + Akk': warten auf dich"
            },
            new Verb
            {
                Infinitive = "fahren",
                TranslationEs = "conducir / ir (en vehículo)",
                Level = "A1",
                IsSeparable = false,
                IsIrregular = true,
                Auxiliary = "sein",
                PartizipII = "gefahren",
                PraeteritumIch = "fuhr",
                Notes = "Verbo de movimiento"
            },
            new Verb
            {
                Infinitive = "laufen",
                TranslationEs = "correr / caminar",
                Level = "A2",
                IsSeparable = false,
                IsIrregular = true,
                Auxiliary = "sein",
                PartizipII = "gelaufen",
                PraeteritumIch = "lief",
                Notes = "Verbo de movimiento"
            }
        };

        await context.Verbs.AddRangeAsync(verbs);
        await context.SaveChangesAsync();

        // Seed Sentences
        var sentences = new List<Sentence>
        {
            new Sentence
            {
                Level = "A1",
                Type = "present",
                SentenceTemplate = "Ich ___ nach Hause.",
                CorrectAnswer = "gehe",
                TranslationEs = "Yo voy a casa.",
                Hint = "Verbo 'gehen' en presente, primera persona",
                VerbId = verbs.First(v => v.Infinitive == "gehen").Id
            },
            new Sentence
            {
                Level = "A1",
                Type = "present",
                SentenceTemplate = "Er ___ jeden Tag zur Arbeit.",
                CorrectAnswer = "geht",
                TranslationEs = "Él va todos los días al trabajo.",
                Hint = "Verbo 'gehen' en presente, tercera persona",
                VerbId = verbs.First(v => v.Infinitive == "gehen").Id
            },
            new Sentence
            {
                Level = "A1",
                Type = "infinitive",
                SentenceTemplate = "Ich möchte nach Berlin ___.",
                CorrectAnswer = "fahren",
                TranslationEs = "Quiero ir a Berlín.",
                Hint = "Infinitivo del verbo de movimiento",
                VerbId = verbs.First(v => v.Infinitive == "fahren").Id
            },
            new Sentence
            {
                Level = "A1",
                Type = "separable",
                SentenceTemplate = "Ich ___ um 7 Uhr ___.",
                CorrectAnswer = "stehe auf",
                TranslationEs = "Me levanto a las 7.",
                Hint = "Verbo separable 'aufstehen' en presente",
                VerbId = verbs.First(v => v.Infinitive == "aufstehen").Id
            },
            new Sentence
            {
                Level = "A1",
                Type = "perfekt",
                SentenceTemplate = "Wir ___ nach Berlin ___.",
                CorrectAnswer = "sind gefahren",
                TranslationEs = "Hemos ido a Berlín.",
                Hint = "Perfekt con sein porque es verbo de movimiento",
                VerbId = verbs.First(v => v.Infinitive == "fahren").Id
            },
            new Sentence
            {
                Level = "A1",
                Type = "preposition",
                SentenceTemplate = "Ich warte ___ dich.",
                CorrectAnswer = "auf",
                TranslationEs = "Te espero.",
                Hint = "Preposición que acompaña a 'warten'",
                VerbId = verbs.First(v => v.Infinitive == "warten").Id
            },
            new Sentence
            {
                Level = "A1",
                Type = "present",
                SentenceTemplate = "Sie ___ Deutsch.",
                CorrectAnswer = "spricht",
                TranslationEs = "Ella habla alemán.",
                Hint = "Verbo 'sprechen', tercera persona singular",
                VerbId = verbs.First(v => v.Infinitive == "sprechen").Id
            },
            new Sentence
            {
                Level = "A1",
                Type = "present",
                SentenceTemplate = "Wir ___ Pizza.",
                CorrectAnswer = "essen",
                TranslationEs = "Comemos pizza.",
                Hint = "Verbo 'essen' en presente, primera persona plural",
                VerbId = verbs.First(v => v.Infinitive == "essen").Id
            }
        };

        await context.Sentences.AddRangeAsync(sentences);
        await context.SaveChangesAsync();
    }
}
