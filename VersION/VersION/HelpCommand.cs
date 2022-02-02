using DryIoc;
using Spectre.Console;
using Spectre.Console.Cli;
using static Spectre.Console.AnsiConsole;

namespace VersION
{
    internal sealed class HelpCommand : AsyncCommand
    {
        private readonly List<Type> _commands;

        public HelpCommand(IContainer container)
        {
            _commands = container.GetServiceRegistrations().Where(x => x.ServiceType == typeof(AsyncCommand))
                .Select(x => x.ImplementationType).ToList();
        }

        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            Write(new FigletText("VersION v0.1"));
            Write(new Rule("Starter Guide // Help"));

            WriteLine();
            MarkupLine("[green1]Available commands:[/]");
            WriteLine();

            foreach (var command in _commands)
            {
                MarkupLine($"Command: [darkorange]{command.Name}[/]");
            }

            return 0;
        }
    }
}
