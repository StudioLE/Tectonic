using BenchmarkDotNet.Running;

namespace Geometrician.Drawings.Benchmarks;

internal sealed class Program
{
    public static void Main(string[] args)
    {
#if DEBUG
        throw new("Benchmarks must be run as Release configuration.");
#endif
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
