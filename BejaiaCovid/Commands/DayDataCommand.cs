using InstagramApiSharp.API;
using System.Text.Json;
using BejaiaCovid.Models;

namespace BejaiaCovid.Commands;

public class DayDataCommand : Command
{
    public DayDataCommand()
    {
        Name = "day";
        Description = "Affiche le bilan du jour";
    }
    public override async Task<string> Execute(IInstaApi api, string[] args)
    {
        var client = new HttpClient();
        var data = await client.GetAsync("https://api.corona-dz.live/country/summary");
        var json = await data.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<SummaryElement>(json, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        var text =
            $"Jour #{result.Days} {result.Latest.Date.ToString("dd/MM/yyyy")}\nBilan journalier national :\nNouveaux cas : {result.Latest.NewConfirmed}\n" +
            $"Guérisons : {result.Latest.NewRecovered}\n" +
            $"Décès : {result.Latest.NewDeaths}\n";
        return text;
    }
}