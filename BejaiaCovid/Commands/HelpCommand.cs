using InstagramApiSharp.API;

namespace BejaiaCovid.Commands;

public class HelpCommand : Command
{
    public HelpCommand()
    {
        Name = "help";
        Description = "Affiche la liste des commandes";
    }
    public override async Task<string> Execute(IInstaApi api, string[] args)
    {
        string result = "";
        foreach (var command in Program.Commands)
        {
            result += $"{Program.Prefix + command.Name + JoinArgs(command.DefaultArgs)} :\n{command.Description}\n";
        }

        return result;
    }

    private static string JoinArgs(string[] args)
    {
        string result = "";
        foreach (var arg in args)
        {
            result += " " + arg;
        }

        return result;
    }
}