using InstagramApiSharp.API;
using System.Text.Json;
using BejaiaCovid.Models;

namespace BejaiaCovid.Commands;

public class SummaryDataCommand : Command
{
    public SummaryDataCommand()
    {
        Name = "summary";
        Description = "Affiche un bilan global";
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
            $"Jour #{result.Days}\nBilan global national :\nCas confirmés depuis le début : {result.Latest.Confirmed}\n" +
            $"Guérisons totales depuis le début : {result.Latest.Recovered}\n" +
            $"Décès totaux depuis le début : {result.Latest.Deaths}\n";
        return text;
    }
}