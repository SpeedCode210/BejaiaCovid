using InstagramApiSharp.API;
using System.Text.Json;
using BejaiaCovid.Models;

namespace BejaiaCovid.Commands;

public class BejaiaDayDataCommand : Command
{
    public BejaiaDayDataCommand()
    {
        Name = "bejaiaday";
        Description = "Affiche le bilan du jour pour la wilaya de Béjaïa";
    }
    public override async Task<string> Execute(IInstaApi api, string[] args)
    {
        var client = new HttpClient();
        var data = await client.GetAsync("https://api.corona-dz.live/province/6/latest");
        var json = await data.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ProvinceElement[]>(json, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        })?[0];
        var text =
            $"{result.LastReported.ToString("dd/MM/yyyy")}\nBilan du jour de la wilaya de Béjaïa :\nNouveaux cas : {result.Data[0].NewConfirmed}\nDécès aujourd'hui : {result.Data[0].NewDeaths}\n";
        return text;
    }
}