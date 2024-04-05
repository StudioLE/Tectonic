using Microsoft.Extensions.DependencyInjection;
using StudioLE.Serialization.Parsing;

namespace Cascade.Workflows.CommandLine;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandBuilderServices(this IServiceCollection services)
    {
        return services
            .AddScoped<CommandContext>()
            .AddTransient<CommandBuilder>()
            .AddTransient<CommandFactory>()
            .AddTransient<IParser, Parser>()
            // TODO: CommandArgumentsStrategy is a factory?
            .AddTransient<ICommandArgumentsStrategy, CommandArgumentsStrategy>()
            .AddTransient<ICommandOptionsStrategy, CommandOptionsStrategy>()
            .AddTransient<ICommandHandlerStrategy, CommandHandlerStrategy>();
    }
}
