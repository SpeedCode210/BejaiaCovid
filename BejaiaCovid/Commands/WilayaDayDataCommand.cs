using InstagramApiSharp.API;
using System.Text.Json;
using BejaiaCovid.Models;

namespace BejaiaCovid.Commands;

public class WilayaDayDataCommand : Command
{
    public WilayaDayDataCommand()
    {
        Name = "wilayaday";
        DefaultArgs = new []{"numero_wilaya"};
        Description = "Affiche le bilan du jour pour la wilaya choisie\nExemple pour Alger : !wilaya 16";
    }
    public override async Task<string> Execute(IInstaApi api, string[] args)
    {
        if (args.Length < 2 || !Int32.TryParse(args[1], out int id) || id > 48 || id < 1)
        {
            return "Arguments invalides";
        }
        var client = new HttpClient();
        var data = await client.GetAsync($"https://api.corona-dz.live/province/{args[1]}/latest");
        var json = await data.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ProvinceElement[]>(json, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        })?[0];
        var text =
            $"{result.LastReported.ToString("dd/MM/yyyy")}\nBilan du jour de la wilaya de {result.Name} :\nNouveaux cas : {result.Data[0].NewConfirmed}\nDécès aujourd'hui : {result.Data[0].NewDeaths}\n";
        return text;
    }
}