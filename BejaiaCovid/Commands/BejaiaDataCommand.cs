using InstagramApiSharp.API;
using System.Text.Json;
using BejaiaCovid.Models;

namespace BejaiaCovid.Commands;

public class BejaiaDataCommand : Command
{
    public BejaiaDataCommand()
    {
        Name = "bejaia";
        Description = "Affiche un bilan global pour la wilaya de Béjaïa";
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
            $"{result.LastReported.ToString("dd/MM/yyyy")}\nBilan global de la wilaya de Béjaïa :\nCas confirmés depuis le début : {result.Data[0].Confirmed}\nDécès totaux depuis le début : {result.Data[0].Deaths}\n";
        return text;
    }
}