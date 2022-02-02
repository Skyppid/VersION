using Spectre.Console;
using Spectre.Console.Cli;

namespace VersION.Main.PackageVersioneer.Commands
{
    internal sealed class VersioneerCommand : AsyncCommand
    {
        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            AnsiConsole.MarkupLine("[red] IT WORKS PLUGINS ARE FANCY![/]");
            return 0;
        }
    }
}
