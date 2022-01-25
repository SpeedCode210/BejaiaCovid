using InstagramApiSharp.API;

namespace BejaiaCovid.Commands;

public abstract class Command
{
    public string Name = "";

    public string Description = "";

    public string[] DefaultArgs = Array.Empty<string>();
    public abstract Task<string> Execute(IInstaApi api, string[] args);
}