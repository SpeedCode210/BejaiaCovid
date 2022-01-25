using InstagramApiSharp.API;
using System.Text.Json;
using BejaiaCovid.Models;

namespace BejaiaCovid.Commands;

public class ByAgeDataCommand : Command
{
    public ByAgeDataCommand()
    {
        Name = "byage";
        Description = "Affiche les données par classe d'âge";
    }
    public override async Task<string> Execute(IInstaApi api, string[] args)
    {
        var client = new HttpClient();
        var data = await client.GetAsync("https://api.corona-dz.live/country/age/latest");
        var json = await data.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AgeElement[]>(json, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        var text = "Données par classe d'âge\n";
        foreach (var element in result)
        {
            text += $"{element.Label} : \nCas confirmés : {element.Confirmed}\nDécès : {element.Deaths}\n";
        }
        return text;
    }
}