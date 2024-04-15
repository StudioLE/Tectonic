using Microsoft.Extensions.DependencyInjection;
using StudioLE.Serialization.Parsing;

namespace Tectonic.Extensions.CommandLine;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandBuilderServices(this IServiceCollection services)
    {
        return services
            .AddTransient<CommandBuilder>()
            .AddTransient<CommandFactory>()
            .AddTransient<IParser, Parser>()
            // TODO: CommandArgumentsStrategy is a factory?
            .AddTransient<ICommandArgumentsStrategy, CommandArgumentsStrategy>()
            .AddTransient<ICommandOptionsStrategy, CommandOptionsStrategy>()
            .AddTransient<ICommandHandlerStrategy, CommandHandlerStrategy>();
    }
}
